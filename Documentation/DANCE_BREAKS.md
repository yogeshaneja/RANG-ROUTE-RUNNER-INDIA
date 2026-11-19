# Five-second rest dance

The runner takes a five-second dance break on entering each clear festival garden: first at approximately 280 m, then 784 m, 1,288 m, and so on (existing 504 m garden spacing). The break triggers once per garden entry, after the normal collision/collection step and only if no collision occurred. Rest rows and their buffer already exclude obstacles and rain visitors.

## Behaviour

- A dedicated DanceBreak state freezes run clock, distance, world/hazards, scoring, coin collection and timed power-up expiry.
- Gameplay input is bypassed before it is read. Swipes, jump, slide and pause keys are ignored; there are no popup buttons and no queued gesture is resumed afterward.
- A large REST TIME! panel shows 5, 4, 3, 2, 1 with “Break time! Dance along.” The existing score/token HUD stays visible. The camera eases closer while the runner faces the viewer.
- The original procedural dance raises the arms, alternates wrists/shoulders, bounces the hips, steps the feet and moves the scarf at 140 BPM. It uses the existing rig and meshes. Reduced effects use smaller movements and a fixed camera framing.
- At five foreground seconds, dance music ends, the regular music playhead is restored, lane movement is re-established and the camera eases back to the run. A short LET'S GO! message marks the restart. No bonus or duplicate token scoring is granted by dancing.
- App background/focus loss pauses the break, sound and countdown. Returning shows the normal pause screen; Continue resumes the remaining dance time. Restart/return to Start clears the break and restores normal music. Native/recorded speech is stopped on interruption and never queued for a later surprise.

## Music

An original 140 BPM drum/clap/pluck groove temporarily replaces the existing background clip on its existing music AudioSource. It is generated once at startup, with edge fades and bounded levels: 75,600 mono samples at 22,050 Hz (3.429 s musical loop, repeated as needed during the five-second break). No music download or additional AudioSource is added. Music OFF still mutes it. A short fade lowers it before returning to running music; speech ducking for the two-word cue is limited to 1.25 seconds rather than the whole break. Hazard/animal loops stop during the break.

## Spoken cue status

The supplied 16 Hindi recordings do not contain “Break time!”. RunnerAudio checks for an optional `Assets/IndianRunner/Resources/Dialogue/hi/break_time.ogg` (other supported audio extensions also work). If supplied, that recording plays through an existing effects source.

**That extra recording has not been supplied.** In its absence the game tries a separate, lazily initialized installed offline English phone voice for “Break time!”. It is attempted only within the first two seconds of the break; unavailable/slow speech stays silent and the visible text still works. This new cue therefore has a phone-voice dependency until its short recording is added. The original 16 Hindi encounter clips remain bundled and independent of phone speech data. Both Sound and Voices must be ON for the cue. No account, internet, paid generation or external voice download is introduced.

## Phone acceptance

1. Run to 280 m and confirm a single five-second break with a readable 5..1 countdown and visible dancing. Repeat at the next garden; it must not retrigger every frame while still inside one garden.
2. Swipe/tap/use arrow keys and pause keys during the countdown; distance, score, coins and hazards must stay fixed and controls must not execute later.
3. Enter while jumping, sliding or changing lanes. The runner should settle, dance and resume safely inside the empty garden, with no duplicated pickups or sudden dangerous obstacle.
4. Background mid-countdown, wait longer than five seconds, return and Continue. Only the remaining foreground time should play. Return to Start/restart from the pause screen should restore normal music and clear old break state.
5. Test Music OFF, Sound OFF, Voices OFF and reduced effects. Listen to the dance groove, cue timing, fade, music restoration and absence of lingering hazard/voice sounds.
6. Test the cue with and without an English phone voice and, later, with a supplied break_time recording. Regular Hindi clips must still work without speech packs.
7. Profile startup/music memory and repeated breaks on the target phone. No Unity compilation, rendered animation/camera audition, native speech test or real-device playtest was possible here.
