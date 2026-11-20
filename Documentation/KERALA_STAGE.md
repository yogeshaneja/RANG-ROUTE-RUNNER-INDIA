# Kerala — Monsoon Trail

## Play

From Welcome tap LET’S RUN, choose the Kerala picture card on step two, then START KERALA RUN. Switching reloads the existing bootstrap scene from the menu so the outgoing stage and its owned resources are disposed. The selected region persists in `runner.region`. Rajasthan retains its existing `runner.best`; Kerala uses `runner.best.kerala`. Outfits, festival progress, controls, voice/audio settings and the runner are shared. Restart keeps the selected stage.

## Route and art

A repeating seven-segment route uses coconut/backwater village sections, two bridge sections, then forest sections. The three gameplay lanes retain their widths and heights across bridge joins. Water sits below the bridge deck; rails remain outside all three lanes. Surroundings include curved palm fronds, banana plants, paddy-green banks, cream houses with pitched terracotta roofs, original kettuvallam-inspired boats, broadleaf forest canopy, off-road langurs, a natural unadorned Asian elephant, kingfisher/hornbill-inspired birds and distant green hills. This is a stylised composite route, not a geographic reconstruction.

Golden lotus-shaped collectibles replace mirror-work tokens in Kerala. Every row retains its safe token lane. Kerala has no camels, desert panorama, kites, peacock displays, procession or Thar Cloud Guardian. Existing motorcycle, umbrella lady, shared runner dance break and shared original music continue. Existing Hindi/English dialogue is reused for shared encounters; no Malayalam speech has been added.

## Obstacles

| Kind | Behaviour |
| --- | --- |
| Ducks | Small clearly warned group on a backwater lane; dodge or jump |
| Wild boar | Approaches the player in a forest lane; dodge |
| Langur | Seated forest-path encounter; dodge |
| Wild Asian elephant | Occasional oncoming forest encounter; shared walking-leg rig, no saddle |
| Coconut baskets / coir mats | Low static obstacles; jump or dodge |
| Fish-market cart | Tall lane obstacle; dodge |
| Woven palm shelter | Slide through centre or change lane; solid visible posts |
| Fallen palm log | Low obstacle in a warned lane; jump or dodge |

Animal contact ends the run with an unharmed-animal message. Animals are original stylised geometry, not downloaded realistic models. Boar, langur and elephant encounters are restricted to forest segments. Duck groups are restricted to nonforest encounters. Root motion is used for the boar and existing elephant leg motion is reused; ducks and seated langurs do not have new limb rigs. Three short, original synthesized calls are built once for Kerala; these are stylised effects, not wildlife recordings.

## Weather and mobile limits

Kerala begins with visible rain and transitions gradually through a 180-second cycle: rain, cloud, sunshine. The existing manual sky selector still works. Linear mist starts at 85 m; it does not hide the roughly 45 m warning window. Rain remains capped at 48 reusable streaks, or 10 with reduced effects. Weather has no traction, damage or control penalties.

Seven pooled tiles, fourteen rows, forty-two obstacle slots and seventy token slots remain fixed. The active region alone constructs its scenery, catalogue and optional effects; inactive Rajasthan panorama/scenery, camel legs and Thar guardian variant are skipped in Kerala. Kerala now constructs its own Monsoon Leaf Keeper, positioned lower like the other state guardians. Rigid geometry is combined once by material; no new scenery meshes or animals are instantiated while running. Region switching creates replacement pools during the menu scene reload. This is bounded resource management, not a measured frame-rate or memory claim. Both regions’ source assets still ship in the APK. Future expansion may need asset bundles/addressable region packs, but none are introduced here.

## Research references

The official Kerala Tourism sources below guided scenery and habitat choices. No photographs, text passages, audio or models from them were copied into the game.

- Alappuzha: backwaters, paddy fields, coconut/banana farming, coir and kettuvallam houseboats: https://www.keralatourism.org/districts/alappuzha/
- Alappuzha canals: coconut groves, paddy fields, kingfishers and village coir work: https://www.keralatourism.org/destination/cruise-canals-alappuzha/369/
- Valiyaparamba: backwaters, coconut groves and footbridge: https://www.keralatourism.org/destination/valiyaparamba-kasaragod/161/
- Aralam: Western Ghats forest habitats, boar, common langur, elephant and hornbills: https://www.keralatourism.org/malabar/aralam-wildlife-sanctuary/121
- Pampadum Shola: mist/cloud, monsoon rain and forest fauna: https://www.keralatourism.org/kerala-article/2022/pampadum-shola-national-park/1177

## Validation and remaining device checks

All 31 C# files pass syntax parsing. Independent numerical mapping checks cover 128,000 rows: safe-lane separation, rest buffers, empty dynamic-neighbour rows, forest restrictions, all ten Kerala hazard kinds, maximum-speed lane timing, bridge-rail clearance and frond mesh indices. These are formula/source checks, not execution of the C# game. The existing Indian Runner core-rule menu now also checks 32,000 Kerala rows in Unity when run locally. Asset metadata GUIDs are complete and unique; whitespace checks pass.

No Unity Editor, APK build, rendering or audio audition was available here. Before accepting: build/run, switch stages several times, verify remembered stage and separate scores, test jump/slide hitboxes, rain-lady timing, rest/restart/pause/home, look for scenery gaps at bridge joins and invisible surfaces, check animal silhouettes, and profile frame times/memory on the target phone over sustained runs and repeated region switches.

The Punjab/Goa expansion adds state-specific guardians to every region; earlier notes about skipping the Rajasthan Guardian refer to that particular Thar design, not the new Kerala Leaf Keeper. See PUNJAB_GOA_STAGES.md.
