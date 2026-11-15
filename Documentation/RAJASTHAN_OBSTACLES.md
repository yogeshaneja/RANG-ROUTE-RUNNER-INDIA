# Rajasthan gameplay obstacles

| Object | Player action | Shape and collision |
| --- | --- | --- |
| Painted matka stack | Jump | Original round pottery with painted rims, below 0.95 m |
| Folded fabric bundles | Jump | Layered colourful cloth and gold borders, below 0.95 m |
| Wooden bazaar cart | Change lane | Wheels, wood rails and textile/pottery cargo; approximately 2.15 m tall collision envelope |
| Cloth awning | Slide through the middle | Hanging fabric begins at 1.25 m; separate solid posts remain collidable at ground level |
| Decorated camel | Follow LEFT/RIGHT escape warning | Slow distance-driven crossing confined to the two non-reserved lanes; broad body collision, no jumping/sliding through the animal |
| Flying festival kite | Enjoy the scenery | Overhead native geometry with optional wind motion; no collision |

The meshes and materials for each obstacle type are built once and shared by every pooled slot. Recycling a segment swaps mesh references and resets transform/visibility without creating new obstacle geometry. Kites animate only near the playable view. The existing background images remain bundled offline.

## Fairness and lifecycle

- First two rows stay clear for the tutorial.
- A fixed obstacle row reserves at least one clear lane.
- A camel row reserves an outer lane and removes other obstacles from that row. The moving camel never enters that reserved lane, including its full visual/collision width.
- Warnings start 45 metres ahead; the HUD names the escape lane. Camel motion is a function of run distance, so pause freezes it and restart resets it.
- Collision sweeps subtract the camel's old/new positions from the player's old/new positions, accounting for relative motion during each frame.
- Contact ends the run with a gentle-stop message, no animal-injury effect.
- Segment spacing is 28 metres, allowing room for the larger obstacle envelopes.

## Local acceptance checks still required

1. Import and compile in Unity; confirm geometry is visible with no material errors.
2. Jump matkas/fabric and dodge carts; confirm collision sizes feel fair.
3. Slide through an awning centre, then deliberately contact a support post.
4. Observe left- and right-escape camel warnings; follow each reserved lane at increasing speeds.
5. Pause halfway through a crossing, resume, then restart; no camel jumps ahead or duplicate models should appear.
6. Check pooled rows after several segment recycles and profile a sustained phone run.

## Asset provenance

All new obstacle and kite shapes are original code-native geometry. No new raster illustration is required for their 3D forms. The user permitted free web assets; ambientCG Wood Floor 001 was verified as CC0, but its download was unavailable here and it is not bundled. Source/license details are in ASSET_REGISTER.md. No new package, cloud call, paid asset or publication is part of this change.
