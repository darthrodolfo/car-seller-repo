namespace AutoVenda.Domain.Catalogue;

/// <summary>
/// Agregado raiz do contexto Catalogue. Representa um Anúncio (entrada de veículo) no catálogo.
/// </summary>
public class Listing
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal PisoDeNegociacao { get; private set; }
    public ListingStatus Status { get; private set; }
    public string? FipeReference { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Listing() { }

    /// <summary>
    /// Cria um novo Anúncio. Usado pela fábrica ou pelo repositório ao reconstituir da persistência.
    /// </summary>
    public static Listing Create(
        Guid id,
        string title,
        decimal price,
        decimal pisoDeNegociacao,
        ListingStatus status = ListingStatus.Publicado,
        string? fipeReference = null,
        string? coverImageUrl = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        return new Listing
        {
            Id = id,
            Title = title,
            Price = price,
            PisoDeNegociacao = pisoDeNegociacao,
            Status = status,
            FipeReference = fipeReference,
            CoverImageUrl = coverImageUrl,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            UpdatedAt = updatedAt,
        };
    }

    /// <summary>
    /// Atualiza título, preço, piso e referência FIPE do Anúncio.
    /// </summary>
    public void Update(string title, decimal price, decimal pisoDeNegociacao, string? fipeReference = null)
    {
        Title = title;
        Price = price;
        PisoDeNegociacao = pisoDeNegociacao;
        FipeReference = fipeReference;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Altera a URL da imagem de capa.
    /// </summary>
    public void SetCoverImageUrl(string? url)
    {
        CoverImageUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Altera o status do Anúncio (Publicado, Arquivado, Rascunho).
    /// </summary>
    public void SetStatus(ListingStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
