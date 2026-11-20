# Festival Rush

A rare, playable bonus version of the existing authored adventure passage. The first is at **112–224 metres**, lasting about 9–10 seconds at the opening pace. It repeats every **3,024 metres of the whole journey** (not on each state entry). Ordinary adventure passages retain their jump/slide challenges and 2× rewards between festivals.

| State | Original festival direction |
| --- | --- |
| Rajasthan | Desert Kite Festival: diamond kites, flowing tails, marigold gateways, pink and turquoise ribbons |
| Kerala | Flower Festival: warm flower garlands, cream rosettes, golden roadside parasols |
| Punjab | Harvest Mela: bright colour panels, diamond motifs inspired by Phulkari, magenta and gold flowers |
| Goa | Coastal Carnival: turquoise colour panels, jewel lanterns, coral/gold flower decorations |

These are stylised, invented celebrations inspired by regional scenery, not reconstructions of a named religious event. There are no downloaded assets or recordings.

## Play

An advance cue appears at 84 m. A gold banner and shrinking teal distance bar identify the active section. The road is clear of obstacles, moving traffic, animal crossings and rain visitors. Steering, jumping and sliding remain responsive; follow the rotating, 22% larger regional token trail. Each token awards **3**, rather than the usual 1 or adventure/vehicle value 2. Twenty pooled tokens offer up to **60 token units**. These values flow through the usual score and goal system exactly once, including a helper pickup. No automatic magnet or invulnerability persists after the section. The completion message reports actual units collected.

The five-source audio system adds a short original 140 BPM instrumental phrase: rounded double drums, handclaps, warm plucks and an answering melody. Each state uses a different tonic. Music smoothly fades between the running and festival loops without changing the running loop's playhead. Music and effects preferences remain separate. Dialogue is suppressed during the bonus section; recorded Hindi dialogue remains unchanged elsewhere. Pause/background freezes the section and pauses every audio source. Restart/menu/game over stops and resets the festival layer. Existing dance-break music stays independent.

## Budget

One baked festival scenery root per each of seven recycled tiles; only the current state's decorations are constructed. No new pickup objects, textures, packages, backend or network calls. A fixed pool of **24** small petals uses six-sided opaque meshes, reduced to **6** in reduced-effects mode with gentler motion. Petals stay above 4.2 m, above the runner's normal jump silhouette; decorations sit beyond the outer lanes or overhead. Static decorations are batched once, never rebuilt during play. One 151,200-sample mono procedural music clip adds about **591 KiB of raw float sample data**, plus Unity/source overhead. Phone frame time and memory remain unmeasured.

## Validation

Editor core checks cover event boundaries, repetition, rewards, all-state obstacle exclusion and separation from rides/rest/transfers. Live pool validation now requires the festival overlay on all seven tiles. Independent source and numerical audits are recorded in VALIDATION.md. Unity Editor and Android tooling are unavailable in the implementation environment: no Unity compilation, rendered preview, audio audition or APK/phone performance test is claimed.

Before release: test each starting state through 224 m; collect with both player and helper; pause/background during the music fade; disable/re-enable music; restart; run into a subsequent ordinary passage and vehicle ride; travel between states; try reduced effects; inspect garland/HUD visibility and sustain a long run for memory/frame pacing. Run Indian Runner → Check Core Rules and the live startup check in Unity, then Build And Run on the phone.
