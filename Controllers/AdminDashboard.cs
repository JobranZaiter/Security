using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class AdminDashboardController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IPermissionService _permissionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MalwareDetector _malwareDetector;
    private readonly ILogService _logService;
    private readonly JwtValidationService _jwtValidationService;

    public AdminDashboardController(MalwareDetector malwareDetector, IUserRepository userRepository, IFileRepository fileRepository, IPermissionService permissionService, IHttpContextAccessor httpContextAccessor, ILogService logService, JwtValidationService jwtValidationService)
    {
        _userRepository = userRepository;
        _fileRepository = fileRepository;
        _permissionService = permissionService;
        _httpContextAccessor = httpContextAccessor;
        _malwareDetector = malwareDetector;
        _logService = logService;
        _jwtValidationService = jwtValidationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }
        var principal = _jwtValidationService.ValidateToken(token);
        if(principal==null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if(!permissed){
            return Unauthorized();
        }

        var files = _fileRepository.GetAllFiles();
        var filePermissions = files.Select(file => new
        {
            File = file,
            CanView = _permissionService.CanReadFile(file.Id),
            CanUpdate = _permissionService.CanUpdateFile(file.Id),
            CanDelete = _permissionService.CanDeleteFile(file.Id)
        }).ToList();

        ViewBag.Files = filePermissions;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file, string fileName)
    {
        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if(principal==null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if(!permissed){
            return Unauthorized();
        }

        if (file != null && file.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                bool isMalicious = await _malwareDetector.ScanFileForMalware(memoryStream.ToArray());
                if (isMalicious)
                {
                    TempData["ErrorMessage"] = "The file contains malware and cannot be uploaded!";
                    return RedirectToAction("Index");
                }

                var userId = _jwtValidationService.GetIdFromToken();

                var newFile = new FileUpload
                {
                    FileName = fileName,
                    FileSize = file.Length,
                    FileType = Path.GetExtension(file.FileName),
                    FileContent = memoryStream.ToArray(),
                    UploadedDate = DateTime.Now,
                    UserId = userId.Value
                };

                _fileRepository.AddFile(newFile);
                CreateFilePermissions(newFile);

                _logService.LogFileAction(newFile.UserId, newFile.FileName, "Uploaded");
            }

            TempData["SuccessMessage"] = "File uploaded successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "No file selected or file is empty!";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DownloadFile(int fileId)
    {
        var file = _fileRepository.GetFileById(fileId);
        if (file == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }
        var principal = _jwtValidationService.ValidateToken(token);
        if(principal==null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if(!permissed){
            return Unauthorized();
        }

        var userId = _jwtValidationService.GetIdFromToken();

        var permission = _permissionService.GetUserFilePermissions(fileId);
        if (permission == null || !permission.CanRead)
        {
            return Unauthorized();
        }

        _logService.LogFileAction(userId.Value, file.FileName, "Downloaded");

        return File(file.FileContent, "application/octet-stream", file.FileName + file.FileType);
    }

    [HttpPost]
    public IActionResult UpdateFileName(int fileId, string newFileName)
    {
        var file = _fileRepository.GetFileById(fileId);
        if (file == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if(principal==null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if(!permissed){
            return Unauthorized();
        }

        var permission = _permissionService.GetUserFilePermissions(fileId);
        if (permission == null || !permission.CanUpdate)
        {
            return Unauthorized();
        }
        var userId = _jwtValidationService.GetIdFromToken();
        if(userId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        file.FileName = newFileName;
        _fileRepository.UpdateFile(file);

        _logService.LogFileAction(userId.Value, file.FileName, "Updated Name");

        TempData["SuccessMessage"] = "File name updated successfully!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteFile(int fileId)
    {
        var file = _fileRepository.GetFileById(fileId);
        if (file == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        var userId = _jwtValidationService.GetIdFromToken();

        var permission = _permissionService.GetUserFilePermissions(fileId);
        if (permission == null || !permission.CanDelete)
        {
            return Unauthorized();
        }

        _fileRepository.DeleteFile(file);

        _logService.LogFileAction(userId.Value, file.FileName, "Deleted");

        TempData["SuccessMessage"] = "File deleted successfully!";
        return RedirectToAction("Index");
    }

    private void CreateFilePermissions(FileUpload file)
    {
        var userId = file.UserId;

        var permissions = new FilePermission
        {
            FileId = file.Id,
            UserId = userId,
            CanRead = true,
            CanUpdate = true,
            CanDelete = true
        };
        _permissionService.SaveFilePermission(permissions);

        var ownerUsers = _userRepository.GetUsersByRole("Owner");
        foreach (var ownerUser in ownerUsers)
        {
            var ownerPermissions = new FilePermission
            {
                FileId = file.Id,
                UserId = ownerUser.Id,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true
            };
            _permissionService.SaveFilePermission(ownerPermissions);
        }
    }
}
