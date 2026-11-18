using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Original offline sound design. Clips are synthesized once, never during play.
    public sealed class RunnerAudio
    {
        private const int Rate = 22050;
        private readonly AudioSource effects, music, approach, animals;
        private readonly List<AudioClip> owned = new List<AudioClip>();
        private readonly AudioClip[] pickups = new AudioClip[4];
        private readonly AudioClip jump, stumble, engine, saucer, camel, elephant, celebration;
        private bool paused, musicEnabled;
        private int pickupIndex, approachKind, lastAnimalKey = -1;
        private float lastPickup = -1, duckUntil;

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
            music.clip=Compose();
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
            effects.Stop(); animals.Stop(); approach.Stop(); approach.volume=0;
            approachKind=0; lastAnimalKey=-1; pickupIndex=0; lastPickup=-1; duckUntil=0;
        }
        public void Coin()
        {
            float now=Time.unscaledTime;
            if(now-lastPickup<.065f) return;
            if(now-lastPickup>.8f) pickupIndex=0;
            effects.PlayOneShot(pickups[pickupIndex++%pickups.Length],.72f);
            lastPickup=now; duckUntil=now+.22f;
        }
        public void Celebrate() { effects.PlayOneShot(celebration,.52f); }
        public void Jump() => effects.PlayOneShot(jump,.65f);
        public void Stumble() { approach.Stop(); animals.Stop(); effects.PlayOneShot(stumble,.8f); }
        public void Tick(float dt,int kind=0,float ahead=100,float pan=0,int encounterKey=-1,bool scenic=false)
        {
            if(paused) return;
            float smoothing=1-Mathf.Exp(-7*dt);
            float musicTarget=musicEnabled ? (Time.unscaledTime<duckUntil ? .16f : scenic ? .25f : .23f) : 0;
            music.pitch=Mathf.Lerp(music.pitch,scenic ? 1.04f : 1,smoothing);
            music.volume=Mathf.Lerp(music.volume,musicTarget,smoothing);
            if(!musicEnabled && music.volume<.001f) music.Stop();
            bool motor=kind==RajasthanObstacles.Motorcycle, ship=kind==RajasthanObstacles.Spaceship;
            int nextKind=motor || ship ? kind : 0;
            // Fade an outgoing loop before changing its clip, avoiding a sharp switch.
            float target=nextKind!=0 && nextKind==approachKind ? .30f*Mathf.Clamp01(1-Mathf.Abs(ahead)/55) : 0;
            approach.volume=Mathf.Lerp(approach.volume,target,smoothing);
            if(approachKind!=nextKind && approach.volume<.006f)
            {
                approach.Stop(); approachKind=nextKind;
                if(nextKind!=0) { approach.clip=motor ? engine : saucer; approach.Play(); }
            }
            approach.panStereo=Mathf.Lerp(approach.panStereo,Mathf.Clamp(pan,-.65f,.65f),smoothing);
            approach.pitch=Mathf.Lerp(approach.pitch,ahead<0 ? .88f : 1.06f,smoothing);
            if((kind==RajasthanObstacles.Camel || kind==8) && ahead<24 && ahead>0 && encounterKey!=lastAnimalKey)
            {
                lastAnimalKey=encounterKey; animals.panStereo=Mathf.Clamp(pan,-.65f,.65f);
                animals.PlayOneShot(kind==8 ? elephant : camel,.65f); duckUntil=Time.unscaledTime+.7f;
            }
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
                else
                {
                    int note=Mathf.Min(4,(int)(t/.18f)); float local=t-note*.18f;
                    int semitone=note==0 ? 0 : note==1 ? 4 : note==2 ? 7 : note==3 ? 12 : 16;
                    float f=hz*Mathf.Pow(2,semitone/12f);
                    value=(Sin(f*local)+.20f*Sin(f*2*local))*Mathf.Sin(Mathf.PI*local/.18f)*Mathf.Exp(-local*8)*.4f;
                }
                // Loops are periodic at one second, so they need no destructive edge fade.
                data[i]=value*(type==3 || type==4 ? 1 : envelope);
            }
            return Clip(name,data);
        }
        private AudioClip Compose()
        {
            // Original 8-bar pentatonic instrumental: plucks, breathy lead, soft
            // hand-drum-like rhythm and drone. No recording or traditional-song claim.
            const float beat=.625f; // 96 BPM, 20 seconds; exact loop boundaries.
            var data=new float[Rate*20];
            int[] melody={0,2,4,7,9,7,4,2,0,4,7,9,12,9,7,4,2,4,7,4,2,0,-3,0,4,7,9,7,4,2,0,-1};
            for(int i=0;i<data.Length;i++)
            {
                float t=(float)i/Rate, b=t/beat;
                int note=Mathf.Min(31,(int)b); float local=t-note*beat;
                float hz=293.6648f*Mathf.Pow(2,melody[note]/12f);
                float attack=Mathf.SmoothStep(0,1,Mathf.Min(1,local/.024f));
                float release=Mathf.SmoothStep(0,1,Mathf.Min(1,(beat-local)/.06f));
                float pluck=(Sin(hz*local)+.23f*Sin(2*hz*local)+.08f*Sin(3*hz*local))*Mathf.Exp(-local*6)*attack*release*.17f;
                float lead=Sin(hz*local+.012f*Sin(5*local))*Mathf.Sin(Mathf.PI*local/beat)*.085f;
                float drumLocal=t-Mathf.Floor(b*2)*beat*.5f;
                bool bass=((int)(b*2))%4==0;
                float drum=Sin((bass ? 76 : 155)*drumLocal+1.2f*(1-Mathf.Exp(-drumLocal*35)))*Mathf.Exp(-drumLocal*(bass ? 19 : 35))*Mathf.Min(1,drumLocal/.004f)*(bass ? .19f : .08f);
                float drone=(Sin(146.8f*t)+.30f*Sin(220*t))*.028f;
                float shimmer=Sin(1760*drumLocal)*Mathf.Exp(-drumLocal*65)*Mathf.Min(1,drumLocal/.003f)*.018f;
                data[i]=pluck+lead+drum+drone+shimmer;
            }
            // A 25 ms release takes the ending to zero, matching the silent first sample.
            int fade=Rate/40;
            for(int i=0;i<fade;i++)
            {
                data[data.Length-fade+i]*=Mathf.SmoothStep(1,0,(float)(i+1)/fade);
            }
            return Clip("Original Thar evening - layered instrumental",data);
        }
        public void Dispose() { foreach(var clip in owned) Object.Destroy(clip); owned.Clear(); }
    }
}
