using FlashGroup.WordCensorship.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlashGroup.WordCensorship.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SanitizeController : ControllerBase
    {
        private readonly ILogger<SanitizeController> _logger;
        private readonly ISanitizeService _sanitizeService;

        public SanitizeController(ILogger<SanitizeController> logger, ISanitizeService sanitizeService)
        { 
            _logger = logger;
            _sanitizeService = sanitizeService;
        }

        /// <summary>
        /// Sanitizes the provided phrase by replacing any sensitive words with their corresponding sanitized versions.
        /// </summary>
        /// <remarks>The string phrase will be broken down and each word compaired against a stored list of sensitive words.
        /// If the phrase contains any of the stored sensitive words, the offending word is replaced in the phrase. Once all offending words are replaced, the sanitized phrase is returned.</remarks>
        /// <param name="phrase">The string value of the phrase needing to be sanitized.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the phrase sanitization process. 
        /// Returns <see cref="BadRequestResult"/> if phrase string value is null, empty or spaces,
        /// <see cref="OkObjectResult"/> if the sanitization of the phrase string is successful, 
        /// <see cref="StatusCodeResult"/> with status 500 if an error occurs.</returns>
        [HttpPost("{phrase}")]
        public async Task<IActionResult> Sanitize(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return BadRequest("Phrase to sanitize cannot be empty.");

            try
            {
                var sanitizedPhrase = await _sanitizeService.SanitizePhrase(new SanitizePhase { Phrase = phrase } );

                return Ok(sanitizedPhrase.Phrase);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sanitizing phrase.");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
}
