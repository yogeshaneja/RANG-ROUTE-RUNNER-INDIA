# Destination picker — second step

## Flow

1. Welcome keeps the runner preview, settings, goals, outfit, voice preview and Home exit. LET’S RUN now opens the destination picker.
2. The picker displays four illustrated cards in stable Rajasthan/Kerala/Punjab/Goa order, with state scenery, a short description and that state’s saved personal best. A gold border and SELECTED badge mark the draft choice. The last played state is preselected; a fresh installation defaults to Rajasthan. Cards are large touch targets.
3. START [STATE] RUN confirms the highlighted state. If it is already loaded, the run starts immediately. Otherwise its ID is saved and the existing bootstrap scene reloads. The new manager consumes an exact-region one-shot request and automatically starts that state, without returning the player to Welcome.

Back (including desktop Escape/Android Back through the existing pause input) returns to Welcome without committing a draft choice. RUN AGAIN remains a quick restart of the same state; BACK TO START returns to Welcome and LET’S RUN opens the picker again. Scores/settings/outfits retain existing persistence keys.

## Presentation and footprint

A dark teal full-screen panel contains a two-column scenic grid, warm gold selection border, readable state names, scenic captions and per-state best scores. The footer names the selected first stop and its hazards/collectible theme. The grid scrolls within the safe area when space is limited; Start and Back remain outside the scroll region. Cards are at least 224 logical units high to keep captions and best scores separate.

StageSelectionScreen loads four original bundled JPEGs once when first opened and caches their labels/scores. Each thumbnail is 512 × 288; all four total 242,273 JPEG bytes (about 237 KiB). Importers request no mipmaps, no readable copy, no NPOT resizing, clamp wrapping and compression. Actual native import/memory results need Unity inspection. No thumbnails are fetched online and browsing a card does not construct another region. The destination screen hides the 3D camera’s render layers and skips idle world updates; Back/same-region Start restores the mask, while a new region creates a fresh camera. Preview assets are unloaded when the screen’s owner is destroyed.

Art and exact generation prompts are recorded in STAGE_SELECTION_ARTWORK.md. The images are scenic illustrations rather than game screenshots. Temporary original-resolution images are not included in the repository.

## Validation

C# syntax parsing, metadata uniqueness, JPEG decode/dimensions/resource names and source wiring checks pass. Numerical layout checks cover ordinary, notched/shorter safe areas and taller portrait layouts; a caption overlap found during these checks was corrected by increasing the minimum card height. These are source/numerical checks, not an executed Unity UI test.

Required local acceptance: LET’S RUN always opens step two; every card highlights correctly; each named Start loads that exact state; switching never briefly runs the old route; saved bests match each state; Back cancels draft selection; reopening preselects the last played state; restart stays in that state; scroll/thumbs/text/footer work on the phone safe area; Sound/Voice/Home and pause/resume still work. Rebuild the APK after pulling the feature branch. No Unity Editor, APK build or phone UI verification was available here.
