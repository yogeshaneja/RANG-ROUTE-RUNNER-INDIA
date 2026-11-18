# Five new Rajasthan encounters

| Encounter | First appearance | Visual and audio | Response |
| --- | --- | --- | --- |
| Rolling cart wheel | 70 m | Open iron/wood ring meshes, visible gold spokes and turquoise hubs; soft wooden rumble | Jump early or use the token lane |
| Falling textiles | 238 m | Turquoise, pink and blue bundles with gold selvedges, descending from above; amber landing rectangle and soft rustle | Avoid the marked lane, or jump the bundle once landed |
| Peacock display | 406 m | Blue neck, green fan, gold/turquoise feather eyes and crest; gentle stylised call | Keep clear of the marked outer lane; contact ends the run without injury |
| Road repairs | 518 m | Sand mound and high-contrast amber/dark barricade in one lane, blue scaffold and low caution bar in another; wooden warning knock | Jump the pile, slide beneath the scaffold, or use the token lane |
| Festival procession | 574 m | Two pink/turquoise decorated carts with flags, drum faces, mirror diamonds and marigold garlands; original soft drum/bell loop | Follow the clear token lane between/beside the carts |

These are original stylised procedural assets and synthesized sounds, not purchased stock models, sampled animal recordings or traditional-performance recordings. The procession is a fictional festive cart grouping; no real festival depiction is claimed.

## Movement and fairness

Wheels approach at 1.45 times runner speed and rotate around their axle at road height. The wheel render pivot is 0.62 m above ground; collision explicitly uses the road-level base so there is no elevated invisible box. Open torus rims allow the spokes to remain visible. Falling bundles descend smoothly from 6.5 m and settle 10 m ahead of the encounter. Peacocks move inwards from the same roadside towards an outer lane, never sweeping across the reserved escape lane. Procession carts travel forwards more slowly than the runner, with both blocked-lane instances animated and swept independently.

Each moving encounter replaces other obstacles in its row and clears both neighbouring rows. Every row retains a reserved token lane. New events are excluded from garden/transition rows; existing goals, pouch power, outfits, pause/restart and rare spaceship remain. Wheel, textile, peacock and procession motifs repeat every 1,008 m. Repair rows recur every 504 m where recovery rules permit. Motorcycles begin at 126 m; the spaceship stays first 910 m/repeat 2,016 m.

Roadwork sand and scaffolds have their own collision shapes, including scaffold posts and amber caps. Actual successful slides through either an awning or scaffold count toward the low-barrier goal. Repeated swipes still do not count. Warning banners name a clear lane; fabric, peacock and spaceship encounter markers stay visible near the approach. Contact with animals remains non-graphic.

## Sound and pooling

Five new startup-generated clips bring the total to 17, using the same four AudioSources. Wheel rumble and procession rhythm share the smoothly faded/panned approach source; the four-beat procession loop is at 96 BPM and starts aligned to the current music sample position. Fabric, repair and peacock cues use once-per-encounter one-shots with restrained gain and brief music ducking. Elephant scenery uses its own sound identifier, distinct from wheel obstacle IDs. All clips/materials/meshes follow existing ownership/disposal; no encounter objects or clips are created per frame.

The shared obstacle catalogue now covers 13 types. Pooled slots rebind shared mesh/material groups, and wheel mesh offsets are cleared when a slot becomes another obstacle. Existing marker/pouch/garden startup allocation is retained. Added geometry/audio require real-phone profiling before performance acceptance.

## Required Unity and phone checks

Pull, press Play and run Indian Runner > 4 - Check live startup and the core checks. Then Build and Run for the Android phone.

- Follow each warning/token lane, then deliberately test each obstacle. Jump wheels/repair sand, slide scaffolds and test fabric contact after it lands. Verify peacock and procession contact messages, visible landing markers and clear escape lanes.
- Watch wheel direction/axle height, fan/garland contrast, flags and textile drops in 9:16. Confirm cart pairs move together and leave enough visible room. Test reduced effects; gameplay motions and hit timing must remain intact.
- Listen on speaker/headphones: new cues should remain distinguishable without overpowering music, procession rhythm should blend, and elephant calls must not play for wheels. Synthesized timbres still need subjective review.
- Pause/background mid-encounter, resume, restart after contact, return to Start and run again. Check no stuck loop, stale mesh offset, repeated one-shot, duplicate scoring/slide credit or old marker/powerup state.
- Play beyond 1,600 m to cover all five types again after tile recycling; verify garden gaps, pouch duration, goals/outfit persistence and sustained phone frame time/memory.

Source parsing, independent formula/PCM/mesh audits and metadata checks passed here. Unity/C# compilation, rendering, actual listening and Android performance were not available in this workspace. Full humanoid festival crowds, traffic AI and artist-authored animal animations are not included in this prototype pass.
