# Thar Cloud Guardian

An original fantasy giant appears briefly in the Rajasthan sky: a broad turquoise-coated man with a bright rose/turquoise turban, curled moustache, gold jewellery, bright eyes and welcoming raised hands. Layered pearl, rose and lavender clouds span the sky, with a few mirror-like glints and a soft reveal chime. It is fictional scenery, not a traditional/religious figure, animal, collision obstacle or source of game-over.

## Appearance rules

The first appearance starts around 288 m, shortly after the first garden dance rest. Later appearances are approximately 1,008 m apart: 1,296 m, 2,304 m, and so on. Trigger windows are the final 20 m of every other garden's first row (row mod 36 == 10, offset >= 8 m). Each garden can trigger only once per run. Reset/restart clears the appearance history.

The event lasts four active run seconds, with a 0.55 s reveal and 0.7 s disappearance. The giant nods/waves gently, the cloud banks drift a little and glints rotate. Reduced effects use a lower opacity, static head/arms/cloud positions, no glints and no scale motion. There is no strobe, camera shake, full-screen opaque flash or added control lock. Running continues normally.

Both trigger-window endpoints plus four seconds at the existing maximum speed remain inside the obstacle-free garden buffer. Guardian updates happen only in Running, after the separate dance-break branch. Pause/background stops the run clock and therefore the appearance; resuming continues it. Game over, return to Start and restart hide/reset it. Distant warning UI still takes priority over the short guardian caption.

## Visibility and implementation

CloudGuardian builds one fixed reusable 3D group during startup. The torso, face/turban, arms and three cloud banks are material-batched once; construction meshes use the existing finalization path. Triggering creates no GameObject, texture, material, mesh or particle. Materials are private to this effect and disposed at shutdown; mesh ownership stays with the existing PrimitiveArt cache.

The giant/cloud group sits about 70 local units ahead, with its base lowered from y=7 to y=2.5 so the face sits below the top notification area. Its lightweight transparent unlit shader gives gentle normal-based shading, uses ordinary depth testing, writes no depth, and fades out below the upper sky area. The lower 54% of the screen receives zero effect opacity, with a transition up to 63%; opaque closer scenery/hazards can also occlude it through normal depth testing. This protects the road/runner view without editing global weather, fog, sunlight, collision rules or camera motion.

No texture download, generated image, external 3D asset, postprocessing bloom, realtime light, extra audio file, extra AudioSource or new package was added. Bright unlit eyes/glints create the glow-like look; this is not physical volumetric cloud/bloom rendering. The reveal reuses the game's original celebration and airy-hum sounds. Profile the additional temporary draw/overdraw cost on the target phone; mobile performance is not guaranteed by the source implementation.

## Required acceptance

1. Import/compile the new CloudGuardian C# and shader assets. Run Indian Runner Check Rules to verify shader support and safe appearance windows.
2. Run through the first dance rest, then reach about 288 m. Expect the large man in the cloudy sky, a short title/chime and a four-second fade. Check his face/turban/hand silhouette and the clouds in portrait framing.
3. Confirm the road, runner, warnings and tokens remain readable; controls, scoring and collisions must stay unchanged. Inspect transparent sorting and shader appearance on the actual Android graphics API, in sun/cloud/rain.
4. Confirm no repeat in the same garden, no appearance during the dance countdown, and no guardian in the next garden; the following eligible garden can reveal it again.
5. Pause/background mid-reveal and resume. Restart/return to Start/game over must not leave a visible giant or stale trigger.
6. Test reduced effects and Sound OFF, then profile a long run and repeated restarts for material/mesh growth and frame-time spikes. The effect is currently part of the Rajasthan prototype; future regions should configure their own themed events.

Workspace checks are syntax/source/design checks only. The new shader has not been compiled by Unity and no rendered visual or real-phone playtest was possible here.
