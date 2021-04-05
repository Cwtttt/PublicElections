using Newtonsoft.Json;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Services.Interfaces;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using PublicElections.Domain.Entities;

namespace PublicElections.Infrastructure.Services
{
    public class VoteService : BaseService, IVoteService
    {
        public VoteService(DataContext context)
            : base(context) { }

        public Result Add(string userId, int electionId, int candidateId)
        {
            try
            {

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Votes",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    UserVote vote = new UserVote()
                    {
                        UserId = userId,
                        ElectionId = electionId,
                        CandidateId = candidateId
                    };

                    var json = JsonConvert.SerializeObject(vote);
                    var body = Encoding.UTF8.GetBytes(json);

                    
                    channel.BasicPublish(exchange: "",
                                         routingKey: "Votes",
                                         basicProperties: properties,
                                         body: body);
                }

                return new Result() { Success = true };
            }
            catch (Exception ex)
            {
                return new Result() { Success = false, Errors = new[] { ex.ToString() } };
            }
        }

        public async Task AddAnonymousVoteAsync(int electionId, int candidateId)
        {
            await _context.Votes.AddAsync(new Vote() { ElectionId = electionId, CandidateId = candidateId });
            await _context.SaveChangesAsync();
        }

        public bool CheckIfUserCanVote(string userId, int electionId)
        {
            return !_context.Participations.Any(x => x.UserId == userId && x.ElectionId == electionId);
        }

        public async Task AddParticipationAsync(string userId, int electionId)
        {
            await _context.Participations.AddAsync(new Participation() { UserId = userId, ElectionId = electionId });
            await _context.SaveChangesAsync();
        }
    }
}
