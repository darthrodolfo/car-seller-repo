namespace AutoVenda.Domain.Catalogue;

/// <summary>
/// Contrato de persistência do agregado Listing (Catalogue context).
/// </summary>
public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Listing> AddAsync(Listing entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Listing entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Listing entity, CancellationToken cancellationToken = default);
}
