using Microsoft.Extensions.Caching.Distributed;
using mvp.tickets.domain.Models;

namespace mvp.tickets.web.Helpers
{
    public static class CacheHelper
    {
        private static string _keysVersion = "1";
        private static string _ticketsRegion = $"{_keysVersion}:tickets:";

        private static string GetTicketsKeysCacheKey(int companyId, int? userId)
        {
            return $"{_ticketsRegion}{companyId}_{userId}";
        }

        private static string GetTicketsCacheKey(int companyId, int? userId, string options)
        {
            return $"{_ticketsRegion}{companyId}_{userId}_{options}";
        }


        public class CacheData<T>
        {
            public T Data { get; set; }
        }

        public static async Task<CacheData<BaseReportQueryResponse<List<TicketModel>>>> GetTicketsReport(this IDistributedCache cache, ILogger logger,
            int companyId, int? userId, string options)
        {
            var keysCacheKey = GetTicketsKeysCacheKey(companyId, userId);
            var keysCacheData = await cache.GetValue<Dictionary<string, DateTimeOffset>>(logger, keysCacheKey);
            if (keysCacheData?.Data == null)
            {
                return null;
            }
            var keys = keysCacheData.Data.Where(s => s.Value > DateTimeOffset.UtcNow.AddMinutes(-5)).ToDictionary(k => k.Key, v => v.Value);
            var cacheKey = GetTicketsCacheKey(companyId, userId, options);
            if (keys.ContainsKey(cacheKey))
            {
                return await cache.GetValue<BaseReportQueryResponse<List<TicketModel>>>(logger, cacheKey);
            }

            return null;
        }

        public static async Task SetTicketsReport(this IDistributedCache cache, ILogger logger, int companyId, int? userId, string options,
            IBaseReportQueryResponse<IEnumerable<ITicketModel>> report)
        {
            var keysCacheKey = GetTicketsKeysCacheKey(companyId, userId);
            var keysCacheData = await cache.GetValue<Dictionary<string, DateTimeOffset>>(logger, keysCacheKey);
            var keys = keysCacheData?.Data?.Where(s => s.Value > DateTimeOffset.UtcNow.AddMinutes(-5)).ToDictionary(k => k.Key, v => v.Value)
                ?? new Dictionary<string, DateTimeOffset>();
            var cacheKey = GetTicketsCacheKey(companyId, userId, options);
            keys[cacheKey] = DateTimeOffset.UtcNow;
            await cache.SetValue(logger, cacheKey, report, TimeSpan.FromMinutes(5));
            await cache.SetValue(logger, keysCacheKey, keys, TimeSpan.FromMinutes(5));
        }

        public static void ClearTicketsReport(this IDistributedCache cache, ILogger logger, int companyId, int? userId)
        {
            var keysCacheKey = GetTicketsKeysCacheKey(companyId, userId);
            cache.DeleteValue(logger, keysCacheKey);
        }

        public static async Task SetValue<T>(this IDistributedCache cache, ILogger logger, string key, T value, TimeSpan? ttl = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions { SlidingExpiration = ttl };
                var str = System.Text.Json.JsonSerializer.Serialize(new CacheData<T> { Data = value });
                await cache.SetStringAsync(key, str, options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public static async Task<CacheData<T>> GetValue<T>(this IDistributedCache cache, ILogger logger, string key)
        {
            try
            {
                var str = await cache.GetStringAsync(key);
                if (str == null)
                {
                    return null;
                }
                var value = System.Text.Json.JsonSerializer.Deserialize<CacheData<T>>(str);
                return value;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return null;
        }

        public static void DeleteValue(this IDistributedCache cache, ILogger logger, string key)
        {
            try
            {
                cache.Remove(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
