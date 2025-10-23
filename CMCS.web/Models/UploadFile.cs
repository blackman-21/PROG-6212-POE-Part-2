namespace CMCS.Web.Models
{
    public class UploadFile
    {
        public int Id { get; set; }

        // The file name used on disk (GUID + extension)
        public string FileName { get; set; }

        // The original file name as uploaded by user
        public string OriginalFileName { get; set; }

        public long Size { get; set; }

        public int ClaimId { get; set; }
        public Claim Claim { get; set; }
    }
}
