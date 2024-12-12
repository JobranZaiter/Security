using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "User Name is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string PasswordHash { get; set; }

    public string PasswordSalt { get; set; }

    [Range(13, 99)]
    public int? Age { get; set; }

    public ICollection<UserTask> UserTasks { get; set; }

    public ICollection<Log> Logs { get; set; }
    public ICollection<FilePermission> Permissions { get; set; }
    public ICollection<FileUpload> Uploads { get; set; }

    [Required]
    [RegularExpression("^(User|Admin|It|Owner)$", ErrorMessage = "Invalid Role")]
    public string UserRole { get; set; } = "User";
}
