﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Services;
using PublicElections.Infrastructure.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.RabbitMQ.Votes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IVoteService, VoteService>()
                .AddDbContext<DataContext>(options =>
                    options.UseSqlServer(
                        "Server=localhost;Database=PublicElections;User Id=sa;Password=ExamplePassword;MultipleActiveResultSets=true"),
                        ServiceLifetime.Transient)
                .BuildServiceProvider();

            var _voteService = serviceProvider.GetService<IVoteService>();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Votes",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var userVote = JsonConvert.DeserializeObject<UserVote>(json);
                    if (_voteService.CheckIfUserCanVote(userVote.UserId, userVote.ElectionId))
                    {
                        await _voteService.AddParticipationAsync(userVote.UserId, userVote.ElectionId);
                        await _voteService.AddAnonymousVoteAsync(userVote.ElectionId, userVote.CandidateId);
                    }
                };
                channel.BasicConsume(queue: "Votes",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
