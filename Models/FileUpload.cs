using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FileUpload
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "File name is required.")]
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
    public string FileName { get; set; }

    [Required(ErrorMessage = "File size is required.")]
    [Range(1, 10485760, ErrorMessage = "File size must be between 1 byte and 10 MB.")]
    public long FileSize { get; set; }

    [Required(ErrorMessage = "File type is required.")]
    public string FileType { get; set; }

    [Required(ErrorMessage = "File content is required.")]
    public byte[] FileContent { get; set; }

    public DateTime UploadedDate { get; set; } = DateTime.Now;
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public ICollection<FileModification> FileModifications { get; set; }
    public ICollection<FilePermission> Permissions { get; set; }
}
