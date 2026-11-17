# Character polish and moving hazards

The original procedural boy has refined face proportions, smaller eyes with highlights, shaped nose/lips/ears/sideburns, sleeve embroidery, shoe stitching and a layered saffron scarf. Head pitch compensates for forward torso lean. These remain stylised articulated meshes, not a scanned person or a production skinned character.

Crossing camels have rounded bodies/necks/heads, ears, muzzle/nostrils, tail/tuft, bridle, saddle/tassels, and four separately animated tapered legs with padded feet. Walking is driven by run distance, so it freezes when paused and resets deterministically. Roadside camels also use rounded anatomy. Leg meshes and body/material groups are shared across fixed pooled slots.

Motorcycles drive towards the player in a fixed lane, with tyres, hubs, red tank, engine, fork, headlamp and helmeted rider. The relative approach rate is 1.65 times runner speed; they meet the runner at their scheduled row. Spaceships descend smoothly from 9 m to 0.8 m above the road between 40 m and 8 m ahead. An amber ground rectangle marks the landing footprint; an on-screen warning names the reserved escape lane. Decorative saucer rotation is disabled with reduced effects; gameplay descent is retained.

First encounters are a motorcycle at 126 m and spaceship at 238 m. Each repeats every 336 m. They replace the other obstacles in their row, and both neighbouring rows are empty. Coins remain in a different lane. Warnings begin 45 m ahead (2.5 seconds at maximum speed). Camel rows do not overlap these recovery rows.

Contact ends the run with a hazard-specific message. Collision sweeps the player relative to each moving object, including ship altitude. Gameplay has no physics colliders or per-encounter instantiation. All hazard slots are allocated at startup and reused; no new external assets/packages were added.

## Required Unity/phone review

Pull the feature branch, let Unity import, run Indian Runner > Run core checks, and Build and Run again. Verify both hazards by deliberately remaining in their lane; then retry and follow the warning/coin lane. Test lateral contact and jumping into a ship, no collision in adjacent lanes, slide interactions, pause mid-descent, restart after contact, and encounter recycling beyond 600 m. Inspect camel legs/feet, facial details, lighting/materials, 9:16 visibility and sustained phone frame times. Refine visual dimensions after this actual playtest if necessary. Source checks do not replace this review.
