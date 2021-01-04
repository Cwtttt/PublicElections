using Newtonsoft.Json;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Services.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services
{
    public class VoteService : BaseService, IVoteService
    {
        public VoteService(DataContext context)
            : base(context) { }

        public async Task<Result> Add(string userId, int electionId, int candidateId)
        {
            try
            {

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    UserVote vote = new UserVote()
                    {
                        UserId = userId,
                        ElectionId = electionId,
                        CandidateId = candidateId
                    };

                    var json = JsonConvert.SerializeObject(vote);
                    var body = Encoding.UTF8.GetBytes(json);

                    
                    channel.BasicPublish(exchange: "",
                                         routingKey: "hello",
                                         basicProperties: null,
                                         body: body);
                }

                return new Result() { Success = true };
            }
            catch (Exception ex)
            {
                return new Result() { Success = false, Errors = new[] { ex.ToString() } };
            }
        }


    }
}
