# Asset provenance — prototype

| Asset | Origin | Status |
| --- | --- | --- |
| Runner, scarf, houses, stalls, animals, birds, coins, obstacles | Original geometric arrangements in this repository, using Unity built-in primitive meshes | Temporary prototype art |
| Matka stacks, textile bundles, wooden cart, fabric awning, decorated crossing camel | Original shared-mesh geometry in RajasthanObstacles.cs, built from Unity primitives | Gameplay models; not downloaded stock |
| Flying kites | Original native geometry in TrackWorld.cs | Decorative only; no collider |
| Character body/face/clothing/shoes | Original rounded custom meshes in RunnerModel.cs and NaturalMesh.cs | Procedural articulated avatar, not a downloaded/skinned production asset |
| Character motion | Original two-bone inverse kinematics, sprint stance/recovery, shoulder/elbow and crouched slide poses | No imported animation clips |
| Natural terrain, arches, trees and cloud forms | Original NaturalMesh/C# geometry and deterministic grain textures | Generated once and batched where rigid |
| Runner contact shadow | Original analytic radial-alpha shader | Shader must be validated in the local rendering pipeline |
| Colour palettes | Original numeric material colours | Regional art direction requires review |
| Coin/jump/stumble cues | Original synthesized sine-wave audio generated in RunnerAudio.cs | Prototype audio |
| Background motif | Original 16-note synthesized sequence in RunnerAudio.cs | Placeholder; not presented as traditional regional music |
| Distant dunes/fort panorama, RajasthanHorizon.jpg | Original AI-generated illustration created for this project; no input reference image | Legacy fallback; new active image is RajasthanHorizonReal.jpg |
| Haveli/bazaar frontage, RajasthanBazaar.jpg | Original AI-generated illustration created for this project; no input reference image | Legacy asset; flat storefront cards are no longer rendered |
| RunnerScenery.shader | Original small unlit texture/sky-cut shader | Needs local built-in/URP shader and device checks |
| Menus / font | Unity built-in IMGUI skin/font | English prototype only; no Devanagari asset |
| Runtime material | Unity Standard shader, or selected pipeline shader when prepared in Editor | Unity dependency; no separate third-party asset download |

No stock photographs, downloaded models, Bollywood recordings, branded characters, logos or third-party fonts are included. Unity's own engine assets remain subject to Unity's terms. Do not add paid or external assets without recording source, license and approval.

Rajasthan references are stylised placeholders, not a claim of exhaustive cultural or wildlife accuracy. Research each additional region before final art production.

## Freely reusable web assets assessed for the obstacle pass

ambientCG Wood Floor 001: https://ambientcg.com/view?id=WoodFloor001 . Its official license page, https://docs.ambientcg.com/license/ , states CC0 1.0 Universal and permits commercial use and redistribution of the asset files. The 1K JPG archive download returned HTTP 403 in this execution environment. The asset was not imported, copied, redistributed or made a runtime dependency. All obstacle artwork in this pass remains original native geometry. This is a source assessment, not a claim that a stock texture is included.

## Art-and-feel assets

| Asset | Origin | Usage |
| --- | --- | --- |
| RajasthanHorizonReal.jpg | Original generated photo-like desert/fort landscape; no reference photograph | Active distant backdrop, max import 1024 |
| SandstoneReal.jpg | Original generated sandstone masonry albedo | Repeating texture on actual 3D walls, max import 512 |
| CottonReal.jpg | Original generated neutral woven-cotton albedo | Tinted kurta/trousers/scarf material, max import 512 |
| Rounded UI panels/buttons | Original analytic rounded-rectangle/gradient texture code in RunnerSkin.cs | Cached at first GUI draw, disposed with the game |
| Coin glints | Original small native mesh pool in RunFeedback.cs | 18 maximum; cosmetic only, shared material |

