# ADR-0002 — Connectivity-Aware Catalogue Sync via Hash Versioning

## Status
Accepted

## Date
2026-02-26

## Context

Given the Offline-First architecture (ADR-0001), the client needs a mechanism to know when its local catalogue is stale and what to download to update it. Two naive approaches were considered and rejected:

1. **Full sync on every app open** — wastes mobile data and is slow, especially on 4G.
2. **Polling the server for individual listing changes** — complex delta logic, harder to implement solo, and still requires a network round-trip per listing.

Additionally, Brazil has a significant population of prepaid mobile users for whom background data consumption is a real cost concern. Downloading HD images on 4G without consent would erode user trust.

## Decision

Catalogue synchronisation will use a **hash-based versioning strategy** combined with **connectivity-aware download modes**:

**Hash Check:**
- The server maintains a single global `catalogueHash` (SHA-256 truncated) that changes every time any listing is created, updated, or deleted.
- On app open (and on connectivity restore), the client sends its local hash to a lightweight `/catalogue/version` endpoint.
- If hashes match → no sync needed. If they differ → download the Pacotão JSON.

**Connectivity-Aware Download Modes:**
- **Wi-Fi:** Download the full Pacotão JSON including all HD images (WebP).
- **4G/5G:** Download the Pacotão JSON and cover images only. HD images are loaded on-demand when the user opens a specific listing.
- **Offline:** No sync attempted. Local data is served as-is with a staleness banner.

**Pacotão JSON** is a complete snapshot of the catalogue (not a delta). This avoids complex merge logic and is safe to retry — re-downloading the same payload is idempotent.

## Consequences

### Positive
- Single endpoint (`/catalogue/version`) is extremely cheap — one request per session to determine sync necessity.
- Full-snapshot approach eliminates merge conflicts and simplifies server logic.
- Users on 4G are not penalised with unexpected data usage.
- Idempotent sync means network interruptions during download are safe to retry.

### Negative
- Full-snapshot approach means the Pacotão grows with the catalogue. For a large catalogue (500+ listings with cover images), payload size must be monitored.
- No granular "what changed" information — the client cannot show "3 new listings added" without parsing the diff client-side.

### Risks
- If the server-side hash generation has a bug and produces false positives, clients will download unnecessary Pacotão payloads. Hash generation logic must be covered by server-side tests.
- Cover image bundling on 4G still consumes data. A future optimisation could make even cover images on-demand, but this degrades the grid browsing experience significantly.

## Related Decisions
- [ADR-0001](ADR-0001-offline-first-architecture.md) — Offline-First is the prerequisite for this strategy.
- [ADR-0004](ADR-0004-webp-and-blur-gate.md) — WebP format choice directly affects Pacotão payload size.
