# Character polish and moving hazards

The original procedural boy has refined face proportions, smaller eyes with highlights, shaped nose/lips/ears/sideburns, sleeve embroidery, shoe stitching and a layered saffron scarf. Head pitch compensates for forward torso lean. These remain stylised articulated meshes, not a scanned person or a production skinned character.

Crossing camels have rounded bodies/necks/heads, ears, muzzle/nostrils, tail/tuft, bridle, saddle/tassels, and four separately animated tapered legs with padded feet. Walking is driven by run distance, so it freezes when paused and resets deterministically. Roadside camels also use rounded anatomy. Leg meshes and body/material groups are shared across fixed pooled slots.

Motorcycles drive towards the player in a fixed lane, with tyres, hubs, red tank, engine, fork, headlamp and helmeted rider. The relative approach rate is 1.65 times runner speed; they meet the runner at their scheduled row. Spaceships descend smoothly from 9 m to 0.8 m above the road between 40 m and 8 m ahead. An amber ground rectangle marks the landing footprint; an on-screen warning names the reserved escape lane. Decorative saucer rotation is disabled with reduced effects; gameplay descent is retained.

First encounters are a motorcycle at 126 m and spaceship at 910 m. The rare spaceship repeats every 2,016 m; motorcycles are scheduled every 336 m, skipping festival gardens and their transition rows. They replace the other obstacles in their row, and both neighbouring rows are empty. Coins remain in a different lane. Warnings begin 45 m ahead (2.5 seconds at maximum speed). Camel rows do not overlap these recovery rows.

Contact ends the run with a hazard-specific message. Collision sweeps the player relative to each moving object, including ship altitude. Gameplay has no physics colliders or per-encounter instantiation. All hazard slots are allocated at startup and reused; no new external assets/packages were added.

## Required Unity/phone review

Pull the feature branch, let Unity import, run Indian Runner > Run core checks, and Build and Run again. Verify both hazards by deliberately remaining in their lane; then retry and follow the warning/coin lane. Test lateral contact and jumping into a ship, no collision in adjacent lanes, slide interactions, pause mid-descent, restart after contact, and encounter recycling beyond 600 m. Inspect camel legs/feet, facial details, lighting/materials, 9:16 visibility and sustained phone frame times. Refine visual dimensions after this actual playtest if necessary. Source checks do not replace this review.

## Oncoming decorated elephant

Obstacle kind 15 reuses the existing roadside elephant's rounded body/head/ears, curved trunk, pink woven saddle and gold trim. The original roadside elephants remain scenery. The shared builder now optionally omits fixed legs; the moving version adds four animated, shared-mesh leg parts. Grey toe geometry is retained on the moving legs. Catalog construction occurs once; all ordinary pooled slots reuse the model and bounded part buffers, with no new objects/meshes created on approach. Configure resets part rotations/positions so a reused elephant slot cannot leave a later obstacle with rotated limbs.

The first elephant meets the runner at row 16, approximately 462 m, then rows 52, 88 and so on (1,008 m apart). It replaces a scheduled motorcycle at those rows; other motorcycles remain. It faces the player and advances towards them using rowZ + 0.35 × (rowZ - distance), slower than the motorcycle's approach factor. It stays inside one non-safe lane; both neighbouring rows remain empty and rest/dance buffers exclude it. Its leg gait is based on distance, with smaller leg swing under reduced effects.

Collision uses the existing elephant-relative swept-box path, with half width 1.05, height 3.0 and half depth 2.45 to cover the trunk after rotation. It must be dodged, not jumped/slid through. The reserved adjacent lane retains 1.26 units of horizontal clearance including player half width. The standard warning starts 45 row-metres away, approximately 2.39 seconds before front contact at maximum speed. Contact causes a gentle stumble/game over without injury animation.

The elephant plays the existing trumpet earlier during approach (within 42 actual forward units when selected as the nearby sound encounter) and shares the supplied elephant Hindi dialogue group. No additional speech recording is required.

Phone checks: reach about 462 m, verify it faces/approaches from ahead, feet alternate without detached geometry, warnings/escape lane are readable, trunk/side contacts match the visual, safe-lane passage does not hit and no jumping/sliding bypasses the body. Restart/recycle and inspect other obstacle rotations. Test reduced effects, voice/Sound mute, Hindi line selection, multiple encounters and sustained renderer/memory/frame-time cost.
