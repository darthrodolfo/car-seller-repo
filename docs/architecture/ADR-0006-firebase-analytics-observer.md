# ADR-0006 — Firebase Analytics via Domain Event Observer Pattern

## Status
Accepted

## Date
2026-02-26

## Context

The MVP requires lightweight product analytics to measure Time to Value (TTV), Activation Rate, and Time to Convert (TTC) — the three instrumentation priorities identified in the product specification (Part 4 of the design document).

Options considered:

| Option | Pros | Cons |
|--------|------|------|
| Firebase Analytics | Free, offline event queuing, zero backend, Flutter SDK mature | Tied to Google ecosystem |
| Mixpanel | Better funnel visualisation | Paid at scale, overkill for MVP volume |
| Custom backend events | Full control | Requires building and maintaining an analytics backend — not viable for solo developer |
| No analytics | Zero effort | Blind to conversion issues; cannot validate product decisions |

A secondary concern is **coupling**: if analytics calls are embedded directly inside business logic (e.g. inside the `submitProposta` use case), swapping providers or removing analytics later becomes risky and requires touching core logic.

## Decision

**Tool:** Firebase Analytics (free tier).

**Pattern:** A dedicated `AnalyticsObserver` class subscribes to the application's domain event bus. When a domain event fires (e.g. `PropostaSubmitted`, `IdentityVerified`), the observer maps it to a Firebase Analytics event and calls `FirebaseAnalytics.instance.logEvent()`. Business logic classes have no knowledge of or dependency on analytics.

**Core events instrumented (MVP):**

| Analytics Event | Source Domain Event |
|----------------|-------------------|
| `app_opened` | App lifecycle |
| `negotiation_initiated` | `NegotiationInitiated` |
| `identity_verified` | `IdentityVerified` |
| `proposta_submitted` | `PropostaSubmitted` |
| `piso_rejection` | `PropostaRejectedByPiso` |
| `blur_gate_triggered` | `MediaBlurGateTriggered` |
| `x9_report_submitted` | `X9ReportSubmitted` |

**Privacy rule:** No PII in any event property. Phone numbers, names, and buyer profile labels are never sent to analytics. Only anonymised `session_id` and `listing_id` values are used.

**Offline behaviour:** Firebase SDK queues events in local storage and flushes them when connectivity is restored — fully consistent with the Offline-First architecture.

## Consequences

### Positive
- Zero infrastructure cost for MVP analytics.
- Analytics are completely decoupled from business logic — swappable with no risk to core flows.
- Offline-first compatible out of the box.
- The Observer pattern means adding new analytics events in the future only requires changes in one place (`AnalyticsObserver`), not scattered across the codebase.

### Negative
- Firebase is a Google product — introduces a Google ecosystem dependency.
- Firebase Analytics has limited real-time capabilities and a ~24h data processing delay for some reports.
- Custom funnel configuration in Firebase requires manual setup per funnel.

### Risks
- If the domain event bus is not implemented as a proper pub/sub system (e.g. if it uses direct method calls), decoupling the observer becomes harder. The event bus architecture must be confirmed before this ADR is implemented.

## Related Decisions
- [ADR-0003](ADR-0003-post-intent-identity-validation.md) — `identity_verified` is the key funnel event that makes Activation Rate measurable.
