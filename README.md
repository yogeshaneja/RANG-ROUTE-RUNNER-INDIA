# Rang Route Runner India — Unity prototype

An original offline Android 3D endless runner. **Stage 1 source implementation**, with procedural Rajasthan-inspired scenery and temporary character art. This is Unity/C#, not Expo or a React Native simulation.

## Open and play

1. Install **Unity Hub** and a supported **Unity 6.3 LTS** patch, including Android Build Support, Android SDK & NDK Tools and OpenJDK. The project baseline is `6000.3.0f1`; use your installed supported 6.3 patch when Hub offers to upgrade. This baseline is a project target, not an Editor version verified on the author's machine.
2. Clone this repository and check out `feature/rajasthan-unity-prototype` (or use `main` after the PR is merged).
3. In Unity Hub choose **Add > Add project from disk** and select the repository folder containing `Assets`, `Packages` and `ProjectSettings`.
4. Wait for Unity to import and compile. Open **Indian Runner > 1 - Create or open Rajasthan scene**. This also prepares the correct build material for the active render pipeline. Unity now prepares it automatically after script import and again before a build.
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
5. The Rajasthan runner launcher icon is applied automatically after import and before building (including Android adaptive, round and legacy slots supported by your Editor). Pull these changes and Build and Run again to update the installed phone icon. If needed, use **Indian Runner > Apply Rajasthan app icon**.
6. Disconnect USB after installation and verify the game works in airplane mode. No store account is needed for direct testing.

Build And Run requires your licensed local Unity Editor and compatible Android toolchain. Neither an APK nor an Android build result is included in this PR. Final package identity and release signing are not configured.

## Included

- A real three-lane 3D runner with automatic forward movement, swipe/keyboard input, eased lane changes, gentle camera follow, jump and slide.
- Original photo-like Rajasthan panorama, weathered sandstone on 3D building walls and woven cotton on the runner's clothes. The old flat illustrated storefront cards are no longer drawn. Chhatris, pots, shrubs and native 3D architecture remain part of the roadside.
- Seven reusable track segments: scenery meshes combined per material, pooled obstacle rows and coins.
- Rajasthan-themed obstacles: painted matka stacks and colourful fabric bundles to jump, wooden market carts to dodge, cloth awnings to slide under, and decorated camel crossings with directional warnings.
- Flying festival kites above the road; kites have no collider and never hide a lane hazard.
- A guaranteed empty lane per row, 28 m row separation, capped speed, and swept collision checks to reduce tunnelling.
- Distance + ten points per coin, best-score persistence, restart, manual/lifecycle pause and explicit resume.
- Original articulated Indian male runner with smooth shaped geometry, facial/hair details, tailored teal kurta, saffron scarf and brown shoes. Hip/knee/ankle and shoulder/elbow articulation drives sprint, jump and crouched slide poses; a soft contact shadow grounds the figure. This is an original procedural character, not an imported production skinned model.
- Varied three-dimensional sandstone courtyard houses, recessed archways/jharokha windows, market awnings, acacia trees, uneven sandy shoulders, scenic camels, peacocks and birds. Animals are scenery, not collision targets.
- Gradual sunny/cloudy/light-rain cycle, weather preview selector, and reduced effects.
- Separate music/sound controls, locally saved preferences and original synthesized placeholder cues/music.
- Portrait-safe-area HUD and temporary English prototype menu.

## Deliberately later stages

- An artist-authored production character, skinned clothing and polished animation clips; current articulation/foot placement is procedural.
- Hindi localization with a licensed Devanagari font and verified shaping. **Hindi is not implemented or verified.**
- Punjab, Kerala, Tamil Nadu, West Bengal, Assam, forests, region selection and Journey mode.
- Region ScriptableObjects, separate Bootstrap/Menu/Game scenes, and reusable authored prefabs. Current Stage 1 geometry is generated in one scene.
- Monsoon, mist, sunset/sunrise, animal crossings, moving traffic and polished regional audio.
- Production uGUI/UI Toolkit screens, accessibility review and text-size controls. Current IMGUI menu is a prototype control surface.
- Real-phone profiling, input-latency checks, release signing and store preparation.

