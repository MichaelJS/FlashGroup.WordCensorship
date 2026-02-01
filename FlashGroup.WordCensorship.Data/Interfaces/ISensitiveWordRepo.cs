namespace FlashGroup.WordCensorship.Data
{
    public interface ISensitiveWordRepo
    {
        Task<IReadOnlyList<SensitiveWord>> GetAll(bool forceUpdateFromDB = false);
        Task<bool> Create(string sensitiveWord);
        Task<bool> Delete(SensitiveWord entity);
        Task<bool> Update(SensitiveWord entity);
    }
}
