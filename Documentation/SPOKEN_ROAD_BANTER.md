# Spoken road banter — Hindi and English

Android uses its installed offline text-to-speech engine for 16 original dialogue lines: three each for motorcycle riders, peacocks, camels, elephants and a festival caller, plus the rain-only umbrella visitor. Every line has Hindi text, a Roman Hindi caption and an English translation. Captions and speech use the selected voice language together. This setting covers dialogue, not full interface translation.

## Language selection

At Start, tap DIALOGUE to cycle through the saved choices:

| Choice | Voice selection |
| --- | --- |
| AUTO (default) | Installed offline Hindi first; otherwise installed offline English. |
| ENGLISH | Installed offline English, with English speech and captions. |
| HINDI | Prefer installed offline Hindi; fall back to English if unavailable. |

The app accepts any installed English locale (such as en-IN, en-GB or en-US). It filters out network-required voices and voices marked not installed. It never sends Hindi Unicode to an English voice. If no suitable voice is installed, gameplay/captions continue with an explicit setup hint. English support avoids needing a Hindi pack, but still requires the phone to have a working offline English voice. No download, recording, account, network service or extra package is added.

The umbrella visitor's English line is “Oh, look! It's raining... it's raining!” Her Hindi line remains “देखो, बारिश हो रही है! इट्स रेनिंग, इट्स रेनिंग!” The previously rejected invitation is not used.

The six character groups now use separate saved voice assignments where installed voices permit. Each line varies pitch/rate within a restrained range instead of the previous extreme animal pitch shifts. These remain synthetic voices, not recorded actors or verified regional accents. Hindi dialogue is playful Rajasthan-flavoured Hindi draft wording, not verified Marwari. Animal dialogue is intentionally cartoon speech alongside natural sound effects.

## Setup and controls

Pull feature/rajasthan-unity-prototype, let Unity compile, and Build and Run again. At Start, keep Sound and Voices ON. Select DIALOGUE ENGLISH to explicitly use English, or leave AUTO for automatic fallback. Wait for the ready/fallback status, increase phone MEDIA volume, then tap TEST ENGLISH VOICE or TEST HINDI VOICE. During rain the test uses the umbrella visitor's line; otherwise it uses the first rider line.

RETRY disposes the old speech engine and creates a fresh one using the saved language choice. Use it after changing phone speech settings or recovering from a timeout/failure. Changing DIALOGUE also stops old speech and replaces the engine immediately. Selection, mute flags and language persist locally. Nothing automatically opens a store or settings screen. CAST opens character voice choices: NEXT cycles installed offline voices and auditions the character when unmuted; LISTEN replays the assigned voice. BACK stops the preview. Voices save separately for each character, language and speech engine.

Sound OFF also mutes dialogue; Voices OFF mutes only speech. Music is separate. Accepted speech requests briefly duck music. Pause, lost focus, background, restart, game over and return to Start stop speech. Old sentences are not queued for later. Unity Editor/desktop is caption-only; AUTO previews English captions there and HINDI previews Roman Hindi.

## Integration and limits

AndroidSpeech uses Unity's existing Android JNI module and the Android TextToSpeech API. Construction is on the activity UI thread; the preserved OnInit proxy records its result, and voice selection occurs on the Unity update. The selected voice must be embedded, installed and match hi or en. The catalog is built only at initialization/retry. Candidates are ranked by engine quality, locale preference (India where available) and latency; equivalent candidates can supply different character voices. Speaker switches call setVoice only when the assigned voice changes. Cached JNI voice references are disposed on retry, language change and shutdown.

Playback uses the API 21+ CharSequence/int/Bundle/String signature with an explicit JNI descriptor, typed float volume and QUEUE_FLUSH. Media/speech audio attributes follow media volume without changing it. JNI argument references are released in a finally block. Setup/playback exceptions log under `Dialogue voice`. A successful request only means queued, not that sound was heard. Asynchronous synthesis errors still require device logs because no UtteranceProgressListener is installed.

RunnerSpeechManifest adds the Android 11+ TTS_SERVICE discovery query to Unity's generated Gradle library manifest, preserving existing manifest data and permissions. There is no new INTERNET permission, Java plugin, audio recording, speech model or bundled clip. Existing Unity sound-effect/music pools remain unchanged.

Lines still deduplicate per encounter, rotate within their group, and use a nine-second global cooldown. Captions last 4.5 seconds below safety warnings. The rain visitor keeps her reserved dialogue slot and rain-only trigger. No gameplay, obstacle or weather timing changes are part of this update.

## Required phone checks

1. Compile in Unity and verify the Start language/test/retry controls and captions in portrait.
2. Build and Run on Android; verify the merged manifest retains TTS_SERVICE discovery.
3. With no Hindi pack but offline English installed, launch with AUTO: expect an English fallback status, English test voice and English gameplay captions/speech.
4. With both languages installed, AUTO prefers Hindi. Explicit ENGLISH must use English despite Hindi being present. Check HINDI and automatic fallback too.
5. Switch language while a preview speaks: the old sentence must stop. Confirm choice survives restart, RETRY retains it, and Sound/Voices mute independently.
6. Verify rain visitor, motorcycle, peacock, camel, elephant and caller speech/caption alignment, cooldown, music ducking and lifecycle stops.
7. Test airplane mode, zero media volume, connected Bluetooth routing, no installed voice and failed initialization. If the phone's own English speech sample works but the game does not, collect the Start status and `Dialogue voice` Android Logcat entries.

Source parsing is not a Unity compile, Android build or audible playback test. Native engine selection, pronunciation, rendering, offline behaviour and device performance need real-device verification.

API references:
- https://developer.android.com/reference/android/speech/tts/TextToSpeech
- https://developer.android.com/reference/android/speech/tts/Voice
- https://developer.android.com/reference/android/speech/tts/TextToSpeech.Engine

## Gender and natural delivery

Rider and festival caller request male casting, and the umbrella lady requests female casting. Android Voice has no standard gender field. Automatic matching therefore uses only explicit whole-word male/man or female/woman labels in engine voice names; opaque IDs are never guessed to be a particular gender. An unknown-label voice is preferred over a known opposite label when the requested gender is unavailable. When engine labels are absent or misleading, use CAST to listen and save the correct actor manually. Lowering pitch alone cannot ensure a male voice. If a phone has only one suitable voice, it cannot produce a genuine second actor without additional offline voice data or recordings. The app does not add/download either automatically.

Speech now keeps pitch in 0.88–1.12 and rate in 0.90–1.12; authored line profiles are narrower still. Rider lines are more conversational, animal voices retain character without the previous deep/helium shifts, and punctuation introduces engine-managed pauses. Changes are per authored line, with stable casting rather than random voice swaps. TTS quality still depends on the device engine; human-quality delivery is not guaranteed.

Device acceptance: audition all six CAST roles in Hindi and English; check that the rider/caller sound male and lady female, correct unknown-ID assignments manually, and verify saved choices survive restart/retry/language switching. Check NEXT/LISTEN mute controls, preview interruption, role switching in runs, offline use and disposal after repeated language changes. These audio qualities cannot be verified by source parsing.
