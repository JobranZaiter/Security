using System.IdentityModel.Tokens.Jwt;


public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionService(IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor)
    {
        _permissionRepository = permissionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentUserId()
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("User is not authorized.");

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var idClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "sub");

        return idClaim != null ? int.Parse(idClaim.Value) : 0;
    }

    public void SaveFilePermission(FilePermission filePermission){
        _permissionRepository.AddPermission(filePermission);
    }

    public FilePermission GetUserFilePermissions(int fileId)
    {
        var userId = GetCurrentUserId();
        return _permissionRepository.GetFilePermissionByFileAndUserId(userId, fileId);
    }

    public bool CanUpdateFile(int fileId)
    {
        var permission = GetUserFilePermissions(fileId);
        return permission != null && permission.CanUpdate;
    }

    public bool CanReadFile(int fileId)
    {
        var permission = GetUserFilePermissions(fileId);
        return permission != null && permission.CanRead;
    }

    public bool CanDeleteFile(int fileId)
    {
        var permission = GetUserFilePermissions(fileId);
        return permission != null && permission.CanDelete;
    }
}
