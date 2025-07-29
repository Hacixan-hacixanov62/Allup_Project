using Microsoft.AspNetCore.OutputCaching;

namespace Allup_Service.Service
{
    public class CacheClearService
    {
        private readonly IOutputCacheStore _outputCacheStore;

        public CacheClearService(IOutputCacheStore outputCacheStore)
        {
            _outputCacheStore = outputCacheStore;
        }

        public async Task ClearCacheAsync()
        {
            await _outputCacheStore.EvictByTagAsync("Tag", default);
        }
    }
}
