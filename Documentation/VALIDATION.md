# Validation record

## Performed in the implementation workspace

- C# grammar parsing: all fifteen runtime/editor files passed without syntax errors using tree-sitter C#.
- JSON manifest parsed successfully.
- All asset/folder metadata exists; GUIDs are unique; scene references resolve to the included RunManager script and enabled build scene.
- Analytical timing bounds checked using constants extracted from RunRules.cs:
  - maximum-speed clear interval between obstacle envelopes: 1.396 seconds;
  - worst-case eased two-lane traversal: 0.568 seconds;
  - remaining reaction allowance: 0.828 seconds;
  - jump flight time: 0.750 seconds;
  - jump apex: 1.688 metres, above the 0.95-metre low obstacle.
- Manual source review covered segment recycling, relative coordinates, collision sweep, coin reset, pause/resume, settings, shader references and source asset provenance.

These are source checks, not a C# compilation or Unity runtime result. The workspace has no Unity Editor, licensed build environment, Android toolchain or attached phone. No APK/AAB, Unity Play Mode run, real-phone test, sustained profiling result or screenshot is claimed.

## Ready to run in Unity

Use Indian Runner > 3 - Run core rule checks. The included assertions cover 32,000 seeded obstacle rows, safe-lane count, obstacle kinds, lane-change/reaction spacing, jump recovery, speed cap, score calculation and several swept collision cases. These assertions have NOT been executed here.

## Required manual acceptance checklist

Record Editor patch, phone model, Android version, build ID, duration and actual outcome before marking items complete.

- [ ] Project imports and compiles with zero Console errors in the selected Unity 6.3 LTS patch.
- [ ] Rajasthan scene launches with readable materials, original runner and route geometry.
- [ ] Keyboard, mouse swipe and phone swipe work in both directions; one gesture produces one action.
- [ ] Pots/fabric can be jumped; awning centres can be slid under; awning posts and wooden carts require avoidance.
- [ ] Camel warnings appear at 45 metres; the indicated escape lane stays clear throughout movement; contact produces a gentle stop with no injury.
- [ ] Kites are above the route, decorative only, and respect reduced effects.
- [ ] Following each open lane remains feasible at maximum speed during a sustained run.
- [ ] First and last coins in each five-coin group can be collected, only once per coin.
- [ ] Restart resets coins, distance, slide/jump state and character pose; prior runs do not continue scoring.
- [ ] Pause freezes character, scenery, score and weather; explicit resume continues without a large time jump.
- [ ] Backgrounding or losing focus pauses; returning does not auto-resume into an obstacle.
- [ ] Best score and music/sound/reduced-effects preferences survive app restart.
- [ ] Sunny/cloudy/light-rain changes preserve obstacle visibility; reduced effects lower rain count.
- [ ] Menu/HUD respect portrait safe areas on the target screen and buttons respond to touch.
- [ ] APK installs through USB and plays independently in airplane mode.
- [ ] At least 10 minutes of phone play is profiled for frame times, memory growth and heat; optimize measured issues before claiming a performance target.
- [ ] Phone logs show no runtime exceptions.

Hindi rendering, additional regions, a production character rig and polished production UI are Stage 2/3 work, not completed acceptance items.

## Material compatibility regression

A user screenshot showed magenta surfaces in Play Mode. Source review found that runtime clones inherited the saved RunnerBase shader even when a different pipeline shader had been selected. Pipeline detection also relied on currentRenderPipeline during Awake.

The repair explicitly rebinds the selected pipeline's default shader, using the Quality override then the Graphics default before the runtime pipeline. It clears stale keywords and writes both supported colour properties. The repair menu persists that shader in the Resources material for build inclusion.

C# syntax checks passed after the repair. The Unity core-check menu now includes shader/tint assertions, but these have not been run here. Required local regression: import a Standard RunnerBase material into a URP project, stop/play without first repairing the asset and verify colours; then run Repair runner materials and verify the saved dependency and Android build. Also confirm built-in rendering still works. No visual verification is claimed from this workspace.

## Rajasthan scenery and smoothing pass

Performed here: C# syntax parsing for all ten files; complete/unique asset metadata checks; both JPEGs decoded successfully. Numerical easing bounds at 20/30/60/90/120 Hz confirm endpoint arrival, no overshoot and lateral speed no greater than 14 m/s for one- and two-lane moves. Row spacing still leaves approximately 0.772 seconds after worst-case eased lane traversal, above the configured 0.7-second allowance. These are mathematical checks, not execution of Unity's player loop.

