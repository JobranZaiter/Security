using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Log
{

    [Key]
    public int Id { get; set; }

    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
    public string Entry { get; set; }
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
}