using System.ComponentModel.DataAnnotations;

namespace AutoVenda.MinimalApi.Features.Catalogue.Contracts;

/// <summary>
/// Contrato para atualização de um Anúncio (Listing) existente.
/// </summary>
public record UpdateListingRequest
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; init; }

    [Range(0, double.MaxValue)]
    public decimal PisoDeNegociacao { get; init; }

    [StringLength(100)]
    public string? FipeReference { get; init; }

    [Url]
    [StringLength(2000)]
    public string? CoverImageUrl { get; init; }
}
