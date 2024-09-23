using Microsoft.AspNetCore.Http;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

namespace NET8.Demo.GlobalAdmin.Application.Contracts.IServices;

public interface IFileService
{
    ValueTask<FileUploadResponse> UploadAsync(IFormFile file);

    ValueTask<ICollection<FileUploadResponse>> UploadAsync(ICollection<IFormFile> files);

    ValueTask<MemoryStream> DownloadAsync(string fileUrl);

    void Delete(string fileUrl);

    void Delete(ICollection<string> fileUrls, bool ignoreMissingFiles = false);

    ValueTask<bool> DeleteWithResouceAsync(Guid fileId);

    ValueTask<bool> DeleteWithResouceAsync(ICollection<Guid> fileIds);
}
