using HomeLibrary.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Worker.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("library");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();

            entity.Property(x => x.Author)
                .HasColumnName("author")
                .IsRequired();

            entity.Property(x => x.Genre)
                .HasColumnName("genre")
                .IsRequired();

            entity.Property(x => x.ImportDate)
                .HasColumnName("import_date")
                .IsRequired();
        });
    }
}