# Random journey across states

## Route order and timing

The state chosen on the picture screen is the first stop. JourneyRules requires 600 m in each leg and restricts transfer initiation to the first 20 m of a protected garden-buffer row (`row % 18 == 9`). The runner must be grounded, not rising/jumping, not sliding, and finished with the current lane change. The first eligible window starts at 756 m; typical subsequent windows start near 1,764, 2,772 m and so on. Input activity may defer a change. This is distance-based scheduling rather than a strict wall-clock timer.

A random choice is made from the unused states in a four-state bag, excluding the current state at bag boundaries. The initial state counts toward the first bag. Each complete four-visit bag contains all four states; neighbouring visits never repeat. The bag and next-state choice survive the scene handoff. No backend or network is involved.

## Safe transfer

The game freezes gameplay in a dedicated RegionTransition state. It fades out over 0.45 s, renders a fully opaque NEXT STOP frame, then loads the existing bootstrap scene with an in-memory checkpoint. Only the next region is constructed. Outgoing runtime resources are released before loading, and cleanup is idempotent, so the old manager’s eventual OnDestroy cannot clear the new region’s shared mesh/material cache. The new scene positions the runner/camera and pooled road around the saved distance, then fades in over 0.65 s before gameplay resumes. Reduced effects shortens each fade to 0.18 s. Actual loading time is additional and depends on the phone; this is not a claim of hitch-free streaming.

The checkpoint retains total distance, run clock, coins, seed, lane/X, starting state, visited-state bag, next distance milestone, manual weather choice and absolute magnet-expiry clock. Goal/outfit progress is saved before transfer and reloaded through the existing local service. Jump/slide motion is not cut: eligibility waits for a stable grounded pose. Speed keeps using total distance. The new weather cycle starts locally for the destination; a manual sky choice remains respected.

Road reset now accepts a distance, rebuilds seven logical segments with their physical variants correctly matched modulo seven, and marks pickups/pouches already behind the handoff boundary as taken. This prevents coins being awarded again because a new pool was constructed. The reserved garden window gives a long obstacle-free approach after arrival, and an immediate dance break is suppressed for the first 120 m of a newly entered leg. The original later dance cadence is retained.

No distance, coins, goals, collisions, hazards, guardian effects or power timers advance during either fade/loading. Gameplay input is discarded, so touches on the transition cannot become delayed lane changes. Transition progress also freezes while the app is paused or unfocused. This is an in-memory run continuation; OS process termination does not resume an unfinished run.

## Scores, restart and selection

Journey score remains `total distance + total coins * 10`. The whole-run best saves separately as `runner.best.journey` and appears in the pause/game-over caption. Existing per-region best keys now record the distance and coin delta of that region’s leg, avoiding assigning an entire multi-region run to each state.

Automatic travel does not overwrite `runner.region`, the user’s manually selected starting state. The picker preselects that first stop. RUN AGAIN returns to it, loading it if the run ended in another region. Restart resets distance/coins/bag/power and creates a fresh run. Back to Welcome saves results before clearing the run and does not record idle-menu distance or leftover coins as a new score.

## Validation and device checks

All 33 C# source files syntax-parse; metadata and whitespace checks pass. Independent numerical tests cover 1,200 random journeys (76,800 visits), all starts, four unique states per bag, no adjacent repeats, arbitrary-distance pool index coverage/variant matching, duplicate-pickup boundaries and score/power conservation formulas. Editor rule checks now include transfer-window boundaries and deterministic multi-bag order examples, to run locally. Source wiring checks confirm checkpoint restoration does not call StartRun/reset the score and automatic travel does not persist a new starting-state preference.

These checks do not compile or execute Unity, render the fade, measure loading or profile memory. On the target phone: run past multiple swaps; verify score/distance/coins do not jump backwards or duplicate; transfer with active magnet; jump/slide through a candidate window and confirm deferral; background the app during both fades; confirm only one handoff occurs, correct region wildlife/guardian/weather appear, and RUN AGAIN returns to the original picture-card choice. Inspect transitions at maximum speed and profile repeated state changes for load duration and memory growth.