There are no cloud calls, backend, accounts, ads, purchases, analytics SDKs or Play Store button. Future regions are not advertised as playable.

## Validation

Run **Indian Runner > 3 - Run core rule checks** in Unity for deterministic safe-lane, camel-envelope, row-spacing, speed, score, jump-recovery and swept-collision assertions. It reports PASS only if executed successfully in your Editor.

See [Documentation/VALIDATION.md](Documentation/VALIDATION.md) for the exact checks performed here and the remaining phone checklist. Source parsing is not a substitute for a Unity compile or playtest.

## Structure

- `Assets/IndianRunner/Runtime/RunManager.cs`: run state, HUD, lifecycle, settings and score orchestration.
- `RunRules.cs`: deterministic layout and numeric gameplay rules.
- `RunnerInput.cs`: touch, mouse and keyboard adapter; supports either Unity input backend.
- `TrackWorld.cs`: segment recycling, obstacle layout, camel warnings/motion, coins and swept AABB collision.
- `RajasthanObstacles.cs`: original shared-mesh pottery, textile bundles, cart, awning and decorated camel models. No obstacle geometry is instantiated when track rows recycle.
- `RunnerModel.cs` / `PrimitiveArt.cs`: original procedural model, geometry and mesh combination.
- `WeatherController.cs` / `RunnerAudio.cs`: pooled weather and generated audio.
- `Assets/IndianRunner/Editor/RunnerSetup.cs`: setup, Android configuration and focused checks.

The illustrated frontages use reusable flat panels; they add detail without claiming full 3D building models. Both images and the small unlit scenery shader are bundled in Resources, so no network is used during play. Texture import is capped at 1024 pixels with mipmaps and compression. There is no new package dependency. See [Documentation/ARTWORK.md](Documentation/ARTWORK.md) for provenance and prompts.

For this scenery pass, lane easing is capped at the prior maximum lateral speed, camera follow uses delta-time-based damping, HUD text refreshes at 10 Hz, and inactive/distant coins and birds skip transform animation. These are implementation optimizations, not a measured frame-rate improvement; profile your actual phone.

All world motion is relative to the runner, keeping rendered coordinates near the origin. Distance and score are session state; best score and settings use PlayerPrefs. Progress deletion can be performed by clearing Android app data. PlayerPrefs is appropriate for this prototype; it is not secure competitive scoring.

## Rajasthan obstacle update

Jump the pottery and fabric bundles; dodge wooden carts. Slide through the middle of a cloth awning: its hanging cloth and its two support posts have separate collision bounds. Follow LEFT/RIGHT warnings for camels; camel contact causes a gentle stop with no injury. Crossing rows reserve an outer escape lane and contain no competing static obstacles. The warning begins 45 metres ahead (2.5 seconds at maximum speed). Row spacing increased to 28 metres for the larger models.

This pass uses original low-poly geometry, with existing original generated images retained for background/side scenery. A CC0 wood source was verified but could not be downloaded in this environment; no stock texture or model from that source is bundled. See Documentation/RAJASTHAN_OBSTACLES.md for checks and provenance.

## Natural scene and avatar pass

The roadside now alternates open and denser village layouts, with varied setbacks, building dimensions, rooflines and small rotations. Recessed arched doors, lattice windows, projecting sills and angled shop shades replace repeated plain houses. Distant illustration panels appear sparsely. Acacias, stones, natural ground grain, subtle off-road height variation, layered clouds and warmer lighting reduce the repeated geometric appearance. The playable road stays flat and collision/spawn rules stay unchanged.

The runner has rounded custom meshes, richer outfit/face/hair details, articulated limbs and inverse-kinematic foot placement. A short sprint stance moves feet backwards at road speed, with a longer knee-lift recovery. Sliding uses a crouched/back-leaning pose instead of scaling the whole body. The camera is closer and narrower (62 degrees), with stronger damped horizontal follow; the menu initially faces the character towards the camera. Built-in rendering requests 2x MSAA; URP keeps its project AA settings. No performance result is implied.

See Documentation/NATURAL_VISUALS.md for scope, source verification and local checks. Unity rendering, shader compilation, clipping during movement and phone frame/memory measurements still require local testing.