Implemented: original bundled panorama and haveli/bazaar image panels; reusable side details; one shared material per illustration; compressed/mipmapped 1024-pixel texture imports; eased lane changes and delta-time-based camera damping. Coin/bird transform updates skip distant objects, coin rotation is calculated once per frame, HUD strings update at 10 Hz and GUI styles are reused.

The core-check menu also validates scenery resources/shader availability and easing endpoints/monotonicity. These Editor assertions remain unexecuted here. Local checks required before accepting this pass:

- [ ] Both illustrations render in portrait without pink shader errors; frontages sit outside the playable road and their skyline trim leaves turquoise shop details readable.
- [ ] Background fort/dunes remain behind road and obstacles, without depth clipping in the active pipeline.
- [ ] Side panels are reused with track tiles, and restart does not duplicate scenery or materials.
- [ ] New scenery shader compiles for the active built-in/URP pipeline and Android graphics API.
- [ ] Lane changes and rapid reversals feel responsive; no overshoot or unfair collision appears at maximum speed.
- [ ] Pause freezes the camera, lean and lane transition; restart clears easing state.
- [ ] Compare frame-time and memory captures before/after on the target phone; no frame-rate improvement is claimed without profiling.

This pass does not change Android export, add paid/downloaded stock assets, add regions, replace the runner rig or publish the application.

## Themed obstacles and camel-crossing pass

Source checks performed: all eleven C# files parse. An independent numerical audit of 32,000 deterministic row layouts found at least one reserved open lane in every row, all five obstacle types, and no static obstacle sharing a camel row. Sampled crossing envelopes stayed at least 0.910 metres away from the player's reserved-lane envelope. With 28-metre spacing and a conservative 1.1-metre obstacle half-depth, the maximum-speed interval is 1.396 seconds; the eased two-lane move takes 0.568 seconds, leaving 0.828 seconds of reaction allowance. Camel warnings start 45 metres ahead, giving 2.5 seconds at maximum speed. These are independent mathematical/source checks, not a Unity runtime result.

Editor rule checks were updated for the five types, open-lane reservation and camel sweep envelope; they remain unexecuted here. Manual local checks must cover sliding between awning posts, jumping both low objects, cart avoidance, warning direction, moving-camel relative collision, row recycling, no duplicate models after restart, offline behaviour, and sustained phone performance. See RAJASTHAN_OBSTACLES.md.

## Natural environment and articulated-avatar pass

Completed source checks: all twelve C# files parse. An independent mathematical audit sampled 8,000 sprint foot targets across speeds 9/12/15/18; all stayed within the 0.880 m two-bone leg length (maximum sampled reach approximately 0.869 m), and forward reconstruction of the solved joints matched targets to numerical precision. The new decorative terrain remains flat across the playable road, and its elevation/first derivative return to the same baseline at tile ends. New shader source blocks balance and asset metadata/GUID checks pass. None of these checks execute Unity or certify shader compilation.

The update replaces rigid avatar limbs with rounded original meshes, hip/knee/ankle placement, bent elbows and a crouched slide; it introduces a soft analytic contact shadow. Environment changes reduce regularity through differing house counts/setbacks/heights/rotations, recessed arches/lattice details, acacias, sparse illustration panels, deterministic ground grain, off-road height variation and layered cloud meshes. Startup-generated texture/material/mesh ownership is explicitly cleaned up. Existing obstacle spawn/collision rules remain unchanged.

Before acceptance, locally verify: model materials/normals and contact shadow; no foot/road penetration or excessive sliding; jump and slide transitions; pause/restart pose reset; outer-lane and slide framing in 9:16; no terrain seams; no decorative object masking a hazard; preserved obstacle collisions/escape warnings; and frame-time/memory behaviour on the target phone. This is still original stylised procedural art, not a photorealistic or artist-authored production asset. No Unity screenshot, visual approval or FPS measurement is claimed here.

## RunnerBase native shader-reference build repair

A Windows Unity Android build reported `Could not extract GUID` in RunnerBase.mat and `Broken text PPtr` for the shader reference. The hand-authored native shader pointer was removed. The tracked source template now has a valid null shader reference, and the user's installed Editor prepares/serializes the actual pipeline shader through AssetDatabase after script import and before a build. Repair saves, synchronously reimports and validates the shader on the saved material. The core-check menu now checks the serialized template as well as a runtime material.

Source validation: the source template contains no hardcoded shader GUID/fileID, all thirteen C# files parse, and asset GUID metadata remains complete/unique. Native material reserialization, shader compilation and Android build success cannot be verified without Unity and the user's toolchain. The next required check is to pull, run Repair runner materials, clear old Console entries and rebuild on the connected phone.

