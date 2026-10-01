using Microsoft.EntityFrameworkCore;
using TaskApi.Domain;


namespace TaskApi.Data;

public sealed class AppDbContext
(
    DbContextOptions<AppDbContext> options): DbContext(options)
{
    public DbSet<TaskItem> Tasks { get; set; }

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
                .IsRequired();
            entity.Property(task => task.CreatedAt)
                .IsRequired();
            entity.Property(task => task.UpdatedAt)
                .IsRequired();

        });
    }
}


