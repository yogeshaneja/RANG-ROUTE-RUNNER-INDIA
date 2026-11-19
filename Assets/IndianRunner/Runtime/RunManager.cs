using UnityEngine;

namespace IndianRunner
{
    public sealed class RunManager : MonoBehaviour
    {
        private enum State { Menu, Running, DanceBreak, Paused, GameOver }
        private State state;
        private TrackWorld world;
        private RunnerModel runner;
        private WeatherController weather;
        private CloudGuardian guardian;
        private RunnerAudio audioPlayer;
        private AndroidSpeech speech, breakSpeech;
        private const float DanceSeconds=3;
        private float danceElapsed,danceLandingHeight,introStartedAt;
        private bool pausedDanceBreak,breakCuePending;
        private static readonly RoadBanter.Line BreakTimeLine=new RoadBanter.Line("RUNNER","ब्रेक टाइम!","Break time!","Break time!",1,1,VoiceRole.Rider,0);
        private readonly RoadBanter banter=new RoadBanter();
        private readonly RunnerInput input = new RunnerInput();
        private Camera gameCamera;
        private float distance, x, height, verticalSpeed, slideUntil, clock;
        private float laneStartX, laneTargetX, laneElapsed, laneDuration, cameraX;
        private int lane = 1, coins, best, seed = 7301;
        private bool reduced, music, sound, voices;
        private DialogueLanguage dialogueLanguage;
        private RunnerSkin skin;
        private RunFeedback feedback;
        private FestivalProgress progress;
        private bool showingGoals, inRest;
        private float rewardRevealUntil;
        private string rewardRevealName="";
        private float magnetUntil;
        private string goalsSummary = "", powerSummary = "";
        private float menuClock, introDuration, milestoneUntil;
        private Vector3 introCameraPosition;
        private Quaternion introCameraRotation;
        private int nextMilestone = 100;
        private string milestoneText = "", hudCoins = "0";
        private string hudScore = "0 pts    0 tokens", hudStatus = "Rajasthan";
        private float nextHudUpdate;
        private string runEndMessage = "Try the open lane for a safe route.";
        public int CurrentScore => RunRules.Score(distance, coins);

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            best = PlayerPrefs.GetInt("runner.best", 0);
            progress = new FestivalProgress(PlayerPrefs.GetInt("festival.set",0),PlayerPrefs.GetInt("festival.tokens",0),PlayerPrefs.GetInt("festival.slides",0),PlayerPrefs.GetInt("festival.distance",0),PlayerPrefs.GetInt("festival.outfit",0));
            reduced = PlayerPrefs.GetInt("runner.reduced", 0) == 1;
            music = PlayerPrefs.GetInt("runner.music", 1) == 1;
            sound = PlayerPrefs.GetInt("runner.sound", 1) == 1;
            voices = PlayerPrefs.GetInt("runner.voices",1)==1;
            // New simple-language preference starts in Hindi, including upgrades
            // from the previous automatic voice setup. Later user choices persist.
            dialogueLanguage=PlayerPrefs.GetInt("runner.dialogueLanguageSimple",2)==1 ? DialogueLanguage.English:DialogueLanguage.Hindi;
            var cameraObject = new GameObject("Runner Camera"); cameraObject.transform.SetParent(transform, false);
            gameCamera = cameraObject.AddComponent<Camera>(); gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.fieldOfView = 62; gameCamera.allowMSAA = true; gameCamera.nearClipPlane = .15f; gameCamera.farClipPlane = 210;
            if (QualitySettings.renderPipeline == null && UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline == null)
                QualitySettings.antiAliasing = 2;
            cameraObject.AddComponent<AudioListener>();
            var lightObject = new GameObject("Rajasthan sunlight"); lightObject.transform.SetParent(transform, false);
            var sunlight = lightObject.AddComponent<Light>(); sunlight.type = LightType.Directional;
            sunlight.shadows = LightShadows.None; lightObject.transform.rotation = Quaternion.Euler(48, -32, 0);
            world = new TrackWorld(transform); world.Reset(seed);
            feedback = new RunFeedback(transform);
            runner = new RunnerModel(transform); runner.SetOutfit(progress.Outfit);
            weather = new WeatherController(transform, gameCamera, sunlight);
            guardian=new CloudGuardian(transform);
            audioPlayer = new RunnerAudio(gameObject); audioPlayer.Settings(sound, music);
            speech=new AndroidSpeech(dialogueLanguage,gameObject); speech.SetEnabled(voices && sound); banter.Reset(seed);
            state = State.Menu; runner.Root.localRotation = Quaternion.Euler(0,180,0); PositionCamera(); world.Position(0, seed, 0, reduced); weather.Tick(0, 0, reduced); world.SetRain(weather.RainIntensity); RefreshHud();
        }
        public void ValidateStartup()
        {
            if (progress == null || world == null || runner == null || weather == null || guardian == null || audioPlayer == null || speech == null || feedback == null || gameCamera == null)
                throw new System.InvalidOperationException("Runner startup did not complete. Check the first Console exception.");
            world.ValidatePool(); runner.ValidateRig();
        }
        private void StartRun()
        {
            bool fromMenu = state == State.Menu; showingGoals=false; magnetUntil=0; inRest=false;
            introCameraPosition = gameCamera.transform.position; introCameraRotation = gameCamera.transform.rotation;
            introDuration = reduced ? 0 : fromMenu ? .7f : .25f;
            danceElapsed=0; pausedDanceBreak=breakCuePending=false; introStartedAt=0; breakSpeech?.Stop();
            milestoneUntil = 0; milestoneText = ""; nextMilestone = 100; feedback.Reset();
            distance = x = height = verticalSpeed = slideUntil = clock = 0;
            laneStartX = laneTargetX = laneElapsed = laneDuration = cameraX = nextHudUpdate = 0;
            runEndMessage = "Try the open lane for a safe route.";
            coins = 0; lane = 1; seed = (seed + 379) & 0x7fffffff;
            world.Reset(seed); world.Position(0, seed, 0, reduced); guardian.Reset(); audioPlayer.ResetRun(); banter.Reset(seed); speech.Stop(); state = State.Running;
            runner.Root.localPosition = Vector3.zero; runner.Root.localRotation = fromMenu && !reduced ? Quaternion.Euler(0,180,0) : Quaternion.identity;
            runner.Animate(0, false, false, 0, reduced); PositionCamera();
            input.ResetGesture(); audioPlayer.Paused(false); RefreshHud();
        }
        private void Update()
        {
            // Clamp large stalls so an OS hitch cannot skip a reaction window.
            float dt = Mathf.Min(Time.deltaTime, .05f); speech.Tick(); breakSpeech?.Tick();
            if(state==State.DanceBreak)
            {
                input.ResetGesture(); TickDanceBreak(Time.unscaledDeltaTime); return;
            }
            input.Read(Screen.safeArea.yMax - 100 * Screen.width / 480f);
            if (input.Pause && !showingGoals)
            {
                if (state == State.Running) Pause();
                else if (state == State.Paused) Resume();
            }
            if (state == State.Menu)
            {
                menuClock += dt; runner.Animate(menuClock,false,false,0,reduced);
                world.Position(0,seed,menuClock,reduced); weather.Tick(menuClock,dt,reduced); world.SetWeather(weather.Cloudiness); world.SetRain(weather.RainIntensity); audioPlayer.Tick(dt);
                return;
            }
            if (state != State.Running) { audioPlayer.Tick(dt); return; }
            Vector3 previous = new Vector3(x, height, distance);
            clock += dt;
            if (input.Horizontal != 0)
            {
                int nextLane = Mathf.Clamp(lane + input.Horizontal, 0, 2);
                if (nextLane != lane)
                {
                    lane = nextLane; laneStartX = x; laneTargetX = RunRules.LaneX(lane);
                    laneElapsed = 0; laneDuration = RunRules.LaneTravelTime(laneTargetX - laneStartX);
                }
            }
            if (input.Jump && height <= .001f && clock >= slideUntil) { verticalSpeed = RunRules.JumpVelocity; audioPlayer.Jump(); }
            bool startedSlide=input.Slide && height<=.001f && verticalSpeed<=0 && clock>=slideUntil;
            if(startedSlide) slideUntil=clock+.7f;
            bool sliding = clock < slideUntil;
            laneElapsed += dt;
            float laneT = laneDuration > 0 ? Mathf.Clamp01(laneElapsed / laneDuration) : 1;
            x = Mathf.Lerp(laneStartX, laneTargetX, RunRules.LaneProgress(laneT));
            if (height > 0 || verticalSpeed > 0)
            {
                // Integrate ballistic motion analytically per step.
                height += verticalSpeed * dt - .5f * RunRules.Gravity * dt * dt;
                verticalSpeed -= RunRules.Gravity * dt;
                if (height <= 0) { height = 0; verticalSpeed = 0; }
            }
            distance += RunRules.Speed(distance) * dt;
            int gained = world.CollectAndCollide(previous, new Vector3(x, height, distance), sliding ? .82f : 2.05f, out bool hit, clock<magnetUntil);
            coins += gained;
            if (gained > 0) { audioPlayer.Coin(); feedback.Coin(x,height,reduced); }
            if(world.PouchesCollected>0 && !hit)
            {
                magnetUntil=clock+8; audioPlayer.Celebrate();
                milestoneText="FESTIVAL POUCH - tokens come to you!"; milestoneUntil=clock+2;
            }
            if(progress.Add(gained,progress.Set==0 ? (startedSlide && !hit ? 1:0):world.SlidesCleared,distance))
            {
                SaveProgress(); audioPlayer.Celebrate();
                milestoneText=progress.Ready ? "THREE GOALS COMPLETE - outfit reward ready!" : "GOAL COMPLETE - beautiful running!";
                milestoneUntil=clock+2.5f;
            }
            bool restNow=RunRules.RestDistance(distance);
            bool enterGarden=restNow && !inRest && !hit;
            inRest=restNow;
            if (hit) { EndRun(world.CamelContact); }
            world.Position(distance, seed, clock, reduced, clock<magnetUntil, x, height); feedback.Tick(dt);
            if(enterGarden && RunRules.DanceRestDistance(distance))
            {
                BeginDanceBreak(); TickDanceBreak(0); RefreshHud(); return;
            }
            if(!hit)
            {
                if(guardian.Tick(distance,clock,reduced))
                {
                    audioPlayer.GuardianReveal(); milestoneText="THE THAR CLOUD GUARDIAN"; milestoneUntil=clock+2;
                }
                audioPlayer.Tick(dt,world.SoundKind,world.SoundAhead,world.SoundPan,world.SoundKey,inRest);
                if((world.SoundKind!=RajasthanObstacles.UmbrellaLady || weather.RainIntensity>.08f) && banter.Tick(clock,world.SoundKind,world.SoundAhead,world.SoundKey,world.UmbrellaAhead<RunRules.Speed(distance)*10.5f) && speech.Speak(banter.Current)) audioPlayer.DuckSpeech();
            }
            if (distance >= nextMilestone && clock >= milestoneUntil)
            {
                milestoneText = nextMilestone + " m  -  GREAT RUN"; milestoneUntil = clock+1.5f; nextMilestone += 100;
            }
            runner.Root.localPosition = new Vector3(x, height, 0);
            runner.Animate(clock, state == State.Running, sliding, height, reduced, RunRules.Speed(distance));
            if (hit) runner.Root.localRotation = Quaternion.Euler(0, 0, -12);
            else
            {
                float lean = reduced ? 0 : Mathf.Clamp((x - previous.x) / Mathf.Max(.001f, dt) / RunRules.LaneSpeed, -1, 1) * -8;
                float turn = reduced ? 0 : Mathf.Clamp((x-previous.x)/Mathf.Max(.001f,dt)/RunRules.LaneSpeed,-1,1)*9;
                runner.Root.localRotation = Quaternion.Slerp(runner.Root.localRotation, Quaternion.Euler(0,turn,lean), 1-Mathf.Exp(-16*dt));
            }
            weather.Tick(clock, dt, reduced); world.SetWeather(weather.Cloudiness); world.SetRain(weather.RainIntensity);
            PositionCamera(dt);
            if (clock >= nextHudUpdate || hit)
            {
                RefreshHud(); nextHudUpdate = clock + .1f;
            }
        }
        private void BeginDanceBreak()
        {
            state=State.DanceBreak; danceElapsed=0; danceLandingHeight=height;
            height=verticalSpeed=0; slideUntil=clock; showingGoals=false; breakCuePending=false;
            speech.Stop(); input.ResetGesture(); SaveBest(); audioPlayer.BeginDanceBreak();
            if(voices && sound)
            {
                if(audioPlayer.HasBreakVoice) { audioPlayer.SayRecordedBreakTime(); audioPlayer.DuckSpeech(1.25f); }
                else
                {
                    // New cue is not in the supplied 16-line recording pack.
                    // Optional English phone voice; no clip is fabricated.
                    if(breakSpeech==null || (!breakSpeech.Ready && breakSpeech.CanRetry))
                    { breakSpeech?.Dispose(); breakSpeech=new AndroidSpeech(DialogueLanguage.English); }
                    breakSpeech.SetEnabled(true); breakCuePending=true;
                }
            }
        }
        private void TickDanceBreak(float elapsedStep)
        {
            danceElapsed=Mathf.Min(DanceSeconds,danceElapsed+Mathf.Max(0,elapsedStep));
            if(breakCuePending)
            {
                if(danceElapsed>2) breakCuePending=false;
                else if(breakSpeech.Ready)
                {
                    if(breakSpeech.Speak(BreakTimeLine)) audioPlayer.DuckSpeech(1.25f);
                    breakCuePending=false;
                }
            }
            float dt=Mathf.Min(elapsedStep,.05f);
            runner.Root.localPosition=new Vector3(x,0,0);
            runner.Dance(clock,danceElapsed,reduced,danceLandingHeight);
            var cameraTarget=transform.position+new Vector3(x+.7f,2.2f,-4.6f);
            var lookTarget=transform.position+new Vector3(x,1.8f,0);
            float cameraBlend=reduced ? 1:1-Mathf.Exp(-7*dt);
            gameCamera.transform.position=Vector3.Lerp(gameCamera.transform.position,cameraTarget,cameraBlend);
            gameCamera.transform.rotation=Quaternion.Slerp(gameCamera.transform.rotation,Quaternion.LookRotation(lookTarget-gameCamera.transform.position),cameraBlend);
            gameCamera.fieldOfView=Mathf.Lerp(gameCamera.fieldOfView,58,cameraBlend);
            feedback.Tick(dt); audioPlayer.DanceRemaining(DanceSeconds-danceElapsed); audioPlayer.Tick(dt);
            if(danceElapsed>=DanceSeconds)
            {
                breakSpeech?.Stop(); breakCuePending=false; audioPlayer.EndSpeechDuck(); audioPlayer.EndDanceBreak();
                state=State.Running; pausedDanceBreak=false; input.ResetGesture();
                laneStartX=x; laneTargetX=RunRules.LaneX(lane); laneElapsed=0;
                laneDuration=RunRules.LaneTravelTime(laneTargetX-laneStartX);
                introCameraPosition=gameCamera.transform.position; introCameraRotation=gameCamera.transform.rotation;
                introDuration=reduced ? 0:.45f; introStartedAt=clock;
                milestoneText="LET'S GO!"; milestoneUntil=clock+1.2f; RefreshHud();
            }
        }
        private void RefreshHud()
        {
            hudScore = CurrentScore.ToString("N0"); hudCoins = coins.ToString();
            hudStatus = $"{(int)distance} m    {weather.Description}";
            goalsSummary = progress.Ready ? "ALL 3 GOALS DONE - reward at Start" : $"GOALS {progress.Completed}/3  |  {progress.Tokens}/{progress.TokenGoal} tokens";
            powerSummary = clock<magnetUntil ? $"POUCH POWER  {Mathf.CeilToInt(magnetUntil-clock)}s" : inRest ? "FESTIVAL GARDEN  |  BREATHE & COLLECT" : goalsSummary;
        }
        private void PositionCamera(float dt = 0)
        {
            if (state == State.Menu)
            {
                gameCamera.transform.position = transform.position+new Vector3(2.7f,1.8f,-4.4f);
                gameCamera.transform.LookAt(transform.position+new Vector3(0,1.2f,0));
                gameCamera.fieldOfView = 62; return;
            }
            float targetX = x*.35f;
            cameraX = dt <= 0 ? targetX : Mathf.Lerp(cameraX,targetX,1-Mathf.Exp(-10*dt));
            var position = transform.position+new Vector3(cameraX,4.7f+(reduced ? 0 : height*.045f),-7.8f);
            var rotation = Quaternion.LookRotation(transform.position+new Vector3(cameraX,1.15f,12.5f)-position);
            float transition = introDuration > 0 ? Mathf.SmoothStep(0,1,Mathf.Clamp01((clock-introStartedAt)/introDuration)) : 1;
            gameCamera.transform.position = Vector3.Lerp(introCameraPosition,position,transition);
            gameCamera.transform.rotation = Quaternion.Slerp(introCameraRotation,rotation,transition);
            float fov = reduced ? 62 : Mathf.Lerp(62,65,Mathf.InverseLerp(9,18,RunRules.Speed(distance)));
            gameCamera.fieldOfView = dt <= 0 ? 62 : Mathf.Lerp(gameCamera.fieldOfView,fov,1-Mathf.Exp(-3*dt));
        }
        private void ReturnToMenu()
        {
            SaveBest(); speech.Stop(); breakSpeech?.Stop(); breakCuePending=pausedDanceBreak=false; banter.Reset(seed); state = State.Menu; showingGoals=false; guardian.Reset(); magnetUntil=0; inRest=false;
            distance = x = height = verticalSpeed = slideUntil = clock = menuClock = 0;
            lane = 1; feedback.Reset(); world.Reset(seed); world.Position(0,seed,0,reduced);
            runner.Root.localPosition = Vector3.zero; runner.Root.localRotation = Quaternion.Euler(0,180,0);
            runner.Animate(0,false,false,0,reduced); audioPlayer.ResetRun(); audioPlayer.Paused(false); PositionCamera(); RefreshHud();
            input.ResetGesture();
        }
        private void Pause() { pausedDanceBreak=state==State.DanceBreak; state = State.Paused; breakSpeech?.Stop(); breakCuePending=false; SaveBest(); RefreshHud(); input.ResetGesture(); speech.Stop(); audioPlayer.EndSpeechDuck(); audioPlayer.Paused(true); }
        private void Resume() { state = pausedDanceBreak ? State.DanceBreak:State.Running; input.ResetGesture(); audioPlayer.Paused(false); }
        private void EndRun(bool camelContact)
        {
            state = State.GameOver; guardian.Reset(); speech.Stop(); audioPlayer.EndSpeechDuck();
            runEndMessage = world.ContactMessage.Length > 0 ? world.ContactMessage : camelContact ? "Camel is safe. Follow the warning lane." : "Jump, slide or use the open lane.";
            SaveBest(); RefreshHud(); feedback.Reset(); audioPlayer.Stumble();
        }
        private void SaveProgress()
        {
            if(progress==null) return;
            PlayerPrefs.SetInt("festival.set",progress.Set); PlayerPrefs.SetInt("festival.tokens",progress.Tokens);
            PlayerPrefs.SetInt("festival.slides",progress.Slides); PlayerPrefs.SetInt("festival.distance",progress.Distance);
            PlayerPrefs.SetInt("festival.outfit",progress.Outfit); PlayerPrefs.Save();
        }
        private void SaveBest()
        {
            SaveProgress();
            if (CurrentScore > best) { best = CurrentScore; PlayerPrefs.SetInt("runner.best", best); PlayerPrefs.Save(); }
        }
        private void SaveSettings()
        {
            PlayerPrefs.SetInt("runner.reduced", reduced ? 1 : 0); PlayerPrefs.SetInt("runner.music", music ? 1 : 0);
            PlayerPrefs.SetInt("runner.sound", sound ? 1 : 0); PlayerPrefs.SetInt("runner.voices",voices ? 1:0); PlayerPrefs.SetInt("runner.dialogueLanguageSimple",(int)dialogueLanguage); PlayerPrefs.Save(); audioPlayer.Settings(sound, music);
            speech.SetEnabled(voices && sound); breakSpeech?.SetEnabled(voices && sound); if(!voices || !sound) audioPlayer.EndSpeechDuck();
            if (state == State.Paused) audioPlayer.Paused(true);
        }
        private void ReturnToPhoneHome()
        {
            SaveBest(); SaveSettings(); speech.Stop(); breakSpeech?.Stop(); audioPlayer.EndSpeechDuck();
#if UNITY_ANDROID && !UNITY_EDITOR
            // Open the actual launcher, preserving this app in Recents. Moving a
            // task back alone can expose a previous app instead of the Home screen.
            try
            {
                using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var activity=player.GetStatic<AndroidJavaObject>("currentActivity");
                    try
                    {
                        activity.Call("runOnUiThread",new AndroidJavaRunnable(()=>
                        {
                            try
                            {
                                using(var home=new AndroidJavaObject("android.content.Intent","android.intent.action.MAIN"))
                                using(var category=home.Call<AndroidJavaObject>("addCategory","android.intent.category.HOME"))
                                using(var flags=home.Call<AndroidJavaObject>("addFlags",0x10000000)) // FLAG_ACTIVITY_NEW_TASK
                                    activity.Call("startActivity",home);
                            }
                            catch(System.Exception error) { Debug.LogWarning("Return to phone Home failed: "+error); }
                            finally { activity.Dispose(); }
                        }));
                    }
                    catch { activity.Dispose(); throw; }
                }
            }
            catch(System.Exception error) { Debug.LogWarning("Return to phone Home unavailable: "+error); }
#else
            Debug.Log("Exit to Home is available in the Android phone build.");
#endif
        }
        private void OnApplicationPause(bool paused) { if(paused) { speech?.Stop(); breakSpeech?.Stop(); } if (paused && (state==State.Running || state==State.DanceBreak)) Pause(); }
        private void OnApplicationFocus(bool focus) { if(!focus) { speech?.Stop(); breakSpeech?.Stop(); } if (!focus && (state==State.Running || state==State.DanceBreak)) Pause(); }
        private void OnApplicationQuit() { SaveBest(); }
        private void OnDestroy() { speech?.Dispose(); breakSpeech?.Dispose(); guardian?.Dispose(); skin?.Dispose(); runner?.Dispose(); world?.Dispose(); audioPlayer?.Dispose(); PrimitiveArt.Clear(); }
        private void WeatherChoice()
        {
            weather.Selection = (WeatherController.Mode)(((int)weather.Selection+1)%4);
            weather.Tick(state == State.Menu ? menuClock : clock,10,reduced); world.SetWeather(weather.Cloudiness); world.SetRain(weather.RainIntensity); RefreshHud();
        }
        private string VoiceLabel => !voices ? "VOICES OFF" : !sound ? "VOICES MUTED":"VOICES ON";
        private void DrawOptions(float xPos, float yPos, float width)
        {
            float half = (width-10)/2;
            float third=(width-16)/3;
            if (GUI.Button(new Rect(xPos,yPos,third,43),"Music "+(music ? "ON":"OFF"),skin.Toggle)) { music=!music; SaveSettings(); }
            if (GUI.Button(new Rect(xPos+third+8,yPos,third,43),"Sound "+(sound ? "ON":"OFF"),skin.Toggle)) { sound=!sound; SaveSettings(); }
            if (GUI.Button(new Rect(xPos+(third+8)*2,yPos,third,43),VoiceLabel,skin.Toggle)) { voices=!voices; SaveSettings(); }
            if (GUI.Button(new Rect(xPos,yPos+52,half,43),"Effects  "+(reduced ? "LOW" : "FULL"),skin.Toggle)) { reduced = !reduced; SaveSettings(); }
            if (GUI.Button(new Rect(xPos+half+10,yPos+52,half,43),"Sky  "+weather.Selection,skin.Toggle)) WeatherChoice();
            if(GUI.Button(new Rect(xPos,yPos+104,width,43),"LANGUAGE  "+(dialogueLanguage==DialogueLanguage.Hindi ? "HINDI":"ENGLISH")+"  >",skin.Toggle))
            {
                dialogueLanguage=dialogueLanguage==DialogueLanguage.Hindi ? DialogueLanguage.English:DialogueLanguage.Hindi;
                speech.Dispose(); audioPlayer.EndSpeechDuck();
                speech=new AndroidSpeech(dialogueLanguage,gameObject); SaveSettings();
            }
        }
        private void DrawGoals(float left,float top,float width,float screenHeight)
        {
            float y=Mathf.Max(top+80,(screenHeight-390)/2);
            GUI.Box(new Rect(left+16,y,width-32,390),GUIContent.none,skin.Card);
            GUI.Label(new Rect(left+28,y+12,width-56,38),"FESTIVAL GOALS  "+(progress.Set+1),skin.Subtitle);
            GUI.Label(new Rect(left+34,y+61,width-68,30),$"MIRROR TOKENS   {progress.Tokens} / {progress.TokenGoal}",skin.Text);
            GUI.Label(new Rect(left+34,y+99,width-68,30),$"{(progress.Set==0 ? "PRACTICE A SLIDE" : "SLIDE UNDER LOW BARRIERS")}   {progress.Slides} / {progress.SlideGoal}",skin.Text);
            GUI.Label(new Rect(left+34,y+137,width-68,30),$"DISTANCE IN ONE RUN   {progress.Distance} / {progress.DistanceGoal} m",skin.Text);
            string reward=progress.Set<3 ? "Reward: "+FestivalProgress.OutfitName(progress.Set+1)+" kurta" : "Reward: another festival badge";
            GUI.Label(new Rect(left+34,y+184,width-68,42),reward+"\nProgress saves across runs, fully offline.",skin.Caption);
            GUI.enabled=progress.Ready && state==State.Menu;
            if(GUI.Button(new Rect(left+34,y+241,width-68,48),progress.Ready ? state==State.Menu ? "CLAIM REWARD & NEXT GOALS" : "CLAIM FROM THE START SCREEN" : "COMPLETE ALL THREE TO UNLOCK",skin.Primary))
            {
                if(progress.Claim())
                {
                    runner.SetOutfit(progress.Outfit); SaveProgress(); audioPlayer.Celebrate(); RefreshHud();
                    rewardRevealName=progress.Set<=3 ? FestivalProgress.OutfitName(progress.Outfit)+" KURTA":"FESTIVAL BADGE";
                    rewardRevealUntil=menuClock+6; showingGoals=false;
                }
            }
            GUI.enabled=true;
            GUI.Label(new Rect(left+34,y+299,width-68,23),$"{progress.Unlocked}/4 colours | {progress.Badges} badges | change outfit at Start",skin.Caption);
            if(GUI.Button(new Rect(left+34,y+334,width-68,40),"BACK",skin.Secondary)) showingGoals=false;
        }
        private void OnGUI()
        {
            if (skin == null) skin = new RunnerSkin();
            float scale = Mathf.Min(Screen.width/480f,Screen.height/800f);
            float screenHeight = Screen.height/scale;
            Rect safe = Screen.safeArea;
            float top = (Screen.height-safe.yMax)/scale+10,left = safe.xMin/scale,width = safe.width/scale;
            float center = left+width/2,bottom = screenHeight-safe.yMin/scale-12;
            var oldMatrix = GUI.matrix; var oldColour = GUI.color;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale,scale,1));
            GUI.enabled=!showingGoals;
            if (state == State.Menu)
            {
                GUI.Label(new Rect(left+16,top+12,width-32,55),"RANG ROUTE",skin.Title);
                GUI.Label(new Rect(left+20,top+65,width-40,30),"THE RAJASTHAN RUN",skin.Subtitle);
                GUI.Box(new Rect(center-85,top+108,170,38),"BEST  "+best.ToString("N0"),skin.Chip);
                if(menuClock<rewardRevealUntil)
                    GUI.Label(new Rect(left+24,top+158,width-48,62),"SURPRISE UNLOCKED!\n"+rewardRevealName,skin.Subtitle);
                float sheet = bottom-548;
                GUI.Box(new Rect(left+12,sheet,width-24,548),GUIContent.none,skin.Card);
                float xPos = left+32,w = width-64;
                if (GUI.Button(new Rect(xPos,sheet+18,w,58),"LET'S RUN",skin.Primary)) StartRun();
                GUI.Label(new Rect(xPos,sheet+86,w,49),"Collect mirror-work tokens. Jump pots and cloth.\nSlide under low bars. Follow hazard warnings.",skin.Text);
                DrawOptions(xPos,sheet+146,w);
                if(GUI.Button(new Rect(xPos,sheet+306,(w-10)/2,44),"GOALS  "+progress.Completed+"/3",skin.Secondary)) showingGoals=true;
                if(GUI.Button(new Rect(xPos+(w+10)/2,sheet+306,(w-10)/2,44),FestivalProgress.OutfitName(progress.Outfit),skin.Secondary))
                { progress.NextOutfit(); runner.SetOutfit(progress.Outfit); SaveProgress(); }
                bool allowPreview=GUI.enabled; GUI.enabled=allowPreview && voices && sound;
                if(GUI.Button(new Rect(xPos,sheet+360,w,44),"PLAY "+speech.LanguageName.ToUpperInvariant()+" VOICE",skin.Secondary))
                {
                    if(!speech.Ready && speech.CanRetry)
                    {
                        speech.Dispose(); audioPlayer.EndSpeechDuck();
                        speech=new AndroidSpeech(dialogueLanguage,gameObject); speech.SetEnabled(voices && sound);
                    }
                    else if(speech.Speak(weather.RainIntensity>.12f ? RoadBanter.RainPreview:RoadBanter.Preview)) audioPlayer.DuckSpeech();
                }
                GUI.enabled=allowPreview;
                string voiceStatus=!voices ? "Voices are OFF. Turn Voices ON to hear dialogue.":
                    !sound ? "Sound is OFF. Turn Sound ON to hear dialogue.":speech.Status;
                GUI.Label(new Rect(xPos,sheet+412,w,72),voiceStatus+"\nKeep looking for updates for new content.",skin.Caption);
                if(GUI.Button(new Rect(xPos,sheet+494,w,44),"EXIT TO HOME",skin.Secondary)) ReturnToPhoneHome();
            }
            else
            {
                GUI.Box(new Rect(left+10,top,width-198,66),GUIContent.none,skin.Card);
                GUI.Label(new Rect(left+24,top+3,width-230,35),hudScore,skin.Score);
                GUI.Label(new Rect(left+18,top+38,width-214,23),hudStatus,skin.Caption);
                GUI.Box(new Rect(left+width-192,top+5,122,54),"TOKENS "+hudCoins,skin.Chip);
                if(state==State.DanceBreak)
                {
                    float restTop=top+75;
                    GUI.Box(new Rect(left+24,restTop,width-48,205),GUIContent.none,skin.Card);
                    GUI.Label(new Rect(left+30,restTop+8,width-60,48),"REST TIME!",skin.Title);
                    GUI.Label(new Rect(left+30,restTop+54,width-60,100),Mathf.CeilToInt(Mathf.Max(0,DanceSeconds-danceElapsed)).ToString(),skin.RestCountdown);
                    GUI.Label(new Rect(left+30,restTop+157,width-60,38),"Break time! Dance along.",skin.Text);
                }
                else if (state == State.Running)
                {
                    if (GUI.Button(new Rect(left+width-61,top+5,50,54),"II",skin.Secondary)) Pause();
                    if (!string.IsNullOrEmpty(world.WarningMessage))
                    {
                        GUI.Box(new Rect(left+12,top+79,width-24,43),GUIContent.none,skin.Card);
                        GUI.Label(new Rect(left+20,top+81,width-40,39),world.WarningMessage,skin.Subtitle);
                    }
                    if (clock < milestoneUntil && string.IsNullOrEmpty(world.WarningMessage))
                    {
                        GUI.Box(new Rect(center-160,top+82,320,42),GUIContent.none,skin.Card);
                        GUI.Label(new Rect(center-152,top+84,304,38),milestoneText,skin.Subtitle);
                    }
                    if(banter.Visible(clock))
                    {
                        GUI.Box(new Rect(left+14,top+131,width-28,78),GUIContent.none,skin.Card);
                        GUI.Label(new Rect(left+26,top+133,width-52,22),banter.Current.Speaker,skin.Subtitle);
                        GUI.Label(new Rect(left+24,top+157,width-48,46),banter.Current.Subtitle(speech.UsesEnglish),skin.Text);
                    }
                    GUI.Box(new Rect(left+14,bottom-85,width-28,40),powerSummary,skin.Chip);
                    if (clock < 12) GUI.Label(new Rect(left+20,bottom-40,width-40,30),"SWIPE TO MOVE  |  UP TO JUMP  |  DOWN TO SLIDE",skin.Caption);
                }
                else
                {
                    GUI.color = new Color(0,0,0,.30f); GUI.DrawTexture(new Rect(0,0,Screen.width/scale,screenHeight),Texture2D.whiteTexture); GUI.color = oldColour;
                    float panel = Mathf.Max(top+100,(screenHeight-548)/2);
                    GUI.Box(new Rect(left+12,panel,width-24,548),GUIContent.none,skin.Card);
                    GUI.Label(new Rect(left+25,panel+15,width-50,50),state == State.Paused ? "TAKE A BREATHER" : "GOOD RUN!",skin.Subtitle);
                    GUI.Label(new Rect(left+25,panel+61,width-50,60),hudScore,skin.Title);
                    GUI.Label(new Rect(left+25,panel+121,width-50,30),$"POINTS   |   {(int)distance} m   |   {coins} tokens",skin.Caption);
                    GUI.Label(new Rect(left+30,panel+158,width-60,48),state == State.Paused ? "Your journey is waiting." : runEndMessage,skin.Text);
                    if (GUI.Button(new Rect(left+32,panel+219,width-64,57),state == State.Paused ? "CONTINUE RUN" : "RUN AGAIN",skin.Primary))
                    { if (state == State.Paused) Resume(); else StartRun(); }
                    if (GUI.Button(new Rect(left+32,panel+286,width-64,46),"BACK TO START",skin.Secondary)) ReturnToMenu();
                    if (GUI.Button(new Rect(left+32,panel+350,(width-74)/2,45),"Music  "+(music ? "ON" : "OFF"),skin.Toggle)) { music = !music; SaveSettings(); }
                    if (GUI.Button(new Rect(center+5,panel+350,(width-74)/2,45),"Effects  "+(reduced ? "LOW" : "FULL"),skin.Toggle)) { reduced = !reduced; SaveSettings(); }
                    float actions=width-74;
                    if(GUI.Button(new Rect(left+32,panel+408,actions*.63f,45),"FESTIVAL GOALS",skin.Secondary)) showingGoals=true;
                    if(GUI.Button(new Rect(left+42+actions*.63f,panel+408,actions*.37f,45),VoiceLabel,skin.Toggle)) { voices=!voices; SaveSettings(); }
                    if(GUI.Button(new Rect(left+32,panel+470,width-64,46),"EXIT TO HOME",skin.Secondary)) ReturnToPhoneHome();
                }
            }
            GUI.enabled=true;
            if(showingGoals) DrawGoals(left,top,width,screenHeight);
            GUI.color = oldColour; GUI.matrix = oldMatrix;
        }
    }
}
