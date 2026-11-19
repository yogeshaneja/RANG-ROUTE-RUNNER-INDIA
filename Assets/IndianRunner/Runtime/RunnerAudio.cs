using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Original offline sound design. Clips are synthesized once, never during play.
    public sealed class RunnerAudio
    {
        private const int Rate = 22050;
        private const int MusicBpm = 126;
        public const int DanceBpm = 140;
        private const float MusicBeat = 60f/MusicBpm;
        private readonly AudioSource effects, music, approach, animals;
        private readonly List<AudioClip> owned = new List<AudioClip>();
        private readonly AudioClip[] pickups = new AudioClip[4];
        private readonly AudioClip danceTrack,breakVoice;
        private readonly AudioClip jump, stumble, engine, saucer, camel, elephant, celebration, rolling, procession, cloth, peacock, repairs;
        private bool paused, musicEnabled, dancing;
        private int savedMusicSample;
        private AudioClip runningTrack;
        private float danceFade=1;
        public bool HasBreakVoice => breakVoice!=null;
        private int pickupIndex, approachKind, lastAnimalKey = -1;
        private float lastPickup = -1, duckUntil, speechDuckUntil;

        public RunnerAudio(GameObject owner)
        {
            effects = Source(owner,.46f); animals = Source(owner,.34f);
            approach = Source(owner,0); approach.loop = true;
            music = Source(owner,0); music.loop = true;
            for (int i=0;i<pickups.Length;i++) pickups[i]=Cue("Mirror-work sparkle "+i,0,.32f,587.33f*Mathf.Pow(2,i*2/12f));
            jump=Cue("Soft jump swish",1,.24f,280);
            stumble=Cue("Gentle end-of-run drum",2,.38f,100);
            engine=Cue("Rounded motorcycle engine loop",3,1,60);
            saucer=Cue("Airy spaceship hum loop",4,1,110);
            camel=Cue("Soft stylised camel call",5,.85f,105);
            elephant=Cue("Distant stylised elephant trumpet",6,1.2f,235);
            celebration=Cue("Original festival welcome arpeggio",7,.90f,587.33f);
            rolling=Cue("Soft wooden wheel rumble loop",8,1,50);
            procession=Cue("Original procession drum and bell loop",9,MusicBeat*4,80);
            cloth=Cue("Soft falling textile rustle",10,.65f,420);
            peacock=Cue("Gentle stylised peacock call",11,.70f,660);
            repairs=Cue("Wooden roadwork warning knock",12,.50f,220);
            runningTrack=Compose(); music.clip=runningTrack;
            danceTrack=ComposeDance(); breakVoice=Resources.Load<AudioClip>("Dialogue/hi/break_time");
        }
        private static AudioSource Source(GameObject owner,float volume)
        {
            var source=owner.AddComponent<AudioSource>(); source.playOnAwake=false;
            source.spatialBlend=0; source.volume=volume; source.dopplerLevel=0; return source;
        }
        public void Settings(bool sound,bool enabled)
        {
            effects.mute=animals.mute=approach.mute=!sound; musicEnabled=enabled;
            if(enabled && !paused && !music.isPlaying) music.Play();
        }
        public void Paused(bool value)
        {
            paused=value;
            foreach(var source in new[]{music,effects,approach,animals})
                if(value) source.Pause(); else source.UnPause();
            if(!value && musicEnabled && !music.isPlaying) music.Play();
        }
        public void ResetRun()
        {
            EndDanceBreak();
            effects.Stop(); animals.Stop(); approach.Stop(); approach.volume=0;
            approachKind=0; lastAnimalKey=-1; pickupIndex=0; lastPickup=-1; duckUntil=speechDuckUntil=0;
        }
        public void Coin()
        {
            float now=Time.unscaledTime;
            if(now-lastPickup<.065f) return;
            if(now-lastPickup>.8f) pickupIndex=0;
            effects.PlayOneShot(pickups[pickupIndex++%pickups.Length],.72f);
            lastPickup=now; duckUntil=now+.22f;
        }
        public void DuckSpeech(float seconds=4.5f) { speechDuckUntil=Time.unscaledTime+seconds; }
        public void EndSpeechDuck() { speechDuckUntil=0; }
        public void Celebrate() { effects.PlayOneShot(celebration,.52f); }
        public void Jump() => effects.PlayOneShot(jump,.65f);
        public void Stumble() { EndDanceBreak(); approach.Stop(); animals.Stop(); effects.PlayOneShot(stumble,.8f); }
        public void Tick(float dt,int kind=0,float ahead=100,float pan=0,int encounterKey=-1,bool scenic=false)
        {
            if(paused) return;
            float smoothing=1-Mathf.Exp(-7*dt);
            float musicTarget=musicEnabled ? (Time.unscaledTime<speechDuckUntil ? .12f : Time.unscaledTime<duckUntil ? .20f : dancing ? .34f*danceFade : scenic ? .29f : .27f) : 0;
            music.pitch=Mathf.Lerp(music.pitch,1,smoothing); // keep the groove in tune and on tempo
            music.volume=Mathf.Lerp(music.volume,musicTarget,smoothing);
            if(dancing) music.volume=Mathf.Min(music.volume,.34f*danceFade);
            if(!musicEnabled && music.volume<.001f) music.Stop();
            if(dancing) return;
            bool motor=kind==RajasthanObstacles.Motorcycle, ship=kind==RajasthanObstacles.Spaceship;
            int nextKind=motor || ship || kind==RajasthanObstacles.Wheel || kind==RajasthanObstacles.Procession ? kind : 0;
            // Fade an outgoing loop before changing its clip, avoiding a sharp switch.
            float target=nextKind!=0 && nextKind==approachKind ? .30f*Mathf.Clamp01(1-Mathf.Abs(ahead)/55) : 0;
            approach.volume=Mathf.Lerp(approach.volume,target,smoothing);
            if(approachKind!=nextKind && approach.volume<.006f)
            {
                approach.Stop(); approachKind=nextKind;
                if(nextKind!=0) { approach.clip=motor ? engine : ship ? saucer : kind==RajasthanObstacles.Wheel ? rolling : procession; if(kind==RajasthanObstacles.Procession) approach.timeSamples=music.timeSamples%approach.clip.samples; approach.Play(); }
            }
            approach.panStereo=Mathf.Lerp(approach.panStereo,Mathf.Clamp(pan,-.65f,.65f),smoothing);
            approach.pitch=Mathf.Lerp(approach.pitch,kind==RajasthanObstacles.Procession ? 1 : ahead<0 ? .88f : 1.06f,smoothing);
            if((kind==RajasthanObstacles.Camel || kind==RajasthanObstacles.ElephantSound || kind==RajasthanObstacles.FallingCloth || kind==RajasthanObstacles.Peacock || kind==RajasthanObstacles.Roadworks) && ahead<24 && ahead>0 && encounterKey!=lastAnimalKey)
            {
                lastAnimalKey=encounterKey; animals.panStereo=Mathf.Clamp(pan,-.65f,.65f);
                animals.PlayOneShot(kind==RajasthanObstacles.ElephantSound ? elephant : kind==RajasthanObstacles.FallingCloth ? cloth : kind==RajasthanObstacles.Peacock ? peacock : kind==RajasthanObstacles.Roadworks ? repairs : camel,.65f); duckUntil=Time.unscaledTime+.7f;
            }
        }
        public void BeginDanceBreak()
        {
            if(dancing) return;
            savedMusicSample=music.timeSamples; dancing=true; danceFade=1;
            effects.Stop(); animals.Stop(); approach.Stop(); approach.volume=0; approachKind=0;
            duckUntil=speechDuckUntil=0;
            music.Stop(); music.clip=danceTrack; music.volume=0; music.pitch=1;
            if(musicEnabled && !paused) music.Play();
        }
        public void DanceRemaining(float remaining) { danceFade=Mathf.Clamp01(remaining/.18f); }
        public void EndDanceBreak()
        {
            if(!dancing) return;
            dancing=false; music.Stop(); music.volume=0; music.clip=runningTrack;
            music.timeSamples=Mathf.Clamp(savedMusicSample,0,runningTrack.samples-1);
            if(musicEnabled && !paused) music.Play();
        }
        public void SayRecordedBreakTime() { if(breakVoice!=null) effects.PlayOneShot(breakVoice,.9f); }
        private AudioClip ComposeDance()
        {
            const int samplesPerBeat=Rate*60/DanceBpm;
            var data=new float[samplesPerBeat*8];
            int[] notes={0,7,9,12,9,7,4,2,0,4,7,9,12,9,7,0};
            var frequencies=new float[notes.Length];
            for(int n=0;n<notes.Length;n++) frequencies[n]=293.6648f*Mathf.Pow(2,notes[n]/12f);
            uint noiseState=0x231a7bu;
            for(int i=0;i<data.Length;i++)
            {
                int beat=i/samplesPerBeat,eighth=i/(samplesPerBeat/2);
                float b=(float)(i%samplesPerBeat)/Rate,h=(float)(i%(samplesPerBeat/2))/Rate;
                float drum=Sin(65*b+1.4f*(1-Mathf.Exp(-b*40)))*Mathf.Exp(-b*20)*Mathf.Clamp01(b/.003f)*.28f;
                float tenor=Sin(180*h)*Mathf.Exp(-h*35)*Mathf.Clamp01(h/.003f)*.12f;
                noiseState^=noiseState<<13; noiseState^=noiseState>>17; noiseState^=noiseState<<5;
                float noise=(noiseState&0xffff)/32767.5f-1;
                float clap=beat%2==1 ? noise*Mathf.Exp(-b*40)*Mathf.Clamp01(b/.003f)*.15f:0;
                float hz=frequencies[eighth];
                float melody=(Sin(hz*h)+.22f*Sin(2*hz*h))*Mathf.Exp(-h*11)*Mathf.Clamp01(h/.006f)*Mathf.Clamp01((60f/DanceBpm*.5f-h)/.02f)*.17f;
                data[i]=drum+tenor+clap+melody;
            }
            int fade=Rate/200;
            for(int i=0;i<fade;i++) { float gain=Mathf.SmoothStep(0,1,(float)i/fade); data[i]*=gain; data[data.Length-1-i]*=gain; }
            return Clip("Original five-second rest dance groove - 140 BPM",data);
        }
        private AudioClip Clip(string name,float[] data)
        {
            float peak=0; foreach(float v in data) peak=Mathf.Max(peak,Mathf.Abs(v));
            float gain=peak>.78f ? .78f/peak : 1;
            for(int i=0;i<data.Length;i++) data[i]*=gain;
            var clip=AudioClip.Create(name,data.Length,1,Rate,false); clip.SetData(data,0); owned.Add(clip); return clip;
        }
        private static float Sin(float phase) => Mathf.Sin(2*Mathf.PI*phase);
        private AudioClip Cue(string name,int type,float duration,float hz)
        {
            var data=new float[Mathf.RoundToInt(Rate*duration)];
            for(int i=0;i<data.Length;i++)
            {
                float t=(float)i/Rate,u=t/duration;
                float envelope=Mathf.SmoothStep(0,1,Mathf.Min(1,t/.018f))*Mathf.SmoothStep(0,1,Mathf.Min(1,(duration-t)/.075f));
                float value;
                if(type==0) value=(Sin(hz*t)*Mathf.Exp(-t*9)+.35f*Sin(hz*2.76f*t)*Mathf.Exp(-t*16)+.16f*Sin(hz*4*t)*Mathf.Exp(-t*23))*.55f;
                else if(type==1) value=Sin(hz*t+330*t*t)*Mathf.Sin(Mathf.PI*u)*.23f;
                else if(type==2) value=(Sin(70*t+2*(1-Mathf.Exp(-t*20)))+.2f*Sin(140*t))*Mathf.Exp(-t*12)*.55f;
                else if(type==3) value=(Sin(60*t)+.36f*Sin(120*t)+.17f*Sin(180*t))*(.72f+.28f*Sin(30*t))*.38f;
                else if(type==4) value=(Sin(110*t+ .4f*Sin(2*t))+.25f*Sin(220*t))*.32f;
                else if(type==5) { float p=hz*t+2*Sin(3*t); value=(Sin(p)+.35f*Sin(p*2)+.12f*Sin(p*3))*Mathf.Pow(Mathf.Sin(Mathf.PI*u),2)*.35f; }
                else if(type==6) { float p=hz*t+3*Sin(.7f*t); value=(Sin(p)+.35f*Sin(p*2)+.16f*Sin(p*3))*Mathf.Pow(Mathf.Sin(Mathf.PI*u),1.5f)*.42f; }
                else if(type==7)
                {
                    int note=Mathf.Min(4,(int)(t/.18f)); float local=t-note*.18f;
                    int semitone=note==0 ? 0 : note==1 ? 4 : note==2 ? 7 : note==3 ? 12 : 16;
                    float f=hz*Mathf.Pow(2,semitone/12f);
                    value=(Sin(f*local)+.20f*Sin(f*2*local))*Mathf.Sin(Mathf.PI*local/.18f)*Mathf.Exp(-local*8)*.4f;
                }
                else if(type==8) value=(Sin(50*t)+.18f*Sin(100*t))*(.65f+.35f*Sin(10*t))*.25f;
                else if(type==9)
                {
                    float local=t-Mathf.Floor(t/MusicBeat)*MusicBeat;
                    value=Sin(80*local)*Mathf.Exp(-local*19)*Mathf.Min(1,local/.008f)*.32f+Sin(1320*t)*(.5f+.5f*Sin(2*t))*.025f;
                }
                else if(type==10) value=(.055f*Sin(2400*t+.2f*Sin(37*t))+.035f*Sin(3100*t)+.12f*Sin(hz*t-130*t*t))*Mathf.Pow(Mathf.Sin(Mathf.PI*u),2);
                else if(type==11) value=(Sin(hz*t+1.5f*Sin(3*t))+.15f*Sin(hz*2*t))*Mathf.Pow(Mathf.Sin(Mathf.PI*u),2)*.25f;
                else value=(Sin(hz*t)+.15f*Sin(330*t))*Mathf.Exp(-t*14)*.35f;
                // Procession uses four beats at the same tempo as the background track.
                data[i]=value*(type==3 || type==4 || type==8 || type==9 ? 1 : envelope);
                if(type==9) data[i]*=Mathf.Clamp01(Mathf.Min(t,duration-t)/.005f);
            }
            return Clip(name,data);
        }
        private AudioClip Compose()
        {
            // Original upbeat eight-bar festival groove, not a traditional-song
            // recording. At 126 BPM every beat is exactly 10,500 mono samples.
            const int samplesPerBeat=Rate*60/MusicBpm;
            var data=new float[samplesPerBeat*32];
            int[] melody={
                0,4,7,9,7,4,2,-99, 0,2,4,7,12,9,7,-99,
                4,7,9,12,9,7,4,2, 0,2,4,7,4,2,0,-99,
                7,9,12,14,12,9,7,-99, 4,7,9,12,9,7,4,-99,
                2,4,7,9,7,4,2,0, 4,7,9,7,4,2,0,-99};
            int[] roots={0,0,7,7,5,5,0,7};
            var frequencies=new float[melody.Length];
            var bassFrequencies=new float[roots.Length];
            for(int note=0;note<melody.Length;note++)
                frequencies[note]=melody[note]==-99 ? 0:293.6648f*Mathf.Pow(2,melody[note]/12f);
            for(int bar=0;bar<roots.Length;bar++) bassFrequencies[bar]=73.4162f*Mathf.Pow(2,roots[bar]/12f);
            uint noiseState=0x7241ab9u; float previousNoise=0;
            for(int i=0;i<data.Length;i++)
            {
                // Sample-index timing keeps off-beats and the procession loop
                // aligned without accumulating float/tempo drift.
                int beatIndex=i/samplesPerBeat, bar=beatIndex/4;
                int eighth=i/(samplesPerBeat/2), sixteenth=i/(samplesPerBeat/4);
                float beatLocal=(float)(i%samplesPerBeat)/Rate;
                float halfLocal=(float)(i%(samplesPerBeat/2))/Rate;
                float tickLocal=(float)(i%(samplesPerBeat/4))/Rate;
                float noteTime=MusicBeat*.5f, hz=frequencies[eighth];
                float attack=Mathf.Clamp01(halfLocal/.008f);
                float release=Mathf.Clamp01((noteTime-halfLocal)/.025f);
                float pluck=hz>0 ? (Sin(hz*halfLocal)+.28f*Sin(2*hz*halfLocal)+.09f*Sin(3*hz*halfLocal))*Mathf.Exp(-halfLocal*10)*attack*release*.16f:0;
                float answer=hz>0 && bar%4>=2 ? Sin(hz*halfLocal+.018f*Sin(5*halfLocal))*Mathf.Sin(Mathf.PI*halfLocal/noteTime)*.035f:0;
                float bassHz=bassFrequencies[bar];
                float bass=(Sin(bassHz*beatLocal)+.16f*Sin(bassHz*2*beatLocal))*Mathf.Sin(Mathf.PI*beatLocal/MusicBeat)*.095f;
                float lowDrum=Sin(58*beatLocal+1.7f*(1-Mathf.Exp(-beatLocal*45)))*Mathf.Exp(-beatLocal*19)*Mathf.Clamp01(beatLocal/.003f)*(beatIndex%2==0 ? .25f:.09f);
                float highDrum=Sin(168*halfLocal+.55f*(1-Mathf.Exp(-halfLocal*45)))*Mathf.Exp(-halfLocal*32)*Mathf.Clamp01(halfLocal/.003f)*(eighth%4==3 ? .13f:.055f);
                noiseState^=noiseState<<13; noiseState^=noiseState>>17; noiseState^=noiseState<<5;
                float noise=(noiseState&0xffff)/32767.5f-1;
                float brightNoise=(noise-previousNoise)*.5f; previousNoise=noise;
                float clap=beatIndex%2==1 ? brightNoise*Mathf.Exp(-beatLocal*38)*Mathf.Clamp01(beatLocal/.003f)*.18f:0;
                float shaker=brightNoise*Mathf.Exp(-tickLocal*85)*Mathf.Clamp01(tickLocal/.002f)*(sixteenth%4==2 ? .065f:.035f);
                data[i]=pluck+answer+bass+lowDrum+highDrum+clap+shaker;
            }
            // Short edge fades prevent a click when the full musical phrase loops.
            int fade=Rate/200;
            for(int i=0;i<fade;i++)
            {
                float gain=Mathf.SmoothStep(0,1,(float)i/fade);
                data[i]*=gain; data[data.Length-1-i]*=gain;
            }
            return Clip("Original Thar festival dash - 126 BPM",data);
        }
        public void Dispose() { foreach(var clip in owned) Object.Destroy(clip); owned.Clear(); }
    }
}
