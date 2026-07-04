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
    }
}