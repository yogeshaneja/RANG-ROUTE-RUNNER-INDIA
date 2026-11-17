# Rajasthan artwork production record

These are original generated illustrations for this game. No stock photograph, reference image, brand or named artist was used. They are stylized Rajasthan-inspired scenery, not a reproduction of a specific monument or an assertion of architectural accuracy.

Generation mode: built-in image generation. Final assets are integrated in the repository as JPEG textures; their generation output was converted to JPEG for source size without cropping or content edits. Both textures have max-size 1024, clamp wrapping, mipmaps and compression configured in Unity metadata.

- `Assets/IndianRunner/Resources/Scenery/RajasthanHorizon.jpg`: distant landscape artwork, on a depth-independent world backdrop behind the route.
- `Assets/IndianRunner/Resources/Scenery/RajasthanBazaar.jpg`: frontage artwork on repeated roadside panels. A shader trims the upper blue sky while preserving lower fabric detail; this needs local visual verification.

This is illustrated scenery on geometry, not newly authored fully 3D buildings. The existing low-poly scene remains interactive. No network image requests happen in the app.

## Horizon prompt

Use case: stylized-concept. Asset type: wide background texture for a colourful low-poly Unity Android endless runner in Rajasthan, India. Create an ORIGINAL richly illustrated wide landscape panorama, roughly 3:2 aspect. Layered ochre Thar sand dunes, a distant sandstone hill fort with small chhatri cupolas, pale Aravalli ridges, scattered desert shrubs, warm golden sandstone village skyline far left and right. Bright turquoise clear sky occupying upper 45 percent, horizon running across middle; bottom 30 percent rolling sand in warm peach. Calm distant scenery with simplified readable shapes, cheerful polished mobile game illustration, warm sunlit palette, no foreground road, no large foreground objects, NO people, text, letters, logos, watermarks, UI or borders. The central distant skyline stays low so it will not distract from the playable road. Full bleed edge to edge, no image frame.

## Frontage prompt

Use case: stylized-concept. Asset type: roadside illustrated panel texture for an original colourful Rajasthan Unity runner. A single wide, front elevation illustration of a Rajasthani sandstone haveli and vibrant bazaar frontage, in polished stylized low-poly mobile game art. Warm pink and golden sandstone facade, scalloped arched doorways, carved jharokha balcony windows, small chhatri roof pavilions, turquoise and saffron market awnings, stacks of colourful fabric and clay pots, lush tiny potted plants, festive bunting. Entire facade visible horizontally in orthographic frontal view, flat building elevation, NO perspective street or vanishing point. Upper skyline with pale blue sky, lower edge meets warm sandy ground. Bright clear sunny colours and attractive details readable in a game. No humans, vehicles, animals, brand marks, text, signs with letters, watermark or border. Original architecture inspired by Rajasthan, not a copy of a specific protected artwork. Full bleed rectangular image about 3:2 aspect.

## Realistic-surface and backdrop pass

Generation mode: built-in image generation; original images with no reference inputs. Project assets are JPEG encodings without content edits. The photo-like scene is generated artwork, not a photograph or a claim about a specific fort/location. Original PNG outputs remain preserved.

- `Assets/IndianRunner/Resources/Scenery/RajasthanHorizonReal.jpg`: 1536x1024 source, 1024 import cap, active backdrop.
- `Assets/IndianRunner/Resources/Scenery/SandstoneReal.jpg`: square source, 512 import cap, repeating masonry on 3D walls.
- `Assets/IndianRunner/Resources/Scenery/CottonReal.jpg`: square source, 512 import cap, repeating/tinted clothing albedo.

### Photo-like panorama prompt

Use case: photorealistic-natural. Asset type: distant background panorama for an original Rajasthan Android running game, no UI. Create a wide landscape photograph-like matte painting at 3:2 aspect with believable sunlit Thar dunes, scattered hardy shrubs, atmospheric Aravalli hills and a distant golden sandstone fort on a low hill, inspired by Rajasthan without reproducing any photograph. Soft warm late-afternoon sunlight, pale realistic blue sky and wispy clouds, subtle dusty atmospheric depth, natural restrained rich colours. The skyline sits around 44 percent up from the bottom, sky above; bottom 35 percent is unobstructed gently rolling sand. Fort and village must stay distant and small. Detailed natural sand and weathered masonry, no cartoon outlines, no low-poly facets. Full bleed, no people, road, foreground vehicles, text, logos, letters, watermark or borders.

### Masonry albedo prompt

Use case: photorealistic-natural. Asset type: seamless diffuse material texture for native 3D Rajasthan buildings in Unity. Original square close-up flat orthographic albedo of authentic-looking warm pink-golden sandstone masonry with worn lime plaster in places, subtle fine grain, irregular shallow joints between rectangular stone blocks, natural weathering, tiny chips and gentle variation. Natural midtone tan/sandy ochre palette, fine realistic surface detail. Even shadowless neutral lighting with NO perspective, NO strong directional shadows, NO ambient vignette, NO glossy reflections, NO objects, architectural windows, doors, sky, text, logo or watermark. Material surface fills every pixel. Tileable left-right and top-bottom, no obvious repeating large marks.

### Cotton albedo prompt

Use case: photorealistic-natural. Asset type: seamless fabric albedo for a 3D Indian male runner's kurta in Unity. Original square orthographic macro surface of finely woven natural cotton, muted light neutral beige-grey so a teal colour tint can be applied in the game. Clearly readable small weave and subtle thread variation, occasional tiny woven slubs and natural soft textile depth. Flat even illumination, seamless all edges. No folds, clothing silhouette, embroidery borders, shadows, gradients, buttons, text, logo, watermarks or other objects. Realistic material, not illustration. Full bleed.
