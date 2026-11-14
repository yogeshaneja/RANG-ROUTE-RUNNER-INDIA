# Validation record

## Performed in the implementation workspace

- C# grammar parsing: all nine runtime/editor files passed without syntax errors using tree-sitter C#.
- JSON manifest parsed successfully.
- All asset/folder metadata exists; GUIDs are unique; scene references resolve to the included RunManager script and enabled build scene.
- Analytical timing bounds checked using constants extracted from RunRules.cs:
  - maximum-speed clear distance between obstacle rows: 1.340 seconds;
  - worst-case two-lane traversal: 0.379 seconds;
  - remaining reaction allowance: 0.961 seconds;
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
- [ ] Yellow crates can be jumped; teal barriers can be slid under; coral barricades require lane avoidance.
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
