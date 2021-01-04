namespace PublicElections.Contracts.Requests.Election
{
    public class AddCandidateRequest
    {
        public string Name { get; set; }
        public int ElectionId { get; set; }
    }
}
