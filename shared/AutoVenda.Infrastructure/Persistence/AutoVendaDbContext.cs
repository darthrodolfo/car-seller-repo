using AutoVenda.Domain.Catalogue;
using Microsoft.EntityFrameworkCore;

namespace AutoVenda.Infrastructure.Persistence;

/// <summary>
/// DbContext principal do AutoVenda. Catálogo (Listings) e demais bounded contexts serão mapeados aqui.
/// </summary>
public class AutoVendaDbContext : DbContext
{
    public AutoVendaDbContext(DbContextOptions<AutoVendaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Listing> Listings => Set<Listing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Listing>(entity =>
        {
            entity.ToTable("Listings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.PisoDeNegociacao).HasPrecision(18, 2);
            entity.Property(e => e.Status);
            entity.Property(e => e.FipeReference).HasMaxLength(100);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.UpdatedAt);
        });
    }
}
