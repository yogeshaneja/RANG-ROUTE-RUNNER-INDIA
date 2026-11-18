using System;
using UnityEngine;

namespace IndianRunner
{
    // Uses only an installed embedded Hindi voice. Never opens a store/download,
    // never selects a network voice, and needs no account or bundled recordings.
    public sealed class AndroidSpeech : IDisposable
    {
        private bool enabled=true;
        public bool Ready { get; private set; }
        public string Status { get; private set; }
#if UNITY_ANDROID && !UNITY_EDITOR
        private readonly object gate=new object();
        private AndroidJavaObject engine,activity;
        private InitListener listener;
        private volatile int initResult=int.MinValue;
        private volatile bool disposed;
        private bool configured;
        private float startedAt;
        [UnityEngine.Scripting.Preserve]
        private sealed class InitListener : AndroidJavaProxy
        {
            private readonly AndroidSpeech owner;
            public InitListener(AndroidSpeech value) : base("android.speech.tts.TextToSpeech$OnInitListener") { owner=value; }
            [UnityEngine.Scripting.Preserve]
            public void onInit(int status) { owner.initResult=status; }
        }
#endif
        public AndroidSpeech()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Status="Preparing offline Hindi voice..."; startedAt=Time.realtimeSinceStartup;
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
                        catch(Exception) { initResult=-1; }
                    }
                }));
            }
            catch(Exception) { configured=true; Status="Hindi speech engine unavailable on this phone."; }
#else
            Status="Voices play on Android; captions preview here.";
#endif
        }
        public void SetEnabled(bool value) { enabled=value; if(!value) Stop(); }
        public void Tick()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(disposed || configured) return;
            if(initResult==int.MinValue)
            {
                if(Time.realtimeSinceStartup-startedAt>15) { configured=true; Status="Hindi voice setup timed out. Check phone speech settings."; }
                return;
            }
            lock(gate)
            {
                if(disposed) return;
                if(initResult!=0) { configured=true; Status="Hindi speech engine unavailable on this phone."; return; }
                if(engine==null) return;
                configured=true;
                try
                {
                    using(var voices=engine.Call<AndroidJavaObject>("getVoices"))
                    {
                        if(voices!=null) using(var iterator=voices.Call<AndroidJavaObject>("iterator"))
                        {
                            while(iterator.Call<bool>("hasNext")) using(var voice=iterator.Call<AndroidJavaObject>("next"))
                            using(var locale=voice.Call<AndroidJavaObject>("getLocale"))
                            using(var features=voice.Call<AndroidJavaObject>("getFeatures"))
                            {
                                if(locale.Call<string>("getLanguage")!="hi" || voice.Call<bool>("isNetworkConnectionRequired")) continue;
                                if(features!=null && features.Call<bool>("contains","notInstalled")) continue;
                                if(engine.Call<int>("setVoice",voice)!=0) continue;
                                Ready=true; Status="Offline Hindi voice ready."; break;
                            }
                        }
                    }
                    if(!Ready) Status="Add an offline Hindi voice in phone speech settings.";
                }
                catch(Exception) { Ready=false; Status="Offline Hindi voice unavailable. Check phone speech settings."; }
            }
#endif
        }
        public bool Speak(RoadBanter.Line line)
        {
            if(!enabled || !Ready) return false;
#if UNITY_ANDROID && !UNITY_EDITOR
            lock(gate)
            {
                if(disposed || engine==null) return false;
                try
                {
                    engine.Call<int>("setPitch",Mathf.Clamp(line.Pitch,.6f,1.3f));
                    engine.Call<int>("setSpeechRate",Mathf.Clamp(line.Rate,.8f,1.15f));
                    // String/HashMap overload remains available on supported Android
                    // versions and maps cleanly through Unity's JNI bridge.
                    using(var parameters=new AndroidJavaObject("java.util.HashMap"))
                    {
                        enginePut(parameters,"volume","0.65");
                        enginePut(parameters,"utteranceId","rang-route-banter");
                        int result=engine.Call<int>("speak",line.Hindi,0,parameters); // QUEUE_FLUSH
                        if(result==0) return true;
                        Ready=false; Status="Offline Hindi voice needs speech data.";
                    }
                }
                catch(Exception) { Ready=false; Status="Voice playback failed. Check phone speech settings."; }
            }
#endif
            return false;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        private static void enginePut(AndroidJavaObject map,string key,string value)
        {
            using(var previous=map.Call<AndroidJavaObject>("put",key,value)) { }
        }
#endif
        public void Stop()
        {
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
#if UNITY_ANDROID && !UNITY_EDITOR
            lock(gate)
            {
                disposed=true; Ready=false;
                if(engine!=null)
                {
                    try { engine.Call<int>("stop"); engine.Call("shutdown"); } catch(Exception) { }
                    engine.Dispose(); engine=null;
                }
                if(activity!=null) { activity.Dispose(); activity=null; }
            }
#endif
        }
    }
}
