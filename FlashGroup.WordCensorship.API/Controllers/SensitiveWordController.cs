using FlashGroup.WordCensorship.API.DTOs;
using FlashGroup.WordCensorship.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlashGroup.WordCensorship.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensitiveWordController : ControllerBase
    {
        private readonly ILogger<SensitiveWordController> _logger;
        private readonly ISensitiveWordService _sensitiveWordService;
        public SensitiveWordController(ILogger<SensitiveWordController> logger, ISensitiveWordService sensitiveWordService)
        { 
            _logger = logger;
            _sensitiveWordService = sensitiveWordService;
        }

        /// <summary>
        /// Handles HTTP GET requests to retrieve the collection of sensitive words.    
        /// </summary>
        /// <remarks>This endpoint returns all sensitive words managed by the service. If no sensitive
        /// words are available, a Not Found response is returned. In case of an internal error, a generic error message
        /// is provided in the response. 
        /// </remarks>
        /// <returns>An <see cref="IActionResult"/> containing the list of sensitive words if found; otherwise, a 404 Not Found
        /// response if no sensitive words exist, or a 500 Internal Server Error response if an unexpected error occurs.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var sensitiveWords = await _sensitiveWordService.GetAll();
                if (sensitiveWords == null)
                    return NotFound("No sensitive words found.");

                _logger.LogDebug("Fetched {Count} sensitive words.{sensitiveWords}", sensitiveWords.Count(), sensitiveWords.ToArray());

                return Ok(sensitiveWords);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching sensitive words.");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new sensitive word entry if it does not already exist.
        /// </summary>
        /// <remarks>This action checks for duplicate sensitive words using a case-insensitive comparison.
        /// If the word already exists, the request will be rejected with a conflict response. Errors during creation
        /// are logged and result in a generic server error response.</remarks>
        /// <param name="sensitiveWordToCreate">The sensitive word to add. Cannot be null, empty, or consist only of whitespace.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see
        /// cref="BadRequestResult"/> if the input is invalid, <see cref="ConflictResult"/> if the word already exists,
        /// <see cref="CreatedAtActionResult"/> if creation succeeds, or <see cref="StatusCodeResult"/> with status 500
        /// if an error occurs.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(string sensitiveWordToCreate)
        {
            if (string.IsNullOrWhiteSpace(sensitiveWordToCreate))
                return BadRequest("Sensitive word cannot be empty.");

            try
            {
                if (await _sensitiveWordService.AddSensitiveWord(sensitiveWordToCreate) == false)
                {
                    _logger.LogError("Failed to create sensitive word '{sensitiveWordToCreate}'", sensitiveWordToCreate);
                    throw new Exception("Failed to create sensitive word.");
                }

                return Ok($"Sensitive word '{sensitiveWordToCreate}' added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating sensitive word.");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }


        /// <summary>
        /// Updates an existing sensitive word with new values provided in the request.
        /// </summary>
        /// <remarks>This method validates the input and ensures that the new sensitive word does not
        /// already exist before performing the update. If the update fails, an error is logged and an internal server
        /// error response is returned.</remarks>
        /// <param name="sensitiveWordUpdateRequest">The request object containing the details of the sensitive word to update, including the original word and
        /// the new word. Cannot be null, and both 'FromWord' and 'ToWord' must be non-empty strings.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation. Returns <see
        /// cref="BadRequestResult"/> if required fields are missing, <see cref="ConflictResult"/> if the new word
        /// already exists, <see cref="CreatedAtActionResult"/> if the update is successful, or <see
        /// cref="StatusCodeResult"/> with status 500 if an error occurs.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SensitiveWordUpdateRequest sensitiveWordUpdateRequest)
        {
            if (string.IsNullOrWhiteSpace(sensitiveWordUpdateRequest.FromWord) || string.IsNullOrWhiteSpace(sensitiveWordUpdateRequest.ToWord))
                return BadRequest("Sensitive word fromWord and toWord cannot be empty.");

            try
            {
                if (await _sensitiveWordService.UpdateSensitiveWord(sensitiveWordUpdateRequest) == false)
                {
                    _logger.LogError("Failed to update sensitive word from '{fromWord}' to '{toWord}'", sensitiveWordUpdateRequest.FromWord, sensitiveWordUpdateRequest.ToWord);
                    throw new Exception("Failed to update sensitive word.");
                }

                return Ok($"Sensitive word was updated successfully from '{sensitiveWordUpdateRequest.FromWord}' to '{sensitiveWordUpdateRequest.ToWord}.'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating sensitive word.");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a sensitive word from the system based on the specified request.
        /// </summary>
        /// <remarks>This method requires both the ID and the word to be provided and valid in the
        /// request. The operation is performed asynchronously. If the sensitive word does not exist, the method returns
        /// a NotFound result. If deletion succeeds, a CreatedAtAction result is returned for consistency with RESTful
        /// conventions.</remarks>
        /// <param name="sensitiveWordRequest">The request containing the sensitive word to delete. Must not be null, and must specify a valid ID greater
        /// than zero and a non-empty word.</param>
        /// <returns>An IActionResult indicating the result of the delete operation. Returns BadRequest if the request is
        /// invalid, NotFound if the sensitive word does not exist, CreatedAtAction if the deletion is successful, or an
        /// error response if an exception occurs.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] SensitiveWordRequest sensitiveWordRequest)
        {
            if (sensitiveWordRequest == null || (sensitiveWordRequest.ID <= 0 && string.IsNullOrWhiteSpace(sensitiveWordRequest.Word)))
                return BadRequest("Sensitive Word or ID is required to delete and cannot be empty.");

            try
            {                
                if (await _sensitiveWordService.RemoveSensitiveWord(sensitiveWordRequest) == false)
                    throw new Exception("Failed to delete sensitive word.");

                return Ok($"Successfully deleted sensitive word '{sensitiveWordRequest.Word}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating sensitive word.");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
}
