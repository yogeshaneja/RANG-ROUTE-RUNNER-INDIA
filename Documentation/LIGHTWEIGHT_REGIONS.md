# Keeping the runner light as states are added

## Current scope and evidence

This remains a single Rajasthan route; multi-region loading is not implemented. The source Assets folder was approximately 1.74 MB before this pass. That is not an APK/install/RAM measurement: Unity engine libraries, imported texture formats, generated meshes/audio, GPU resources, native speech service and build settings determine the actual phone footprint.

The existing bounded pools remain seven road tiles, 14 obstacle rows, 42 obstacle slots, 70 shared-geometry tokens, 18 pickup glints, 48 rain streaks (up to 10 in reduced effects), eight clouds and five skinned runner renderers. Catalogue meshes/materials are shared. Long runs recycle objects instead of continually creating new ones. Rain/voice/character/gameplay features are preserved.

## Completed cleanup

- Final static mesh merges now opt in to consuming uniquely owned construction meshes. Copies used to build scenery/head details/clouds/catalogues are removed from the ownership cache and destroyed after copying, rather than retained alongside the finished mesh until the game ends. Imported/built-in primitive meshes are never destroyed by this step. Destruction remains deferred to Unity's end-of-frame; startup peak/hitches still need profiling.
- Finished static batches call UploadMeshData(true), releasing their script-readable CPU copy. The skinned arms/legs/scarf retain their required mesh data. Current final batches are only rendered/shared afterwards, never fed into another combine. Future builders that need intermediate reusable/readable geometry must leave finalizeStatic false until their last merge. Opt-in consumption assumes source meshes are uniquely owned; do not consume a mesh referenced outside that build.
- Static groups use 16-bit indices when total vertex count permits, with 32-bit fallback for larger groups. Final renderers do not request shadows or light/reflection probes in this procedurally lit route. Batches still need draw-call/GPU review on the phone; no FPS result is claimed.
- Older RajasthanBazaar.jpg and RajasthanHorizon.jpg, plus their metadata, move byte-for-byte to Documentation/ArtArchive outside Unity Assets. These 570,268 JPEG source bytes are no longer unconditional Resources build inputs. The active realistic horizon, sandstone and cotton textures are retained, with existing compression/mip/read-only import settings. Actual APK savings depend on Unity's imported/platform formats and build compression.
- Weather only activates/deactivates streak objects when visibility changes, and only updates atmosphere/material values when shade/cloudiness changes. Existing rain/wind animation and visible counts remain the same. Token local positions are updated only while being attracted or restored after attraction; parent road scrolling already positions stationary tokens. Blink transforms are updated only when eye scale changes.
- No forced frame-rate reduction, texture downgrade, native dependency, background resource unloading or GC.Collect call is added. Existing Effects LOW remains available.

## Review the real build before setting budgets

Use Indian Runner > 5 - Inspect owned mesh memory (Play Mode) for a development snapshot: owned mesh count, readable vs finalized counts, and engine-reported owned mesh bytes. This does not include total textures/audio/GPU/native speech memory or measure an Android APK. Do not compare an Editor snapshot to installed phone size as if they were the same metric.

Profile a Development Build on the actual Motorola phone over USB, then confirm user-facing behaviour with a normal build. Capture a warm five-minute route and repeat rain + spoken dialogue + pouch/skin animation/restarts. Record the following before adding another state:

| Measurement | Evidence to record |
| --- | --- |
| Download/install size | Actual built APK/AAB and installed app size |
| Frame time | CPU/GPU frame-time spikes and sustained smoothness at the chosen target |
| Memory | Unity used memory, textures, meshes/audio and Android process memory; account for the speech engine separately |
| Allocations | GC.Alloc spikes during normal play, pickups, captions and restart |
| Startup | Time/hitch peak until the menu is usable and voice status settles |
| Sustained behaviour | Heat, battery use and memory trend after repeated restarts/region changes |

No fixed FPS/RAM/APK-size promise is justified without that baseline. Current IMGUI and procedural startup generation remain areas to investigate if the device profile identifies them as bottlenecks. Run the startup/core checks and inspect all materials/models, skinning, weather, pouch restoration and voices after this pass. Scene teardown/reopen must not retain or invalidate cached meshes.

## Required design for the next state

1. Keep one shared game core: input, runner/rig, scoring/save, UI, pool logic, weather framework and voice service. New states should be data/theme content, not copies of the entire game with their own player/audio/pool stacks.
2. Load one region's scenery/catalogue/music at a time. Use a visible loading boundary for region selection and Journey transitions; release the old region before constructing the next. Do not instantiate Rajasthan, Punjab and Kerala together and merely hide two.
3. Before implementing Punjab, separate region-specific constructors/settings from RunManager/TrackWorld and establish ownership/unloading. The current PrimitiveArt caches are global to the one active game: overlapping old/new RunManagers during an additive transition is unsafe because the old manager clears those caches. Use a safe single-scene unload/load boundary initially, or implement scoped caches before overlapping regions. Do not claim seamless streaming is already available.
4. Store region content under per-state folders/scenes with explicit references or locally built bundles. Avoid placing whole future art libraries in Resources or using LoadAll. Reuse shared cotton/road materials, base meshes, runner/clothing, token/pouch art and common effects. Keep state-specific atlases and audio compressed, with modest texture resolution and art/triangle limits informed by the phone baseline.
5. Gate each new region on measured active-region memory/startup/frame times and cleanup after switching. Adding more local states can still increase the installed APK even when inactive states are unloaded; memory isolation does not eliminate installation size. Optional downloaded packs would change the current offline delivery policy and are not introduced here.

Punjab/Kerala can follow this design without a backend. Research authentic visual/dialogue details before final assets, and keep repository creation, paid purchases and publication under existing human gates.

References checked during implementation:
- https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/mesh/uploadmeshdata
- https://docs.unity.com/en-us/engine/6000.3/manual/assets-and-media/assets-managing-runtime/assets-resources-system/load
- https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/scenemanagement/scenemanager/loadsceneasync

This pass was verified with source, ownership-model, visibility/state and asset-hash checks. Unity compilation/rendering, actual native memory release, final build size and phone profiling remain unverified in this workspace.
