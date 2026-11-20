# Punjab and Goa stage expansion

## Selection and saves

RegionProfile centralises stable IDs, names, stage titles, token labels, best-score keys and guardian titles. The menu selector cycles Rajasthan (0), Kerala (1), Punjab (2), Goa (3), then returns to Rajasthan. Existing region IDs and best-score keys are preserved; new scores use runner.best.punjab and runner.best.goa. Invalid saved region values fall back to Rajasthan. The chosen region is remembered offline. Outfits, goal progress, the runner and general settings stay shared.

Selection still reloads the existing bootstrap scene from the menu. Only the selected state’s scenery, obstacle recipes and guardian variant are constructed. Seven tiles, fourteen rows, forty-two obstacle slots, seventy token slots and the capped rain pool are retained. Unused camel/elephant leg recipes are skipped where those animals do not belong. All stage source assets ship together; this is not downloadable content.

## Punjab — Harvest Hustle

The route is an original stylised farming-village composite: green/gold crop beds, double-sided golden flower/harvest silhouettes, irrigation channels, flat-roof farmhouses, shade trees, charpoy-style cots, Phulkari-inspired wall decorations and haystacks. Domestic buffalo and goats decorate the roadside and appear as warned lane encounters. An original green tractor with a seated farmer approaches the runner. Hay bales and grain sacks are low jump obstacles; cloth stalls use the existing slide opening and solid-post collision layout. Golden geometric Phulkari-flower tokens replace mirror tokens. No camels, forest elephants or beach creatures are generated.

## Goa — Coastal Colour Run

An original sand-and-road promenade runs beside a turquoise sea, pale surf ribbons and coconut palms. Colourful houses with cream window surrounds and sloped terracotta roofs reference Fontainhas’ domestic architecture. Shacks and colourful fishing boats continue the coastal setting. Stylised, deliberately enlarged crabs and domestic street dogs are warned lane encounters. Coconut baskets, fishing baskets, boat handcarts and low drying-net stalls form the other obstacles; motorcycles and the umbrella visitor remain shared encounters. Collect golden fan-shell tokens. Dolphins appear offshore and a turtle stays safely outside gameplay as background decoration, with no turtle collision obstacle.

The sea and animals are simple prototype geometry. Dolphin poses are static scenic silhouettes, not a simulated marine ecosystem. Domestic animal models use original continuous rounded body forms with simple legs; new full limb animation rigs are not supplied. These routes are imaginative composites, not maps or seasonal/ecological reconstructions.

## Guardian changes

All four states now construct one original fantasy sky character:

| State | Visual identity |
| --- | --- |
| Rajasthan | Existing rose/turquoise turban and embroidered coat |
| Kerala | Green coat, cream leaf trim and leaf-canopy headwear |
| Punjab | Indigo coat, orange turban, fan crest and a decorative dhol |
| Goa | Coral coat, broad straw captain hat, shell medallion and blue cloud accents |

These characters are invented for this game. They do not depict deities, real people or asserted local folklore. No sacred symbols are used. The effect root drops from Y=2.5 to Y=0.25 at Z=70; the existing screen mask still removes it from the lower gameplay view. A simple nominal runner-camera projection moves the face centre from approximately 81.4% to 78.8% of viewport height (measured from the bottom). This calculation is not a rendered screenshot or a guarantee for every device aspect ratio. Four-second duration, rare garden cadence, reduced-effects support and no collision/scoring impact are preserved. The headwear, coat and clouds differ by state; only one variant exists in the active scene.

## Weather and audio

Kerala keeps its rain-first cycle. Punjab keeps a sunny/long-cloud/short-rain cycle; Goa begins with a longer bright coastal interval before cloud and light rain. Manual weather selection and the 48/10 streak limits continue. No traction penalties or visibility-reducing gameplay are added.

Punjab receives short original synthesized buffalo/goat calls and a lower-pitched shared tractor engine. Goa receives a short original friendly dog-bark approximation. Existing Hindi/English recordings remain for shared speaking characters. No Punjabi, Konkani or Malayalam dialogue recordings are fabricated, and background music remains the existing original game loop rather than a claimed authentic regional recording. No network, paid assets or new runtime packages are required.

## Research/provenance

Public sources were used as factual visual references only; no source photos, text passages, music or models were copied into Assets.

- Incredible India, Punjab Phulkari: colourful floral/geometric embroidery and textile tradition: https://www.incredibleindia.gov.in/en/punjab/phulkari
- Incredible India, Nawanpind: preserved rural village life and tractor farming: https://www.incredibleindia.gov.in/en/rural-tourism/nawanpind
- Incredible India, Sadda Pind: rural buildings, agricultural implements, Phulkari and folk dance: https://www.prod.incredibleindia.gov.in/content/incredible-india-v2/en/destinations/amritsar/sadda-pind.html
- Incredible India, Vaisakhi in Bathinda: harvest celebrations, Bhangra/Giddha and dhol: https://www.incredibleindia.gov.in/en/festivals-and-events/punjab/vaisakhi-mela-bathinda-april
- Goa Tourism Development Corporation, Fontainhas: colourful domestic facades, raised plinths and Mangalore-tile roofs: https://goa-tourism.com/popular-destination/experience-fontainhas/
- GTDC, Bambolim: golden sand, coconut palms and Arabian Sea: https://goa-tourism.com/beach/bambolim-beach/
- GTDC, Patnem: bamboo/palm beach shacks: https://goa-tourism.com/beach/patnem-beach/
- GTDC, wildlife: dolphins off quieter beaches: https://goa-tourism.com/best-read/wildlife-in-goa/
- GTDC, Galgibaga: protected Olive Ridley turtle nesting habitat; informs the non-obstacle turtle choice: https://goa-tourism.com/beach/galgibaga-turtle-beach/
- Goa Directorate of Fisheries, fishing/crab context: https://fisheries.goa.gov.in/wp-content/uploads/2024/09/Goan-Fish-Trails-2023.pdf

## Validation

All 31 C# source files parse; 65 asset metadata GUIDs are complete and unique; whitespace checks pass. Independent numerical mapping checks cover 384,000 rows across Kerala/Punjab/Goa: each has a reserved clear lane, regional kind membership, rest buffers, empty approach/recovery rows and safe-lane separation using obstacle dimensions. Crop indices and a nominal guardian projection are checked. The Unity core-rule menu now contains an additional 64,000 Punjab/Goa row check plus region/save/guardian/tractor contracts, to execute in the user’s Editor. Existing Rajasthan generation is passed through unchanged; the added tractor velocity applies only to its new kind.

These checks do not compile or execute Unity C#, render the scene, audition audio, measure memory or produce an APK. Required local checks: build, cycle all four states repeatedly, confirm persisted selection and independent bests, start/restart/pause in each, inspect crop/frond lighting, test tractor warning/collision and slide posts, inspect the guardian below HUD on each phone aspect, verify animal contact remains non-graphic, and profile sustained play plus repeated region switching. Use Effects LOW if necessary; performance claims await the target device profile.
