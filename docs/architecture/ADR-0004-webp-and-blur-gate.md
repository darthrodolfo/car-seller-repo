# ADR-0004 — WebP Image Format with Blur-Gate Pattern for HD Media

## Status
Accepted

## Date
2026-02-26

## Context

Vehicle listings are heavily visual — photo quality directly impacts buyer trust and conversion. However, high-resolution images on mobile 4G connections create two competing problems:

1. **Data cost:** Downloading HD images for every listing during a browse session wastes significant mobile data, particularly for prepaid users.
2. **Perceived performance:** Showing placeholder spinners or broken image states while photos load destroys the browsing experience.

Standard approaches (lazy loading with spinners, progressive JPEG) partially address performance but do not address data cost or the perception of content being "hidden".

## Decision

**Image format:** All listing images will be stored and served as **WebP**. The server converts uploaded images to WebP on ingest. WebP provides 25–35% smaller file sizes compared to JPEG at equivalent perceptual quality, with full support on all modern Android and iOS versions.

**Blur-Gate pattern for HD images on 4G:**
- When the app is on a 4G/5G connection, only cover images (one per listing, low-to-medium resolution) are included in the Pacotão sync.
- HD gallery images are **not pre-loaded**.
- In the listing detail view, un-loaded HD images are displayed as a **blurred low-resolution placeholder** (the cover image scaled up and blurred via CSS/Flutter ImageFilter).
- An overlay reads: *"Toque para carregar foto em HD"*.
- On user tap, the HD image is fetched on-demand and replaces the blur with a smooth cross-fade transition.
- On Wi-Fi, all HD images are pre-fetched as part of the full sync — the Blur-Gate is never shown.

**Zoom HD:** Once loaded, images support pinch-to-zoom at full resolution.

## Consequences

### Positive
- WebP reduces total sync payload size, improving sync speed and reducing data consumption.
- Blur-Gate communicates "there is more content here" rather than showing a broken or empty state — psychologically preferable and maintains visual continuity.
- On-demand HD loading means users only pay the data cost for listings they are genuinely interested in.
- Wi-Fi users get the full HD experience with no interaction friction.

### Negative
- Blur-Gate adds one extra tap to view HD images on 4G, which may frustrate power users. This is a deliberate data-cost trade-off.
- Requires the server to maintain two resolutions per image (cover + HD) or generate covers dynamically from HD assets.
- WebP conversion must happen server-side at upload time, adding latency to the Backoffice upload flow.

### Risks
- If blur placeholder generation fails (e.g. corrupt cover image), the fallback must gracefully show a neutral placeholder rather than crashing.
- Very large HD images (>5MB per photo) could still cause poor UX on slow 4G. A maximum upload resolution limit should be enforced in the Backoffice.

## Related Decisions
- [ADR-0002](ADR-0002-connectivity-aware-sync.md) — sync mode determines when Blur-Gate is active.
- [ADR-0005](ADR-0005-server-side-media-protection.md) — watermark is applied to the HD asset at the same pipeline stage as WebP conversion.
