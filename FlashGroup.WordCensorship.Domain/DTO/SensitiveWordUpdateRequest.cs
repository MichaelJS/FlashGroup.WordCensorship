namespace FlashGroup.WordCensorship.API.DTOs
{
    public class SensitiveWordUpdateRequest
    {
        public int? ID { get; set; }
        public string FromWord { get; set; } = string.Empty;
        public string ToWord { get; set; } = string.Empty;
    }
}
