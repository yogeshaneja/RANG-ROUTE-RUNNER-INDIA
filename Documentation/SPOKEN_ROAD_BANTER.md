# Spoken road banter

Android now has actual synthesized spoken dialogue through its installed Hindi text-to-speech engine, alongside short Roman Hindi captions. This is separate from the existing generated engine/animal sound effects. No audio recordings, voice account, paid API, external runtime request or automatic voice download is added.

## Characters and lines

There are 15 original lines, three each for motorcycle riders, peacocks, camels, elephants and a fictional festival caller. Each group has a different pitch/rate profile; these are variations of the same installed Hindi voice, not five recorded actors or a verified Rajasthani/Marwari accent. The regional wording is playful Rajasthan-flavoured Hindi draft dialogue. Animal speech is deliberately cartoon dialogue, in addition to the existing animal effects.

Examples:

- Motorcycle rider: “ज़रा किनारे हो जा, महाराजा!” — Zara kinaare ho ja, Maharaja!
- Peacock: “पहले म्हारा नाच देख ले, छोरा!” — Pehle mhara naach dekh le, chhora!
- Camel: “ओ छोरा, म्हारी चाल तो शाही है!” — O chhora, mhari chaal toh shaahi hai!
- Elephant: “ज़रा हट जा, बड़ा मेहमान आया है!” — Zara hat ja, bada mehmaan aaya hai!
- Festival caller: “नाच बाद में, पहले रास्ता दे!” — Naach baad mein, pehle raasta de!

Lines trigger near the passing encounter, are deduplicated per encounter, rotate within each speaker group and have a nine-second global cooldown. Captions last 4.5 seconds and sit below safety warnings, above the road/runner view. They use Roman Hindi because full Devanagari shaping has not yet been verified in the prototype IMGUI font. The native speech engine receives the actual Hindi Unicode text.

## Phone setup and controls

Build and Run the updated Android APK. At Start, keep Sound and Voices enabled, wait for “Offline Hindi voice ready”, then press TEST HINDI VOICE. The test speaks the motorcycle rider's first line without needing a run. Gameplay selects other lines/speakers automatically.

The engine must have an installed embedded Hindi voice. The app filters out network-required voices and those marked not installed. If none is available it shows a message to add offline Hindi speech data in the phone's text-to-speech settings; it keeps the rest of the game and captions working. Install the pack manually if needed, then restart the game. Selection/status is checked on each app launch; there is no forced settings screen, store launch or download.

Sound OFF also mutes spoken lines; Voices OFF mutes only the spoken banter. Music remains separately controlled. Voice preference saves locally. Music ducks briefly when a native speech request is accepted. Pause, lost focus, backgrounding, restart, game over and return-to-start stop speech; old sentences are not resumed/queued. Disposal shuts down only this game's TextToSpeech instance. Unity Editor/desktop previews captions; spoken playback is Android-only in this implementation.

## Android integration

AndroidSpeech uses built-in Unity Android JNI and the Android TextToSpeech API. Initialization runs on the activity UI thread; the preserved OnInit proxy only records status, with voice selection on the main Unity update. Failure/timeouts are nonfatal. Calls use an explicitly selected embedded Hindi voice. The supported String/HashMap speak overload bridges directly through Unity JNI, with a restrained volume and QUEUE_FLUSH to avoid overlapping preview speech.

RunnerSpeechManifest adds the Android 11+ TTS service discovery query to the generated Gradle library manifest. It is idempotent and preserves Unity's Activity/GameActivity and existing permissions/queries. It does not replace the project manifest, add INTERNET permission, introduce a Java/Android plugin or require an extra Unity package. The existing native Android JNI module is reused. Existing 17 Unity sound clips/four AudioSources remain; speech is played by the phone service outside Unity's audio mixer, so lifecycle/muting is handled explicitly.

API references checked during implementation:
- https://developer.android.com/reference/android/speech/tts/TextToSpeech
- https://developer.android.com/reference/android/speech/tts/TextToSpeech.Engine
- https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/android/ipostgenerategradleandroidproject

## Required local acceptance

1. Pull, let Unity compile and press Play. Check captions, voice status/control layout and existing core/live-startup checks. Editor silence is expected; native speech is Android-only.
2. Build and Run. Verify the generated/merged manifest contains exactly one TTS_SERVICE discovery action and preserves the configured Activity and existing permissions.
3. On the real phone, test the Hindi voice from Start. Listen to pronunciation, volume, timing and character pitch/rate; verify the music dips/restores. This is synthetic Hindi, so native-speaker review is still needed for regional phrasing/pronunciation.
4. Follow passing motorcycles and living scenery/hazards; verify speaker/caption matches the encounter, lines rotate and do not repeat every frame. No caption should cover a safety warning or the runner.
5. Toggle Voices and Sound independently, including while speech plays. Pause/background/restart/quit mid-line and ensure no lingering speech or stale queued sentence. Confirm settings persist.
6. With an installed offline Hindi pack, test a fresh launch in airplane mode. Also test a phone without Hindi data: the game/captions should continue and show the setup hint. Check APK build/IL2CPP callback preservation and sustained phone performance.

No Unity compilation, Android Gradle build, installed-engine selection, actual speech audition or real-device offline test was possible in this workspace. Source/API-contract checks and independent scheduler/manifest simulations do not establish native runtime success.
