using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace FlashGroup.WordCensorship.Data
{
    public class SensitiveWordRepo : ISensitiveWordRepo
    {
        private readonly IDBClient _dbClient;
        private readonly IMemoryCache _memCache;
        private const string _cacheKey = "SensitiveWords.All";
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public SensitiveWordRepo(IDBClient dbClient, IMemoryCache memoryCache)
        {
            _dbClient = dbClient;
            _memCache = memoryCache;
        }

        public async Task<IReadOnlyList<SensitiveWord>> GetAll(bool forceUpdateFromDB = false)
        {
            if (forceUpdateFromDB && (_memCache.TryGetValue(_cacheKey, out var cached) && cached is IReadOnlyList<SensitiveWord> cachedSensitiveWordsList))
            {
                return cachedSensitiveWordsList;
            }

            var sensitiveWordsList = new List<SensitiveWord>();
            using var reader = await _dbClient.GetReaderAsync("SELECT Id, Word\r\nFROM SensitiveWords WITH (NOLOCK)");

            while (await reader.ReadAsync())
            {
                sensitiveWordsList.Add(new SensitiveWord
                {
                    ID = reader.GetInt32(0),
                    Word = reader.GetString(1)
                });
            }

            _memCache.Set(_cacheKey
                , sensitiveWordsList
                , new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheDuration
                    });

            return sensitiveWordsList;
        }

        public async Task<bool> Create(string sensitiveWord)
        {
            var results = await _dbClient.Execute(SensitiveWordScripts.InsertSensitiveWord,new SqlParameter("Word", sensitiveWord));
            
            _memCache.Remove(_cacheKey);

            return results > 0;
        }

        public async Task<bool> Delete(SensitiveWord entity)
        {
            var results = await _dbClient.Execute(SensitiveWordScripts.DeleteSensitiveWord, new SqlParameter("Id", entity.ID));
            
            _memCache.Remove(_cacheKey);

            return results > 0;
        }

        public async Task<bool> Update(SensitiveWord entity)
        {
            var results = await _dbClient.Execute(SensitiveWordScripts.UpdateSensitiveWord,
                new SqlParameter("Id", entity.ID),
                new SqlParameter("Word", entity.Word));

            _memCache.Remove(_cacheKey);

            return results > 0;
        }
    }
}
