using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEngine;

namespace IndianRunner.Editor
{
    public static class RunnerSetup
    {
        public const string ScenePath = "Assets/IndianRunner/Scenes/Rajasthan.unity";

        [MenuItem("Indian Runner/1 - Create or open Rajasthan scene")]
        public static void CreateScene()
        {
            if (Application.isPlaying) { Debug.LogWarning("Stop Play Mode before setting up the scene."); return; }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            RepairMaterials();
            if (File.Exists(ScenePath))
            {
                EditorSceneManager.OpenScene(ScenePath);
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
                return;
            }
            Directory.CreateDirectory("Assets/IndianRunner/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Rajasthan Run Manager").AddComponent<RunManager>();
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.Refresh(); Debug.Log("Rajasthan scene ready. Select portrait Game view and press Play.");
        }
        [MenuItem("Indian Runner/Repair runner materials")]
        public static void RepairMaterials()
        {
            if (Application.isPlaying) { Debug.LogWarning("Stop Play Mode before repairing material assets."); return; }
            Directory.CreateDirectory("Assets/IndianRunner/Resources");
            const string path = "Assets/IndianRunner/Resources/RunnerBase.mat";
            var shader = PrimitiveArt.ResolveShader();
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            PrimitiveArt.ConfigureMaterial(material, shader, Color.white);
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var saved = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (saved == null || saved.shader == null || saved.shader != shader)
                throw new InvalidOperationException("RunnerBase material could not be serialized. Check asset write permissions, then run Indian Runner > Repair runner materials again.");
            Debug.Log("Runner material prepared with " + shader.name + ". Press Play to regenerate scene colours.");
        }
        [MenuItem("Indian Runner/2 - Configure Android prototype")]
        public static void ConfigureAndroid()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.companyName = "Runner Prototype";
            PlayerSettings.productName = "Rang Route Runner - Prototype";
            // Internal testing ID only; choose the final identity before publishing.
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.prototype.rangrouterunner");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.stripEngineCode = false;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("Prototype settings saved. Install Android Build Support including SDK/NDK/OpenJDK, then use File > Build Profiles > Android. No build was performed.");
        }
        [MenuItem("Indian Runner/3 - Run core rule checks")]
        public static void CheckRules()
        {
            // Regression: an imported template must not retain an incompatible shader
            // or lose its tint when materials use _BaseColor instead of _Color.
            var shader = PrimitiveArt.ResolveShader();
            var savedTemplate = AssetDatabase.LoadAssetAtPath<Material>("Assets/IndianRunner/Resources/RunnerBase.mat");
            Require(savedTemplate != null && savedTemplate.shader == shader, "RunnerBase serialized shader is invalid; run Repair runner materials");
            var probe = new Material(shader);
            try
            {
                var tint = new Color(.1f, .6f, .8f, 1);
                PrimitiveArt.ConfigureMaterial(probe, shader, tint);
                Require(probe.shader == shader && probe.shader.isSupported, "Unsupported material shader");
                bool tinted = false;
                if (probe.HasProperty("_BaseColor")) { Require(probe.GetColor("_BaseColor") == tint, "Base colour lost"); tinted = true; }
                if (probe.HasProperty("_Color")) { Require(probe.GetColor("_Color") == tint, "Legacy colour lost"); tinted = true; }
                Require(tinted, "Pipeline shader has no supported tint property");
            }
            finally { UnityEngine.Object.DestroyImmediate(probe); }
            var sceneryShader = UnityEngine.Resources.Load<Shader>("Scenery/RunnerScenery");
            Require(sceneryShader != null && sceneryShader.isSupported, "Scenery shader missing or unsupported");
            Require(UnityEngine.Resources.Load<Texture2D>("Scenery/RajasthanHorizon") != null, "Horizon image missing");
            Require(UnityEngine.Resources.Load<Texture2D>("Scenery/RajasthanBazaar") != null, "Bazaar image missing");
            Require(UnityEngine.Resources.Load<Texture2D>("Scenery/RajasthanHorizonReal") != null, "Realistic horizon missing");
            Require(UnityEngine.Resources.Load<Texture2D>("Scenery/SandstoneReal") != null, "Sandstone surface missing");
            var cotton = UnityEngine.Resources.Load<Texture2D>("Scenery/CottonReal");
            Require(cotton != null && cotton.wrapMode == TextureWrapMode.Repeat, "Cotton surface missing or not repeatable");
            Require(RunRules.LaneProgress(0) == 0 && RunRules.LaneProgress(1) == 1, "Lane easing endpoints");
            float lastProgress = 0;
            for (int step = 1; step <= 600; step++)
            {
                float progress = RunRules.LaneProgress(step / 600f);
                Require(progress >= lastProgress && progress <= 1, "Lane easing overshoot");
                lastProgress = progress;
            }
            int checkedRows = 0;
            float maxLaneTravel = RunRules.LaneTravelTime(RunRules.LaneWidth * 2);
            float betweenRows = (RunRules.RowSpacing - 2 * (RunRules.ObstacleHalfDepth + RunRules.PlayerHalfWidth)) / RunRules.MaxSpeed;
            // Safe lane changes need at least .7 seconds of human reaction allowance.
            Require(betweenRows > maxLaneTravel + .7f, "Insufficient safe-route reaction allowance");
            for (int seed = 0; seed < 32; seed++) for (int row = 0; row < 1000; row++)
            {
                int safe = RunRules.SafeLane(row, seed), clear = 0;
                Require(safe >= 0 && safe < 3, "Safe lane out of range");
                for (int lane = 0; lane < 3; lane++)
                {
                    if (!RunRules.IsBlocked(row, lane, seed)) clear++;
                    int kind = RunRules.ObstacleFor(row, lane, seed);
                    Require(kind >= 0 && kind <= RajasthanObstacles.Spaceship, "Unknown obstacle kind");
                }
                Require(clear >= 1 && !RunRules.IsBlocked(row, safe, seed), "Every generated obstacle row needs a reserved safe lane");
                if (RunRules.HasCamel(row, seed))
                {
                    Require(safe != 1, "Camel must never cross the middle escape lane");
                    for (int sample = 0; sample <= 20; sample++)
                    {
                        float ahead = RunRules.CamelWarningDistance - sample * (RunRules.CamelWarningDistance + 12) / 20;
                        float gap = Mathf.Abs(RunRules.CamelX(safe, ahead) - RunRules.LaneX(safe));
                        Require(gap > RunRules.CamelHalfWidth*RajasthanObstacles.VisualScale(RajasthanObstacles.Camel) + RunRules.PlayerHalfWidth, "Camel sweep blocks the reserved lane");
                    }
                }
                if(RunRules.DynamicKind(row) != 0)
                {
                    int target=RunRules.HazardLane(row,seed);
                    Require(target != safe,"Dynamic hazard blocks coin lane");
                    Require(Mathf.Abs(RunRules.LaneX(target)-RunRules.LaneX(safe)) > .92f*RajasthanObstacles.VisualScale(RajasthanObstacles.Spaceship)+RunRules.PlayerHalfWidth,"Dynamic swept width blocks escape lane");
                    foreach(int neighbour in new[]{row-1,row+1}) for(int lane=0;lane<3;lane++)
                        Require(RunRules.ObstacleFor(neighbour,lane,seed) == 0,"Moving hazard needs empty recovery rows");
                }
                checkedRows++;
            }
            Require(RunRules.Speed(0) == 9 && RunRules.Speed(100000) == RunRules.MaxSpeed, "Speed bounds");
            Require(RunRules.Score(100.5f, 3) == 130 && RunRules.Score(0, 0) == 0, "Score calculation");
            float airTime = 2 * RunRules.JumpVelocity / RunRules.Gravity;
            Require(airTime < betweenRows, "Jump recovery must fit between rows");
            Require(RunRules.JumpVelocity * RunRules.JumpVelocity / (2 * RunRules.Gravity) > 1.1f, "Jump clearance");
            Require(TrackWorld.SegmentBox(new Vector3(0, 0, -5), new Vector3(0, 0, 5), new Vector3(-1, -1, -1), Vector3.one), "Swept collision missed");
            Require(!TrackWorld.SegmentBox(new Vector3(3, 0, -5), new Vector3(3, 0, 5), new Vector3(-1, -1, -1), Vector3.one), "Safe lane falsely collided");
            Require(!TrackWorld.SegmentBox(new Vector3(0, 1.2f, -5), new Vector3(0, 1.2f, 5), new Vector3(-1, -2, -1), new Vector3(1, .93f, 1)), "Jump incorrectly hit low obstacle");
            Require(!TrackWorld.SegmentBox(new Vector3(0, 0, -5), new Vector3(0, 0, 5), new Vector3(-1, .45f, -1), new Vector3(1, 3, 1)), "Slide incorrectly hit high barrier");
            Require(RunRules.CamelWarningDistance / RunRules.MaxSpeed >= 2, "Camel warning must allow at least two seconds at maximum speed");
            Require(RunRules.HazardY(7,40) == 9 && Mathf.Abs(RunRules.HazardY(7,8)-.8f)<.001f,"Spaceship descent endpoints");
            Require(RunRules.HazardZ(6,100,90)>100 && RunRules.HazardZ(6,100,110)<100,"Motorcycle must approach from the front");
            // Relative sweeps include both player movement and obstacle motion.
            Require(TrackWorld.SegmentBox(new Vector3(0,0,3),new Vector3(0,0,-3),new Vector3(-.84f,-2.03f,-1.4f),new Vector3(.84f,1.97f,1.4f)),"Motorcycle sweep missed");
            Require(TrackWorld.SegmentBox(new Vector3(0,-4,0),new Vector3(0,-.8f,0),new Vector3(-1.26f,-2.03f,-1.26f),new Vector3(1.26f,.74f,1.26f)),"Descending ship sweep missed");
            Debug.Log($"PASS: {checkedRows} deterministic rows, route timing, speed, score, jump recovery and swept collision checks. Phone gameplay/performance remain separate manual checks.");
        }
        [MenuItem("Indian Runner/4 - Check live startup (Play Mode)")]
        public static void CheckLiveStartup()
        {
            Require(Application.isPlaying, "Press Play in the Rajasthan scene before checking live startup.");
            var manager = UnityEngine.Object.FindFirstObjectByType<RunManager>();
            Require(manager != null, "No runner manager found in the playing scene.");
            manager.ValidateStartup();
            Debug.Log("PASS: runner, weather, audio, camera, feedback and all 14 pooled rows (markers, obstacles, coins) initialized.");
        }
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    }
}
