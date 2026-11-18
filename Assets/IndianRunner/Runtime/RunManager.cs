using UnityEngine;

namespace IndianRunner
{
    public sealed class RunManager : MonoBehaviour
    {
        private enum State { Menu, Running, Paused, GameOver }
        private State state;
        private TrackWorld world;
        private RunnerModel runner;
        private WeatherController weather;
        private RunnerAudio audioPlayer;
        private readonly RunnerInput input = new RunnerInput();
        private Camera gameCamera;
        private float distance, x, height, verticalSpeed, slideUntil, clock;
        private float laneStartX, laneTargetX, laneElapsed, laneDuration, cameraX;
        private int lane = 1, coins, best, seed = 7301;
        private bool reduced, music, sound;
        private RunnerSkin skin;
        private RunFeedback feedback;
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
            reduced = PlayerPrefs.GetInt("runner.reduced", 0) == 1;
            music = PlayerPrefs.GetInt("runner.music", 1) == 1;
            sound = PlayerPrefs.GetInt("runner.sound", 1) == 1;
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
            runner = new RunnerModel(transform);
            weather = new WeatherController(transform, gameCamera, sunlight);
            audioPlayer = new RunnerAudio(gameObject); audioPlayer.Settings(sound, music);
            state = State.Menu; runner.Root.localRotation = Quaternion.Euler(0,180,0); PositionCamera(); world.Position(0, seed, 0, reduced); weather.Tick(0, 0, reduced);
        }
        public void ValidateStartup()
        {
            if (world == null || runner == null || weather == null || audioPlayer == null || feedback == null || gameCamera == null)
                throw new System.InvalidOperationException("Runner startup did not complete. Check the first Console exception.");
            world.ValidatePool();
        }
        private void StartRun()
        {
            bool fromMenu = state == State.Menu;
            introCameraPosition = gameCamera.transform.position; introCameraRotation = gameCamera.transform.rotation;
            introDuration = reduced ? 0 : fromMenu ? .7f : .25f;
            milestoneUntil = 0; milestoneText = ""; nextMilestone = 100; feedback.Reset();
            distance = x = height = verticalSpeed = slideUntil = clock = 0;
            laneStartX = laneTargetX = laneElapsed = laneDuration = cameraX = nextHudUpdate = 0;
            runEndMessage = "Try the open lane for a safe route.";
            coins = 0; lane = 1; seed = (seed + 379) & 0x7fffffff;
            world.Reset(seed); world.Position(0, seed, 0, reduced); audioPlayer.ResetRun(); state = State.Running;
            runner.Root.localPosition = Vector3.zero; runner.Root.localRotation = fromMenu && !reduced ? Quaternion.Euler(0,180,0) : Quaternion.identity;
            runner.Animate(0, false, false, 0, reduced); PositionCamera();
            input.ResetGesture(); audioPlayer.Paused(false); RefreshHud();
        }
        private void Update()
        {
            // Clamp large stalls so an OS hitch cannot skip a reaction window.
            float dt = Mathf.Min(Time.deltaTime, .05f);
            input.Read(Screen.safeArea.yMax - 100 * Screen.width / 480f);
            if (input.Pause)
            {
                if (state == State.Running) Pause();
                else if (state == State.Paused) Resume();
            }
            if (state == State.Menu)
            {
                menuClock += dt; runner.Animate(menuClock,false,false,0,reduced);
                world.Position(0,seed,menuClock,reduced); weather.Tick(menuClock,dt,reduced); world.SetWeather(weather.Cloudiness); audioPlayer.Tick(dt);
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
            if (input.Slide && height <= .001f && clock >= slideUntil) slideUntil = clock + .7f;
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
            int gained = world.CollectAndCollide(previous, new Vector3(x, height, distance), sliding ? .82f : 2.05f, out bool hit);
            coins += gained;
            if (gained > 0) { audioPlayer.Coin(); feedback.Coin(x,height,reduced); }
            if (hit) { EndRun(world.CamelContact); }
            world.Position(distance, seed, clock, reduced); feedback.Tick(dt);
            if(!hit) audioPlayer.Tick(dt,world.SoundKind,world.SoundAhead,world.SoundPan,world.SoundKey);
            if (distance >= nextMilestone)
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
            weather.Tick(clock, dt, reduced); world.SetWeather(weather.Cloudiness);
            PositionCamera(dt);
            if (clock >= nextHudUpdate || hit)
            {
                RefreshHud(); nextHudUpdate = clock + .1f;
            }
        }
        private void RefreshHud()
        {
            hudScore = CurrentScore.ToString("N0"); hudCoins = coins.ToString();
            hudStatus = $"{(int)distance} m    {weather.Description}";
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
            float transition = introDuration > 0 ? Mathf.SmoothStep(0,1,Mathf.Clamp01(clock/introDuration)) : 1;
            gameCamera.transform.position = Vector3.Lerp(introCameraPosition,position,transition);
            gameCamera.transform.rotation = Quaternion.Slerp(introCameraRotation,rotation,transition);
            float fov = reduced ? 62 : Mathf.Lerp(62,65,Mathf.InverseLerp(9,18,RunRules.Speed(distance)));
            gameCamera.fieldOfView = dt <= 0 ? 62 : Mathf.Lerp(gameCamera.fieldOfView,fov,1-Mathf.Exp(-3*dt));
        }
        private void ReturnToMenu()
        {
            SaveBest(); state = State.Menu;
            distance = x = height = verticalSpeed = slideUntil = clock = menuClock = 0;
            lane = 1; feedback.Reset(); world.Reset(seed); world.Position(0,seed,0,reduced);
            runner.Root.localPosition = Vector3.zero; runner.Root.localRotation = Quaternion.Euler(0,180,0);
            runner.Animate(0,false,false,0,reduced); audioPlayer.ResetRun(); audioPlayer.Paused(false); PositionCamera(); RefreshHud();
            input.ResetGesture();
        }
        private void Pause() { state = State.Paused; SaveBest(); RefreshHud(); input.ResetGesture(); audioPlayer.Paused(true); }
        private void Resume() { state = State.Running; input.ResetGesture(); audioPlayer.Paused(false); }
        private void EndRun(bool camelContact)
        {
            state = State.GameOver;
            runEndMessage = world.ContactMessage.Length > 0 ? world.ContactMessage : camelContact ? "Camel is safe. Follow the warning lane." : "Jump, slide or use the open lane.";
            SaveBest(); RefreshHud(); feedback.Reset(); audioPlayer.Stumble();
        }
        private void SaveBest()
        {
            if (CurrentScore > best) { best = CurrentScore; PlayerPrefs.SetInt("runner.best", best); PlayerPrefs.Save(); }
        }
        private void SaveSettings()
        {
            PlayerPrefs.SetInt("runner.reduced", reduced ? 1 : 0); PlayerPrefs.SetInt("runner.music", music ? 1 : 0);
            PlayerPrefs.SetInt("runner.sound", sound ? 1 : 0); PlayerPrefs.Save(); audioPlayer.Settings(sound, music);
            if (state == State.Paused) audioPlayer.Paused(true);
        }
        private void OnApplicationPause(bool paused) { if (paused && state == State.Running) Pause(); }
        private void OnApplicationFocus(bool focus) { if (!focus && state == State.Running) Pause(); }
        private void OnApplicationQuit() { SaveBest(); }
        private void OnDestroy() { skin?.Dispose(); runner?.Dispose(); world?.Dispose(); audioPlayer?.Dispose(); PrimitiveArt.Clear(); }
        private void WeatherChoice()
        {
            weather.Selection = (WeatherController.Mode)(((int)weather.Selection+1)%4);
            weather.Tick(state == State.Menu ? menuClock : clock,10,reduced); world.SetWeather(weather.Cloudiness); RefreshHud();
        }
        private void DrawOptions(float xPos, float yPos, float width)
        {
            float half = (width-10)/2;
            if (GUI.Button(new Rect(xPos,yPos,half,43),"Music  "+(music ? "ON" : "OFF"),skin.Toggle)) { music = !music; SaveSettings(); }
            if (GUI.Button(new Rect(xPos+half+10,yPos,half,43),"Sound  "+(sound ? "ON" : "OFF"),skin.Toggle)) { sound = !sound; SaveSettings(); }
            if (GUI.Button(new Rect(xPos,yPos+52,half,43),"Effects  "+(reduced ? "LOW" : "FULL"),skin.Toggle)) { reduced = !reduced; SaveSettings(); }
            if (GUI.Button(new Rect(xPos+half+10,yPos+52,half,43),"Sky  "+weather.Selection,skin.Toggle)) WeatherChoice();
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
            if (state == State.Menu)
            {
                GUI.Label(new Rect(left+16,top+12,width-32,55),"RANG ROUTE",skin.Title);
                GUI.Label(new Rect(left+20,top+65,width-40,30),"THE RAJASTHAN RUN",skin.Subtitle);
                GUI.Box(new Rect(center-85,top+108,170,38),"BEST  "+best.ToString("N0"),skin.Chip);
                float sheet = bottom-318;
                GUI.Box(new Rect(left+12,sheet,width-24,318),GUIContent.none,skin.Card);
                float xPos = left+32,w = width-64;
                if (GUI.Button(new Rect(xPos,sheet+18,w,58),"LET'S RUN",skin.Primary)) StartRun();
                GUI.Label(new Rect(xPos,sheet+86,w,49),"Collect mirror-work tokens. Jump pots and cloth.\nSlide under awnings. Follow hazard warnings.",skin.Text);
                DrawOptions(xPos,sheet+146,w);
                GUI.Label(new Rect(xPos,sheet+252,w,50),"Keep looking for updates for new content.",skin.Caption);
            }
            else
            {
                GUI.Box(new Rect(left+10,top,width-198,66),GUIContent.none,skin.Card);
                GUI.Label(new Rect(left+24,top+3,width-230,35),hudScore,skin.Score);
                GUI.Label(new Rect(left+18,top+38,width-214,23),hudStatus,skin.Caption);
                GUI.Box(new Rect(left+width-192,top+5,122,54),"TOKENS "+hudCoins,skin.Chip);
                if (state == State.Running)
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
                    if (clock < 12) GUI.Label(new Rect(left+20,bottom-40,width-40,30),"SWIPE TO MOVE  |  UP TO JUMP  |  DOWN TO SLIDE",skin.Caption);
                }
                else
                {
                    GUI.color = new Color(0,0,0,.30f); GUI.DrawTexture(new Rect(0,0,Screen.width/scale,screenHeight),Texture2D.whiteTexture); GUI.color = oldColour;
                    float panel = Mathf.Max(top+100,(screenHeight-440)/2);
                    GUI.Box(new Rect(left+12,panel,width-24,440),GUIContent.none,skin.Card);
                    GUI.Label(new Rect(left+25,panel+15,width-50,50),state == State.Paused ? "TAKE A BREATHER" : "GOOD RUN!",skin.Subtitle);
                    GUI.Label(new Rect(left+25,panel+61,width-50,60),hudScore,skin.Title);
                    GUI.Label(new Rect(left+25,panel+121,width-50,30),$"POINTS   |   {(int)distance} m   |   {coins} tokens",skin.Caption);
                    GUI.Label(new Rect(left+30,panel+158,width-60,48),state == State.Paused ? "Your journey is waiting." : runEndMessage,skin.Text);
                    if (GUI.Button(new Rect(left+32,panel+219,width-64,57),state == State.Paused ? "CONTINUE RUN" : "RUN AGAIN",skin.Primary))
                    { if (state == State.Paused) Resume(); else StartRun(); }
                    if (GUI.Button(new Rect(left+32,panel+286,width-64,46),"BACK TO START",skin.Secondary)) ReturnToMenu();
                    if (GUI.Button(new Rect(left+32,panel+350,(width-74)/2,45),"Music  "+(music ? "ON" : "OFF"),skin.Toggle)) { music = !music; SaveSettings(); }
                    if (GUI.Button(new Rect(center+5,panel+350,(width-74)/2,45),"Effects  "+(reduced ? "LOW" : "FULL"),skin.Toggle)) { reduced = !reduced; SaveSettings(); }
                }
            }
            GUI.color = oldColour; GUI.matrix = oldMatrix;
        }
    }
}
