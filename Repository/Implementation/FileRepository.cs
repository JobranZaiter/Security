using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class FileRepository : IFileRepository
{
    private readonly CmsDbContext context;

    public FileRepository(CmsDbContext _context)
    {
        context = _context;
    }

    // Add a new file record
    public void AddFile(FileUpload file)
    {
        context.FileUploads.Add(file);
        context.SaveChanges();
    }

    // Delete a file record
    public void DeleteFile(FileUpload file)
    {
        context.FileUploads.Remove(file);
        context.SaveChanges();
    }

    public IEnumerable<FileUpload> GetAllFiles()
    {
        return context.FileUploads
                .Include(f => f.User) 
                .Include(f => f.Permissions)
                .Include(f => f.FileModifications) 
                .ToList();
    }

    public FileUpload GetFileById(int id)
    {
        return context.FileUploads
                .Where(f => f.Id == id)
                .Include(f => f.User)
                .Include(f => f.Permissions)
                .Include(f => f.FileModifications)
                .FirstOrDefault();
    }

   
    public FileUpload GetFileByName(string name)
    {
        return context.FileUploads
                .Where(f => f.FileName == name)
                .Include(f => f.User) 
                .Include(f => f.Permissions)    
                .Include(f => f.FileModifications)
                .FirstOrDefault();
    }

    // Update a file record
    public void UpdateFile(FileUpload file)
    {
        context.FileUploads.Update(file);
        context.SaveChanges();
    }
}
