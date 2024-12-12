using System.Collections.Generic;

public interface IFileRepository
{
    FileUpload GetFileById(int id);
    FileUpload GetFileByName(string name);
    IEnumerable<FileUpload> GetAllFiles();
    void AddFile(FileUpload file);
    void UpdateFile(FileUpload file);
    void DeleteFile(FileUpload file);
}
