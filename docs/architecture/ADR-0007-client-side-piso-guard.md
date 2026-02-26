# ADR-0007 — Client-Side Piso de Negociação Enforcement

## Status
Accepted

## Date
2026-02-26

## Context

The seller configures a minimum acceptable offer value (Piso de Negociação) per listing in the Backoffice. Offers below this value should be rejected before reaching the seller, to avoid cluttering their lead dashboard with non-serious proposals and to guide buyers toward realistic offers.

Two enforcement locations were considered:

1. **Server-side enforcement:** The client submits the offer, the server validates against the Piso, and returns an error response.
2. **Client-side enforcement:** The client validates the offer value against the cached Piso before making any network call.

## Decision

Piso enforcement is performed **client-side**, by the `PisoGuard` domain service, before any network request is made.

- The Piso value is included in the Pacotão JSON and cached locally alongside each listing.
- When the user taps "Enviar Proposta", `PisoGuard.validate(offeredValue, listing.piso)` is called.
- If the check fails, the submission is blocked, a rejection UI is shown, and **no network request is fired**.
- The nearest valid pill suggestion above the Piso is highlighted to guide the buyer toward an acceptable offer.

Server-side validation of the Piso is intentionally **not implemented in the MVP**. The Piso is not a security-critical constraint (it is a UX guidance tool, not a financial contract), and the non-binding disclaimer on the Confete screen makes clear that proposals have no legal force regardless.

## Consequences

### Positive
- Instant feedback — no network latency on rejection.
- Zero server load for rejected proposals.
- Works fully offline — Piso validation does not require connectivity.
- Reduces noise in the seller's lead dashboard.

### Negative
- A technically motivated user could bypass client-side validation by directly calling the API. This is an accepted risk given the non-binding nature of proposals.
- If the Piso value changes in the Backoffice after the client's last sync, the client may enforce a stale Piso value. This is mitigated by the connectivity-aware sync (ADR-0002) which updates the catalogue frequently.

### Risks
- Stale Piso risk (described above) is the primary concern. In the worst case, a buyer sees a rejection for an offer that would now be acceptable after a Piso reduction. The mitigation is ensuring the sync badge is visible and users are nudged to sync when stale.

## Related Decisions
- [ADR-0002](ADR-0002-connectivity-aware-sync.md) — Piso freshness depends on sync recency.
- [ADR-0001](ADR-0001-offline-first-architecture.md) — Piso value lives in the local database alongside the listing.