## Art-and-feel polish validation

The user reports the preceding build runs on their phone. The following checks concern new source and do not certify this newer APK:

- All fifteen C# files parse without grammar errors; complete/unique asset metadata checked; the three JPEGs decode.
- Mesh index/count/UV contracts audited for 16 ring/sides combinations. Side triangles no longer interpolate across the texture wrap seam; matching seam normals are averaged.
- Independent gesture-state traces cover early threshold commit, no second action on release, short taps, header exclusion and cancellation. These are mathematical/contract traces; native touch still needs phone testing.
- Camera intro finishes before the first obstacle window. Menu-sheet bounds were checked against sampled 16:9, 19.5:9 and 20:9 portrait aspects. Actual text rendering/framing needs Unity.
- Native material preparation/build repair is retained; the source template still contains no manually encoded shader GUID.
- Coin effects use 18 preallocated objects; mesh/material caches and skin textures are disposed. No frame-rate, GC or memory-growth result is claimed without profiling.

Local acceptance: rebuild and verify realistic surfaces/no pink errors; full character and controls on the start screen; intro and reduced-effects behaviour; immediate single-action swipes; pause/resume/restart/back-to-start; score/coin accuracy; glints/milestones; reserved camel escape lanes; long-run recycling; and sustained Android frame-time/memory captures. Production character/rigged animation quality remains outside this procedural-art pass.

## Character and moving-hazard pass

All 15 C# files parse, and asset metadata remains complete with unique GUIDs. An independent numerical audit covers 32,000 deterministic layouts, all seven obstacle types, camel swept escape envelopes, empty rows adjacent to dynamic encounters, and 48 moving-hazard collision/safe-lane traces across speeds 9/12/18 m/s and 20/30/60/120 Hz. Direct motorcycle/ship contacts register while the neighbouring lane stays clear. This audit independently mirrors formulas; it does not execute C# or Unity.

Editor core checks now include dynamic type bounds, reserved lanes, recovery rows, descent endpoints, oncoming direction and relative collision sweeps. They have not run here. Unity shader/mesh rendering, native compilation, actual visual quality and phone performance are pending the local Build and Run review in MOVING_HAZARDS.md. Existing native material repair is preserved.

## Startup freeze repair

The moving-hazard change declared and used Row.LandingMarker but omitted its construction. RunManager.Awake built the scenery, then world.Reset called Configure and dereferenced a null marker before the runner, camera placement and menu dependencies were ready. This explains the reported scenery-only view; no device/Editor exception log was supplied to independently confirm it.

TrackWorld now creates, parents, batches and initially hides one landing marker in every pooled row before the first Reset. All 14 rows get markers, including empty rows that can later recycle into spaceship encounters. A source regression check verifies unconditional constructor assignment before marker access. Added a live Play Mode menu check (Indian Runner > 4 - Check live startup) to inspect actual initialized runner dependencies and every pooled marker/obstacle/coin reference. This live check remains unexecuted here because Unity is unavailable. Rebuild after verifying the start menu, starting/restarting a run, the spaceship marker at 238 m and recycling beyond 600 m locally.

## Festival audio, ornaments and enlarged props

All 16 C# files parse. The source regression confirms all 14 landing markers are allocated before use, alongside 42 obstacle and 70 collectible slots; three elephant scenery groups are bounded by the tile pool. Metadata is complete and GUIDs are unique. An independent numerical audit of 32,000 enlarged-layout envelopes finds the minimum camel escape clearance is 0.742 m, enlarged low obstacles remain jumpable, slides fit the awnings and landing-marker bounds contain the saucer. Editor envelope checks have been updated for enlarged camels/ships but remain unexecuted.

An independent NumPy reproduction of the music formulas yields finite PCM with approximately 0.310 peak / 0.079 RMS before AudioSource volume and a zero endpoint seam; both one-second approach loops are periodic. Source inspection checks pause/retrigger guards and no object/clip allocation in audio Tick. These checks are not an auditory quality assessment, C# execution or Unity profiling result. See FESTIVAL_AUDIO_AND_TOKENS.md for the required phone/Editor acceptance checks and precise asset limitations.

## Goals, rewards, pouch power and garden pacing

All 17 C# files parse. Asset metadata is complete/unique. Startup source checks cover unconditional initialization of all 14 landing markers, pouches and gardens before Reset; pooled taken/slide flags reset on recycling. An independent 32,000-layout audit finds reserved routes intact, all garden/transition rows clear, every pouch row safe, and neighbouring moving encounters outside the garden interior. Spaceship timing is now first 910 m, repeat 2,016 m. Source inspection verifies the active-run timer, per-row deduplication, actual-awning slide condition, menu-only reward claim and modal pause guard.

