using FlashGroup.WordCensorship.API.DTOs;
using FlashGroup.WordCensorship.Data;

namespace FlashGroup.WordCensorship.Domain
{
    public interface ISensitiveWordService
    {
        Task<IReadOnlyList<SensitiveWord>> GetAll();
        Task<bool> AddSensitiveWord(string newSensitiveWord);
        Task<bool> UpdateSensitiveWord(SensitiveWordUpdateRequest sensitiveWordRequest);
        Task<bool> RemoveSensitiveWord(SensitiveWordRequest sensitiveWordRequest);
    }
}
