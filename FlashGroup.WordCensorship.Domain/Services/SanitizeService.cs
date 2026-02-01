using FlashGroup.WordCensorship.Data;
using Microsoft.Extensions.Logging;

namespace FlashGroup.WordCensorship.Domain
{
    public class SanitizeService : ISanitizeService
    {
        private readonly ILogger<SanitizeService> _logger;
        private readonly ISensitiveWordRepo _sensitiveWordRepo;
        private const char _censorChar = '*';

        public SanitizeService(ILogger<SanitizeService> logger, ISensitiveWordRepo sensitiveWordRepo)
        {
            _logger = logger;
            _sensitiveWordRepo = sensitiveWordRepo;
        }

        #region Public Methods
        /// <summary>
        /// Accepts a phrase and censors any sensitive words found within it.
        /// </summary>
        /// <param name="sanitizeRequest">The Phrase is required. The Phrase will be updated with the replacment character for each word found in the sensitive words list.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Exception is raised if the Phrase is null, empty or spaces.</exception>
        public async Task<SanitizePhase> SanitizePhrase(SanitizePhase phraseToSanitize)
        {
            _logger.LogInformation("Sanitizing phrase '{Phrase}'", phraseToSanitize);

            if (phraseToSanitize == null || string.IsNullOrWhiteSpace(phraseToSanitize.Phrase))
                throw new ArgumentNullException("Missing or invalid phrase to sanitize.");   
                        
            var sensitiveWordList = await _sensitiveWordRepo.GetAll();
            if (sensitiveWordList == null || !sensitiveWordList.Any())
            {
                _logger.LogWarning("No sensitive words loaded.");
            }
            else
            {
                ReplaceSensitiveWords(phraseToSanitize, sensitiveWordList);
            }
            
            return phraseToSanitize;
        }
        #endregion

        #region Private Methods
        private void ReplaceSensitiveWords(SanitizePhase sanitizePhase, IReadOnlyList<SensitiveWord> sensitiveWords)
        {
            if (string.IsNullOrEmpty(sanitizePhase.Phrase)) return;

            var lowerPhrase = sanitizePhase.Phrase.ToLower();
            
            foreach (var sw in sensitiveWords)
            {
                var lowerWord = sw.Word.ToLower();
                if (lowerPhrase.Contains(lowerWord))
                {
                    _logger.LogDebug("Found sensitive word '{word}' in phrase.", sw.Word);

                    sanitizePhase.Phrase = sanitizePhase.Phrase.Replace(sw.Word, new string(_censorChar, sw.Word.Length), StringComparison.OrdinalIgnoreCase);
                    lowerPhrase = sanitizePhase.Phrase.ToLower();
                }
            }
        }
        #endregion
    }
}
