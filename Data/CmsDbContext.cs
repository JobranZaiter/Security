using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class CmsDbContext : DbContext
{

    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }
    public DbSet<FileUpload> FileUploads { get; set; }
    public DbSet<FileModification> FileModifications { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<FilePermission> FilePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilePermission>()
        .HasOne(fp => fp.File)
        .WithMany(f => f.Permissions)
        .HasForeignKey(fp => fp.FileId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FilePermission>()
        .HasOne(fp => fp.User)
        .WithMany(u => u.Permissions)
        .HasForeignKey(fp => fp.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserTask>()
        .HasOne(t => t.User)
        .WithMany(u => u.UserTasks)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileUpload>()
        .HasOne(f => f.User)
        .WithMany(u => u.Uploads)
        .HasForeignKey(f => f.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileModification>()
        .HasOne(m => m.FileUpload)
        .WithMany(f => f.FileModifications)
        .HasForeignKey(m => m.FileId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Log>()
        .HasOne(l => l.User)
        .WithMany(U => U.Logs)
        .HasForeignKey(U => U.UserId)
        .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}