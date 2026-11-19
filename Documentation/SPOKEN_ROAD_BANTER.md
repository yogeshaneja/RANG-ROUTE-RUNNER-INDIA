# Simple dialogue audio — Hindi default

## Current delivery status

Hindi is the default, including the first launch after upgrading from the old voice setup. LANGUAGE switches only between HINDI and ENGLISH and saves the later choice locally. The old AUTO/CAST/RETRY interface is removed. PLAY HINDI VOICE (or English) plays a sample; after failed phone-speech setup it retries initialization.

**All 16 Hindi dialogue recordings are now bundled.** They were cut from the user-supplied MALE.mp3 (12 rider/camel/elephant/caller lines) and FEMALE.mp3 (three peacock lines and one umbrella-lady line). The spoken GAP separators and surrounding excess silence were excluded. English recordings are not supplied and English still uses optional phone speech.

Select HINDI after updating, with Sound/Voices ON. PLAY HINDI VOICE previews the rider; rain previews the lady. The complete Hindi pack skips Android speech-engine initialization and also plays in Unity Editor. No Hindi voice pack or network is needed for these bundled clips. Original source recordings were left unchanged and are not duplicated in the repository.

## Recording playback

Each line now has a stable audio key, such as rider_1 or lady_1. Add short original/licensed recordings to:

- Assets/IndianRunner/Resources/Dialogue/hi/ for Hindi.
- Assets/IndianRunner/Resources/Dialogue/en/ for English.

The full list of exact file names, roles and text is in DIALOGUE_RECORDINGS.csv. Use the right actor for rider/caller (male) and umbrella lady (female); animal dialogue can use original character performances. The rain line stays about rain, with no invitation wording.

AudioClip lookup is by extension-free key; .ogg or .wav files with the listed base names are supported through Unity's normal audio importer. Let Unity create/commit their .meta files. Use mono, short clips (roughly 2–4 seconds), consistent levels and a mobile compressed import setting. Listen after compression; measure APK/memory impact on the phone rather than promising a fixed size. Recordings and license/actor consent details must be added to ASSET_REGISTER.md when supplied.

Playback prioritizes a bundled recording, uses its original pitch and delivery, and stops any previous speech/recording. All 16 clips for the selected language are loaded once per dialogue-player instance, not per frame. With a complete 16-clip pack, no Android TextToSpeech engine is initialized: no phone voice pack, account, internet or installation step is needed. Recordings also play in Unity Editor.

Partial packs play available recordings and use optional installed offline speech for missing lines. The status explicitly identifies missing recordings or phone-generated playback. Hindi stays Hindi; it is no longer silently replaced with English. English is one language-button tap away. If both a recording and the selected-language phone voice are absent, captions continue but that line is silent. Adding the actual recordings is therefore essential to the intended simple experience.

One additional Unity AudioSource is created for dialogue and destroyed when the player is replaced/disposed. Language changes stop old playback. Pause, background, lost focus, restart, game over and return to Start stop both paths. Sound OFF or Voices OFF stops dialogue; music remains separate and ducks on accepted playback. Old recordings are not queued to resume later. The 16 supplied Hindi audio clips are added; no speech model, network request or third-party audio plugin is needed.

## Optional phone fallback

AndroidSpeech retains installed embedded Hindi/English speech as a temporary fallback. It rejects network-only/not-installed voices, ranks quality/locale/latency and retains previous saved character assignments. Explicit gender labels may guide automatic casting; Android Voice has no standard gender field and opaque names are not gender evidence. This fallback still sounds synthetic and is not the recording solution.

It uses Unity JNI, the existing generated TTS_SERVICE manifest query and the API 21+ Bundle speak call, with media-volume hints and logged failures. A queued request does not prove audible playback. Android synthesis errors still require device logs. Editor has no native phone fallback. No download or store is opened automatically.

## Required acceptance

1. Pull and compile in Unity. Confirm Hindi is selected on upgrade/new install, English can be selected and persists, and no CAST/AUTO setup is shown.
2. Before recordings exist, verify the status honestly identifies phone-generated/missing audio; test a phone with and without Hindi data and confirm there is no silent language switch.
3. Supply all 16 Hindi recordings from the CSV, reimport, and verify PLAY HINDI VOICE and every encounter play the correct recording/caption in Editor and Android.
4. With all 16 recordings present and no Hindi TTS pack, verify launch and playback in airplane mode and that no TTS initialization is needed. Check partial packs and English packs separately.
5. Check male/female actors, natural delivery, consistent levels, music ducking, mute settings, interruption and stop on all lifecycle transitions. No overlapping old/new source should remain after language switches.
6. Check compressed APK size, sustained phone memory and repeated language switching. Source parsing is not a Unity compile, Android build or audition.

API reference: https://developer.android.com/reference/android/speech/tts/Voice

## Supplied recording processing

The latest MALE.mp3 and FEMALE.mp3 were used; the earlier FEMAL.mp3 upload was superseded. Repeated GAP sections were located as short speech spans between pauses, with within-source waveform correlations above 0.96 (male) and 0.98 (female). There were 11 male and three female separators. Source order is the two scripts provided in chat. No ASR transcript or human audition was performed in this workspace; confirm wording/pronunciation in the phone playtest.

Cuts retain short lead/tail room and internal expressive pauses. Conservative per-clip level adjustment, 8 ms edge fades and 80 ms trailing padding were applied; speed and pitch were not changed. Exports are mono 24 kHz Ogg/Vorbis. All 16 decode successfully, are finite, peak below full scale, and last 1.61–4.31 seconds. Total source clip size is 88,705 bytes (about 87 KiB); this is not a measured APK-size increase. Unity import uses compressed-in-memory Vorbis and retains 24 kHz mono. HINDI_AUDIO_CUTS.csv records source hashes, boundaries, output hashes, sizes and durations. Audio quality, exact word boundaries and Unity import/playback still need audition on the target device.
