using AutoVenda.Domain.Catalogue;

namespace AutoVenda.MinimalApi.Features.Catalogue.Contracts;

/// <summary>
/// DTO de resposta para um Anúncio (Listing).
/// </summary>
public record ListingResponse(
    Guid Id,
    string Title,
    decimal Price,
    decimal PisoDeNegociacao,
    ListingStatus Status,
    string? FipeReference,
    string? CoverImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
