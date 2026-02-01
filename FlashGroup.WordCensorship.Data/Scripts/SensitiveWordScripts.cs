namespace FlashGroup.WordCensorship.Data
{
    internal static class SensitiveWordScripts
    {
        /// <summary>
        /// Select script to return ID and Word from SensitiveWords table
        /// </summary>
        public const string GetAllSensitiveWords =
@"
SELECT Id, Word
FROM SensitiveWords WITH (NOLOCK)
";

        /// <summary>
        /// Insert script to add a new sensitive word to SensitiveWords table
        /// </summary>
        public const string InsertSensitiveWord =
@"
INSERT INTO SensitiveWords (Word) values (@Word)
";

        /// <summary>
        /// Update script to modify an existing sensitive word in SensitiveWords table by the ID
        /// </summary>
        public const string UpdateSensitiveWord =
@"
UPDATE SensitiveWords SET Word = @Word WHERE Id = @Id
";

        /// <summary>
        /// Delete script to remove a sensitive word from SensitiveWords table by the ID
        /// </summary>
        public const string DeleteSensitiveWord =
@"
DELETE FROM SensitiveWords WHERE Id = @Id
";
    }
}
