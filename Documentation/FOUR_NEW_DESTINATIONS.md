# Shillong, Kolkata, Madhya Pradesh and Himachal

Four original playable destination configurations extend the existing Unity/C# offline prototype. Shillong is presented as a Meghalaya route; Kolkata (formerly Calcutta) as a West Bengal city route. The picker now says “destination” and scrolls through eight cards in four rows, automatically revealing the saved selection.

| Stable ID | Destination | Procedural world | Obstacles / wildlife | Token |
| --- | --- | --- | --- | --- |
| 4 | Shillong / Meghalaya | Pine-covered hills, lake, distant waterfall, little lakeside bridge, colourful sloping-roof cottages, wet/cloudy/bright intervals | Hill buses, low leaf shelter, fruit baskets, logs, goats and langurs | Golden pine cone |
| 5 | Kolkata / West Bengal | Hooghly-inspired river, cantilever-bridge silhouette, ochre heritage facades, lamps, ferry, a decorative tram on a separate side track | Yellow taxis, sweet-shop crates, low flower stalls and street dogs | Golden clay tea cup |
| 6 | Madhya Pradesh | Sal-inspired forest, forest river, pale riverside rocks, warm village cottages and original dotted-leaf wall motifs | Supply jeeps, craft baskets, low leafy canopy, logs, spotted deer and peacocks | Golden forest leaf |
| 7 | Himachal Pradesh | Distant snow-capped peaks, layered deodar-inspired trees, timber-and-stone cottages, apple orchards and guardrails | Supply jeeps, apple crates, low wool stalls, sheep and goats | Golden apple |

The existing boy and Chutku continue across all routes. New deer/sheep obstacle models have four independently swinging shared-mesh legs. Background animals are static scenery. No graphic harm. There are no tiger attacks or narrow unguarded cliff routes. Terrain remains level for reliable lane/jump/slide physics; distant peaks are scenery, not a new climbing/snow simulation.

## Shared feature parity

Each new region has a named adventure passage, colour-coded trolley/cart/buggy ride, a regional Festival Rush name and palette, a fantasy cloud guardian with its own cap/crest, a separate personal best, a preview illustration and participation in the global journey shuffle bag. Rain visitors and reduced effects continue to work. Guardians are invented characters: explorer, river storyteller, forest keeper and orchard guide, rather than depictions of real religious or cultural figures.

Festival and ride row timing, 3×/2× rewards, collection flags, companion cooldown, pause/background, restart and scene checkpoint rules are shared. New traffic uses the existing swept relative-motion collision path and approach-engine loop. All eight destinations are visited before a bag repeats; consecutive destinations differ. IDs 0–3 and all previous score keys stay unchanged. No new backend, downloads, account or paid assets.

Weather remains cosmetic. Shillong begins with light rain, then cloud/mist and sunshine; Kolkata and MP cycle through sunshine, cloud and rain; Himachal has a longer clear interval with cloud/haze and brief rain. Manual weather selection still takes precedence. Hill fog begins at 85 m, keeping the warning/reaction zone readable. Snow is only on distant Himachal mountain caps.

## Mobile budget

The same seven road tiles, 42 obstacle slots, 70 pickup slots, fixed rain/cloud/feedback pools and six-petal reduced festival option are retained. Only the selected destination’s scenery and obstacle catalogue are built. New model IDs 36–47 are filtered by region, then shared by pooled instances. Art is batched per tile; animated deer/sheep legs share meshes. Additional leg renderers are fixed allocations, not spawned each encounter. Preview additions total **229,081 source JPEG bytes**, all 512×288 with compression requested, mipmaps/readability disabled; all eight previews total 471,354 bytes. Actual imported texture memory and phone frame pacing still require profiling.

Preview illustrations are original AI-generated destination art, not screenshots or promises of matching rendered detail. The playable world uses original procedural low-poly meshes. No downloaded tourism photographs are embedded. New source art and prompt descriptions are in NEW_DESTINATION_ARTWORK.md.

## Regional research

Research checked 21 November 2026. These sources inform a fictional route; this is not a geographically exact map, seasonal forecast or claim about current public transport service.

- [Meghalaya Tourism — Shillong nature itinerary](https://www.meghalayatourism.in/plan/suggested-itineraries/itineraries/1-day/natural-tranquility-shillong/): lakeside scenery and Elephant Falls.
- [Incredible India — Shillong natural wonders](https://www.incredibleindia.gov.in/en/meghalaya/shillong/top-natural-wonders-of-shillong-that-will-take-your-breath-away): Umiam, pine trees, Khasi hills and waterfalls.
- [Howrah district — Howrah Bridge](https://howrah.gov.in/tourist-place/howrah-bridge/): Kolkata/Hooghly steel-bridge landmark.
- [West Bengal Transport — tram heritage](https://transport.wb.gov.in/about-us/department-at-a-glance/corporation/wbtc/ctc/): basis for a static heritage-tram prop, not a current timetable.
- [MP Tourism — Kanha beyond tigers](https://www.mptourism.com/kanha-beyond-tigers-sal-forests-meadows-tribal-heritage.html): sal forests, river valleys and spotted deer. The dotted wall art is original decoration, not a reproduced traditional artwork.
- [Himachal Tourism — unexplored destinations](https://himachaltourism.gov.in/destination/unexplored/): deodar woods, hills and orchards (search-index summary available; direct page fetch returned 502).
- [Incredible India — Narkanda](https://www.incredibleindia.gov.in/en/himachal-pradesh/shimla/narkanda): mixed conifer forest and apple-growing landscape.
- [Himachal Horticulture Department](https://eudyan.hp.gov.in/cms/en/aboutdepartment/): apple-growing context.

## Validation and local acceptance

`python Tools/check_region_expansion.py` performs independent source/formula checks, not Unity execution. It covers 256,000 new-region rows, regional catalogue membership, reserved safe lanes, ride-width clearance, clear Festival Rush rows and dynamic recovery buffers; 1,200 eight-destination journeys / 76,800 transfers; jump/slide and roadside clearances; scroll reachability, stable score keys and thumbnail imports. Editor core rules add all-destination encounter loops, new traffic and preview contracts. Live startup still validates real constructed pools and the runner rig.

43 C# syntax trees parse, metadata GUID uniqueness and whitespace checks pass. No Unity Editor or Android toolchain is installed here. Unity semantic compilation, actual scene/GUI/rig rendering, audio audition, APK build and performance were not tested. Run the Editor’s core and startup checks, then Build And Run on the phone. Select all four new cards, verify a normal obstacle section, Festival Rush, ride and guardian, pause/background and restart, saved separate bests, random handoff across eight destinations, reduced effects, airplane mode and sustained memory/frame pacing. The original Hindi voice recordings remain unchanged; no new regional spoken dialogue is claimed.