Independent state traces cover save/reload, cumulative tokens/awning passes, non-cumulative run distance, incomplete/double claim rejection, four outfit colours, subsequent badge counts, invalid-save bounds, once-only pickup and pause-safe expiry. Sampled portrait UI rectangles fit virtual heights 800/853/960/1040; actual fonts/device safe areas remain a manual check. C# Editor core checks include corresponding progress and garden/rarity assertions, but were not run here. These independent mirrors/source contracts do not execute C# or establish Unity/Android correctness. See FESTIVAL_GOALS_AND_GARDENS.md for local acceptance and profiling.

## Five themed encounters

All 18 C# files parse. A new independent 32,000-layout audit covers all 13 obstacle types, reserved lanes, both procession instances, all moving recovery rows, and unchanged empty gardens/pouch rows. Sampled new dynamic swept envelopes retain at least 1.114 m of reserved-lane clearance; prior camel envelope checks remain applicable. There are 156 numeric moving-hit/safe-lane/wheel-jump traces across 9/12/18 m/s and 20/30/60/120 Hz. Repair pile jump and scaffold slide clearance are checked independently.

Wheel ring audits verify triangle/index counts, nondegenerate faces and axis-centred radius bounds. Independent PCM reproduction checks wheel/procession loop samples are finite, below 0.78 peak and share matching endpoints; procession length is four beats at 96 BPM. Startup pool construction, motion-base/mesh-offset separation, both procession transforms, no encounter instantiation, unique metadata and separate elephant/obstacle sound identifiers were inspected. Editor rule checks cover the new schedule/bounds but were not run here. These tests mirror formulas/contracts and do not execute Unity or C#. See FIVE_THEMED_ENCOUNTERS.md for required runtime, audio and performance review.

## Editor CS0136 repair

The user's Console identified RunnerSetup.cs(106,23): nested local `progress` conflicted with the later outer `FestivalProgress progress` declaration in CheckRules. Renamed the lane-easing sample to `laneProgress` and its two references. The gameplay source is unchanged. All 18 C# files parse, and an additional method/constructor nested-local declaration scan finds no overlapping variable_declarator names. This scan addresses a semantic scope issue that grammar parsing alone missed; it is not a Unity compilation result. Pull, allow Unity to compile, clear historical Console entries and press Play; the core/live-startup checks and Android Build and Run remain local verification.

## Sculpted runner and skinning

All 19 C# files parse and pass the nested-local declaration scan; asset metadata is complete with unique GUIDs. Independent mesh contract checks cover ring indexing and outward winding for both ascending/descending profiles, plus normalized two-bone/scarf weights. A 14,400-target numerical IK audit covers four speeds and sampled jump/slide blends, with maximum reconstruction error approximately 1.1e-15 m. Conservative running sole bounds stay at least 0.0218 m above world zero against the 0.015 m road top. Static full-slide head/arm envelope contracts, owned outfit disposal and no per-frame mesh/object creation are checked. Live startup validation now inspects native skinned mesh/bone/weight/material arrays but remains unexecuted.

These are independent formula/source checks, not C# execution, shader/skin compilation, native bone baking, rendered mesh or Android performance validation. Visual fit during lane lean and transitions, bind-pose correctness in Unity, skinning/culling, face quality, cuff/scarf intersections and sustained phone frame times remain local acceptance checks in SCULPTED_RUNNER.md. Hitboxes and the camel are unchanged.

## Spoken Hindi road dialogue

All 22 C# files parse and pass the nested-local declaration scan, including native preprocessor code. Fifteen unique original Hindi/Roman captions pass script/ASCII and pitch/rate bounds. Independent scheduler traces cover pass windows, per-encounter deduplication, group rotation, nine-second cooldown, pause-safe clock and deterministic reset. Source contracts inspect embedded-Hindi-only selection, not-installed/network filters, preserved OnInit callback, UI-thread initialization, timeout/failure handling, explicit mute/stop/shutdown and no voice download/network calls. Independent manifest-edit mirrors cover missing/existing TTS discovery queries, idempotency and preservation of Activity/other permissions. Metadata and sampled caption/menu bounds pass.

