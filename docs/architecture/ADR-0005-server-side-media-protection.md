# ADR-0005 — Server-Side Media Protection (Watermark, Video Overlay, Audio Noise Injection)

## Status
Accepted

## Date
2026-02-26

## Context

A common problem for independent vehicle sellers in Brazil is media theft: photos and videos from legitimate listings are scraped and reused to create fraudulent listings on OLX, Facebook Marketplace, and WhatsApp groups. These fake listings damage the seller's reputation and confuse genuine buyers.

Additionally, AI voice cloning tools have become accessible enough that a seller's audio pitch could theoretically be cloned and used in scam calls or fake content. This is a novel but credible threat for a seller who records personal voice content.

Client-side watermarking is not sufficient — the original unwatermarked assets would still be accessible to anyone intercepting network traffic or with filesystem access on a rooted device.

## Decision

All media protection is applied **server-side at upload time**, before assets are stored or served:

**Images:**
- Uploaded images are converted to WebP (see ADR-0004) and a **semi-transparent watermark** (seller logo/brand) is composited onto the image server-side.
- The watermarked WebP is the only version stored in the publicly accessible asset store.
- The original unwatermarked upload is deleted after processing.

**Videos:**
- Videos are processed through **FFmpeg** on the server.
- A **text/logo overlay** is burned into the video stream (not as a separate track — it cannot be stripped).
- Processed video replaces the original.

**Audio (Pitch de Áudio):**
- The server injects **inaudible high-frequency noise** into the audio file before storage. This noise is imperceptible to human listeners but disrupts AI voice fingerprinting and cloning algorithms.
- Audio files are stored and served with an obfuscated extension (`.dat`) and a randomised filename to impede automated scraping and file-type detection.
- Local storage of audio uses encrypted, obfuscated formats.

## Consequences

### Positive
- Protection is applied universally — no client-side implementation required, no way to bypass by intercepting client code.
- Watermarked assets serve as an implicit attribution signal if stolen content is spotted on other platforms.
- Audio noise injection provides a meaningful barrier against commodity AI voice cloning tools without any perceptible quality degradation.

### Negative
- Server-side processing adds latency to the Backoffice upload flow. The seller must wait for FFmpeg jobs to complete before a listing can be published.
- FFmpeg processing requires server resources. For a solo MVP, this must be managed carefully to avoid blocking the main request thread (use a job queue).
- Watermark design and placement must be decided upfront — repositioning it later requires reprocessing all existing assets.

### Risks
- A sophisticated attacker with video editing skills can remove a watermark from a video. This ADR does not claim to prevent determined theft — it raises the cost and provides attribution evidence.
- If the FFmpeg job fails, the upload pipeline must have a safe failure mode that does not store unprotected assets publicly.
- The `.dat` extension obfuscation is a weak security measure on its own — it must be combined with server-side access controls on the audio endpoint.

## Related Decisions
- [ADR-0004](ADR-0004-webp-and-blur-gate.md) — WebP conversion and watermarking happen in the same server-side pipeline.
