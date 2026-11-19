# Festival audio and mirror-work collectibles

The road pickups are now original mirror-work-inspired ornaments: turquoise fabric centres, gold embroidered petals and diamond mirrors on both faces. They face the player with gentle rocking instead of turning edge-on. The HUD/results say tokens; collecting one still awards ten points. Existing local best-score data is retained, and the internal coin counters/method names are kept for compatibility. There are 70 fixed pooled ornaments sharing three material/mesh groups, with no new meshes allocated when collecting/recycling.

## Original offline audio

The old single-tone cues and short motif are replaced with an original 15.238-second, 126 BPM eight-bar instrumental loop using brighter pentatonic plucks, answering lead phrases, moving bass, stronger hand-drum-like pulses, backbeat claps and shakers. Notes have attack/release envelopes and the loop ends at zero. This is synthesized original music, not a recording of traditional performers or a claim to reproduce an authentic regional instrument.

Pickups play a gentle four-note chime sequence with a short retrigger limit; jumps have a soft rising swish and game over has a short rounded drum. A pooled engine source fades/pans in as a motorcycle approaches and lowers pitch after passing; the spaceship uses a separate airy hum on the same source. Camel calls and occasional roadside-elephant trumpets are stylised synthesized effects, not realistic animal recordings or spoken voice acting. They trigger once per nearby encounter and duck the music briefly. Music/effects retain separate controls; pause freezes audio, restart clears encounter sounds and returning to menu stops vehicle/animal sounds.

All 12 clips are generated once at startup (four pickup variations, jump, stumble, engine, spaceship, camel, elephant, festival arpeggio, music), tracked for disposal and played through four AudioSources. No network, downloads, paid services or third-party audio rights are required. There is no clip creation/allocation in the per-frame audio Tick method. Mixer levels are conservative, but final loudness/timbre must be auditioned on the target phone.

## Larger decorated scenery and obstacles

Camels, carts, pottery and fabric are 12% larger; the motorcycle is 22% larger, the spaceship 30%, and awnings 5%. Their collision envelopes use the same scale. Spaceship landing markers now enclose the enlarged saucer. Camels get mirror-work saddle embroidery; roadside camels/peacocks, birds and kites are larger. Three decorated elephant scenery groups are allocated across the seven-tile pool beside the road. They do not cross lanes, collide with the player or represent wild desert animals.

The main runner and the safe-route rules are preserved. At the enlarged camel envelope the reserved lane retains at least 0.742 m clearance beyond the player's collision envelope. Camera visibility, actual mesh alignment and phone frame time remain runtime acceptance checks.

## Local acceptance

1. Pull the feature branch, open Rajasthan and press Play. Confirm menu/runner appear; the previous landing-marker startup fix remains in place. Run Indian Runner > 4 - Check live startup while playing.
2. Listen through phone speaker and headphones: music fades in, chimes remain gentle during repeated pickups, and engine/animal/ship effects do not overpower the music. Toggle music/sound independently in menu, pause and results.
3. Pause/background while a sound plays, resume, end the run and restart several times. Confirm no stuck engine, repeated animal call or duplicate music source.
4. Check both mirror faces, token labels/count/score, larger collision contact vs adjacent lane, jumping enlarged pottery/fabric, sliding through enlarged awnings and the spaceship's enlarged landing rectangle.
5. Play beyond 600 m to exercise recycling; inspect elephants beside the road, camel saddle details and sustained frame time/memory on the real Android phone. Build and Run again to update the installed APK.

No Unity Editor/audio-device audition or Android performance measurement was available in the implementation workspace.

The subsequent goals/gardens pass adds safe festival sections, pouch power, outfit rewards, a welcome arpeggio and a rare spaceship schedule. See FESTIVAL_GOALS_AND_GARDENS.md for current timing and acceptance checks.

## Faster festival groove and start

The new original background track runs at 126 BPM (up from 96), with 64 eighth-note melody/rest slots, bass movement, low/high drum accents, claps and sixteenth-note shaker texture. Short attack/release envelopes and loop-edge fades avoid hard starts/clicks. Frequencies are precomputed at initialization; the track is still generated only once. Default music level is slightly higher and pickup ducking is less deep, while dialogue keeps its stronger duck. Scenic passages change level gently without speeding up/detuning the track. Procession drums now use the same beat clock (four-beat loop), synchronized with music samples.

The mono 22,050 Hz music clip is 336,000 samples instead of the former 441,000; no audio downloads, music assets or additional AudioSources were added. This is original synthesized music, not recorded Rajasthani instruments or a traditional-song performance. Startup time, music enjoyment and device mix require audition/profiling.

RunRules.StartSpeed is now 11 units/s (was 9), about 22% faster at launch. Speed still rises gradually by distance and caps at the existing 18. Lane-change speed, jump/slide physics, obstacle rows and maximum-speed reaction allowance are unchanged. Check swipes, warnings, the initial camera transition and the easier first goals on the phone at the faster launch pace.

## Comic game-over crank

Game over now plays a short original 0.78-second raspy descending non-verbal groan/crank over a softer end drum. It is a synthesized sound effect, not a recorded actor, spoken Hindi line or new phone-speech request. The clip is created once (17,199 mono samples at 22,050 Hz) and reused on the existing effects source. Game over stops outgoing effect/animal/approach sounds first, briefly lowers the music, and restart stops the crank so it cannot bleed into the next run. Sound OFF mutes it. Audition volume/tone on the phone and verify only one game-over cue is played per run.
