using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;
using NET8.Demo.Shared;
using static Newtonsoft.Json.JsonConvert;
using static System.DateTime;
using static System.IO.Path;

namespace NET8.Demo.GlobalAdmin.Application.Services;

public class FileService : ServiceBase, IFileService
{
    private long orderCounter = 0;
    private readonly string _storagePath;
    private readonly IMapper _mapper;
    private readonly IRepository<FileUpload> _fileUploadRepository;

    public FileService(
        IRepository<FileUpload> fileUploadRepository,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<FileService> logger,
        IStringLocalizer<FileService> localizer) : base(unitOfWork, httpContextAccessor, logger, localizer)
    {
        _fileUploadRepository = fileUploadRepository;
        _mapper = mapper;
        _storagePath = Combine(Directory.GetCurrentDirectory(), SharedConstant.FOLDER_ROOT, SharedConstant.FOLDER_UPLOAD, UtcNow.Year.ToString(), UtcNow.Month.ToString());
    }

    public async ValueTask<FileUploadResponse> UploadAsync(IFormFile file)
    {
        if (file is null || file.Length is 0)
        {
            Logger.LogError("FileUploaderService-UploadAsync-Error: No file uploaded");
            throw new BusinessException(ErrorCode.NoContent, Localizer["File.NoFileUpload"]);
        }

        try
        {
            var filePath = Combine(_storagePath, GenerateUniqueKey(file.FileName));
            await SaveFileAsync(filePath, file);
            var resource = new FileUpload
            {
                FileName = file.FileName,
                FileUrl = filePath,
                FileSize = file.Length,
                FileExtension = GetExtension(file.FileName)
            };
            await _fileUploadRepository.InsertAsync(UserId, resource);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<FileUploadResponse>(resource);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-UploadAsync-Exception");
            throw;
        }
    }

    public async ValueTask<ICollection<FileUploadResponse>> UploadAsync(ICollection<IFormFile> files)
    {
        if (files is null || files.Count is 0)
        {
            Logger.LogError("FileUploaderService-UploadAsync-Error: No file uploaded");
            throw new BusinessException(ErrorCode.NoContent, Localizer["File.NoFileUpload"]);
        }
        try
        {
            var resources = new List<FileUpload>();

            foreach (var file in files)
            {
                var fileUrl = Combine(_storagePath, GenerateUniqueKey(file.FileName));
                await SaveFileAsync(fileUrl, file);
                resources.Add(new FileUpload
                {
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                    FileSize = file.Length,
                    FileExtension = GetExtension(file.FileName)
                });
            }

            await _fileUploadRepository.InsertAsync(UserId, resources);
            await UnitOfWork.SaveChangesAsync();
            return _mapper.Map<List<FileUploadResponse>>(resources);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-UploadAsync-Exception");
            throw;
        }
    }

    public async ValueTask<MemoryStream> DownloadAsync(string fileUrl)
    {
        try
        {
            if (!File.Exists(fileUrl))
            {
                throw new BusinessException(ErrorCode.NoContent, Localizer["File.NoFileUpload"]);
            }

            await using var sourceStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileStream = new MemoryStream(await File.ReadAllBytesAsync(fileUrl));
            return fileStream;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-DownloadAsync-Exception: {filePath}", fileUrl);
            throw;
        }
    }

    public void Delete(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new BusinessException(ErrorCode.NoContent, Localizer["File.InvalidUrl"]);
        }

        try
        {
            if (!File.Exists(fileUrl))
            {
                throw new BusinessException(ErrorCode.NoContent, Localizer["File.FileNotFound"]);
            }

            File.Delete(fileUrl);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-Delete-Exception: {fileUrl}", fileUrl);
            throw;
        }
    }

    public void Delete(ICollection<string> fileUrls, bool ignoreMissingFiles = false)
    {
        if (fileUrls is null || fileUrls.Count == 0)
        {
            throw new BusinessException(ErrorCode.NoContent, Localizer["File.InvalidUrl"]);
        }

        try
        {
            if (!ignoreMissingFiles && !fileUrls.All(File.Exists))
            {
                throw new BusinessException(ErrorCode.NoContent, Localizer["File.FileNotFound"]).WithData("fileUrls", fileUrls);
            }
            else if (ignoreMissingFiles)
            {
                fileUrls = fileUrls.Where(File.Exists).ToList();
            }

            foreach (var fileUrl in fileUrls)
            {
                File.Delete(fileUrl);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-Delete-Exception: {fileUrl}", SerializeObject(fileUrls));
            throw;
        }
    }

    public async ValueTask<bool> DeleteWithResouceAsync(Guid fileId)
    {
        try
        {
            var file = await _fileUploadRepository.GetByIdAsync(fileId) ?? throw new BusinessException(ErrorCode.NoContent, Localizer["File.FileNotFound"]).WithData("fileId", fileId);
            _fileUploadRepository.Delete(file);
            var result = await UnitOfWork.SaveChangesAsync() > 0;

            if (result && !string.IsNullOrWhiteSpace(file.FileUrl) && File.Exists(file.FileUrl))
            {
                File.Delete(file.FileUrl);
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-DeleteWithResouceAsync-Exception: {fileId}", fileId);
            throw;
        }
    }

    public async ValueTask<bool> DeleteWithResouceAsync(ICollection<Guid> fileIds)
    {
        try
        {
            var files = await _fileUploadRepository.GetListAsync(x => fileIds.Contains(x.Id)) ?? throw new BusinessException(ErrorCode.NoContent, Localizer["File.FileNotFound"]).WithData("fileIds", SerializeObject(fileIds));
            _fileUploadRepository.Delete(files.ToArray());
            var result = await UnitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                files.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.FileUrl) && File.Exists(x.FileUrl))
                    {
                        File.Delete(x.FileUrl);
                    }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FileUploaderService-DeleteWithResouceAsync-Exception: {fileIds}", SerializeObject(fileIds));
            throw;
        }
    }

    #region Priviate method
    private string GenerateUniqueKey(string fileName)
    {
        long currentCounter = Interlocked.Increment(ref orderCounter);
        return $"{GetFileNameWithoutExtension(fileName)}_{UtcNow.Ticks}{currentCounter}{GetExtension(fileName)}";
    }

    private static async ValueTask SaveFileAsync(string fileUrl, IFormFile file)
    {
        var directory = GetDirectoryName(fileUrl);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = new FileStream(fileUrl, FileMode.Create);
        await file.CopyToAsync(stream);
    }
    #endregion
}
