using Polly;
using Polly.Timeout;
using Serilog;

namespace API.ResiliencePolicies;

public static class Timeout
{
        public static TimeoutPolicy GetPolicy()
        {
            return Policy.Timeout(TimeSpan.FromSeconds(5),
                onTimeout: (_, _, _) => { Log.Information("Operation timed out."); });
        }
}