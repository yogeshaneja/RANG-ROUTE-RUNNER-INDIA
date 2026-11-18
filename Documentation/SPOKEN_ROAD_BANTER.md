# Spoken road banter

Android now has actual synthesized spoken dialogue through its installed Hindi text-to-speech engine, alongside short Roman Hindi captions. This is separate from the existing generated engine/animal sound effects. No audio recordings, voice account, paid API, external runtime request or automatic voice download is added.

## Characters and lines

There are 15 original lines, three each for motorcycle riders, peacocks, camels, elephants and a fictional festival caller, plus one fixed rain-only umbrella visitor line (16 total). Each group has a different pitch/rate profile; these are variations of the same installed Hindi voice, not five recorded actors or a verified Rajasthani/Marwari accent. The regional wording is playful Rajasthan-flavoured Hindi draft dialogue. Animal speech is deliberately cartoon dialogue, in addition to the existing animal effects.

Examples:

- Motorcycle rider: “ज़रा किनारे हो जा, महाराजा!” — Zara kinaare ho ja, Maharaja!
- Peacock: “पहले म्हारा नाच देख ले, छोरा!” — Pehle mhara naach dekh le, chhora!
- Camel: “ओ छोरा, म्हारी चाल तो शाही है!” — O chhora, mhari chaal toh shaahi hai!
- Elephant: “ज़रा हट जा, बड़ा मेहमान आया है!” — Zara hat ja, bada mehmaan aaya hai!
- Festival caller: “नाच बाद में, पहले रास्ता दे!” — Naach baad mein, pehle raasta de!

Lines trigger near the passing encounter, are deduplicated per encounter, rotate within each speaker group and have a nine-second global cooldown. Captions last 4.5 seconds and sit below safety warnings, above the road/runner view. They use Roman Hindi because full Devanagari shaping has not yet been verified in the prototype IMGUI font. The native speech engine receives the actual Hindi Unicode text.

## Phone setup and controls

Pull the latest feature branch and Build and Run the updated Android APK. At Start, keep Sound and Voices enabled, wait for “Offline Hindi ready”, then press TEST HINDI VOICE. The test speaks the motorcycle rider's first line without needing a run. Gameplay selects other lines/speakers automatically.

The engine must have an installed embedded Hindi voice. The app filters out network-required voices and those marked not installed. If none is available it shows a message to add offline Hindi speech data in the phone's text-to-speech settings; it keeps the rest of the game and captions working. Install the pack manually if needed, return to Start and tap RETRY. Retry disposes the old engine and initializes a fresh one, including after a timeout or a playback failure. Selection/status is checked on each app launch and explicit retry; there is no forced settings screen, store launch or download.

Sound OFF also mutes spoken lines; Voices OFF mutes only the spoken banter. Music remains separately controlled. Voice preference saves locally. Music ducks briefly when a native speech request is accepted. Pause, lost focus, backgrounding, restart, game over and return-to-start stop speech; old sentences are not resumed/queued. Disposal shuts down only this game's TextToSpeech instance. Unity Editor/desktop previews captions; spoken playback is Android-only in this implementation.

## Android integration

AndroidSpeech uses built-in Unity Android JNI and the Android TextToSpeech API. Initialization runs on the activity UI thread; the preserved OnInit proxy only records status, with voice selection on the main Unity update. Failure/timeouts are nonfatal. Calls use an explicitly selected embedded Hindi voice. Playback uses the API 21+ CharSequence/int/Bundle/String signature with an explicit JNI method descriptor, a typed float volume and QUEUE_FLUSH. Media/speech audio attributes align playback with the phone media volume; a zero media volume shows a prompt without changing the device volume. JNI argument references are released in a finally block. Native setup/playback exceptions are logged instead of swallowed. A successful queue return is labelled “Voice requested”, not proof of audible playback; asynchronous engine synthesis errors still require device logs because no UtteranceProgressListener is installed.

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

The rain-visitor update adds a sixth pitch/rate group, a reserved speech slot and TEST RAIN VOICE while rain is active. See RAIN_UMBRELLA_VISITOR.md for the revised Hindi/English phrase and spawning rules.

## Caption-only troubleshooting update

The Start card has separate TEST and RETRY controls and shows explicit muted/setup/failure status. TEST is available only with Sound ON, Voices ON and a selected offline Hindi voice. RETRY is available after initialization finishes or times out, even when no voice was found. In the Unity Editor, Android speech remains intentionally unavailable and the status says so. No recorded audio fallback, speech-model download, new dependency, network voice or extra music file is added.

For a silent real device: check the Start status, keep Sound and Voices ON, increase **media** volume, and test Hindi from the phone's text-to-speech settings. If the phone's own sample is silent, install/enable offline Hindi speech data manually. Then return to the game and tap RETRY followed by TEST. Also check whether audio is routed to connected Bluetooth headphones. If the game still stays silent despite the phone's own Hindi sample working, capture Android Logcat messages beginning `Hindi voice` and include the visible status.

Source parsing and JNI-signature review can be performed here; a Unity Android build and actual speech playback must still be verified on a real device.
