namespace PublicElections.Contracts.Response.Identity
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
    }
}