### RunnerBase GUID / Broken text PPtr build error

The source template no longer hardcodes a native shader GUID or fileID. The installed Unity Editor assigns and serializes the active pipeline shader after scripts import and before packaging. The explicit repair menu also saves, synchronously reimports and validates the material.

After pulling this repair, stop Play Mode and let scripts recompile. Run **Indian Runner > Repair runner materials**, clear the Console's old entries, and retry **Build and Run**. Do not manually edit shader GUIDs. A successful repair is not itself evidence that the whole Android build has passed.

## Art and feel polish pass

- New original photo-like dune/fort panorama, sandstone albedo and neutral woven-cotton albedo; all bundled offline. The panorama imports at 1024 pixels; repeating material maps at 512 with mipmaps/compression. These are generated images, not location photographs.
- Immediate one-shot swipe recognition on threshold crossing; lifting the finger does not generate another action. Short taps, header gestures and cancelled touches do not issue a move.
- A character-focused start camera and 0.7-second eased transition into running; reduced effects skip the transition and speed-FOV effects.
- Varied elbow movement and subtle idle breathing, small lane-turn yaw, a cleaner rounded dark/gold interface, clear pause/results navigation, bounded coin glints and distance milestones.
- Roadside mesh groups receive small deterministic offsets/scale changes when reused. Road, ground and gameplay obstacles are not moved by this decoration variation.
- Texture seams on rounded meshes use duplicated UV vertices with averaged seam normals.

The artwork/feel source pass is complete; its Unity import, UI touch behaviour, camera framing and frame-time/memory impact still require the local phone build. The character remains procedurally articulated; a production skinned character and authored run animations are not included. See Documentation/ART_AND_FEEL.md.

Latest character/hazard update: rounded walking camels, refined original boy, oncoming motorcycles and descending spaceships with warned escape lanes and contact game-over. See [moving hazards and local playtest](Documentation/MOVING_HAZARDS.md). Pulling source requires a fresh Android Build and Run to update the installed phone game.

Latest festival pass replaces coin visuals with turquoise/gold mirror-work tokens, adds layered original offline music, softer pickup/jump/game-over effects, fading/panning motorcycle and spaceship sounds, occasional camel/elephant calls, larger obstacle meshes with matching collision bounds, decorated roadside elephants and larger scenic animals/kites. The main boy is unchanged. See [audio, ornament details and phone acceptance](Documentation/FESTIVAL_AUDIO_AND_TOKENS.md). Audio is synthesized and must be auditioned on the phone; no FPS or production-realism claim is made.

Latest progression/pacing update: three saved festival goals (tokens, actual awning slides, best distance in one run), claimable outfit colours, local festival badges, eight-second token-attracting pouch, clear flower/bunting garden sections with gentle musical lift, and rare spaceship encounters. First garden 280 m, pouch 322 m, spaceship 910 m. See [goals, rewards and required playtest](Documentation/FESTIVAL_GOALS_AND_GARDENS.md). Claim rewards and change outfits from Start; rebuild the Android APK after pulling.

Five new encounter motifs: rolling cart wheels (70 m), falling fabric (238 m), peacock displays (406 m), repair sand/scaffolding (518 m) and festive cart processions (574 m). Distinct colours, warning sounds and reserved token lanes; moving events clear adjacent rows and preserve garden gaps. Successful scaffold slides now also count toward the low-barrier goal. See [encounter details and phone checks](Documentation/FIVE_THEMED_ENCOUNTERS.md). Pulling requires a fresh Android Build and Run.

Latest boy refinement: continuous sculpted face and swept hair, more natural proportions, tailored kurta, smaller shaped shoes/hands, soft skinned elbows/knees, foot roll, blinking and a three-joint scarf. Original procedural art using existing outfit colours; no external account or downloaded character needed. See [runner changes and visual playtest](Documentation/SCULPTED_RUNNER.md). Pull, press Play and run the live startup check; then Build and Run for Android. Native Unity skinning/rendering and phone performance still require local verification.

