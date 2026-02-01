namespace FlashGroup.WordCensorship.Domain
{
    public interface ISanitizeService
    {
        Task<SanitizePhase> SanitizePhrase(SanitizePhase sanitizeRequest);
    }
}
