# Short rides, authored adventure sections and Chutku

## Encounters

AdventureRules reserves two distinct stretches in every 36-row / 1,008 m cycle. The existing road segment pool remains seven tiles; each tile adds two prebuilt, reusable overlay roots (one passage, one ride), selected from its current logical index. Only the current state's art is built.

| Event | First occurrence | Rules |
| --- | --- | --- |
| Authored passage | 112–224 m, rows 4–7 | Alternating low jump obstacle / slide shelter; one hazardous lane and an open token lane; 2x token value |
| Vehicle warning | 476–504 m, row 17 | On-screen left/right steering cue; road clear |
| Vehicle ride | 504–672 m, rows 18–23 | Automatic grounded boarding in first row, left/right steering, static dodge obstacles in alternating rows, clear boarding/recovery/dismount rows, 2x tokens |
| Exit/recovery | 672–756 m, rows 24–26 | Clear approach back to regular controls and later journey transfer |

Jump/slide inputs are ignored during the boarding row and while mounted. A jump/slide already in progress completes before the grounded boarding pose begins; the first row is clear. Left/right remains responsive. The camera follows lateral vehicle motion slightly more closely so the wider ride stays in view. A ride skipped through nonstandard external state changes still leaves a valid run-on-foot route. At normal speeds the 168 m ride lasts roughly 9–12 seconds depending on boarding timing; it is not a fixed-duration cutscene. Speed/distance continue normally. Departure sends the empty vehicle gently forward/right through the clear exit buffer for 0.65 seconds while the runner returns to foot controls.

Existing moving traffic, crossing animals, roadwork and rain-visitor admissions are suppressed throughout the reserved encounter buffers. Regular events continue elsewhere; oncoming motorcycle/tractor traffic is moved to ordinary cycle-boundary rows (from row 36), with clear neighbouring rows. Rajasthan alternates roadworks and possible camel crossings at ordinary row 34 in successive cycles. The umbrella visitor can replace that row when it is rainy, so reserving its previous slots for adventures does not remove the rain encounter entirely. The shared safe lane is retained. Ride obstacles are 56 m apart, leaving recovery time for the wider vehicle. Wide vehicle collision bounds are used while riding: 0.80 m half width / 1.45 m half depth; adjacent lanes remain reachable and clear. Jump/slide passage collision uses the existing obstacle/post definitions. Visible passage rails/posts stay outside the three lanes; decorative ropes/arches remain above jump clearance. Double-value pickups contribute two tokens, including to goal progress, and are announced in the mode HUD.

## Original regional art

- Rajasthan: pink/teal/gold cart with spinning wheels and a saffron pennant; sandstone walls, tall golden arches and lanterns for the fort passage.
- Kerala: rounded turquoise/gold canoe with paddle props and floating coral/gold navigation-buoy obstacles; water-channel ride surface; dark wooden bridge planks, green rails, high bunting and lanterns for the canopy bridge.
- Punjab: cream/teal harvest trolley with flowers and animated wheels; festive pennants, gold/pink accents and Phulkari stall hazards for the harvest alley.
- Goa: coral scooter with cream saddle, golden headlamp, animated wheels and a small companion platform; cream coastal boardwalk, colourful pennants and fishing-stall hazards.

All are original stylised Unity geometry, not downloaded production models. The canoe has decorative paddle shafts rather than a fully animated rowing rig; the trolley/cart use fantasy self-moving runner-game motion. These are playable lane-steering modes, not realistic vehicle simulations. The runner's existing five skinned renderers are reused for a seated/reaching pose, with feet placed through the existing leg IK solver. No extra input buttons, paid assets, services, image textures or packages are required.

## Chutku

The companion is an original rounded tan/cream street-dog design with expressive eyes, floppy ears, a turquoise collar, saffron bandana, four animated legs and a wagging tail. On foot he follows just behind/beside the runner. When an uncollected token passes behind the player, the shared pickup loop can mark it taken and award its normal/adventure value once, then animate a short 0.85-second fetch. He can retrieve at most one missed pickup per eight seconds; an adventure token has value two. Regular collection has priority and already-taken flags prevent companion/magnet/player double scoring.

The dog has no collision or harm behaviour and does not provide a shield. He does not fetch while the player is riding; he occupies a companion seat/platform on the canoe/trolley/cart/scooter. During jump/slide sections he stays on the ground. Pause, dance and scene-transition logic do not advance his pickup cooldown because it uses the run clock. JourneyCheckpoint carries the absolute ready time across state changes. Restart clears fetch state/cooldown; repeated region changes rebuild only one companion with the active world.

## Audio and reduced effects

Existing original effects provide boarding/passage chimes and pickup feedback. Cart rides use the original soft wheel loop, Punjab adds the existing procession percussion, Goa uses the existing scooter/motor loop and Kerala keeps the background groove. These are game sound designs, not asserted traditional recordings. No additional AudioSource or streaming dependency is added. Sound/music preferences remain respected. Reduced effects suppress vehicle bob/roll/scale flourishes and tail wag, and lower leg swing while retaining essential steering, wheel motion and helper collection.

## Validation and acceptance

All 37 C# files syntax-parse. Independent numerical checks cover 256,000 four-state rows for safe lanes, vehicle-width clearance, moving-hazard recovery and separation from rest/transfer buffers; pickup/player/helper single-award behaviour at four frame rates; seated IK reach; rail/arch clearance. Metadata and whitespace checks pass. Unity core checks also gain 64,000 authored-encounter row checks, boarding/value/cooldown contracts and live overlay/actor existence checks, to execute locally.

These are source/formula checks, not C# execution, a Unity compile, rendered art or a phone build. Required device checks: first passage at 112 m and first ride near 504 m; cue/boarding while jumping or sliding; left/right ride dodge collisions with visible wide vehicle bounds; no jump/slide during rides; grounded dismount and normal controls; all four vehicle/passages after state changes; double token accounting; companion fetching a missed token once and cooling down; magnet/companion interactions; pause/background, death/restart and journey handoff; rig/seat/camera fit; reduced effects; repeated-cycle frame time and memory profiling. Props are pooled/batched, but mobile performance is not yet measured.

## Festival Rush variation

The passage at 112–224 m is now the first rare **Festival Rush**, with a clear road and triple-value tokens. It replaces one passage every third 36-row cycle; the two passages between festivals retain their authored jump/slide challenges and double-value trails. Vehicle adventures and Chutku stay unchanged. See [FESTIVAL_RUSH.md](FESTIVAL_RUSH.md) for regional decoration, audio, timing and budgets. The earlier source-check counts above describe the pre-festival implementation; current checks are recorded in VALIDATION.md.
