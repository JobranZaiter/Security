using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FilePermission
{

    [Key]
    public int Id { get; set; }
    public int FileId { get; set; }

    [ForeignKey("FileId")]
    public FileUpload File { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanRead { get; set; }
    public bool CanDelete { get; set; }
}