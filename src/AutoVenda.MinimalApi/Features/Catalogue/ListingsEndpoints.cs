using AutoVenda.Domain.Catalogue;
using AutoVenda.MinimalApi.Features.Catalogue.Contracts;
using AutoVenda.MinimalApi.Validation;
using Microsoft.AspNetCore.Mvc;

namespace AutoVenda.MinimalApi.Features.Catalogue;

/// <summary>
/// Mapeamento das rotas CRUD de Listing (Anúncio) — contexto Catalogue.
/// </summary>
public static class ListingsEndpoints
{
    public static IEndpointRouteBuilder MapListingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/listings")
            .WithTags("Listings")
            .WithOpenApi();

        group.MapGet("/", GetAllListings);
        group.MapGet("/{id:guid}", GetListingById);
        group.MapPost("/", CreateListing).ValidateRequest<CreateListingRequest>();
        group.MapPut("/{id:guid}", UpdateListing).ValidateRequest<UpdateListingRequest>();
        group.MapDelete("/{id:guid}", DeleteListing);

        return app;
    }

    private static async Task<IResult> GetAllListings(
        IListingRepository repository,
        CancellationToken cancellationToken)
    {
        var list = await repository.GetAllAsync(cancellationToken);
        var response = list.Select(ToResponse).ToList();
        return Results.Ok(response);
    }

    private static async Task<IResult> GetListingById(
        Guid id,
        IListingRepository repository,
        CancellationToken cancellationToken)
    {
        var listing = await repository.GetByIdAsync(id, cancellationToken);
        return listing is null
            ? Results.NotFound()
            : Results.Ok(ToResponse(listing));
    }

    private static async Task<IResult> CreateListing(
        [FromBody] CreateListingRequest request,
        IListingRepository repository,
        CancellationToken cancellationToken)
    {
        var listing = Listing.Create(
            id: Guid.NewGuid(),
            title: request.Title,
            price: request.Price,
            pisoDeNegociacao: request.PisoDeNegociacao,
            status: ListingStatus.Publicado,
            fipeReference: request.FipeReference,
            coverImageUrl: request.CoverImageUrl);

        await repository.AddAsync(listing, cancellationToken);
        return Results.Created($"/api/listings/{listing.Id}", ToResponse(listing));
    }

    private static async Task<IResult> UpdateListing(
        Guid id,
        [FromBody] UpdateListingRequest request,
        IListingRepository repository,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Results.NotFound();

        var listing = Listing.Create(
            id: existing.Id,
            title: request.Title,
            price: request.Price,
            pisoDeNegociacao: request.PisoDeNegociacao,
            status: existing.Status,
            fipeReference: request.FipeReference,
            coverImageUrl: request.CoverImageUrl,
            createdAt: existing.CreatedAt,
            updatedAt: DateTime.UtcNow);

        await repository.UpdateAsync(listing, cancellationToken);
        return Results.Ok(ToResponse(listing));
    }

    private static async Task<IResult> DeleteListing(
        Guid id,
        IListingRepository repository,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Results.NotFound();

        await repository.DeleteAsync(existing, cancellationToken);
        return Results.NoContent();
    }

    private static ListingResponse ToResponse(Listing listing) =>
        new(
            listing.Id,
            listing.Title,
            listing.Price,
            listing.PisoDeNegociacao,
            listing.Status,
            listing.FipeReference,
            listing.CoverImageUrl,
            listing.CreatedAt,
            listing.UpdatedAt);
}
