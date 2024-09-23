using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using static System.IO.Path;

namespace NET8.Demo.GlobalAdmin.Controllers.Api;

[Route("api/files")]
public class FileController : ApiControllerBase
{
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly IService<FileUpload> _fileUploadService;

    public FileController(
        IFileService fileService,
        IMapper mapper,
        IService<FileUpload> fileUploadService)
    {
        _fileService = fileService;
        _mapper = mapper;
        _fileUploadService = fileUploadService;
    }

    [HttpGet("{id}")]
    public async ValueTask<ActionResult<FileUploadResponse>> GetById(Guid id) => Ok(_mapper.Map<FileUploadResponse>(await _fileUploadService.GetByIdAsync(id)));

    [HttpPost("get-by-ids")]
    public async ValueTask<ActionResult<IEnumerable<FileUploadResponse>>> GetByIds([FromBody] ICollection<Guid> ids) => Ok(_mapper.Map<IEnumerable<FileUploadResponse>>(await _fileUploadService.GetListAsync(x => ids.Contains(x.Id))));

    [HttpPost("upload")]
    public async ValueTask<ActionResult<FileUploadResponse>> Upload(IFormFile file) => Ok(await _fileService.UploadAsync(file));

    [HttpPost("uploads")]
    public async ValueTask<ActionResult<FileUploadResponse>> Uploads(ICollection<IFormFile> files) => Ok(await _fileService.UploadAsync(files));

    [HttpPost("download")]
    public async ValueTask<IActionResult> Download([FromBody] FileDownloadRequest request)
      => File((await _fileService.DownloadAsync(request.FileUrl)).ToArray(), "application/octet-stream", request.FileName ?? GetFileName(request.FileUrl));

    [HttpDelete("delete/{id}")]
    public async ValueTask<ActionResult<bool>> DeleteWithResouce(Guid id) => Ok(await _fileService.DeleteWithResouceAsync(id));

    [HttpDelete("delete-by-ids")]
    public async ValueTask<ActionResult<bool>> DeleteWithResouces([FromBody] ICollection<Guid> ids) => Ok(await _fileService.DeleteWithResouceAsync(ids));
}