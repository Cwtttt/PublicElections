using System.Collections.Generic;

namespace PublicElections.Domain.Dto
{
    public class Result
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
