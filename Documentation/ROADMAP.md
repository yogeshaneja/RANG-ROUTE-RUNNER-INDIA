# Delivery stages

## Stage 1 — this branch

Playable prototype source: one Rajasthan road, primitive animated runner, swipes/keyboard, coins, three obstacles, scores, pause and restart. Some early Stage 2 scaffolding is included (scenic animals, simple weather and audio/settings).

**Exit gate:** compile and run in a supported Unity Editor, then install and play on a real phone. Do not call this a phone-validated build until evidence is recorded.

## Stage 2 — polished Rajasthan

Replace placeholder character with an original rigged model and animation clips; improve environment and animal animation; implement production UI, Hindi/English with verified shaping and scalable text; test weather visibility and accessibility; profile lower-spec Android hardware and optimize measured bottlenecks. Split configuration into ScriptableObjects and scene/prefab assets when content stabilizes.

## Stage 3 — regional journey

Add researched Punjab and Kerala content first, then Tamil Nadu, West Bengal and Assam as distinct environments. Add forest tracks, appropriate wildlife, richer regional weather, region selection and optional outfit cosmetics. Do not flatten distinct regional identities into one generic template.

Repository creation was performed by the user. No merge, paid assets, release signing or publication is authorized by this implementation. All merge/deploy/publish decisions remain with the user.

Before adding the next state, follow LIGHTWEIGHT_REGIONS.md: establish per-region ownership/loading and keep one active region, reuse the player/game core, avoid growing a global Resources library, and profile real phone startup/frame-time/memory/build size. The current project has no multi-region loader; global PrimitiveArt caches must not be shared by overlapping old/new RunManagers without scoping. The completed cleanup reduces retained mesh copies, duplicate state updates and unconditional legacy artwork inputs; it does not certify a device budget.
