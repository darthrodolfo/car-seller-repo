# ADR-0008 — WhatsApp / SMS OTP for Identity Verification

## Status
Accepted

## Date
2026-02-26

## Context

Following the post-intent identity validation decision (ADR-0003), the app needs a verification channel to confirm a user's phone number. Options considered:

| Option | Fit for Brazil | Effort | Cost |
|--------|---------------|--------|------|
| WhatsApp OTP (via Twilio / Z-API / official WhatsApp Business API) | Excellent — 96%+ WhatsApp penetration in Brazil | Medium | Low–medium per message |
| SMS OTP | Good — universal fallback | Low | Low per message |
| Email OTP | Poor — not a primary contact channel in this context | Low | Very low |
| Google Sign-In / Apple ID | Medium — requires app store accounts | High | Free |
| Password-less magic link (email) | Poor — same as email OTP | Low | Very low |

WhatsApp is the dominant communication channel in Brazil across all demographics and income levels. Users are significantly more likely to respond to and trust a WhatsApp message than an SMS from an unknown number. However, WhatsApp API availability and delivery can be intermittent, making SMS an essential fallback.

## Decision

Identity verification will use **WhatsApp OTP as the primary channel** with **SMS OTP as an explicit user-selectable fallback**:

- At the identity interstitial, the user enters their phone number and selects their preferred channel: **WhatsApp** or **SMS**.
- A 6-digit OTP is dispatched to the selected channel.
- The OTP is valid for 5 minutes.
- Maximum 3 OTP attempts before a temporary lockout (10 minutes) to prevent brute-force.
- On verification success, the phone number is stored as the `BuyerIdentity` and the BuyerProfile selector is shown.

**Provider selection for MVP:** A lightweight provider such as Twilio Verify (which supports both WhatsApp and SMS via a single API) is recommended to avoid maintaining two separate integrations.

## Consequences

### Positive
- WhatsApp OTP feels native and familiar to Brazilian users — significantly lower psychological friction than SMS from an unknown number.
- Single provider (Twilio Verify) handles both channels, reducing integration complexity.
- Phone number verification produces a directly actionable seller lead — the seller can WhatsApp the buyer immediately.
- No need to build or maintain an account/password system.

### Negative
- Ongoing per-verification cost (Twilio pricing). For MVP volumes this is negligible, but must be monitored at scale.
- WhatsApp Business API requires a verified business account. Setup time and approval can add days to the launch timeline.
- If the user changes their phone number, there is no account recovery flow in the MVP.

### Risks
- WhatsApp API rate limits or downtime could block all new lead conversions simultaneously. The SMS fallback option mitigates this at the UX level, but a server-level circuit breaker should be implemented to auto-route to SMS if WhatsApp delivery fails.
- Brazilian carriers occasionally block OTP SMS as spam. Using a reputable provider with a short-code number reduces this risk.

## Related Decisions
- [ADR-0003](ADR-0003-post-intent-identity-validation.md) — this ADR defines the mechanism for the identity gate described there.
