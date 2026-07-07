using HomeLibrary.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();

    public DbSet<ImportHistory> ImportHistories => Set<ImportHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("library");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property(x => x.Author)
                .IsRequired();

            entity.Property(x => x.Genre)
                .IsRequired();

            entity.Property(x => x.ImportDate)
                .IsRequired();
        });

        modelBuilder.Entity<ImportHistory>(entity =>
        {
            entity.ToTable("import_history");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedNever();

            entity.Property(x => x.FileName)
                .IsRequired();

            entity.Property(x => x.FileHash)
                .IsRequired();

            entity.Property(x => x.ImportDate)
                .IsRequired();

            entity.HasIndex(x => x.FileHash)
                .IsUnique();
        });
    }
}