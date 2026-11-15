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
        private GUIStyle title, subtitle, text, button, badge, footnote;
        private string hudScore = "0 pts    0 coins", hudStatus = "Rajasthan";
        private float nextHudUpdate;
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
            gameCamera.fieldOfView = 68; gameCamera.nearClipPlane = .15f; gameCamera.farClipPlane = 210;
            cameraObject.AddComponent<AudioListener>();
            var lightObject = new GameObject("Rajasthan sunlight"); lightObject.transform.SetParent(transform, false);
            var sunlight = lightObject.AddComponent<Light>(); sunlight.type = LightType.Directional;
            sunlight.shadows = LightShadows.None; lightObject.transform.rotation = Quaternion.Euler(48, -32, 0);
            world = new TrackWorld(transform); world.Reset(seed);
            runner = new RunnerModel(transform);
            weather = new WeatherController(transform, gameCamera, sunlight);
            audioPlayer = new RunnerAudio(gameObject); audioPlayer.Settings(sound, music);
            state = State.Menu; PositionCamera(); world.Position(0, seed, 0, reduced); weather.Tick(0, 0, reduced);
        }
        private void StartRun()
        {
            distance = x = height = verticalSpeed = slideUntil = clock = 0;
            laneStartX = laneTargetX = laneElapsed = laneDuration = cameraX = nextHudUpdate = 0;
            coins = 0; lane = 1; seed = (seed + 379) & 0x7fffffff;
            world.Reset(seed); world.Position(0, seed, 0, reduced); state = State.Running;
            runner.Root.localPosition = Vector3.zero; runner.Root.localRotation = Quaternion.identity;
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
            if (state != State.Running) return;
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
            coins += gained; if (gained > 0) audioPlayer.Coin();
            if (hit) { EndRun(); }
            world.Position(distance, seed, clock, reduced);
            runner.Root.localPosition = new Vector3(x, height, 0);
            runner.Animate(clock, state == State.Running, sliding, height, reduced);
            if (hit) runner.Root.localRotation = Quaternion.Euler(0, 0, -12);
            else
            {
                float lean = reduced ? 0 : Mathf.Clamp((x - previous.x) / Mathf.Max(.001f, dt) / RunRules.LaneSpeed, -1, 1) * -8;
                runner.Root.localRotation = Quaternion.Slerp(runner.Root.localRotation, Quaternion.Euler(0, 0, lean), 1 - Mathf.Exp(-16 * dt));
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
            hudScore = $"{CurrentScore} pts    {coins} coins";
            hudStatus = $"Rajasthan  |  {(int)distance} m  |  {weather.Description}";
        }
        private void PositionCamera(float dt = 0)
        {
            float targetX = x * .18f;
            cameraX = dt <= 0 ? targetX : Mathf.Lerp(cameraX, targetX, 1 - Mathf.Exp(-10 * dt));
            gameCamera.transform.position = transform.position + new Vector3(cameraX, 5.2f, -8.5f);
            gameCamera.transform.LookAt(transform.position + new Vector3(cameraX, 1.4f, 14));
        }
        private void Pause() { state = State.Paused; SaveBest(); input.ResetGesture(); audioPlayer.Paused(true); }
        private void Resume() { state = State.Running; input.ResetGesture(); audioPlayer.Paused(false); }
        private void EndRun() { state = State.GameOver; SaveBest(); audioPlayer.Stumble(); }
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
        private void OnDestroy() { world?.Dispose(); audioPlayer?.Dispose(); PrimitiveArt.Clear(); }
        private void Styles()
        {
            if (title != null) return;
            title = new GUIStyle(GUI.skin.label) { fontSize = 35, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            title.normal.textColor = new Color(1, .80f, .25f);
            subtitle = new GUIStyle(title) { fontSize = 18 }; subtitle.normal.textColor = Color.white;
            text = new GUIStyle(subtitle) { fontSize = 17, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(12, 12, 6, 6) };
            button = new GUIStyle(GUI.skin.button) { fontSize = 20, fontStyle = FontStyle.Bold, wordWrap = true };
            badge = new GUIStyle(text) { fontSize = 19, fontStyle = FontStyle.Bold };
            footnote = new GUIStyle(text) { fontSize = 14, alignment = TextAnchor.MiddleCenter };
        }
        private bool Button(float xPos, float yPos, float w, string label) => GUI.Button(new Rect(xPos, yPos, w, 52), label, button);
        private void OnGUI()
        {
            Styles();
            float scale = Mathf.Min(Screen.width / 480f, Screen.height / 760f);
            float width = Screen.width / scale, fullHeight = Screen.height / scale;
            Rect safe = Screen.safeArea;
            float top = (Screen.height - safe.yMax) / scale + 10;
            float left = safe.xMin / scale, safeWidth = safe.width / scale;
            float center = left + safeWidth / 2;
            var previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            GUI.Box(new Rect(left + 8, top, safeWidth - 16, 72), GUIContent.none);
            GUI.Label(new Rect(left + 18, top + 5, safeWidth - 120, 32), hudScore, badge);
            GUI.Label(new Rect(left + 18, top + 38, safeWidth - 30, 28), hudStatus, text);
            if (state == State.Running)
            {
                if (GUI.Button(new Rect(left + safeWidth - 105, top + 9, 88, 44), "Pause", button)) Pause();
                GUI.Label(new Rect(center - 210, fullHeight - safe.yMin / scale - 52, 420, 40), "SWIPE: change lane / jump / slide", subtitle);
            }
            else
            {
                float panelWidth = Mathf.Min(440, safeWidth - 24), panelX = center - panelWidth / 2;
                float panelY = Mathf.Max(top + 90, (fullHeight - 595) / 2);
                GUI.Box(new Rect(panelX, panelY, panelWidth, 585), GUIContent.none);
                GUI.Label(new Rect(panelX + 10, panelY + 12, panelWidth - 20, 92), state == State.Menu ? "RANG ROUTE\nRUNNER" : state == State.Paused ? "RUN PAUSED" : "NICE RUN!", title);
                GUI.Label(new Rect(panelX + 12, panelY + 108, panelWidth - 24, 32), $"Rajasthan prototype  /  Best: {best}", subtitle);
                GUI.Label(new Rect(panelX + 14, panelY + 145, panelWidth - 28, 82), state == State.GameOver ? $"Distance: {(int)distance} m   Coins: {coins}\nScore: {CurrentScore}\nTry the open lane for a safe route." : "Swipe left/right to switch lanes.\nSwipe up to jump low crates; down to slide under high barriers. Avoid tall barricades.", text);
                float bx = panelX + 22, bw = panelWidth - 44;
                if (Button(bx, panelY + 238, bw, state == State.Paused ? "RESUME" : state == State.GameOver ? "RUN AGAIN" : "START RUN"))
                { if (state == State.Paused) Resume(); else { runner.Root.localRotation = Quaternion.identity; StartRun(); } }
                if (Button(bx, panelY + 300, bw, "Reduced effects: " + (reduced ? "ON" : "OFF"))) { reduced = !reduced; SaveSettings(); }
                if (Button(bx, panelY + 362, bw / 2 - 5, "Music: " + (music ? "ON" : "OFF"))) { music = !music; SaveSettings(); }
                if (Button(bx + bw / 2 + 5, panelY + 362, bw / 2 - 5, "Sound: " + (sound ? "ON" : "OFF"))) { sound = !sound; SaveSettings(); }
                if (Button(bx, panelY + 424, bw, "Weather: " + weather.Selection + " (tap)"))
                { weather.Selection = (WeatherController.Mode)(((int)weather.Selection + 1) % 4); weather.Tick(clock, 10, reduced); world.SetWeather(weather.Cloudiness); RefreshHud(); }
                GUI.Label(new Rect(bx, panelY + 480, bw, 94), "Original placeholder art and audio.\nEnglish prototype; Hindi and other regions are planned.\nKeep looking for updates for new content.", footnote);
            }
            GUI.matrix = previousMatrix;
        }
    }
}
