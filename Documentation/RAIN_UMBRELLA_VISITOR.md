# Rain-only umbrella visitor

A new original adult woman in a bright pink lehenga, turquoise blouse and dupatta holds a turquoise/pink umbrella with gold ribs. During rain she can occupy one marked obstacle lane, leaving the reserved token lane clear.

Her exact revised line is:

**“देखो, बारिश हो रही है! It's raining, it's raining!”**

The native Hindi voice receives the English words phonetically in Devanagari (“इट्स रेनिंग, इट्स रेनिंग”) for the existing Hindi speech engine. The matching on-screen caption is “Dekho, baarish ho rahi hai! It's raining, it's raining!” The previously discussed invitation line is not used. English mode/fallback says “Look, it is raining! It is raining, it is raining!” with a matching English caption. This remains synthesized dialogue from an installed offline Hindi or English voice, with a slightly brighter pitch, not a recorded actress or verified regional accent.

## Rain and safe spawning

- Visible rain intensity, rather than just the selected weather label, controls admission. A visitor can be committed only above 0.12 intensity and 180–210 m ahead. Late rain never pops an obstacle into the nearby play area.
- Candidate rows start at 182 m and repeat every 504 m. They avoid garden/transition rows, moving-encounter recovery rows and repair rows. The visitor replaces the candidate row's existing blockers (including an occasional camel); other blocked lanes are cleared, while the same reserved token lane/coins remain.
- Once committed she stays until the row is passed/recycled, even if rain clears, so obstacles do not suddenly vanish. No new visitors are admitted in dry weather. Her rain line is triggered only while rain is still visible.
- A normal warning names the clear lane, and the amber marker identifies her lane. Body, high umbrella canopy and shaft have separate swept collision boxes: walking/sliding under the outside canopy edge does not hit an invisible floor-to-roof wall, but body/shaft contact or jumping into the canopy ends the run with a gentle stumble message.
- All visuals are shared through the existing fixed obstacle pool. No objects/meshes are created during admission. Reset/recycling clears the visitor flag and restores ordinary row generation. The rain effects/settings themselves are unchanged.

## Voice and preview

The spoken-banter catalogue now contains 16 lines across six speaker groups. The visitor has one fixed requested line. Reset/index selection uses each group's actual length, so the one-line group cannot index outside its array. Nearby committed visitors reserve a 10.5-second speech slot; this keeps other chatter from consuming the existing nine-second cooldown just before her line. Safety warnings and natural sound effects are unaffected.

At Start select **Sky → Rain** and, with Sound/Voices enabled and the installed Hindi voice ready, tap **the voice test**. This previews her line immediately. Then run to the first candidate at 182 m. With Auto weather, visitors only appear once actual rain arrives and a candidate is still sufficiently far ahead. Unity Editor previews captions/visuals; speech playback remains Android-only.

## Local acceptance

1. Pull, let Unity compile, press Play and run the core/live-startup checks. Select Rain to inspect umbrella panels, face/clothing and warning/marker contrast in 9:16.
2. Build and Run again. Test her Hindi/English line from Start with an installed offline Hindi voice; audition the actual English pronunciation, timing and volume on the phone. Check the caption matches the revised wording.
3. Take the token lane, deliberately contact her body/pole, walk/slide under the outside edge and jump into the canopy. Check collisions match the visible shapes; review/tune the conservative canopy box if required.
4. Start dry, switch to rain and clear the weather again. Check no close pop-in, no dry admissions, preserved garden/recovery rows and stable committed visitors. Test reduced effects, pause/resume, restart and recycling past 1,000 m.
5. Confirm the nine-second voice cooldown still applies, the visitor's reserved line isn't crowded out by a rider, and no native speech continues after background/pause/game over. Verify sustained Android frame time/memory and airplane-mode voice playback.

Source parsing/scope, independent numerical/mesh/collision/data-contract checks passed here. Unity compilation/rendering, native speech audition and actual Android performance remain unverified in this workspace.
