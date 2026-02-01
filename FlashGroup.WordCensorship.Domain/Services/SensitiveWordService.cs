using FlashGroup.WordCensorship.API.DTOs;
using FlashGroup.WordCensorship.Data;
using Microsoft.Extensions.Logging;

namespace FlashGroup.WordCensorship.Domain
{
    public class SensitiveWordService : ISensitiveWordService
    {
        private readonly ILogger<SanitizeService> _logger;
        private readonly ISensitiveWordRepo _sensitiveWordRepo;

        public SensitiveWordService(ILogger<SanitizeService> logger, ISensitiveWordRepo sensitiveWordRepo)
        {
            _logger = logger;
            _sensitiveWordRepo = sensitiveWordRepo;
        }

        /// <summary>
        /// Retrieves IReadOnlyList list of sensitive words.
        /// The sensitive words are stored in the database and fetched via the ISensitiveWordRepo repository.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<SensitiveWord>> GetAll()
        {
            _logger.LogInformation("Fetching all sensitive words from the repository.");
            return await _sensitiveWordRepo.GetAll();
        }

        /// <summary>
        /// Adds a new sensitive word to the SensitiveWords list. 
        /// </summary>
        /// <param name="newSensitiveWord">String SensitiveWord is required.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">Exception raised if the newSensitiveWord already exists.</exception>
        public async Task<bool> AddSensitiveWord(string newSensitiveWord)
        {
            var allSensitiveWords = await GetAll();

            var hasExistingSensitiveWord = allSensitiveWords.Any(sw => sw.Word.Equals(newSensitiveWord, StringComparison.OrdinalIgnoreCase));
            if (hasExistingSensitiveWord)
            {
                _logger.LogWarning("The sensitive word '{Word}' already exists in the sensitive words list.", newSensitiveWord);
                throw new InvalidDataException($"Add failed. The word '{newSensitiveWord}' already exists in the sensitive words list.");
            }

            return await _sensitiveWordRepo.Create(newSensitiveWord);
        }

        /// <summary>
        /// Updates an existing sensitive word with a FromWord to a ToWord.
        /// </summary>
        /// <param name="sensitiveWordUpdateRequest">ID is optional. FromWord and ToWord is required. FromWord should be the existing value and ToWord should be the new value.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">Exception raised if the FromWord does not already exist or the ToWord already exists.</exception>
        public async Task<bool> UpdateSensitiveWord(SensitiveWordUpdateRequest sensitiveWordUpdateRequest)
        {
            var allSensitiveWords = await GetAll();

            var existingSensitiveWord = allSensitiveWords.FirstOrDefault(sw => sw.ID == sensitiveWordUpdateRequest.ID || sw.Word.Equals(sensitiveWordUpdateRequest.FromWord, StringComparison.OrdinalIgnoreCase));
            if (existingSensitiveWord == null)
            {
                _logger.LogWarning("The word '{FromWord}' does not exists in the sensitive words list.", sensitiveWordUpdateRequest.FromWord);
                throw new InvalidDataException($"Update failed. The word '{sensitiveWordUpdateRequest.FromWord}' does not exists in the sensitive words list.");
            }

            var isDuplicateWord = allSensitiveWords.Any(sw => sw.Word.Equals(sensitiveWordUpdateRequest.ToWord, StringComparison.OrdinalIgnoreCase));
            if (isDuplicateWord)
            {
                _logger.LogWarning("The word '{ToWord}' already exists in the sensitive words list.", sensitiveWordUpdateRequest.ToWord);
                throw new InvalidDataException($"Update failed. The word '{sensitiveWordUpdateRequest.ToWord}' already exists in the sensitive words list.");
            }

            return await _sensitiveWordRepo.Update(existingSensitiveWord);
        }

        /// <summary>
        /// Removes a sensitive word.
        /// </summary>
        /// <param name="sensitiveWordRequest">ID is optional. Word is required.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">Exception is raised if the ID or SensitiveWord does not exist.</exception>
        public async Task<bool> RemoveSensitiveWord(SensitiveWordRequest sensitiveWordRequest)
        {
            var allSensitiveWords = await GetAll();

            var hasExistingSensitiveWord = allSensitiveWords.Any(sw => sw.ID == sensitiveWordRequest.ID || sw.Word.Equals(sensitiveWordRequest.Word, StringComparison.OrdinalIgnoreCase));
            if (!hasExistingSensitiveWord)
            {
                _logger.LogWarning("The word '{sensitiveWord}' does not exists in the sensitive words list.", sensitiveWordRequest.Word);
                throw new InvalidDataException($"Delete Failed. The word '{sensitiveWordRequest.Word}' does not exists in the sensitive words list.");
            }

            return await _sensitiveWordRepo.Delete(new SensitiveWord
            {
                ID = sensitiveWordRequest.ID ?? 0,
                Word = sensitiveWordRequest.Word
            });
        }
    }
}