Prompts and generation details for the three images are in ARTWORK.md. They are not photographs of actual locations or people. Source images were encoded as JPEG without content editing; no third-party stock images were imported in this pass.

- Moving-hazard/character pass: original code-generated motorcycle with helmeted rider, flying saucer with rim lights, landing marker, rounded/leg-animated crossing camel and rounded scenic camel. Runner face/clothing/accessory refinements are original procedural geometry. No downloaded models, new licenses, paid services or external dependencies.

- Festival pass: original procedural mirror-work-inspired ornament meshes, elephant scenery, camel embroidery and enlarged existing props. New RajasthanDecor.cs uses existing owned-material/mesh caches. All eleven audio clips (20-second original instrumental and synthesized pickup, jump, game-over, engine, spaceship, camel and elephant effects) are generated from original code; no third-party recording, composition, voice, sample library or downloaded audio is bundled. Animal timbres are stylised approximations. Existing cotton texture is reused for elephant saddle cloth.

- Goals/gardens pass: original procedural embroidered pouch, pink/marigold roadside flowers, turquoise/gold/pink bunting and owned kurta palette material. One original synthesized festival-welcome arpeggio joins the existing music/effects (12 clips total). No third-party assets, recordings, paid purchases or package dependencies were added.

- Five-encounter pass: original open-ring cart wheel/spokes, festive cart pair, textile bundles, sand/barricades/scaffolding and fan-tailed peacock geometry. Adds five original synthesized wheel/procession/fabric/peacock/repair clips (17 clips total). No stock assets, copyrighted songs, third-party animal recordings, packages, paid purchases or external services were introduced.

- Sculpted-boy pass: original continuous face/hair/kurta meshes, curved facial detail surfaces, almond eyes, relaxed hands, shaped shoes and skin-weighted arm/leg/scarf meshes. All geometry/weights/bind poses are authored by project code in RunnerSculpt.cs; no Mixamo, stock character, mocap, external account, image asset or paid licence is used. Existing original cotton texture and owned palette material are reused.

- Spoken-banter pass: 15 original playful Rajasthan-flavoured Hindi lines and Roman Hindi captions, covering rider, peacock, camel, elephant and festival caller. Playback is synthesized by an installed embedded Hindi Android voice; no third-party recording/model, cloud TTS, downloaded voice asset or recorded-actor claim. Regional authenticity requires native-speaker review. Voice data is supplied by the phone's selected engine, not redistributed in the repository/APK. Existing generated sound/music assets are unchanged.

- Rain visitor: original adult female roadside character in lehenga/blouse/dupatta, double-sided alternating umbrella panels and gold ribs, created by RainVisitorArt.cs and shared in the existing pool. Uses existing original cotton texture and cached materials. One user-requested revised weather line is synthesized by the installed offline Hindi engine; no song recording, purchased character, actress recording, external TTS or new package is bundled.

- Lightweight pass: the original RajasthanBazaar.jpg and RajasthanHorizon.jpg and their metadata are archived byte-for-byte in Documentation/ArtArchive, outside Unity's build Assets tree. Their original ownership/licences are unchanged. The active realistic horizon/sandstone/cotton art and all current models/audio/voices are retained; no new external assets or dependencies.

### User-supplied Hindi dialogue recordings

16 Hindi clips in Resources/Dialogue/hi are derived from the user uploads MALE.mp3 and FEMALE.mp3, explicitly supplied for use as the male/female game voices. The older FEMAL.mp3 was superseded and not used. Rider/camel/elephant/caller use the male batch; peacock/lady use the female batch. Spoken GAP sections are excluded; gentle level/edge cleanup preserves pitch/speed. The source originals remain unchanged outside the repository. Provenance hashes and exact cuts are in HINDI_AUDIO_CUTS.csv.

The conversation discussed TTSMaker, but the user did not independently confirm which service/voice produced these files. Provider, voice identity and generation-license evidence have therefore not been independently verified; no actor-recording or verified-Marwari-accent claim is made. The user supplied/authorized these audio files for the game. No paid generation, audio service, voice model or cloned identity was introduced. English recordings remain unsupplied.

