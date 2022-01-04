using Polly;
using Polly.Retry;
using Serilog;

namespace API.ResiliencePolicies;

public static class Retry
{

    public static RetryPolicy GetPolicy()
    {
        return Policy.Handle<AggregateException>().WaitAndRetry(3, s => TimeSpan.FromSeconds(2), (_, _) =>
        {
            Log.Information("Attempt failed, retrying...");
        });
    }

}