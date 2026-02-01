namespace FlashGroup.WordCensorship.API.DTOs
{
    /// <summary>
    /// DTO for retrieving sensitive word information
    /// </summary>
    public class GetSensitiveWord
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Gets or sets the word associated with this instance.
        /// </summary>
        public string Word { get; set; } = string.Empty;
    }
}
