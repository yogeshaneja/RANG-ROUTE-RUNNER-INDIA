# Rang Route Runner India — Unity prototype

An original offline Android 3D endless runner. **Stage 1 source implementation**, with procedural Rajasthan-inspired scenery and temporary character art. This is Unity/C#, not Expo or a React Native simulation.

## Open and play

1. Install **Unity Hub** and a supported **Unity 6.3 LTS** patch, including Android Build Support, Android SDK & NDK Tools and OpenJDK. The project baseline is `6000.3.0f1`; use your installed supported 6.3 patch when Hub offers to upgrade. This baseline is a project target, not an Editor version verified on the author's machine.
2. Clone this repository and check out `feature/rajasthan-unity-prototype` (or use `main` after the PR is merged).
3. In Unity Hub choose **Add > Add project from disk** and select the repository folder containing `Assets`, `Packages` and `ProjectSettings`.
4. Wait for Unity to import and compile. Open **Indian Runner > 1 - Create or open Rajasthan scene**. This also prepares the correct build material for the active render pipeline.
5. Set Game view to **portrait / 9:16**, then press **Play** and **START RUN**.
6. Use arrow keys or WASD; Space also jumps. P/Escape pauses. Mouse drags emulate swipes.

The route and character appear at runtime; the scene is intentionally an empty setup object outside Play Mode. If no image appears, check Console errors and confirm `Rajasthan.unity` is the open scene.

The starter uses the built-in render pipeline with no third-party packages. The material resolver uses the configured pipeline (including Quality overrides) and assigns its default shader to every generated material. Imported Standard templates cannot override that shader in a URP project. Runtime rendering in each pipeline still requires local verification.

### If surfaces appear bright pink

1. Stop Play Mode and pull the latest feature branch.
2. Wait for Unity to finish recompiling.
3. Select **Indian Runner > Repair runner materials**. This updates the saved shader dependency for your current pipeline, including Android builds.
4. Choose **9:16 / portrait** in the Game view aspect dropdown, then press Play again.

The runtime also repairs cloned material shaders automatically. If surfaces remain pink, copy the first shader-related Console error and the Editor version; do not convert unrelated project materials or delete your project settings.

## Android phone

1. Run **Indian Runner > 2 - Configure Android prototype**. This sets portrait orientation, an internal-test application ID, ARM64 and IL2CPP. It does not build or publish anything.
2. In **File > Build Profiles**, add/activate Android. Confirm the Rajasthan scene is enabled in the build scene list.
3. Connect the phone via USB, enable Developer options / USB debugging and accept its authorization prompt.
4. Select the phone as Run Device and choose **Build and Run** with an APK output. Use Unity Hub's SDK/NDK/JDK for your installed Editor, not an arbitrary system Java installation.
5. Disconnect USB after installation and verify the game works in airplane mode. No store account is needed for direct testing.

Build And Run requires your licensed local Unity Editor and compatible Android toolchain. Neither an APK nor an Android build result is included in this PR. Final package identity and release signing are not configured.

## Included

- A real three-lane 3D runner with automatic forward movement, swipe/keyboard input, eased lane changes, gentle camera follow, jump and slide.
- Original bundled Rajasthan illustrations: a distant Thar-dune/fort panorama and haveli/bazaar frontages beside both sides of the road, plus chhatri pavilions, pots, shrubs and sandstone walls.
- Seven reusable track segments: scenery meshes combined per material, pooled obstacle rows and coins.
- Three colour-coded obstacle types: yellow low crate (jump), teal overhead barrier (slide), coral tall barricade (avoid).
- A guaranteed empty lane per row, 26 m row separation, capped speed, and swept collision checks to reduce tunnelling.
- Distance + ten points per coin, best-score persistence, restart, manual/lifecycle pause and explicit resume.
- Original primitive character in a teal kurta and orange scarf, with procedural limb motion; no imported rig yet.
- Sandstone-inspired houses, market awnings, scenic camels, peacocks and birds. Animals are scenery, not collision targets.
- Gradual sunny/cloudy/light-rain cycle, weather preview selector, and reduced effects.
- Separate music/sound controls, locally saved preferences and original synthesized placeholder cues/music.
- Portrait-safe-area HUD and temporary English prototype menu.

## Deliberately later stages

- A rigged production character and polished run/jump/slide/stumble animation clips.
- Hindi localization with a licensed Devanagari font and verified shaping. **Hindi is not implemented or verified.**
- Punjab, Kerala, Tamil Nadu, West Bengal, Assam, forests, region selection and Journey mode.
- Region ScriptableObjects, separate Bootstrap/Menu/Game scenes, and reusable authored prefabs. Current Stage 1 geometry is generated in one scene.
- Monsoon, mist, sunset/sunrise, animal crossings, moving traffic and polished regional audio.
- Production uGUI/UI Toolkit screens, accessibility review and text-size controls. Current IMGUI menu is a prototype control surface.
- Real-phone profiling, input-latency checks, release signing and store preparation.

There are no cloud calls, backend, accounts, ads, purchases, analytics SDKs or Play Store button. Future regions are not advertised as playable.

## Validation

Run **Indian Runner > 3 - Run core rule checks** in Unity for deterministic safe-lane, row-spacing, speed, score, jump-recovery and swept-collision assertions. It reports PASS only if executed successfully in your Editor.

See [Documentation/VALIDATION.md](Documentation/VALIDATION.md) for the exact checks performed here and the remaining phone checklist. Source parsing is not a substitute for a Unity compile or playtest.

## Structure

- `Assets/IndianRunner/Runtime/RunManager.cs`: run state, HUD, lifecycle, settings and score orchestration.
- `RunRules.cs`: deterministic layout and numeric gameplay rules.
- `RunnerInput.cs`: touch, mouse and keyboard adapter; supports either Unity input backend.
- `TrackWorld.cs`: segment recycling, obstacle layout, coins and swept AABB collision.
- `RunnerModel.cs` / `PrimitiveArt.cs`: original procedural model, geometry and mesh combination.
- `WeatherController.cs` / `RunnerAudio.cs`: pooled weather and generated audio.
- `Assets/IndianRunner/Editor/RunnerSetup.cs`: setup, Android configuration and focused checks.

The illustrated frontages use reusable flat panels; they add detail without claiming full 3D building models. Both images and the small unlit scenery shader are bundled in Resources, so no network is used during play. Texture import is capped at 1024 pixels with mipmaps and compression. There is no new package dependency. See [Documentation/ARTWORK.md](Documentation/ARTWORK.md) for provenance and prompts.

For this scenery pass, lane easing is capped at the prior maximum lateral speed, camera follow uses delta-time-based damping, HUD text refreshes at 10 Hz, and inactive/distant coins and birds skip transform animation. These are implementation optimizations, not a measured frame-rate improvement; profile your actual phone.

All world motion is relative to the runner, keeping rendered coordinates near the origin. Distance and score are session state; best score and settings use PlayerPrefs. Progress deletion can be performed by clearing Android app data. PlayerPrefs is appropriate for this prototype; it is not secure competitive scoring.
