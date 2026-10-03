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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("tasks");
                
                entity.HasKey(task => task.Id);
                
                entity.Property(task => task.Id)
                    .ValueGeneratedNever();
                
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
            }
        );

    }
}