- Dance break: original procedural pose animation on the existing runner rig and original generated 140 BPM percussion/pluck music. No external music/animation sample or paid asset added. Optional break_time recording is not supplied; the temporary spoken cue uses an installed offline English phone voice and is not a bundled actor recording. The existing 16 user-supplied Hindi clips are unchanged.

- Thar Cloud Guardian: original procedural fantasy figure, turban/moustache/coat/jewellery geometry, cloud banks, glints and a new lightweight transparent sky shader authored in project source. No traditional/religious figure likeness or external artwork/model is claimed. Uses existing original celebration/hum audio, with no new recording or paid asset. Geometry is built once and owned through existing mesh finalization; private effect materials are disposed at shutdown.

- Oncoming elephant/game-over pass: reuses the original roadside elephant builder and existing elephant trumpet/Hindi recordings, with four original shared-mesh walking legs for the new moving obstacle. Existing scenery elephants are retained. The short raspy game-over crank is original procedural non-verbal synthesis; no recorded voice, downloaded music, new actor likeness or paid asset is introduced.

## Android launcher artwork

`Assets/IndianRunner/Editor/Branding/RangRouteIcon.png`: original AI-generated illustration using the built-in image generation tool, no reference image or third-party stock asset. Resized to 512 x 512 opaque PNG for build-time use; kept in Editor assets, outside runtime Resources. The face remains central under launcher masks. Both adaptive layers use the same full-bleed illustration; this is a flat icon, not separate parallax artwork. Android performs launcher masking.

Generation prompt: “Square Android launcher icon for an original Indian endless runner: young adult Indian male runner with warm brown skin, short dark hair, turquoise kurta and flowing saffron scarf; dynamic running pose, centered readable head and torso; winding golden desert road, Rajasthan pink sandstone palace arches and warm sunset against teal sky. Polished colourful stylized 3D illustration, turquoise/coral/saffron/gold, crisp silhouette, important details central for circular cropping, full-bleed opaque background, no text, letters, watermark or logos.”

Integration uses Unity 6.3 icon APIs: https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/playersettings/setplatformicons and https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/platformicon/settextures . Source checks and PNG decode passed; actual Unity import, APK packaging and launcher appearance require local Build and Run.

Runner portrait polish: original C# sculpt/hair/embroidery geometry in RunnerSculpt.cs and RunnerModel.cs, informed by this project’s generated launcher art. No new downloaded character, image texture or animation asset. See SCULPTED_RUNNER.md for scope and verification limits.

## Kerala route assets

All Kerala palms/fronds, forest canopy, houseboats, bridges, houses, banks/hills, lotus tokens, duck/boar/langur models, market props and shelter geometry are original C#/Unity meshes in KeralaScenery.cs and KeralaArt.cs. The existing original elephant builder supports an unadorned forest variant. No downloaded images, models, fonts or packages were added. Three original short animal-call approximations are synthesized in RunnerAudio.cs; existing background music/runner/voice recordings are reused. Research links and habitat decisions are recorded in KERALA_STAGE.md. These are stylised prototype assets requiring device visual/audio review.

## Punjab / Goa and four-state guardians

RegionalArt.cs and RegionalScenery.cs contain original procedural farms, crop rows, irrigation channels, houses, coastal sea/surf, boats, shacks, palms, buffalo/goat/dog/crab models, tractor/farmer, harvest/fishing props and Phulkari/shell tokens. No downloaded images, characters or packages are included. Three added short animal-call approximations are original synthesized audio, allocated only in their active region. CloudGuardian.cs adds original regional costume/headwear/colour variants; these are invented fantasy characters, not depictions of real individuals or sacred figures. Public reference sources and validation limits are in PUNJAB_GOA_STAGES.md.
