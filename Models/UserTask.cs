using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserTask
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; }

    [RegularExpression("^(Design|Software|HR)$", ErrorMessage = "Invalid Role")]
    public string Category { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters.")]
    public string Slug { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression("^(Idle|Started|Completed)$", ErrorMessage = "Invalid Role")]
    public string Status { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? FinishedDate { get; set; }

}