C# Editor core checks include banter scheduler assertions but remain unexecuted. No Unity compiler/Gradle build, JNI invocation, native speech engine, IL2CPP callback, audio listening or airplane-mode phone result was available. The native voice depends on an installed embedded Hindi pack; absence is a nonfatal status/caption fallback. See SPOKEN_ROAD_BANTER.md for exact limits and local acceptance. Unity Editor is caption-only for this feature.

## Rain umbrella visitor

All 23 C# files parse and pass the nested-local declaration scan. An independent 32,000-layout rain-candidate audit finds 1,792 eligible seed/row combinations, all outside protected gardens/moving recovery/repair rows and with an unchanged reserved lane. Admission traces cover dry/late/once-only conditions across sampled speeds/frame rates. Separate body/shaft/canopy sweep traces verify body contact, clear adjacent lanes, walking/sliding beneath the outer edge and jumping into the canopy. Canopy index/triangle checks cover nondegenerate top/bottom layers, opposing normal winding and bounds.

Dialogue data checks verify the exact revised Hindi/English wording, absence of the old invitation, one-line group index safety across 1,000 reset seeds and a sufficient 10.5-second reserved speech interval at sampled speeds. Startup pool reuse, rain/reset guards, no encounter allocation and complete/unique metadata pass. Editor checks include candidate/admission/voice assertions but remain unexecuted. These mirrors/contracts are not Unity/C# execution, rendered collision acceptance or native voice testing; see RAIN_UMBRELLA_VISITOR.md for required local review.

## Lightweight ownership and regional-growth pass

Source/independent ownership traces check opt-in consumption of uniquely owned construction meshes, preservation of imported/built-in shared meshes, reusable non-final builders, finalized CPU-copy release and 16/32-bit index thresholds. All 15 current static merge sites explicitly finalize; the four soft limbs and scarf are not merged/finalized. An independent two-minute weather trace in both effects modes yields unchanged visible states with 116 transition activation writes versus 691,200 unconditional calls in the prior pattern (theoretical script counts, not measured Native CPU/FPS). Token pull/reset and blink scale cache traces retain equivalent states.

Both archived JPEGs retain their exact original Git blob hashes: 570,268 JPEG source bytes move outside Assets/Resources. Active realistic texture import settings remain mipmapped/read-only/configured for compression, and runtime/core checks no longer reference the legacy resources. All 23 C# files parse/pass the nested-local scan; metadata and whitespace checks pass. Existing independent runner IK and rain-visitor safety checks remain passing. A Play Mode owned-mesh inspection menu is added, but no Unity/native memory, APK size or phone frame-time result is available. See LIGHTWEIGHT_REGIONS.md for actual profiling and the required single-active-region/scoped-cache design before adding states.

### Caption-only speech follow-up

Updated Android speech to the API 21+ Bundle overload with an explicit JNI descriptor, media/speech audio attributes, a zero-media-volume hint and logged setup/playback exceptions. Start now exposes RETRY after setup completes/fails, and explains Sound/Voices mute states. Retry replaces/disposes the old engine. No bundled voice recordings or network fallback were introduced. Verify the Android build, voice-pack availability, media/Bluetooth routing, retry after installing Hindi data, Sound/Voices toggles, offline playback and lifecycle stops on the target phone. Native playback remains unverified in this workspace.

Workspace checks for this follow-up: all 23 C# source files passed tree-sitter syntax parsing; AndroidSpeech also parsed after independently selecting its Android and Editor preprocessor branches. Git whitespace checks passed. These are source checks, not a Unity/C# compile or an Android runtime test.

### English dialogue fallback

All 16 lines now include English speech/captions. Saved DIALOGUE AUTO/ENGLISH/HINDI settings choose installed embedded voices; AUTO/HINDI fall back to English if Hindi is missing, while ENGLISH selects English directly. Voice/caption text uses the actual selected language; retry and language changes dispose the old engine, retain mute preferences and clear music ducking. Selection stays out of the per-frame speech path and no clips/models/dependencies are added. Required device checks and limitations are in SPOKEN_ROAD_BANTER.md. Unity compilation, Android builds and audible playback are unavailable in this workspace.

Workspace checks: all 23 C# files passed syntax parsing, including separately selected Android/Editor branches of AndroidSpeech. All 16 dialogue constructors contain nonempty Hindi, Roman Hindi and English strings and the expected argument count. Git whitespace checks passed. These checks do not compile or run Unity/native speech.

### Character voice casting and easier first reward

Added cached, ranked offline voice choices and per-role/language/engine casting, with explicit name-label gender hints for rider/caller (male) and lady (female). Android has no standard gender field; CAST audition/selection is available for opaque names and automatic gender cannot be guaranteed. All 16 line profiles use restrained, varied pitch/rate instead of extreme pitch shifts. No recordings, models, cloud calls or new packages were added. JNI catalog references are disposed with speech instances.

