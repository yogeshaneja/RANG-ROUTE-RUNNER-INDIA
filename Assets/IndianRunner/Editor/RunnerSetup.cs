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
            int keralaRows=0;
            for(int regionSeed=0;regionSeed<32;regionSeed++) for(int row=0;row<1000;row++)
            {
                int safe=RunRules.SafeLane(row,regionSeed),moving=RegionRules.DynamicKind(row,true);
                Require(RegionRules.ObstacleFor(row,safe,regionSeed,true)==0,"Kerala reserved lane blocked");
                for(int lane=0;lane<3;lane++)
                {
                    int kind=RegionRules.ObstacleFor(row,lane,regionSeed,true);
                    Require(kind==0 || RegionRules.UsesKind(RegionProfile.Kerala,kind),"Foreign Kerala obstacle");
                    if(kind==RegionRules.Boar || kind==RegionRules.Langur || kind==RajasthanObstacles.OncomingElephant)
                        Require(RegionRules.ForestRow(row),"Wild forest animal in backwater village");
                    if(RunRules.RestBuffer(row)) Require(kind==0,"Kerala rest buffer blocked");
                    if(kind!=0)
                    {
                        var shape=FestivalObstacles.Shape(kind)*RajasthanObstacles.VisualScale(kind);
                        Require(Mathf.Abs(RunRules.LaneX(lane)-RunRules.LaneX(safe))>shape.x+RunRules.PlayerHalfWidth,"Kerala visual bounds overlap safe lane");
                    }
                }
                if(moving!=0) foreach(int neighbour in new[]{row-1,row+1}) for(int lane=0;lane<3;lane++)
                    Require(RegionRules.ObstacleFor(neighbour,lane,regionSeed,true)==0,"Kerala moving hazard recovery row occupied");
                keralaRows++;
            }
            Require(RegionRules.BridgeTile(1) && RegionRules.BridgeTile(3) && !RegionRules.BridgeTile(4),"Bridge/forest sequence lost");
            Require(FestivalObstacles.Shape(RegionRules.Bamboo).x+RunRules.PlayerHalfWidth<RunRules.LaneWidth,"Shelter overlaps neighbouring lane");
            int newRegionRows=0;
            for(int region=RegionProfile.Punjab;region<=RegionProfile.Goa;region++)
            {
                for(int regionSeed=0;regionSeed<32;regionSeed++) for(int row=0;row<1000;row++)
                {
                    int safe=RunRules.SafeLane(row,regionSeed),moving=RegionRules.DynamicKind(row,region);
                    Require(RegionRules.ObstacleFor(row,safe,regionSeed,region)==0,"New region safe lane blocked");
                    for(int lane=0;lane<3;lane++)
                    {
                        int kind=RegionRules.ObstacleFor(row,lane,regionSeed,region);
                        if(RunRules.RestBuffer(row))Require(kind==0,"New region rest buffer blocked");
                        if(kind==0)continue;
                        Require(RegionRules.UsesKind(region,kind),"Obstacle does not belong to selected region");
                        var shape=FestivalObstacles.Shape(kind)*RajasthanObstacles.VisualScale(kind);
                        Require(Mathf.Abs(RunRules.LaneX(lane)-RunRules.LaneX(safe))>shape.x+RunRules.PlayerHalfWidth,"New region hazard overlaps safe lane");
                    }
                    if(moving!=0)foreach(int neighbour in new[]{row-1,row+1})for(int lane=0;lane<3;lane++)
                        Require(RegionRules.ObstacleFor(neighbour,lane,regionSeed,region)==0,"New region recovery row occupied");
                    newRegionRows++;
                }
            }
            int adventureRows=0;
            for(int stage=0;stage<RegionProfile.Count;stage++)for(int routeSeed=0;routeSeed<16;routeSeed++)for(int row=0;row<1000;row++)
            {
                int safe=RunRules.SafeLane(row,routeSeed);
                Require(RegionRules.ObstacleFor(row,safe,routeSeed,stage)==0,"Adventure blocked the reserved token lane");
                if(AdventureRules.OverridesRow(row))
                {
                    Require(RegionRules.DynamicKind(row,stage)==0,"Old moving hazard entered an adventure stretch");
                    for(int lane=0;lane<3;lane++)
                    {
                        int kind=RegionRules.ObstacleFor(row,lane,routeSeed,stage);
                        Require(kind==0 || RegionRules.UsesKind(stage,kind),"Adventure borrowed an incompatible regional obstacle");
                        if(kind!=0 && AdventureRules.RideRow(row))
                        {
                            var bounds=AdventureArt.RideObstacleShape(stage);
                            Require(Mathf.Abs(RunRules.LaneX(lane)-RunRules.LaneX(safe))>bounds.x+AdventureRules.RideHalfWidth,"Vehicle body intersects safe lane");
                        }
                    }
                }
                if(RunRules.RestBuffer(row))Require(!AdventureRules.RideRow(row) && !AdventureRules.SectionRow(row),"Adventure overlaps a rest/transfer buffer");
                adventureRows++;
            }
            Require(AdventureRules.BoardingWindow(504) && !AdventureRules.BoardingWindow(532),"Boarding window incorrect");
            Require(AdventureRules.PickupValue(4)==3 && AdventureRules.PickupValue(40)==2 && AdventureRules.PickupValue(18)==2 && AdventureRules.PickupValue(9)==1,"Adventure token values incorrect");
            // The rare bonus replaces only a complete authored passage, never a vehicle,
            // traffic, rain visitor, rest or state-transfer encounter.
            int festivalRows=0;
            for(int row=0;row<FestivalRushRules.PeriodRows*20;row++)
            {
                if(!FestivalRushRules.ActiveRow(row))continue;
                festivalRows++;
                Require(AdventureRules.SectionRow(row) && !AdventureRules.RideRow(row) && !RunRules.RestBuffer(row),"Festival overlaps another encounter");
                Require(AdventureRules.PickupValue(row)==3,"Festival reward value changed");
                for(int stage=0;stage<RegionProfile.Count;stage++)
                {
                    Require(RegionRules.DynamicKind(row,stage)==0,"Moving hazard entered festival");
                    Require(!AdventureRules.RainVisitorRow(row,stage),"Rain visitor entered festival");
                    for(int lane=0;lane<3;lane++)
                        Require(RegionRules.ObstacleFor(row,lane,7301,stage)==0,"Festival bonus road is blocked");
                }
            }
            Require(festivalRows==80,"Festival repeats too frequently");
            Require(!FestivalRushRules.ActiveRow(3) && FestivalRushRules.ActiveRow(4) && FestivalRushRules.ActiveRow(7) && !FestivalRushRules.ActiveRow(8),"Festival entry/exit boundaries incorrect");
            Require(!FestivalRushRules.ActiveRow(40) && !FestivalRushRules.ActiveRow(76) && FestivalRushRules.ActiveRow(112),"Festival rarity incorrect");
            Require(FestivalRushRules.Approaching(3) && FestivalRushRules.Remaining(112)==112 && FestivalRushRules.Remaining(224)==0,"Festival cue/progress incorrect");
            Require(AdventureRules.CompanionCooldown>=8,"Companion collection cooldown too short");
            Debug.Log($"PASS: {adventureRows} adventure/vehicle generation rows; companion cooldown and token values.");
            Require(!JourneyRules.InTransferWindow(252,600) && JourneyRules.InTransferWindow(756,600),"Journey first transfer window incorrect");
            Require(JourneyRules.InTransferWindow(775.9f,600) && !JourneyRules.InTransferWindow(776,600),"Journey window must end before unsafe approach");
            for(int firstRegion=0;firstRegion<RegionProfile.Count;firstRegion++)
            {
                int current=firstRegion,visited=1<<current,bag=1<<current;
                for(int visit=1;visit<64;visit++)
                {
                    int next=JourneyRules.NextRegion(current,visited,visit*31337,out visited);
                    Require(next!=current && next>=0 && next<RegionProfile.Count,"Journey repeated current region");
                    if(visit%RegionProfile.Count==0)bag=0;
                    Require((bag&(1<<next))==0,"Journey shuffle bag repeated before all destinations");
                    bag|=1<<next;current=next;
                }
            }
            Require(RegionProfile.Count==8 && RegionProfile.Valid(-1)==0 && RegionProfile.Valid(9)==0,"Region selector bounds invalid");
            Require(RegionProfile.BestKey(0)=="runner.best" && RegionProfile.BestKey(1)=="runner.best.kerala","Legacy scores lost");
            for(int destination=RegionProfile.Shillong;destination<RegionProfile.Count;destination++)
            {
                Require(ExpandedRegionRules.IsExpanded(destination),"Missing expanded destination");
                Require(RegionRules.IsSlide(ExpandedRegionRules.Slide(destination)),"Regional slide opening not configured");
                Require(RegionRules.UsesKind(destination,ExpandedRegionRules.Traffic(destination)),"Regional traffic missing from catalogue");
                int moving=ExpandedRegionRules.Traffic(destination);
                Require(RunRules.HazardZ(moving,100,90)>100 && RunRules.HazardZ(moving,100,110)<100,"New vehicle is not approaching");
                for(int other=0;other<RegionProfile.Count;other++)if(other!=destination)
                    Require(RegionProfile.BestKey(destination)!=RegionProfile.BestKey(other),"Destination best scores overlap");
                for(int row=0;row<1200;row++)for(int lane=0;lane<3;lane++)
                {
                    int kind=RegionRules.ObstacleFor(row,lane,7301,destination);
                    Require(kind==0 || RegionRules.UsesKind(destination,kind),"Expanded route uses absent artwork");
                }
            }
            foreach(string preview in new[]{"Shillong","Kolkata","MadhyaPradesh","Himachal"})
                Require(Resources.Load<Texture2D>("StageSelection/"+preview)!=null,"New destination preview missing");
            Require(CloudGuardian.BaseHeight<2.5f,"Sky guardian was not lowered");
            Require(RunRules.HazardZ(RegionRules.Tractor,100,90)>100 && RunRules.HazardZ(RegionRules.Tractor,100,110)<100,"Tractor must approach the runner");
            Debug.Log($"PASS: {newRegionRows} Punjab/Goa rows and region/guardian contracts.");
            Debug.Log($"PASS: {keralaRows} Kerala safe-route/habitat/bounds rows. Run live startup checks in each selected region.");
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
