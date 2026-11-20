using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    public enum DialogueLanguage { Auto, English, Hindi }

    // Play bundled dialogue recordings first. Optional installed offline speech
    // fills missing clips; never downloads voices or selects a network voice.
    public sealed class AndroidSpeech : IDisposable
    {
        private bool enabled=true;
        private readonly DialogueLanguage preference;
        public bool UsesEnglish { get; private set; }
        public string LanguageName => UsesEnglish ? "English" : "Hindi";
        private bool nativeReady;
        private readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
        private readonly AudioSource recordedSource;
        public bool Ready => clips.Count>0 || nativeReady;
        public string Status { get; private set; }
        public bool CanRetry
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return configured && !disposed;
#else
                return false;
#endif
            }
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        private readonly object gate=new object();
        private AndroidJavaObject engine,activity;
        private InitListener listener;
        private volatile int initResult=int.MinValue;
        private volatile bool disposed;
        private bool configured;
        private float startedAt;
        private sealed class InstalledVoice
        {
            public AndroidJavaObject Voice;
            public string Name, Country;
            public int Quality, Latency, Gender;
        }
        private readonly List<InstalledVoice> catalog=new List<InstalledVoice>();
        private readonly int[] cast=new int[6];
        private int activeVoice=-1;
        private string castKey="";
        [UnityEngine.Scripting.Preserve]
        private sealed class InitListener : AndroidJavaProxy
        {
            private readonly AndroidSpeech owner;
            public InitListener(AndroidSpeech value) : base("android.speech.tts.TextToSpeech$OnInitListener") { owner=value; }
            [UnityEngine.Scripting.Preserve]
            public void onInit(int status) { owner.initResult=status; }
        }