First goals are 10 tokens, one newly started grounded practice slide and 150 m; later sets retain actual barrier-slide goals with reduced thresholds. Old saves clamp to lower targets without clearing progress. Claim closes Goals and displays the unlocked outfit with a brief static reveal. Existing Editor rule checks were updated for 149/150 m completion, old-save migration, single claim and voice role/sample bounds. Those Unity checks have not been executed here; native speech, UI, phone performance and Unity compilation remain manual acceptance.

Workspace results: all 23 C# sources and separately selected Android/Editor speech branches passed syntax parsing. All 16 bilingual lines retained complete role assignments and bounded pitch/rate; reference casting/control row arithmetic fits the cards. Git whitespace checks passed. Editor rule-check updates are source only and were not executed. These checks do not establish a Unity compile, rendered layout, Android build or audible gender/naturalness.

### Simplified Hindi default and recording playback path

New simple-language preference defaults to Hindi on first upgrade/new install; subsequent Hindi/English choices persist. Removed AUTO/CAST and separate retry controls from the main UI. Added stable per-line clip keys and prioritized optional bundled recordings via one owned AudioSource. A complete 16-clip language pack bypasses native TTS initialization; partial/missing packs retain explicit phone-generated fallback in the selected language. Hindi no longer silently becomes English. No audio recordings are actually supplied, so the replacement of mechanical voices is incomplete pending those assets. Recording/source shutdown follows existing mute/lifecycle paths. The sample button retries failed native setup. See the updated dialogue guide and CSV for exact source files and acceptance checks.

Workspace results for simple dialogue: all 23 C# files and separately selected Android/Editor speech branches passed syntax parsing. The CSV has 16 unique bilingual recording entries and none of the referenced audio files is present. A source audit confirmed CAST removal and AudioSource host injection; Git whitespace checks passed. These checks do not compile Unity, play audio or verify Android lifecycle/native speech.

### Exit to phone Home

Added EXIT TO HOME to Start, Pause and Game Over. Saves best score, festival progress and settings and stops dialogue before launching Android ACTION_MAIN + CATEGORY_HOME with FLAG_ACTIVITY_NEW_TASK on the activity UI thread. This explicitly opens the phone launcher and preserves the game task in Recents; it does not force-quit, clear progress, or return only to the game's Start menu. JNI activity ownership lasts through the asynchronous UI callback. Editor/desktop logs an Android-only hint. Existing background callbacks keep an active run paused.

Phone acceptance: use the button from Start, Pause and Game Over; expect the actual launcher even if the game was opened over another app. Reopen from icon/Recents, verify saved progress/settings and a paused run, and confirm no dialogue continues after leaving. Android may reclaim background processes, so restoration of a live run after process death is not promised. Unity build, launcher behaviour and rendered layout require device verification.

Workspace verification: all 23 C# files passed syntax parsing and Git whitespace checks passed. Start and pause/result button bounds fit their enlarged cards by inspection. No Unity compilation, rendered phone layout or launcher transition was executed here.

### Upbeat music and faster start

Changed original music from 96 to 126 BPM with a denser eight-bar melodic/percussion arrangement, stronger music level, lighter token ducking and matching four-beat procession tempo. Precomputed note frequencies; retained 22,050 Hz mono and the same AudioSource count. The shorter loop has 336,000 samples (15.238 s), down from 441,000. Starting speed is 11 instead of 9 with the same 18 cap and distance-based acceleration. Updated Editor speed-bound expectation; this Unity check was not run here.

Independent numerical design model: finite audio samples, peak 0.381 (under the 0.78 clip limit), RMS 0.081, zero-valued loop endpoints and exact four-beat procession alignment. This models the synthesis design, not compiled C# playback. At unchanged max speed the row-bound allowance is 1.396 s, exceeding the two-lane change time 0.568 s plus 0.7 s reaction; jump recovery 0.75 s also fits. No Unity build, actual audio audition or phone performance test was possible. Verify mix/loop seams, dialogue/warning audibility, music mute/pause/restart and faster-start controls on Android.

Source verification for this update: all 23 C# files passed syntax parsing; Git whitespace checks passed. These source checks and numerical design checks do not establish a Unity compilation or audible/device runtime result.

### User-supplied Hindi voice clips

