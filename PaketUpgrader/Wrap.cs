using Octokit;
using System;
using System.Threading.Tasks;

namespace PaketUpgrader
{
    public static class Wrap
    {
        public static async Task<T> RateLimiting<T>(GitHubClient client, Func<GitHubClient, Task<T>> action)
        {
            try
            {
                var result = await action(client);
                var apiInfo = client.GetLastApiInfo();
                var remaining = apiInfo.RateLimit.Remaining;

                if (remaining % 100 == 0)
                {
                    Console.WriteLine($"API warning: you have {remaining} requests remaining before you are rate-limited");
                }

                return result;
            }
            catch (RateLimitExceededException ex)
            {
                var timeToGo = ex.Reset.Subtract(DateTimeOffset.Now);
                Console.WriteLine($"You've exceeded your limit, and it will reset in {timeToGo.TotalSeconds} seconds");
                Environment.Exit(1);
                return default(T);
            }
        }
    }
}
