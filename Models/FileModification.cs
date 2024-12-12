using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FileModification
{

    [Key]
    public int Id { get; set; }

    [RegularExpression("^(Add|Update|Delete)$", ErrorMessage = "Invalid Role")]
    public string Type { get; set; }

    public int FileId { get; set; }

    [ForeignKey("FileId")]
    public FileUpload FileUpload { get; set; }
}