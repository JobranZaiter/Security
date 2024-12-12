using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class PermissionRepository : IPermissionRepository
{
    private readonly CmsDbContext _context;

    public PermissionRepository(CmsDbContext context)
    {
        _context = context;
    }
    public FilePermission GetFilePermissionByFileAndUserId(int uid, int fid)
    {
        return _context.FilePermissions
            .Include(fp => fp.File)
            .Include(fp => fp.User)
            .FirstOrDefault(fp => fp.UserId == uid && fp.FileId == fid);
    }

    public FilePermission GetPermissionById(int id)
    {
        return _context.FilePermissions
            .Include(fp => fp.File)
            .Include(fp => fp.User)
            .FirstOrDefault(fp => fp.Id == id);
    }

    public FilePermission GetPermissionByFileId(int id)
    {
        return _context.FilePermissions
            .Include(fp => fp.File)
            .Include(fp => fp.User)
            .FirstOrDefault(fp => fp.File.Id == id);
    }

    public IEnumerable<FilePermission> GetAllPermissions()
    {
        return _context.FilePermissions
            .Include(fp => fp.File)
            .Include(fp => fp.User)
            .ToList();
    }

    public void AddPermission(FilePermission file)
    {
        _context.FilePermissions.Add(file);
        _context.SaveChanges();
    }

    public void UpdatePermission(FilePermission file)
    {
        _context.FilePermissions.Update(file);
        _context.SaveChanges();
    }

    public void DeletePermission(FilePermission file)
    {
        _context.FilePermissions.Remove(file);
        _context.SaveChanges();
    }
}
