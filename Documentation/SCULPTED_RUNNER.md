# Sculpted runner and soft-joint animation

The runner is now an original young-adult character with a continuous sculpted head and deforming limbs, rather than separate oval features and rigid upper/lower limb pieces. This remains stylised procedural art; no photorealistic, artist-authored or motion-captured result is claimed.

## Appearance

- The new head uses a shaped jaw/chin profile and continuous nose bridge/tip, cheeks and recessed eye sockets. Smaller almond-shaped eyes, tapered eyebrows, curved mouth/eyelid lines, subtle nostrils and shaped ears replace oversized floating facial ovals. The eyes blink briefly during idle/running, unless reduced effects is selected.
- A single swept hair surface follows a shaped hairline, with restrained asymmetry and combed relief. The boy retains a warm complexion and short dark hair.
- Longer legs, narrower shoes, slimmer hands and a more tailored kurta give the silhouette less doll-like proportions. The kurta has shallow cloth folds, a band collar, small ivory buttons, fine piping and seams. Existing cotton texture and the four saved outfit colours remain; the runner's owned material does not recolour scenery.
- Hands have shaped palms, curled fingers and mirrored thumbs. Smaller leather shoes have thin soles and subtle vamp stitching. The saffron scarf is a continuous three-joint mesh with finer fringe.

## Rig and movement

Four SkinnedMeshRenderers deform the two arms and two trouser legs across elbow/knee joints, using normalized two-bone weights and bind poses. Upper sleeve/skin material groups share one continuous arm mesh. The three-joint scarf is a fifth skinned renderer. Geometry, skin weights and materials are built once at startup and reused; Animate creates no new meshes, objects or components.

Run animation uses longer leg proportions, inverse-kinematic foot targets projected inside reach, a tuned cadence/recovery arc, heel-to-toe pitch and toe clearance. Head counter-rotation reduces torso sway at the face; shoulders, elbows and relaxed wrists move together. The scarf follows run motion with damped rotation and bends backwards instead of acting as a rigid hanging prop. Slide arms tuck down and the head counter-rotates to keep a readable low silhouette. Pause freezes the run clock; restart resets all animation blends/scarf state deterministically. Reduced effects removes decorative sway, blinking and scarf flutter while retaining running/jump/slide poses.

Gameplay lane movement, jump physics, hitboxes, scoring, obstacles, camel model, goals, pouch and audio remain unchanged. This update adds no external account, paid asset, downloaded model, package or image-generation dependency.

## Verification

Source checks pass for all 19 C# files, including a scan for nested local name conflicts that caused the previous CS0136 error. Independent audits check face/hair/cloth/limb/scarf index ranges, ascending/descending surface winding and two-bone/scarf weight normalization. An independent 14,400-target IK audit across four speeds and sampled run/jump/slide blends reconstructs targets to floating-point precision. Conservative shoe-sole bounds in the sampled running poses stay above the 0.015 m road top (minimum approximately 0.0218 m); full neutral slide head/arm envelope contracts are checked. These are formula/source checks, not native Unity animation or collision acceptance.

Indian Runner > 4 - Check live startup (Play Mode) now also inspects the actual five skinned renderers, bone references, weight sums, bind-pose/material group counts and mesh references. This live check was not executed here because Unity is unavailable.

## Local review

1. Pull the feature branch and let Unity compile. Press Play; inspect the boy's face/hair and silhouette in the start menu. Cycle any unlocked outfit colours and verify both sleeves/body change together.
2. Start a run. Inspect foot contact/roll, knees/elbows, skin deformation, scarf follow-through and transitions among running, jump and slide. Verify no holes, inverted faces, disappearing skin meshes, clipping cuffs, excessive joint pinching or limbs outside camera framing. In particular review fast lane changes and slide/jump transitions against unchanged hitboxes.
3. Run the live startup check. Test pause/resume, background/foreground, game over, restart and return-to-menu; confirm no pose pop, scarf reset failure, saved outfit loss or duplicated character.
4. Compare reduced effects. Rebuild with Build and Run to update Android; inspect materials/mesh bounds and sustained CPU/GPU frame time/memory on the actual phone. Skinning quality, lighting, perceived realism and actual phone performance require this visual/runtime review.

No Unity screenshot, Unity compiler result, phone capture, subjective visual approval or measured FPS is claimed for this implementation workspace.
