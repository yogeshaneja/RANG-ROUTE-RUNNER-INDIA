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
                        catch(Exception error) { Debug.LogWarning("Hindi voice initialization: "+error); initResult=-1; }
                    }
                }));
            }
            catch(Exception error) { configured=true; Status="No speech engine. Check phone text-to-speech settings."; Debug.LogWarning("Hindi voice activity: "+error); }
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
                                // Match the game's media volume rather than relying on an engine's
                                // default stream. Do not change the user's device volume.
                                using(var builder=new AndroidJavaObject("android.media.AudioAttributes$Builder"))
                                using(var usage=builder.Call<AndroidJavaObject>("setUsage",1)) // USAGE_MEDIA
                                using(var content=builder.Call<AndroidJavaObject>("setContentType",1)) // CONTENT_TYPE_SPEECH
                                using(var attributes=builder.Call<AndroidJavaObject>("build"))
                                    if(engine.Call<int>("setAudioAttributes",attributes)!=0)
                                        throw new InvalidOperationException("TextToSpeech rejected media audio attributes.");
                                Ready=true; Status="Offline Hindi ready. Turn up MEDIA volume, then test."; break;
                            }
                        }
                    }
                    if(!Ready) Status="Hindi voice missing. Add Hindi in phone text-to-speech settings, then RETRY.";
                }
                catch(Exception error) { Ready=false; Status="Voice setup failed. See Unity/Android logs; tap RETRY."; Debug.LogWarning("Hindi voice configuration: "+error); }
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
                        object[] arguments={line.Hindi,0,parameters,"rang-route-banter"}; // QUEUE_FLUSH
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
                            Status="Voice requested. Silent? Check MEDIA volume / Hindi speech data.";
                            return true;
                        }
                        Ready=false; Status="Voice request rejected ("+result+"). Check Hindi data; tap RETRY.";
                        Debug.LogWarning("Hindi voice speak returned "+result);
                    }
                }
                catch(Exception error)
                {
                    Ready=false; Status="Voice playback failed. See Unity/Android logs; tap RETRY.";
                    Debug.LogWarning("Hindi voice playback: "+error);
                }
            }
#endif
            return false;
        }
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
