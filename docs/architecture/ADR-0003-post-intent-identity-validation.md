# ADR-0003 — Post-Intent Identity Validation (No Login Wall)

## Status
Accepted

## Date
2026-02-26

## Context

Most vehicle marketplace apps require account creation or login before the user can view listings or contact the seller. This pattern creates a significant conversion barrier: users who are just browsing are forced to commit personal data before receiving any value, leading to high early drop-off.

For a single-seller MVP with no multi-user features (wishlists, saved searches, purchase history), a persistent account is not necessary for the browsing experience. The seller's primary goal is to receive qualified leads with contact information — not to maintain long-term user accounts.

The tension is between: collecting identity early (seller preference) vs. removing all friction to browse (buyer preference and conversion optimisation).

## Decision

The app will adopt a **Zero-Friction / Post-Intent identity validation pattern**:

- Navigation is completely free. Any user can open the app, browse the full catalogue, view listing details, play audio pitches, and run the financing simulator without providing any personal data.
- Identity validation is triggered **only at the moment of intent** — when the user taps "Negociar Agora" and proceeds to either "Tenho Interesse" or "Fazer Oferta".
- Validation is performed via **WhatsApp OTP or SMS OTP** (user's choice), not email/password account creation.
- Immediately after phone verification, the user selects a **BuyerProfile category** (PF, Vendedor Autônomo, Lojista, Investidor) to qualify the lead for the seller.
- Once verified in a session, the identity gate is not shown again for subsequent proposals in the same session.

## Consequences

### Positive
- Eliminates the login wall that causes the majority of early-funnel drop-off on marketplace apps.
- WhatsApp OTP is familiar to Brazilian users — WhatsApp penetration in Brazil exceeds 96%.
- Phone-based verification produces immediately actionable seller leads (direct WhatsApp contact available).
- BuyerProfile collection at the moment of highest intent produces more honest self-categorisation.

### Negative
- No persistent user account means users cannot retrieve their proposal history across devices or after app reinstall.
- Re-verification required if the user clears app data or switches devices.
- Cannot send push notifications to anonymous users before they verify.

### Risks
- OTP delivery failure (network issues, WhatsApp API downtime) would block the entire conversion flow. An SMS fallback channel mitigates this.
- Users may provide a false phone number. However, OTP verification ensures the number is live and owned by the user — sufficient for the seller's contact needs.

## Related Decisions
- [ADR-0008](ADR-0008-whatsapp-sms-otp.md) — details the OTP provider selection.
