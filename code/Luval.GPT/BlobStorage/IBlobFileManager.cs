namespace Luval.GPT.BlobStorage
{
    public interface IBlobFileManager
    {
        Task<BlobResult> UploadAsync(Blob blob);
        BlobResult Upload(Blob blob);
    }
}