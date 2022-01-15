namespace Files.Service.Models
{
    public record UploadFile
    {
        /// <summary>
        /// The identifier of the file
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The internal filename
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// The original filename
        /// </summary>
        public string? OriginalFileName { get; set; }

        /// <summary>
        /// The upload date of the file
        /// </summary>
        public DateTime UploadDate { get; set; }
    }
}