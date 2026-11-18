# Festival goals, outfit rewards and breathing spaces

## Three goals and local rewards

The first set asks for 30 mirror-work tokens, five successful slides underneath awnings, and 500 metres in one run. Tokens and valid awning passes accumulate across runs; the distance counter retains the longest run in the current set rather than adding separate runs together. Progress saves locally on goal completion, pause/background, game over, return to start and application quit. Current best-score and audio settings remain compatible.

Open GOALS from Start, pause or results. Once all three are complete, claim from Start. Claiming is unavailable during an active/paused/finished run, prevents double claims, resets the goal counters for the next set and equips the new colour. Complete successive sets to unlock marigold, festival pink and royal blue, alongside the original turquoise. Tap the outfit-name button at Start to cycle unlocked colours. Body/sleeves use one owned material, so changing the boy's outfit does not recolour scenery. Subsequent sets award locally counted festival badges; goal difficulty rises to bounded caps.

Only crossing an awning's centre while sliding and avoiding contact counts toward the slide goal. Repeated down swipes on empty road do not count. Distance progress is bounded and saved snapshots clamp invalid counters/outfit selections.

## Gardens and the festival pouch

The first festival garden occupies 280-364 metres, recurring every 504 metres. Three rows are clear of obstacles; an additional empty row before/after provides entry/exit space. Pink and marigold flowers and turquoise/gold/pink roadside bunting decorate the route. They stay outside playable lanes. A short original welcome arpeggio accompanies entry, while the existing instrumental's pitch/volume lift gradually by a small amount. Hazard warnings still take priority over celebration banners.

A bright embroidered pouch waits on the token lane at 322 metres, then every 504 metres. Touch it to attract tokens from within 3.2 metres laterally for eight seconds. Nearby tokens visually pull towards the player; reduced effects keeps them steady while preserving the same pickup range. Power does not protect against obstacles. Its timer uses run time, so it freezes while paused; restarting clears it and resets all pooled taken flags.

Every one of the 14 pooled rows owns its pouch/garden props before Reset. Only scheduled props are active. No per-encounter object creation, downloads, accounts, paid services or extra Unity packages are introduced. There are now 12 original synthesized clips through the same four AudioSources, including the festival arpeggio.

## Rare spaceship

The spaceship first appears at 910 metres and repeats every 2,016 metres, with its existing descending motion, landing marker and 45-metre warning. It stays outside gardens and their transition rows. Motorcycles still begin at 126 metres; encounters scheduled inside a garden/transition are skipped. Rajasthan scenery, carts, pottery, fabric, awnings and camels remain the normal route.

## Required local review

- Pull, let Unity import, press Play and run Indian Runner > 4 - Check live startup (Play Mode). All 14 markers, pouches, gardens, obstacle slots and token slots must exist. Run the core rule checks as well.
- Verify GOALS opens/closes over the menu, pause and results. Keyboard pause cannot resume a run behind the goals overlay. Resume/back-to-start and Android safe-area framing must remain usable.
- Collect tokens and pass awnings while sliding; check that empty-road swipes, upright passes and restarts do not duplicate credit. Reach the distance goal in one run; shorter later runs must not accumulate distance. Complete a set, return to Start, claim once, change outfit and relaunch to confirm saved colours/goals.
- At 280 m inspect clear gardens, flowers/bunting and music transition. At 322 m collect the pouch, observe the eight-second HUD countdown, adjacent-lane pickup range and pause-safe timer; compare reduced effects. Play past the next garden and verify recycled objects/pickups reset correctly.
- At 910 m test spaceship contact versus the marked escape lane. Profile sustained frame time/memory on the actual Android device; inspect material/mesh rendering, bright colours, UI text and sound through both speaker/headphones. Rebuild with Build and Run to update the phone APK.

Implementation validation here is source parsing and independent numerical/state simulation. Unity compilation, Editor play, actual audio audition, Android rendering and performance remain unverified in this workspace.
