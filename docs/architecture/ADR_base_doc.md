# Architecture Decision Records (ADR)

This folder contains the Architecture Decision Records for the **AutoVenda MVP** project.

ADRs document significant technical and architectural decisions, the context that led to them, and the consequences of adopting them. They are written at the time the decision is made and treated as immutable history — if a decision changes, a new ADR supersedes the old one rather than editing it.

## Status Legend

| Status | Meaning |
|--------|---------|
| `Proposed` | Under discussion, not yet adopted |
| `Accepted` | Decision made and in effect |
| `Deprecated` | Was accepted, but superseded by a later ADR |
| `Superseded by ADR-XXXX` | Replaced — links to the new record |

## Index

| # | Title | Status | Date |
|---|-------|--------|------|
| [ADR-0001](ADR-0001-offline-first-architecture.md) | Offline-First Architecture with SQLite/Hive Local Database | Accepted | 2026-02-26 |
| [ADR-0002](ADR-0002-connectivity-aware-sync.md) | Connectivity-Aware Catalogue Sync via Hash Versioning | Accepted | 2026-02-26 |
| [ADR-0003](ADR-0003-post-intent-identity-validation.md) | Post-Intent Identity Validation (No Login Wall) | Accepted | 2026-02-26 |
| [ADR-0004](ADR-0004-webp-and-blur-gate.md) | WebP Image Format with Blur-Gate Pattern for HD Media | Accepted | 2026-02-26 |
| [ADR-0005](ADR-0005-server-side-media-protection.md) | Server-Side Media Protection (Watermark, Video Overlay, Audio Noise Injection) | Accepted | 2026-02-26 |
| [ADR-0006](ADR-0006-firebase-analytics-observer.md) | Firebase Analytics via Domain Event Observer Pattern | Accepted | 2026-02-26 |
| [ADR-0007](ADR-0007-client-side-piso-guard.md) | Client-Side Piso de Negociação Enforcement | Accepted | 2026-02-26 |
| [ADR-0008](ADR-0008-whatsapp-sms-otp.md) | WhatsApp / SMS OTP for Identity Verification | Accepted | 2026-02-26 |

## How to Add a New ADR

1. Copy the template below into a new file: `ADR-XXXX-short-title.md`
2. Fill in all sections
3. Add a row to the index above
4. Commit alongside the code or design change it documents

```
# ADR-XXXX — Title

## Status
Proposed

## Date
YYYY-MM-DD

## Context

## Decision

## Consequences

### Positive
### Negative
### Risks
```
