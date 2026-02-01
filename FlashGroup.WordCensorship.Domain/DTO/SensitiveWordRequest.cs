namespace FlashGroup.WordCensorship.API.DTOs
{
    public class SensitiveWordRequest
    {
        public int? ID { get; set; }
        public string Word { get; set; } = string.Empty;
    }
}
