using Microsoft.Extensions.Caching.Distributed;

namespace DevHub.Utilities;

public static class CompleteIdempotent
{
    extension(IDistributedCache _distributedCache)
    {
        public async Task CompleteIdempotentItemAsync(string idempotencyKey)
        {

            var value = await _distributedCache.GetStringAsync(idempotencyKey!);

            var completedStatusIdempotentObject = value!.CompleteIdempontentString();

            await _distributedCache.SetStringAsync(idempotencyKey!, completedStatusIdempotentObject);

        }
    }
}