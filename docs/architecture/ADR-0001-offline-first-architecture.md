# ADR-0001 — Offline-First Architecture with SQLite/Hive Local Database

## Status
Accepted

## Date
2026-02-26

## Context

AutoVenda targets buyers in Brazil who frequently browse on mobile devices under unstable 4G connections or in low-signal environments (parking lots, junkyards, auction houses). A traditional network-dependent architecture would result in blank screens, loading spinners, and high drop-off rates precisely at the moments when buyers are most motivated to browse.

The single-seller model means the catalogue is relatively small and changes infrequently (new listings, price updates), making local caching a viable and high-value strategy. The seller does not require real-time inventory depletion logic (e.g. preventing two buyers from bidding on the same unit simultaneously), which further simplifies offline consistency requirements.

## Decision

The mobile client will adopt an **Offline-First architecture**. All catalogue data is stored in a local database (SQLite via `drift` package, or Hive for simpler key-value assets) and this local store is the **primary read source** for all UI rendering.

- The app renders the catalogue grid immediately from local data on every launch, with no network dependency for the initial paint.
- Network calls are used exclusively to **synchronise** the local store, not to serve UI directly.
- All search and filter operations run against the local database.

## Consequences

### Positive
- Instant catalogue load on every app open regardless of connectivity.
- Full search and filter functionality available offline.
- Reduced server load — the server only serves sync payloads, not individual listing requests.
- Resilient UX in the Brazilian mobile market where 4G instability is common.

### Negative
- The local database schema must be versioned and migrated carefully as the catalogue structure evolves.
- First-install experience requires an initial full sync before the catalogue is populated — a loading state must be designed for this case.
- Increases app binary size slightly due to embedded SQLite/Hive engine.

### Risks
- If a migration fails on an older device, the local DB could become corrupted. A safe fallback strategy (wipe and re-sync) must be implemented.
- Stale data risk if the user has not connected for an extended period. Mitigated by the sync badge and the catalogue version timestamp shown in the UI.

## Related Decisions
- [ADR-0002](ADR-0002-connectivity-aware-sync.md) — defines how and when the local store is updated.
