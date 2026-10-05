using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;

namespace TaskApi.Data;

/// <summary>
/// Represents the Entity Framework Core database context.
/// </summary>
public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
    ) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Id)
                .ValueGeneratedNever();

            entity.Property(task => task.UserId)
                .IsRequired();

            entity.HasIndex(task => task.UserId);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(task => task.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(task => task.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(task => task.Description)
                .HasMaxLength(2000);

            entity.Property(task => task.IsCompleted)
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .IsRequired();

            entity.Property(task => task.UpdatedAt);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.Id)
                .ValueGeneratedNever();

            entity.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.Property(user => user.PasswordHash)
                .IsRequired();

            entity.Property(user => user.CreatedAt)
                .IsRequired();

            entity.Property(user => user.RefreshTokenHash)
                .HasMaxLength(128);

            entity.Property(user => user.Role)
                .IsRequired()
                .HasMaxLength(20);
        });
    }
}