Split the latest MALE.mp3 and FEMALE.mp3 into 12 male and four female clips using 11/3 repeated spoken GAP spans. Confirmed repeated-marker waveform correlations above 0.96/0.98. Added all 16 stable Hindi resource keys, Unity folder/audio metadata and source/output-hash cut manifest. Outputs decode to finite mono 24 kHz audio with no full-scale clipping; duration 1.61–4.31 s, total 88,705 compressed source bytes. Source order maps to the chat scripts; no ASR transcription or human audition was available. The original recordings are unchanged.

The existing complete-pack path now has all required Hindi files and bypasses native speech initialization. Editor Check Rules now checks presence and sane duration for all 16 clips; that Unity check has not been executed here. Phone/Editor acceptance: import/compile, select Hindi, Sound/Voices ON, audition every encounter, confirm no GAP bleed/cut words and correct male/female assignment, test Play Voice and rain voice, airplane mode without Hindi speech data, ducking/mute/lifecycle stops and sustained performance. English remains device-generated.

Workspace checks: all 23 C# sources passed syntax parsing; all 16 manifest keys resolve to existing Ogg files and AudioImporter metadata; all Unity GUIDs are well-formed/unique; file SHA-256 values match the cut manifest. Git whitespace checks passed. Full ffmpeg decoding and peak/duration checks passed for all clips. These checks do not execute Unity import, playback or device timing.

### Five-second garden dance break

Added a dedicated DanceBreak state on clear-garden entry, with a five-second foreground-time countdown, frozen run clock/world/scoring, input bypass, large noninteractive rest popup, existing-rig dance animation and close camera framing. Resume re-establishes lane travel and eases the camera back. Background/focus loss pauses the remaining break and resumes through Pause/Continue. Restart/Start cleanup restores run music and stops speech. Added one generated 140 BPM musical loop on the existing music source, restoring the prior track/playhead afterward. A short “Break time!” cue supports optional recorded audio; the missing extra recording uses available offline English phone speech, with a two-second readiness deadline. Existing Hindi clips remain unchanged.

Workspace checks: all 23 C# files passed syntax parsing. Source audit confirmed the break branch returns before input/scoring and the countdown panel has no buttons. Independent timing model covers 5..1 at 20/30/60 Hz and settles exactly at five seconds. Independent settled-pose calculations keep dance foot targets inside leg reach in normal/reduced effects modes. Generated dance groove has 75,600 samples and an analytical signal-envelope bound below the existing 0.78 clip ceiling. These are source/design checks, not Unity runtime tests. Rendered layout, transition poses, background/resume state, audio mix/cue, native speech and phone performance still require the acceptance checks in DANCE_BREAKS.md.

### Thar Cloud Guardian sky event

Added a pooled original giant figure and coloured cloud-bank reveal, four-second duration, gentle animation/glints, reduced-effects mode and existing sound reuse. Safe trigger window begins at 288 m, once per every-other garden, approximately 1,008 m apart. Only Running ticks it; restart/menu/game over reset it. A transparent depth-tested shader masks the lower gameplay view and leaves global weather/collision/camera untouched. Added Unity resource metadata and Editor shader/window/buffer checks. No textures, audio files, particles, extra AudioSource or external package added.

Workspace results: all 24 C# sources passed syntax parsing, all Unity GUIDs are well-formed and unique, and the new assets have metadata. Independent worst-case calculations checked 56 appearance spans through repeated gardens at maximum speed; they remain inside obstacle-free rest buffers. Source review confirmed batching happens before hiding the pooled root. Git whitespace checks passed. The Unity Editor checks, shader compilation, rendered depth/alpha sorting, actual giant framing, sound and phone draw/overdraw/memory behaviour remain untested; see CLOUD_GUARDIAN.md.

### Shorter, less frequent rest and lower Maharaja

Dance countdown is three seconds instead of five. A new DanceRestDistance gate permits every third garden (280 m, 1,792 m, 3,304 m), skipping the two intervening gardens without changing their safe route/scenery. Timer/pause/resume and automatic-control restoration still use the existing duration field. Updated Editor checks cover eligible/skipped gardens. Lowered Cloud Guardian base from y=7 to y=2.5 with the same z=70 and sky mask, so the face is below the top notification band. No new assets or runtime sources added. Rendered phone framing and actual countdown/flow remain manual acceptance.

Checks for this adjustment: all 24 C# files passed syntax parsing and whitespace checks passed. Independent schedule calculations match 280/1,792/3,304 m. Reference portrait projection places the eyes around 151 px from the top, below the usual 134 px notification band; this is a projection estimate, not a rendered Unity/phone test.

### Oncoming elephants and game-over crank

