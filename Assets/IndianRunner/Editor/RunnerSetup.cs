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
            RunnerAppIcon.Apply();
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
            Require(UnityEngine.Resources.Load<Texture2D>("Scenery/RajasthanHorizonReal") != null, "Realistic horizon missing");
            Require(UnityEngine.Resources.Load<Texture2D>("Scenery/SandstoneReal") != null, "Sandstone surface missing");
            var cotton = UnityEngine.Resources.Load<Texture2D>("Scenery/CottonReal");
            Require(cotton != null && cotton.wrapMode == TextureWrapMode.Repeat, "Cotton surface missing or not repeatable");
            Require(RunRules.LaneProgress(0) == 0 && RunRules.LaneProgress(1) == 1, "Lane easing endpoints");
            float lastProgress = 0;
            for (int step = 1; step <= 600; step++)
            {
                float laneProgress = RunRules.LaneProgress(step / 600f);
                Require(laneProgress >= lastProgress && laneProgress <= 1, "Lane easing overshoot");
                lastProgress = laneProgress;
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
                    Require(kind >= 0 && kind <= RajasthanObstacles.MaxKind, "Unknown obstacle kind");
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
                    int moving=RunRules.DynamicKind(row);
                    var bounds=FestivalObstacles.Shape(moving)*RajasthanObstacles.VisualScale(moving);
                    for(int lane=0;lane<3;lane++) if(RunRules.ObstacleFor(row,lane,seed)==moving)
                        for(int sample=0;sample<=10;sample++)
                            Require(Mathf.Abs(RunRules.HazardX(moving,lane,45-sample*5)-RunRules.LaneX(safe))>bounds.x+RunRules.PlayerHalfWidth,"Dynamic sweep blocks escape lane");
                    foreach(int neighbour in new[]{row-1,row+1}) for(int lane=0;lane<3;lane++)
                        Require(RunRules.ObstacleFor(neighbour,lane,seed) == 0,"Moving hazard needs empty recovery rows");
                }
                if(RunRules.RestBuffer(row))
                    for(int lane=0;lane<3;lane++) Require(RunRules.ObstacleFor(row,lane,seed)==0,"Rest garden or transition contains a hazard");
                if(RunRules.PouchRow(row)) Require(RunRules.RestRow(row),"Pouch pickup must be in a safe garden");
                if(RunRules.RainVisitorRow(row))
                {
                    Require(!RunRules.RestBuffer(row) && !RunRules.NearDynamic(row) && !RunRules.RoadworksRow(row),"Rain visitor occupies a protected row");
                    Require(Mathf.Abs(RunRules.LaneX((safe+1)%3)-RunRules.LaneX(safe))>1.16f+RunRules.PlayerHalfWidth,"Umbrella blocks the reserved lane");
                }
                checkedRows++;
            }
            Require(RunRules.Speed(0) == RunRules.StartSpeed && RunRules.StartSpeed==11 && RunRules.Speed(100000) == RunRules.MaxSpeed, "Speed bounds");
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
            Require(RunRules.DynamicKind(16)==RajasthanObstacles.OncomingElephant && RunRules.DynamicKind(52)==RajasthanObstacles.OncomingElephant,"Oncoming elephant schedule incorrect");
            Require(RunRules.HazardZ(RajasthanObstacles.OncomingElephant,100,90)>100 && RunRules.HazardZ(RajasthanObstacles.OncomingElephant,100,110)<100,"Elephant must approach the runner from the front");
            var elephantShape=FestivalObstacles.Shape(RajasthanObstacles.OncomingElephant);
            Require(elephantShape.x+RunRules.PlayerHalfWidth<RunRules.LaneWidth && elephantShape.z>=2.4f,"Elephant bounds block a safe lane or miss its trunk");
            Require(TrackWorld.SegmentBox(new Vector3(0,0,5),new Vector3(0,0,-5),new Vector3(-elephantShape.x-RunRules.PlayerHalfWidth,-2,-elephantShape.z-RunRules.PlayerHalfWidth),new Vector3(elephantShape.x+RunRules.PlayerHalfWidth,elephantShape.y,elephantShape.z+RunRules.PlayerHalfWidth)),"Elephant relative sweep missed");
            Require(RunRules.HazardZ(6,100,90)>100 && RunRules.HazardZ(6,100,110)<100,"Motorcycle must approach from the front");
            // Relative sweeps include both player movement and obstacle motion.
            Require(TrackWorld.SegmentBox(new Vector3(0,0,3),new Vector3(0,0,-3),new Vector3(-.84f,-2.03f,-1.4f),new Vector3(.84f,1.97f,1.4f)),"Motorcycle sweep missed");
            Require(TrackWorld.SegmentBox(new Vector3(0,-4,0),new Vector3(0,-.8f,0),new Vector3(-1.26f,-2.03f,-1.26f),new Vector3(1.26f,.74f,1.26f)),"Descending ship sweep missed");
            Require(RunRules.DynamicKind(32)==RajasthanObstacles.Spaceship,"Spaceship rarity schedule");
            var guardianShader=UnityEngine.Resources.Load<Shader>("Scenery/CloudGuardian");
            Require(guardianShader!=null && guardianShader.isSupported,"Cloud Guardian shader missing or unsupported");
            Require(!RunRules.GuardianWindow(287.9f) && RunRules.GuardianWindow(288) && !RunRules.GuardianWindow(308),"Guardian intro window incorrect");
            Require(!RunRules.GuardianWindow(792) && RunRules.GuardianWindow(1296),"Guardian rarity incorrect");
            for(int gardenRow=10;gardenRow<1000;gardenRow+=36)
                for(int boundary=0;boundary<2;boundary++)
                {
                    float appearanceStart=gardenRow*RunRules.RowSpacing+(boundary==0 ? 8:27.99f);
                    float appearanceEnd=appearanceStart+RunRules.MaxSpeed*CloudGuardian.Duration;
                    for(float sample=appearanceStart;sample<=appearanceEnd;sample+=1)
                        Require(RunRules.RestBuffer((int)(sample/RunRules.RowSpacing)),"Guardian overlaps an obstacle row");
                }
            Require(RunRules.DanceRestDistance(280) && !RunRules.DanceRestDistance(784) && !RunRules.DanceRestDistance(1288) && RunRules.DanceRestDistance(1792),"Dance rest must skip two gardens between breaks");
            Require(RunRules.RestDistance(280) && !RunRules.RestDistance(364),"Rest garden boundaries");
            Require(RunRules.DynamicKind(2)==RajasthanObstacles.Wheel && RunRules.DynamicKind(8)==RajasthanObstacles.FallingCloth,"Intro encounter schedule");
            Require(RunRules.DynamicKind(14)==RajasthanObstacles.Peacock && RunRules.DynamicKind(20)==RajasthanObstacles.Procession,"Peacock/procession schedule");
            Require(RunRules.RoadworksRow(18),"Roadwork schedule");
            Require(RunRules.HazardBaseY(RajasthanObstacles.Wheel,20)==0,"Wheel collision base must stay on road while mesh rotates about axle");
            Require(RunRules.HazardY(RajasthanObstacles.FallingCloth,40)>6 && RunRules.HazardY(RajasthanObstacles.FallingCloth,10)==0,"Fabric drop endpoints");
            var progress=new FestivalProgress();
            Require(!progress.Claim(),"Incomplete goals granted a reward");
            progress.Add(10,1,149); Require(progress.Completed==2 && !progress.Ready,"Distance goal completed early");
            Require(progress.Add(0,0,150) && progress.Ready,"Completed goals not ready");
            Require(progress.Claim() && progress.Set==1 && progress.Unlocked==2 && progress.Outfit==1,"First outfit reward missing");
            Require(!progress.Claim() && progress.Tokens==0 && progress.Distance==0,"Reward claimed twice or counters not reset");
            var migrated=new FestivalProgress(0,30,5,499);
            Require(migrated.Ready && migrated.Tokens==10 && migrated.Slides==1 && migrated.Distance==150,"Existing saved progress lost after easier goals");
            var loaded=new FestivalProgress(progress.Set,progress.Tokens,progress.Slides,progress.Distance,progress.Outfit);
            Require(loaded.Outfit==1 && loaded.Unlocked==2,"Outfit lost in progress snapshot");
            var invalid=new FestivalProgress(-8,-1,-1,-1,99); Require(invalid.Set==0 && invalid.Outfit==0 && invalid.Tokens==0,"Invalid snapshot not clamped");
            foreach(VoiceRole voiceRole in Enum.GetValues(typeof(VoiceRole)))
            {
                var sample=RoadBanter.Sample(voiceRole);
                Require(sample.Role==voiceRole && sample.Variant==1 && !string.IsNullOrEmpty(sample.English),"Character voice role or English sample missing");
                Require(sample.Pitch>=.88f && sample.Pitch<=1.12f && sample.Rate>=.90f && sample.Rate<=1.12f,"Character voice outside natural delivery range");
            }
            foreach(var spokenLine in RoadBanter.AllLines)
            {
                var recordedClip=UnityEngine.Resources.Load<AudioClip>("Dialogue/hi/"+spokenLine.AudioKey);
                Require(recordedClip!=null && recordedClip.length>0 && recordedClip.length<5,"Hindi recording missing, empty or unexpectedly long: "+spokenLine.AudioKey);
            }
            var chatter=new RoadBanter(); chatter.Reset(7301);
            Require(!chatter.Tick(1,RajasthanObstacles.Motorcycle,9,11),"Voice triggered outside passing range");
            Require(chatter.Tick(1,RajasthanObstacles.Motorcycle,8,11),"Rider dialogue missing");
            string firstCaption=chatter.Current.Caption;
            Require(chatter.Visible(2) && !chatter.Tick(2,RajasthanObstacles.Motorcycle,7,11),"Repeated voice for same encounter");
            var elephantChatter=new RoadBanter(); elephantChatter.Reset(1);
            Require(elephantChatter.Tick(1,RajasthanObstacles.OncomingElephant,7,9) && elephantChatter.Current.Role==VoiceRole.Elephant,"Oncoming elephant did not use elephant recordings");
            Require(!chatter.Tick(3,RajasthanObstacles.Camel,6,22),"Voice cooldown ignored");
            Require(chatter.Tick(10,RajasthanObstacles.Camel,7,33),"Animal dialogue missing after cooldown");
            Require(!chatter.Visible(15),"Subtitle expiry ignored");
            chatter.Reset(7301);
            Require(!chatter.Visible(0) && chatter.Tick(1,RajasthanObstacles.Motorcycle,0,11) && chatter.Current.Caption==firstCaption,"Voice reset is not deterministic");
            Require(RunRules.CanAdmitRainVisitor(.5f,180) && RunRules.CanAdmitRainVisitor(.5f,210),"Rain admission window lost");
            Require(!RunRules.CanAdmitRainVisitor(0,200) && !RunRules.CanAdmitRainVisitor(1,179),"Rain visitor spawned dry or too close");
            chatter.Reset(7301);
            Require(!chatter.Tick(0,RajasthanObstacles.Motorcycle,8,1,true),"Other chatter did not leave room for rain line");
            Require(chatter.Tick(0,RajasthanObstacles.UmbrellaLady,8,2,true) && chatter.Current.Caption.Contains("It's raining"),"Requested rain dialogue missing");
            Require(!chatter.Tick(1,RajasthanObstacles.UmbrellaLady,7,2,true),"Rain dialogue repeats every frame");
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
        [MenuItem("Indian Runner/5 - Inspect owned mesh memory (Play Mode)")]
        public static void CheckMeshMemory()
        {
            Require(Application.isPlaying,"Press Play before inspecting runtime mesh ownership.");
            PrimitiveArt.GetOwnedMeshStats(out int count,out int readable,out long bytes);
            Require(count>0 && readable<count,"No finalized static meshes found; check scenery setup.");
            Debug.Log($"Owned meshes: {count}; readable: {readable}; finalized: {count-readable}; engine-reported mesh memory: {bytes/1048576f:F2} MiB. This is owned mesh memory only, not total app/GPU/TTS memory or APK size. Profile Android separately.");
        }
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    }
}
