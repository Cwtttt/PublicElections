using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PublicElections.Api.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }

        public static string DeleteWhiteSpaces(this string str)
        {
            return str.Replace(" ", string.Empty);
        }
    }
}
