using AutoVenda.Domain.Catalogue;
using AutoVenda.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoVenda.Infrastructure.Catalogue;

/// <summary>
/// Implementação do repositório de Listing usando EF Core.
/// </summary>
public class ListingRepository : IListingRepository
{
    private readonly AutoVendaDbContext _db;

    public ListingRepository(AutoVendaDbContext db)
    {
        _db = db;
    }

    public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Listings.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Listings.AsNoTracking().OrderBy(l => l.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<Listing> AddAsync(Listing entity, CancellationToken cancellationToken = default)
    {
        _db.Listings.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Listing entity, CancellationToken cancellationToken = default)
    {
        _db.Listings.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Listing entity, CancellationToken cancellationToken = default)
    {
        _db.Listings.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
