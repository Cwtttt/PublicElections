using System;

namespace PublicElections.Contracts.Response.Elections
{
    public class ElectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
