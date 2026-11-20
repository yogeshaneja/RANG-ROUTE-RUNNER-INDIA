# Natural environment and runner presentation

## Character

- Original adult Indian male design: shaped face/jaw, ears, dark eyes/brows/hair, tailored teal kurta with collar/placket/buttons/hem piping, scarf fringe, cream trousers and brown shoes.
- Elliptical ring meshes provide smooth silhouettes; rigid decorative parts are combined by material. Joint transforms remain separate.
- Hip/knee/ankle inverse kinematics reach ground/swing targets; short sprint stance is tied to ground speed, with knee lift in recovery. Shoulders and elbows counter-swing, the torso leans/twists slightly, and the head counter-rotates.
- Slide lowers the pelvis and leans the torso back while folding legs/arms. No full-body scale squash is used. Pose transitions, scarf motion and a radial contact shadow soften the result. Reduced effects remove cosmetic bob/twist/flutter.
- This is a procedural articulated model. A skinned production character, cloth simulation and artist-authored animations remain future work.

## Environment

- Seven pooled sections now vary between open roadside and denser village scenes; object distribution is deterministic, offline and generated once.
- Houses vary in size, setback and rotation, with actual 3D door arches, lattice windows, sills, cornices and occasional fabric shades.
- Original frontage artwork is retained as occasional distant detail. The repeated close flat-wall treatment is removed.
- Irregular acacia branches/crowns, sparse shrubs, scattered stones and pots replace evenly spaced pot/ball combinations. Scenic animal density is reduced.
- Road and sand use original generated 128x128 grain maps with mipmaps; sand shoulders have gentle height variation outside the playable road. Terrain endpoints return to the same height/slope to avoid tile steps.
- Warmer lighting, softer sky colours, moderated illustration saturation and layered clouds support the natural palette.

## Constraints and review

The lane/obstacle/camel mechanics are unchanged. Camera framing, shading and animation are new and need Unity/phone validation. The perspective camera requests MSAA and uses a 62-degree vertical view; built-in rendering is configured for 2x MSAA, while URP retains its existing quality settings. All new art is original native geometry/shader/texture code. No stock download, paid asset, cloud request, added package, merge or publication is included.

Check portrait outer-lane framing, joint motion, contact shadow, material compatibility, terrain seams, warning visibility, restart/pause behaviour and phone profiling before calling this visually or performance validated.
