namespace FlashGroup.WordCensorship.Data
{
    public interface ISensitiveWord
    {
        int ID { get; set; }
        string Word { get; set; }
    }

    public class SensitiveWord : ISensitiveWord
    {
        public int ID { get; set; }
        public string Word { get; set; } = string.Empty;
    }
}
