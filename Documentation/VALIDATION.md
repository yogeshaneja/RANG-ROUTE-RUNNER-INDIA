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