Spoken road banter: **Hindi is now the default**. LANGUAGE switches between Hindi and English; PLAY VOICE auditions the current language. Bundled recordings are preferred and work without phone speech data when all 16 clips are supplied. **All 16 Hindi recordings are now included**, cut from the supplied male/female batches with spoken GAP removed. Hindi uses the bundled clips without phone speech data; English still uses optional device speech. See [recording status, file names and phone checks](Documentation/SPOKEN_ROAD_BANTER.md) and Documentation/DIALOGUE_RECORDINGS.csv. Pull and Build and Run again.

Rain-only encounter: a brightly dressed woman with a turquoise/pink umbrella, marked lane and the spoken line “Dekho, baarish ho rahi hai! It's raining, it's raining!” New visitors are admitted only during actual rain and far ahead; the token lane stays clear. Select Sky → Rain, use the voice test on Android, then reach the first candidate at 182 m. See [umbrella visitor and phone checks](Documentation/RAIN_UMBRELLA_VISITOR.md). Pull and Build and Run again; native speech uses installed offline Hindi or English.

Lightweight cleanup: final static mesh batches release construction copies and CPU-readable output data, use smaller indices where possible, and avoid redundant weather/token/blink updates. Two unused legacy JPEGs (570,268 source bytes) are preserved outside build assets in Documentation/ArtArchive. Active visuals/gameplay stay the same. Play Mode menu 5 inspects owned mesh memory. See [phone profiling and future state-loading design](Documentation/LIGHTWEIGHT_REGIONS.md); multi-region loading is not implemented yet, and actual APK/RAM/FPS must be measured on the phone.

Simple voice/reward refinement: the old CAST/AUTO setup is removed; Hindi defaults on and English remains selectable. Hindi recording playback now has all 16 supplied clips; English recordings remain unsupplied. First reward remains 10 tokens, one practice slide and 150 m, with existing progress retained and an outfit reveal when claimed. See the spoken-banter and festival-goals guides. Native speech, recordings and phone behaviour need device verification.

EXIT TO HOME is available on Start, Pause and Game Over in Android builds. It saves progress/settings and opens the phone Home screen while keeping the app available in Recents. Pull and Build and Run to install the update.

Faster start/music: launch speed is now 11 instead of 9, retaining the 18 maximum. A new original 126 BPM festival groove adds stronger drums, claps, shaker and brighter melodic phrases; procession drums match its tempo. Music remains generated offline with no extra audio files. Pull and Build and Run to hear/play the change; phone mix and feel need local testing.

Hindi audio pack: 12 male clips and four female clips now ship in Resources/Dialogue/hi. GAP separators are removed and original pitch/speed retained. Pull, let Unity import, select LANGUAGE HINDI and Build and Run again. Hindi playback no longer requires installed TTS data; audition the cuts on your phone.

Three-second dance rest: near 280 m and every third garden afterward (1,512 m apart), gameplay/controls freeze, REST TIME counts down 3..1, the runner dances facing the camera and a new original 140 BPM groove plays. Running resumes automatically; app backgrounding pauses the remaining break time. The new “Break time!” cue uses an optional recording or available English phone speech, because that phrase is not in the supplied Hindi pack. See Documentation/DANCE_BREAKS.md. Pull and Build and Run; phone behaviour/music/animation still need playtesting.

Sky surprise: the original Thar Cloud Guardian appears around 288 m after the first dance rest, then roughly every 1,008 m. A giant turbaned man emerges through colourful clouds for four seconds, with a gentle wave, mirror glints and a reveal chime. It remains background scenery; the lower gameplay view stays clear. Reduced effects simplify the motion. See Documentation/CLOUD_GUARDIAN.md. Pull and Build and Run; shader/visual/phone performance checks remain local.

Pacing/framing adjustment: dance rests are now three seconds and skip two gardens between appearances. The Cloud Guardian/Maharaja is positioned 4.5 world units lower so the face clears top notifications; the lower-screen sky mask remains in place. Pull and Build and Run again.

Oncoming elephants now reuse the decorated roadside look, walk towards the runner in one warned lane and leave an escape route. First encounter around 462 m, then occasionally; existing elephant Hindi dialogue/trumpet are reused. Game over has a short original raspy comic crank/groan (a non-verbal sound effect). Pull and Build and Run; visual/collision/audio/device checks remain local.
