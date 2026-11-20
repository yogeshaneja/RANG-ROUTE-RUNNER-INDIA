# Art and feel polish

## Intent

Improve the phone experience through realistic-looking surfaces and a cohesive game presentation, while retaining the offline Unity architecture and existing scoring/collision rules.

## Art

A new generated photo-like Rajasthan desert/fort panorama replaces the saturated illustrated backdrop. Sandstone texture is applied to real 3D wall geometry; cotton weave is applied to tinted character clothing. Flat illustrated storefront cards are disabled. Repeating textures use appropriate wrap/mipmap/compression metadata, and rounded meshes now have separate UV seam vertices with matched normals.

The character remains original procedural geometry. More lifelike production motion requires a properly skinned character and authored/retargeted animation clips; these assets are not part of this pass.

## Input, camera and movement presentation

Swipes commit once as soon as the distance threshold is crossed, without waiting for release. Cancelled gestures, small taps and header touches are excluded. A front-facing three-quarter menu camera introduces the character; start transitions into the running view over 0.7 seconds. The initial two obstacle-free rows cover that introduction. Camera FOV changes only slightly with speed, with damped follow; reduced effects skip intro/speed-FOV changes.

Idle breathing, asymmetric elbow flex and small directional yaw soften the procedural motion. The underlying obstacle/camel collision and scoring contracts are retained.

## UI and feedback

Cached rounded dark/glass panels and gold primary buttons replace default grey GUI controls. The start view exposes the character; HUD focuses on score, distance, coins and pause. Pause/results screens provide Continue/Run Again and Back to Start. Implementation notes are removed from player-facing screens; the offline future-content reminder remains.

Coin feedback uses an 18-object bounded pool, with fewer glints on reduced effects; milestones appear briefly every 100 metres. Effects freeze during pause and reset on restart/return to the menu. Roadside decoration groups get bounded deterministic variation on recycling, independently of fixed road/ground/obstacle coordinates.

## Review

This source pass has not been run inside Unity in the execution workspace. Rebuild on the phone and check textures, gesture feel, camera/UI framing and long-run resource behaviour. Source/numerical checks are described in VALIDATION.md. No store publication, merge, paid assets, runtime network request or new package is included.