Added pooled obstacle kind 15: original existing elephant body, four shared walking-leg parts, facing/approaching the runner, 1,008 m spacing starting at 462 m, preserved safe lane and empty adjacent rows. Shared elephant builder keeps roadside geometry. Collision uses swept relative coordinates with trunk-aware bounds; warnings/contact messages, earlier trumpet and existing elephant dialogue group are integrated. Pooled part rotations reset on reconfiguration. Added a generated non-verbal raspy game-over crank, mixed with the existing end drum and stopped on restart; no extra AudioSource or speech asset.

Workspace checks: all 24 C# files passed syntax parsing and Git whitespace checks passed. Independent collision design calculations give 1.26 units of adjacent-lane clearance and roughly 2.39 s from warning to front contact at max speed. Independent sound model produced 17,199 finite samples with peak 0.441 under the existing 0.78 normalization ceiling. Editor rule checks were extended for schedule, approach direction, trunk bounds, swept contact and elephant dialogue selection; those Unity checks were not executed here. Native compilation, rendered foot motion, actual collisions/pooling, sound audition and phone performance require local playtesting.

## Kerala selectable stage

Added a persisted start-menu region selector with scene reload, separate regional best scores, active-only scenery/catalogue creation, original green backwater/bridge/forest geometry, eight region-specific hazard kinds, lotus tokens, capped rain-first weather and three bounded synthesized animal cues. Shared runner, outfits, dialogue, settings and gameplay controls continue.

Validation: 28 C# files syntax-parse, all 62 asset metadata GUIDs are unique, whitespace checks pass. Independent numerical audit covers 128,000 Kerala rows, safe lanes, habitat mapping, rest/recovery rows, ten encountered hazard kinds, lane timing, bridge clearance and frond mesh indices. The Unity core-rule menu now additionally checks 32,000 region rows when executed locally; it has not been executed here. APK/Unity compile, runtime visuals/audio, scene-reload cleanup ordering and phone performance are unverified. See KERALA_STAGE.md for acceptance steps and research sources.

## Punjab / Goa and lowered regional guardians

Four stable region IDs, independent bests, state-specific source/catalogue filtering, two new route designs and eleven new obstacle kinds are connected to the existing start-screen selector. All four states now have their own rare sky guardian; base Y is lowered from 2.5 to 0.25 with the road mask preserved. Double-sided crop/frond normals are explicitly set so opposite triangles cannot cancel their lighting normals.

Source checks: 31 C# syntax trees, 65 unique metadata GUIDs and whitespace checks pass. Independent numerical mapping audits cover 384,000 Kerala/Punjab/Goa rows plus crop-index and nominal guardian-projection checks. Editor checks now also include 64,000 Punjab/Goa rows and selection/save/height/tractor contracts, but are unexecuted here. Unity compilation, generated mesh appearance, guardian/HUD overlap, audio quality, stage-reload behavior and phone performance remain manual acceptance checks; see PUNJAB_GOA_STAGES.md.

## Second-step scenic destination selection

Added a dedicated StageSelect state and four-card picture picker; Welcome now opens it instead of running immediately. Selection is a draft until the named Start button. Same-region starts reuse the active scene; cross-region starts persist the exact chosen ID and consume a one-shot target in the new scene’s Awake. Back cancels the draft and RUN AGAIN remains a direct restart. Four compressed-source previews total 242,273 bytes.

Source/numerical checks cover syntax, metadata, image decodes/dimensions/resource order, start/persistence wiring and four safe-area layout cases. Card minimum height was adjusted to avoid caption/best overlap. Actual Unity GUI input/rendering, scene handoff, APK build, imported texture footprint and phone performance remain unverified. See STAGE_SELECTION.md.

## Automatic random state journey

A protected grounded transfer window triggers a four-state shuffle-bag selection. The active world reloads behind a fade with an in-memory run checkpoint; total score/distance/power survive and passed pickup rows cannot score twice. Segment scores use per-leg baselines; a new whole-run Journey Best is separate. Auto travel preserves the manually selected first-stop preference. Restart returns to that first stop. Background/unfocused transfers freeze and gameplay input is cleared during the fade.

Source/formula validation: all 33 C# files parse, metadata uniqueness and whitespace checks pass; 1,200 random journey orders / 76,800 visits pass bag/no-repeat checks; arbitrary-distance pool mapping, pickup boundary and score/power formulas pass. Added Editor transfer/order contracts remain unexecuted here. Unity compilation, real scene handoff, lifecycle sequencing, fade appearance/audio, phone loading time and repeated-transition memory need the checks listed in JOURNEY_MODE.md.
