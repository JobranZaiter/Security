using System.Collections.Generic;

public interface IPermissionRepository
{
    FilePermission GetPermissionById(int id);
    FilePermission GetPermissionByFileId(int id);

    FilePermission GetFilePermissionByFileAndUserId(int uid, int fid);
    IEnumerable<FilePermission> GetAllPermissions();
    void AddPermission(FilePermission file);
    void UpdatePermission(FilePermission file);
    void DeletePermission(FilePermission file);
}
