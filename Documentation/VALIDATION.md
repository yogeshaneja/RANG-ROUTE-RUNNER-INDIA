# Validation record

## Performed in the implementation workspace

- C# grammar parsing: all eleven runtime/editor files passed without syntax errors using tree-sitter C#.
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
