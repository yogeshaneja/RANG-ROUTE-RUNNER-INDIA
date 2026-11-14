using UnityEngine;

namespace IndianRunner
{
    public sealed class RunnerAudio
    {
        private readonly AudioSource effects, music;
        private readonly AudioClip coin, jump, stumble, tune;
        public RunnerAudio(GameObject owner)
        {
            effects = owner.AddComponent<AudioSource>(); effects.playOnAwake = false; effects.volume = .2f;
            music = owner.AddComponent<AudioSource>(); music.playOnAwake = false; music.loop = true; music.volume = .08f;
            coin = Tone("Original coin cue", 880, .10f);
            jump = Tone("Original jump cue", 440, .13f);
            stumble = Tone("Original stumble cue", 130, .25f);
            tune = Music(); music.clip = tune;
        }
        public void Settings(bool sound, bool enabled) { effects.mute = !sound; if (enabled && !music.isPlaying) music.Play(); if (!enabled) music.Stop(); }
        public void Paused(bool paused) { if (paused) music.Pause(); else music.UnPause(); }
        public void Coin() { effects.PlayOneShot(coin); }
        public void Jump() { effects.PlayOneShot(jump); }
        public void Stumble() { effects.PlayOneShot(stumble); }
        private static AudioClip Tone(string name, float hz, float seconds)
        {
            const int rate = 22050; var samples = new float[(int)(rate * seconds)];
            for (int i = 0; i < samples.Length; i++)
            {
                float t = (float)i / rate, envelope = Mathf.Sin(Mathf.PI * i / samples.Length);
                samples[i] = Mathf.Sin(2 * Mathf.PI * hz * t) * envelope * .4f;
            }
            var clip = AudioClip.Create(name, samples.Length, 1, rate, false); clip.SetData(samples, 0); return clip;
        }
        private static AudioClip Music()
        {
            const int rate = 22050; const float beat = .4f;
            // Short original synthesized placeholder motif; not a recording or regional music claim.
            int[] notes = { 0, 7, 4, 2, 9, 7, 2, 4, 0, 4, 7, 11, 9, 4, 2, 7 };
            var samples = new float[(int)(rate * beat * notes.Length)];
            for (int i = 0; i < samples.Length; i++)
            {
                float t = (float)i / rate; int n = Mathf.Min(notes.Length - 1, (int)(t / beat)); float local = t - n * beat;
                float hz = 220 * Mathf.Pow(2, notes[n] / 12f);
                float attack = Mathf.Min(1, local / .01f), decay = Mathf.Pow(1 - local / beat, 2);
                samples[i] = (Mathf.Sin(2 * Mathf.PI * hz * t) + .25f * Mathf.Sin(4 * Mathf.PI * hz * t)) * attack * decay * .25f;
            }
            var clip = AudioClip.Create("Original prototype motif", samples.Length, 1, rate, false); clip.SetData(samples, 0); return clip;
        }
        public void Dispose()
        {
            Object.Destroy(coin); Object.Destroy(jump); Object.Destroy(stumble); Object.Destroy(tune);
        }
    }
}