#endif
        public AndroidSpeech(DialogueLanguage language=DialogueLanguage.Hindi,GameObject audioHost=null)
        {
            // Legacy Auto preferences now mean Hindi. Public UI has two choices.
            preference=language==DialogueLanguage.English ? DialogueLanguage.English:DialogueLanguage.Hindi;
            UsesEnglish=preference==DialogueLanguage.English;
            if(audioHost!=null)
            {
                recordedSource=audioHost.AddComponent<AudioSource>();
                recordedSource.playOnAwake=false; recordedSource.loop=false;
                recordedSource.spatialBlend=0; recordedSource.volume=.9f; recordedSource.priority=32;
                foreach(var line in RoadBanter.AllLines)
                {
                    var clip=Resources.Load<AudioClip>("Dialogue/"+(UsesEnglish ? "en/":"hi/")+line.AudioKey);
                    if(clip!=null) clips[line.AudioKey]=clip;
                }
                if(clips.Count==16) { Status=LanguageName+" recordings ready."; return; }
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            Status="Preparing offline dialogue voice..."; startedAt=Time.realtimeSinceStartup;
            try
            {
                using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer")) activity=player.GetStatic<AndroidJavaObject>("currentActivity");
                listener=new InitListener(this);
                activity.Call("runOnUiThread",new AndroidJavaRunnable(()=>
                {
                    lock(gate)
                    {
                        if(disposed) return;
                        try { engine=new AndroidJavaObject("android.speech.tts.TextToSpeech",activity,listener); }
                        catch(Exception error) { Debug.LogWarning("Dialogue voice initialization: "+error); initResult=-1; }
                    }
                }));
            }
            catch(Exception error) { configured=true; Status="No speech engine. Check phone text-to-speech settings."; Debug.LogWarning("Dialogue voice activity: "+error); }
#else
            Status=clips.Count>0 ? LanguageName+" recordings loaded ("+clips.Count+"/16).":"Recordings not supplied. Phone-generated voices play on Android.";
#endif
        }
        public void SetEnabled(bool value) { enabled=value; if(!value) Stop(); }
        public void Tick()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(clips.Count==16 || disposed || configured) return;
            if(initResult==int.MinValue)
            {
                if(Time.realtimeSinceStartup-startedAt>15) { configured=true; Status="Voice setup timed out. Check phone speech settings."; }
                return;
            }
            lock(gate)
            {
                if(disposed) return;
                if(initResult!=0) { configured=true; Status="Speech engine unavailable on this phone."; return; }
                if(engine==null) return;
                configured=true;
                try
                {
                    // Keep the selected language; do not silently replace Hindi dialogue
                    // with English while recordings are being added.
                    bool hindi=preference!=DialogueLanguage.English && TrySelectVoice("hi");
                    bool english=preference==DialogueLanguage.English && TrySelectVoice("en");
                    if(!hindi && !english)
                    {
                        Status=LanguageName+" recordings not supplied; phone voice unavailable. Try "+(UsesEnglish ? "Hindi":"English")+".";
                        return;
                    }
                    UsesEnglish=english;
                    using(var builder=new AndroidJavaObject("android.media.AudioAttributes$Builder"))
                    using(var usage=builder.Call<AndroidJavaObject>("setUsage",1)) // USAGE_MEDIA
                    using(var content=builder.Call<AndroidJavaObject>("setContentType",1)) // CONTENT_TYPE_SPEECH
                    using(var attributes=builder.Call<AndroidJavaObject>("build"))
                        if(engine.Call<int>("setAudioAttributes",attributes)!=0)
                            throw new InvalidOperationException("TextToSpeech rejected media audio attributes.");
                    nativeReady=true;
                    Status="Phone-generated "+LanguageName+" ready. Dialogue recordings are not complete yet.";
                }
                catch(Exception error) { nativeReady=false; Status="Voice setup failed. See Unity/Android logs; tap PLAY VOICE."; Debug.LogWarning("Dialogue voice configuration: "+error); }
            }
#endif
        }
        public bool Speak(RoadBanter.Line line)
        {
            if(!enabled) return false;
            if(recordedSource!=null && clips.TryGetValue(line.AudioKey,out var recording))
            {
                Stop(); recordedSource.clip=recording; recordedSource.pitch=1;
                recordedSource.Play(); Status="Playing "+LanguageName+" recording.";
                return true;
            }
            if(recordedSource!=null) recordedSource.Stop();
            if(!nativeReady)
            { Status="This "+LanguageName+" recording is not supplied; no phone voice is ready."; return false; }
#if UNITY_ANDROID && !UNITY_EDITOR
            lock(gate)
            {
                if(disposed || engine==null) return false;
                try
                {
                    int choice=cast[(int)line.Role];
                    if(choice!=activeVoice)
                    {
                        if(engine.Call<int>("setVoice",catalog[choice].Voice)!=0)
                            throw new InvalidOperationException("Selected character voice is unavailable. Retry or choose another cast voice.");
                        activeVoice=choice;
                    }
                    // Keep voices near their natural range. Extreme pitch shifts
                    // make low-quality engines sound slowed down or squeaky.
                    engine.Call<int>("setPitch",Mathf.Clamp(line.Pitch,.88f,1.12f));
                    engine.Call<int>("setSpeechRate",Mathf.Clamp(line.Rate,.90f,1.12f));
                    using(var audio=activity.Call<AndroidJavaObject>("getSystemService","audio"))
                    {
                        if(audio!=null && audio.Call<int>("getStreamVolume",3)==0) // STREAM_MUSIC
                        { Status="Phone MEDIA volume is zero. Turn it up, then test again."; return false; }
                    }
                    using(var parameters=new AndroidJavaObject("android.os.Bundle"))
                    {
                        parameters.Call("putFloat","volume",.9f);
                        // Use the API 21+ signature explicitly: CharSequence is an
                        // interface, so do not infer a String/HashMap overload.
                        var method=AndroidJNIHelper.GetMethodID(engine.GetRawClass(),"speak",
                            "(Ljava/lang/CharSequence;ILandroid/os/Bundle;Ljava/lang/String;)I");
                        object[] arguments={line.Text(UsesEnglish),0,parameters,"rang-route-banter"}; // QUEUE_FLUSH
                        var nativeArguments=AndroidJNIHelper.CreateJNIArgArray(arguments);
                        int result;
                        try
                        {
                            result=AndroidJNI.CallIntMethod(engine.GetRawObject(),method,nativeArguments);
                            var exception=AndroidJNI.ExceptionOccurred();
                            if(exception!=IntPtr.Zero)
                            {
                                AndroidJNI.ExceptionClear(); AndroidJNI.DeleteLocalRef(exception);
                                throw new InvalidOperationException("Android TextToSpeech.speak raised a Java exception.");
                            }
                        }
                        finally { AndroidJNIHelper.DeleteJNIArgArray(arguments,nativeArguments); }
                        if(result==0)
                        {
                            // Queued is not a playback/completion callback. Never
                            // report that the user has actually heard the sentence.
                            Status="Phone-generated "+LanguageName+" voice requested. Check MEDIA volume if silent.";
                            return true;
                        }
                        nativeReady=false; Status="Voice request rejected ("+result+"). Check speech data; tap PLAY VOICE.";
                        Debug.LogWarning("Dialogue voice speak returned "+result);
                    }
                }
                catch(Exception error)
                {
                    nativeReady=false; Status="Voice playback failed. See Unity/Android logs; tap PLAY VOICE.";
                    Debug.LogWarning("Dialogue voice playback: "+error);
                }
            }
#endif
            return false;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        private static int NamedGender(string name)
        {
            // Android has no standard gender property. Only use explicit whole
            // labels; opaque engine IDs are not treated as gender evidence.
            var words=name.ToLowerInvariant().Split(new[]{'-','_',' ','.','#'},StringSplitOptions.RemoveEmptyEntries);
            foreach(string word in words) if(word=="female" || word=="woman") return 2;
            foreach(string word in words) if(word=="male" || word=="man") return 1;
            return 0;
        }
        private void ClearVoices()
        {
            foreach(var voice in catalog) voice.Voice.Dispose();
            catalog.Clear(); activeVoice=-1;
        }
        private int DefaultCast(VoiceRole role)
        {
            int gender=role==VoiceRole.Lady ? 2 : role==VoiceRole.Rider || role==VoiceRole.Caller ? 1 : 0;
            int winner=0,best=int.MinValue;
            for(int i=0;i<catalog.Count;i++)
            {
                var candidate=catalog[i];
                int score=candidate.Quality*10+(candidate.Country=="IN" ? 250:0)-Math.Min(candidate.Latency,1000)/10;
                if(gender!=0 && candidate.Gender!=0) score+=candidate.Gender==gender ? 10000:-10000;
                // Break equivalent-quality ties into distinct character voices.
                if(i==(int)role%catalog.Count) score+=5;
                if(score>best) { best=score; winner=i; }
            }
            return winner;
        }
        private bool TrySelectVoice(string language)
        {
            ClearVoices();
            using(var voices=engine.Call<AndroidJavaObject>("getVoices"))
            {
                if(voices==null) return false;
                using(var iterator=voices.Call<AndroidJavaObject>("iterator"))
                    while(iterator.Call<bool>("hasNext"))
                    {
                        var voice=iterator.Call<AndroidJavaObject>("next"); bool retained=false;
                        try
                        {
                            using(var locale=voice.Call<AndroidJavaObject>("getLocale"))
                            using(var features=voice.Call<AndroidJavaObject>("getFeatures"))
                            {
                                if(locale.Call<string>("getLanguage")!=language || voice.Call<bool>("isNetworkConnectionRequired")) continue;
                                if(features!=null && features.Call<bool>("contains","notInstalled")) continue;
                                string name=voice.Call<string>("getName") ?? "";
                                catalog.Add(new InstalledVoice { Voice=voice,Name=name,Country=locale.Call<string>("getCountry"),
                                    Quality=voice.Call<int>("getQuality"),Latency=voice.Call<int>("getLatency"),Gender=NamedGender(name) });
                                retained=true;
                            }
                        }
                        finally { if(!retained) voice.Dispose(); }
                    }
            }
            if(catalog.Count==0) return false;
            catalog.Sort((a,b)=>StringComparer.Ordinal.Compare(a.Name,b.Name));
            castKey="runner.voiceCast."+engine.Call<string>("getDefaultEngine")+"."+language+".";
            for(int role=0;role<cast.Length;role++)
            {
                string saved=PlayerPrefs.GetString(castKey+(VoiceRole)role,"");
                int restored=catalog.FindIndex(v=>v.Name==saved);
                cast[role]=restored>=0 ? restored:DefaultCast((VoiceRole)role);
            }
            if(engine.Call<int>("setVoice",catalog[cast[0]].Voice)==0) { activeVoice=cast[0]; return true; }
            ClearVoices(); return false;
        }
#endif
        public void Stop()
        {
            if(recordedSource!=null) recordedSource.Stop();
#if UNITY_ANDROID && !UNITY_EDITOR
            lock(gate)
            {
                if(engine==null || disposed) return;
                try { engine.Call<int>("stop"); } catch(Exception) { }
            }
#endif
        }
        public void Dispose()
        {
            if(recordedSource!=null)
            {
                recordedSource.Stop(); recordedSource.clip=null;
                UnityEngine.Object.Destroy(recordedSource);
            }
            clips.Clear();
#if UNITY_ANDROID && !UNITY_EDITOR
            lock(gate)
            {
                disposed=true; nativeReady=false;
                if(engine!=null)
                {
                    try { engine.Call<int>("stop"); engine.Call("shutdown"); } catch(Exception) { }
                    engine.Dispose(); engine=null;
                }
                ClearVoices();
                if(activity!=null) { activity.Dispose(); activity=null; }
            }
#endif
        }
    }
}
