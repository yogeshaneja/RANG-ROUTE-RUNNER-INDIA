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
