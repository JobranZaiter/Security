public interface IPermissionService
{
    FilePermission GetUserFilePermissions(int fileId);
    bool CanUpdateFile(int fileId);
    bool CanReadFile(int fileId);
    bool CanDeleteFile(int fileId);
    void SaveFilePermission(FilePermission filePermission);
}
