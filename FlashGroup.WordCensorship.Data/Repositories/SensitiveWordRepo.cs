using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace FlashGroup.WordCensorship.Data
{
    public class SensitiveWordRepo : ISensitiveWordRepo
    {
        private readonly IDBClient _dbClient;
        private readonly IMemoryCache _memCache;

        // Cache key and duration can be loaded from configuration as needed
        private const string _cacheKey = "SensitiveWords.All";
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public SensitiveWordRepo(IDBClient dbClient, IMemoryCache memoryCache)
        {
            _dbClient = dbClient;
            _memCache = memoryCache;
        }
        /// <summary>
        /// Gets all the sensitive words, with optional cache refresh
        /// </summary>
        /// <param name="forceUpdateFromDB">True: the data will force load from the DB. False: if the cached value exists, it'll load from cache, otherwise load from the DB.</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<SensitiveWord>> GetAll(bool forceUpdateFromDB = false)
        {
            if (forceUpdateFromDB && (_memCache.TryGetValue(_cacheKey, out var cached) && cached is IReadOnlyList<SensitiveWord> cachedSensitiveWordsList))
            {
                return cachedSensitiveWordsList;
            }

            var sensitiveWordsList = new List<SensitiveWord>();
            using var reader = await _dbClient.GetReaderAsync(SensitiveWordScripts.GetAllSensitiveWords);

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

        /// <summary>
        /// Creates a new sensitive word and adds to the database
        /// </summary>
        /// <param name="sensitiveWord">string word to add to the database</param>
        /// <returns></returns>
        public async Task<bool> Create(string sensitiveWord)
        {
            var results = await _dbClient.Execute(SensitiveWordScripts.InsertSensitiveWord,new SqlParameter("Word", sensitiveWord));
            
            _memCache.Remove(_cacheKey);

            return results > 0;
        }

        /// <summary>
        /// Deletes a sensitive word from the database
        /// </summary>
        /// <param name="entity">SensitiveWord entity with the ID and\or Word to be deleted.</param>
        /// <returns></returns>
        public async Task<bool> Delete(SensitiveWord entity)
        {
            var results = await _dbClient.Execute(SensitiveWordScripts.DeleteSensitiveWord, new SqlParameter("Id", entity.ID));
            
            _memCache.Remove(_cacheKey);

            return results > 0;
        }

        /// <summary>
        /// Updates an existing sensitive word in the database
        /// </summary>
        /// <param name="entity">SensitiveWord entity with the ID and\or Word to be updated.</param>
        /// <returns></returns>
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
