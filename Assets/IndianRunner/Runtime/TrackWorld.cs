using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    public sealed class TrackWorld
    {
        private const int TileCount = 7;
        private const float TileLength = RunRules.RowSpacing * 2;
        private sealed class Row
        {
            public float Z;
            public readonly int[] Kind = new int[3];
            public readonly RajasthanObstacles.Instance[] Obstacles = new RajasthanObstacles.Instance[3];
            public bool CamelCrossing, Roadworks;
            public int DynamicKind, HazardLane;
            public Transform LandingMarker, Pouch, Garden;
            public bool HasPouch, PouchTaken, SlideTaken;
            public readonly Transform[] Coins = new Transform[5];
            public readonly bool[] Taken = new bool[5];
            public int Safe;
        }
        private sealed class Tile
        {
            public Transform Root;
            public Transform Village;
            public int Index;
            public bool HasElephant;
            public readonly Row[] Rows = { new Row(), new Row() };
        }
        private readonly Tile[] tiles = new Tile[TileCount];
        private readonly List<Transform> birds = new List<Transform>();
        private readonly Material yellow, coral, teal;
        private readonly RajasthanObstacles obstacleArt;
        private readonly RajasthanDecor decor;
        private sealed class Kite
        {
            public Transform Root;
            public Vector3 BasePosition;
            public float Phase;
        }
        private readonly List<Kite> kites = new List<Kite>();
        public int PouchesCollected { get; private set; }
        public int SlidesCleared { get; private set; }
        public string WarningMessage { get; private set; } = "";
        public int SoundKind { get; private set; }
        public int SoundKey { get; private set; } = -1;
        public float SoundAhead { get; private set; } = 100;
        public float SoundPan { get; private set; }
        public bool CamelContact { get; private set; }
        public string ContactMessage { get; private set; } = "";
        private int nextIndex;
        private readonly RajasthanScenery scenery;
        public TrackWorld(Transform parent)
        {
            scenery = new RajasthanScenery(parent);
            obstacleArt = new RajasthanObstacles(); decor = new RajasthanDecor();
            yellow = PrimitiveArt.Mat(new Color(1f, .75f, .05f));
            coral = PrimitiveArt.Mat(new Color(.91f, .23f, .14f));
            teal = PrimitiveArt.Mat(new Color(.04f, .43f, .50f));
            var road = scenery.RoadSurface;
            var white = PrimitiveArt.Mat(new Color(.68f,.57f,.41f));
            var camel = PrimitiveArt.Mat(new Color(.66f, .40f, .19f));
            var blue = PrimitiveArt.Mat(new Color(.03f, .27f, .8f));
            var green = PrimitiveArt.Mat(new Color(.08f, .56f, .28f));
            for (int i = 0; i < TileCount; i++)
            {
                var tile = new Tile { Root = new GameObject("Pooled Rajasthan segment " + i).transform };
                tile.Root.SetParent(parent,false); tiles[i] = tile;
                var ground = new GameObject("Batched road and terrain").transform; ground.SetParent(tile.Root,false);
                tile.Village = new GameObject("Reusable varied roadside").transform; tile.Village.SetParent(tile.Root,false);
                scenery.AddGround(ground,i,TileLength);
                PrimitiveArt.Box(ground, "Road", new Vector3(0, -.045f, TileLength / 2), new Vector3(8.5f, .12f, TileLength), road);
                for (int dash = 0; dash < 8; dash++) foreach (float x in new[] { -RunRules.LaneWidth / 2, RunRules.LaneWidth / 2 })
                    PrimitiveArt.Box(ground, "Lane dash", new Vector3(x, .025f, dash * 6.5f + 2), new Vector3(.11f, .03f, 2.4f), white);
                if (i%3 == 1) PrimitiveArt.Camel(tile.Village,-7.7f,17+i*2,camel,RajasthanScenery.GroundY(i,-7.7f,17+i*2,TileLength));
                if (i%3 == 2) PrimitiveArt.Peacock(tile.Village,6.3f,29+i,blue,green,RajasthanScenery.GroundY(i,6.3f,29+i,TileLength));
                tile.HasElephant = i%3 == 0;
                if(tile.HasElephant) RajasthanDecor.Elephant(tile.Village,new Vector3(9.5f,RajasthanScenery.GroundY(i,9.5f,31,TileLength),31));
                scenery.AddGeometry(tile.Village,i);
                PrimitiveArt.CombineScenery(ground);
                PrimitiveArt.CombineScenery(tile.Village);
                for (int b = 0; b < 2; b++)
                {
                    var bird = new GameObject("Scenic bird").transform; bird.SetParent(tile.Root, false);
                    bird.localPosition = new Vector3(b == 0 ? -6 : 6, 8 + b, 12 + b * 23); bird.localScale=Vector3.one*1.25f;
                    PrimitiveArt.Box(bird, "Left wing", new Vector3(-.3f, 0, 0), new Vector3(.7f, .06f, .16f), teal).localRotation = Quaternion.Euler(0, 0, -15);
                    PrimitiveArt.Box(bird, "Right wing", new Vector3(.3f, 0, 0), new Vector3(.7f, .06f, .16f), teal).localRotation = Quaternion.Euler(0, 0, 15);
                    birds.Add(bird);
                }
                for (int k = 0; k < (i%2 == 0 ? 2 : 0); k++)
                {
                    var kiteRoot = new GameObject("Flying Rajasthan festival kite").transform; kiteRoot.SetParent(tile.Root, false);
                    var position = new Vector3(k == 0 ? -3.9f : 3.9f, 6.6f + k, 8 + k * 28);
                    kiteRoot.localPosition = position; kiteRoot.localScale=Vector3.one*1.25f;
                    var diamond = PrimitiveArt.Shape(kiteRoot, "Colourful diamond kite", PrimitiveType.Cube, Vector3.zero, new Vector3(.75f, 1.05f, .04f), k == 0 ? yellow : teal);
                    diamond.localRotation = Quaternion.Euler(0, 0, 45);
                    for (int tail = 0; tail < 3; tail++)
                        PrimitiveArt.Box(kiteRoot, "Short decorative ribbon", new Vector3(Mathf.Sin(tail) * .12f, -.85f - tail * .23f, 0), new Vector3(.13f, .3f, .035f), tail % 2 == 0 ? coral : yellow);
                    kites.Add(new Kite { Root = kiteRoot, BasePosition = position, Phase = i * .7f + k * 2 });
                }
                foreach (var row in tile.Rows)
                {
                    for (int lane = 0; lane < 3; lane++)
                        row.Obstacles[lane] = new RajasthanObstacles.Instance(tile.Root, obstacleArt, lane == 1);
                    // Every pooled row owns a marker, including initially empty rows:
                    // recycling can turn any of them into a spaceship encounter.
                    row.LandingMarker = new GameObject("Spaceship landing warning - avoid marked lane").transform;
                    row.LandingMarker.SetParent(tile.Root, false);
                    foreach (float side in new[] { -1f, 1f })
                    {
                        PrimitiveArt.Box(row.LandingMarker, "Landing marker edge", new Vector3(side*1.25f,.04f,0), new Vector3(.07f,.025f,2.5f), yellow);
                        PrimitiveArt.Box(row.LandingMarker, "Landing marker edge", new Vector3(0,.04f,side*1.25f), new Vector3(2.5f,.025f,.07f), yellow);
                    }
                    PrimitiveArt.CombineScenery(row.LandingMarker);
                    row.LandingMarker.gameObject.SetActive(false);
                    row.Pouch=RajasthanDecor.Pouch(tile.Root); row.Garden=RajasthanDecor.RestGarden(tile.Root);
                    for (int c = 0; c < 5; c++)
                        row.Coins[c] = decor.Token(tile.Root);
                }
            }
        }
        // Inspect the actual constructed pool, not just the generation formulas.
        public void ValidatePool()
        {
            foreach (var tile in tiles) foreach (var row in tile.Rows)
            {
                if (row.LandingMarker == null || row.LandingMarker.parent != tile.Root)
                    throw new System.InvalidOperationException("Pooled row is missing its landing marker.");
                if(row.Pouch==null || row.Garden==null) throw new System.InvalidOperationException("Pooled row is missing festival props.");
                foreach (var obstacle in row.Obstacles)
                    if (obstacle == null || obstacle.Root == null)
                        throw new System.InvalidOperationException("Pooled row is missing an obstacle slot.");
                foreach (var coin in row.Coins)
                    if (coin == null) throw new System.InvalidOperationException("Pooled row is missing a coin slot.");
            }
        }
        public void Reset(int seed)
        {
            nextIndex = TileCount; WarningMessage = ""; CamelContact = false; ContactMessage = "";
            for (int i = 0; i < TileCount; i++) Configure(tiles[i], i, seed);
        }
        private static void Configure(Tile tile, int index, int seed)
        {
            tile.Index = index;
            // Reuse the same meshes with a small deterministic layout variation.
            // Ground, lanes and gameplay obstacles are never shifted or scaled.
            float lateral = (RunRules.SafeLane(index*3,seed)-1)*.12f;
            float forward = (RunRules.Kind(index*2,index%3,seed)-2.5f)*.8f;
            tile.Village.localPosition = new Vector3(lateral,0,forward);
            tile.Village.localScale = new Vector3(1,1,.96f+.016f*RunRules.Kind(index,index%3,seed));
            for (int r = 0; r < 2; r++)
            {
                var row = tile.Rows[r]; int rowNumber = index * 2 + r;
                row.Z = index * TileLength + RunRules.RowSpacing * .5f + r * RunRules.RowSpacing;
                row.Safe = RunRules.SafeLane(rowNumber, seed);
                row.HasPouch=RunRules.PouchRow(rowNumber); row.PouchTaken=row.SlideTaken=false;
                row.Pouch.gameObject.SetActive(row.HasPouch);
                row.Pouch.localPosition=new Vector3(RunRules.LaneX(row.Safe),1.1f,row.Z-index*TileLength);
                row.Garden.gameObject.SetActive(RunRules.RestRow(rowNumber));
                row.Garden.localPosition=new Vector3(0,0,row.Z-index*TileLength);
                row.CamelCrossing = RunRules.HasCamel(rowNumber, seed);
                row.Roadworks=RunRules.RoadworksRow(rowNumber) && !RunRules.NearDynamic(rowNumber);
                row.DynamicKind = RunRules.DynamicKind(rowNumber); row.HazardLane = RunRules.HazardLane(rowNumber,seed);
                row.LandingMarker.gameObject.SetActive(row.DynamicKind == RajasthanObstacles.Spaceship || row.DynamicKind == RajasthanObstacles.FallingCloth || row.DynamicKind == RajasthanObstacles.Peacock);
                row.LandingMarker.localPosition = new Vector3(RunRules.LaneX(row.HazardLane),0,row.Z-index*TileLength);
                for (int lane = 0; lane < 3; lane++)
                {
                    int kind = RunRules.ObstacleFor(rowNumber, lane, seed);
                    // A crossing replaces the entire row's other obstacles. Its reserved
                    // escape lane never intersects the camel's complete swept envelope.
                    row.Kind[lane] = kind;
                    var obstacle = row.Obstacles[lane]; obstacle.Configure(kind);
                    obstacle.Root.localPosition = new Vector3(RunRules.LaneX(lane), 0, RunRules.RowSpacing * .5f + r * RunRules.RowSpacing);
                }
                for (int c = 0; c < 5; c++)
                {
                    row.Taken[c] = false; row.Coins[c].gameObject.SetActive(true);
                    row.Coins[c].localPosition = new Vector3(RunRules.LaneX(row.Safe), 1.1f, RunRules.RowSpacing * .5f + r * RunRules.RowSpacing + (c - 2) * 2.2f);
                    row.Coins[c].localRotation = Quaternion.identity;
                }
            }
        }
        // Relative-world movement avoids unbounded runner/world coordinates.
        public void Position(float distance, int seed, float clock, bool reduced, bool magnet = false, float playerX = 0, float playerHeight = 0)
        {
            WarningMessage = ""; float nearestCamel = RunRules.CamelWarningDistance;
            SoundKind=0; SoundKey=-1; SoundAhead=100; SoundPan=0;
            Quaternion coinRotation = Quaternion.Euler(0, reduced ? 0 : Mathf.Sin(clock*1.7f)*24, reduced ? 0 : Mathf.Sin(clock*2)*5);
            foreach (var tile in tiles)
            {
                if ((tile.Index + 1) * TileLength - distance < -18) Configure(tile, nextIndex++, seed);
                tile.Root.localPosition = new Vector3(0, 0, tile.Index * TileLength - distance);
                if(tile.HasElephant) ConsiderSound(RajasthanObstacles.ElephantSound,tile.Index*TileLength+31-distance,.65f,tile.Index*2+1);
                foreach (var row in tile.Rows)
                {
                    float ahead = row.Z-distance;
                    if(row.CamelCrossing) ConsiderSound(RajasthanObstacles.Camel,ahead,RunRules.CamelX(row.Safe,ahead)/8,row.Z.GetHashCode());
                    if(row.Roadworks)
                    {
                        ConsiderSound(RajasthanObstacles.Roadworks,ahead,0,row.Z.GetHashCode());
                        if(ahead>0 && ahead<=nearestCamel)
                        {
                            nearestCamel=ahead;
                            WarningMessage=FestivalObstacles.Warning(RajasthanObstacles.Roadworks)+" - "+LaneName(row.Safe)+" lane clear";
                        }
                    }
                    if(row.DynamicKind != 0)
                    {
                        ConsiderSound(row.DynamicKind,RunRules.HazardZ(row.DynamicKind,row.Z,distance)-distance,RunRules.LaneX(row.HazardLane)/5,row.Z.GetHashCode());
                        for(int lane=0;lane<3;lane++)
                        {
                            if(row.Kind[lane]!=row.DynamicKind) continue;
                            var hazard=row.Obstacles[lane].Root;
                            hazard.localPosition=new Vector3(RunRules.HazardX(row.DynamicKind,lane,ahead),RunRules.HazardY(row.DynamicKind,ahead),RunRules.HazardZ(row.DynamicKind,row.Z,distance)-tile.Index*TileLength);
                            if(row.DynamicKind==RajasthanObstacles.Spaceship) hazard.localRotation=Quaternion.Euler(0,reduced ? 0:clock*24,0);
                            // Rolling is gameplay motion: reduced effects preserves it.
                            if(row.DynamicKind==RajasthanObstacles.Wheel) hazard.localRotation=Quaternion.Euler(-distance*1.45f/.62f*Mathf.Rad2Deg,0,0);
                        }
                        row.LandingMarker.gameObject.SetActive((row.DynamicKind==RajasthanObstacles.Spaceship || row.DynamicKind==RajasthanObstacles.FallingCloth || row.DynamicKind==RajasthanObstacles.Peacock) && ahead<48 && ahead>-5);
                        if(ahead>0 && ahead<=nearestCamel)
                        {
                            nearestCamel=ahead;
                            WarningMessage=FestivalObstacles.Warning(row.DynamicKind)+" - take "+LaneName(row.Safe)+" lane";
                        }
                    }
                    if (!row.CamelCrossing) continue;
                    var camel = row.Obstacles[1].Root;
                    camel.localPosition = new Vector3(RunRules.CamelX(row.Safe, ahead), 0, row.Z - tile.Index * TileLength);
                    row.Obstacles[1].Walk(distance);
                    camel.localRotation = Quaternion.Euler(0, row.Safe == 0 ? 180 : 0, 0);
                    if (ahead > 0 && ahead <= nearestCamel)
                    {
                        nearestCamel = ahead;
                        WarningMessage = row.Safe == 0 ? "CAMEL CROSSING - take LEFT lane" : "CAMEL CROSSING - take RIGHT lane";
                    }
                }
                // Do not update coin transforms on segments outside the playable view.
                if (tile.Root.localPosition.z < -52 || tile.Root.localPosition.z > 115) continue;
                foreach (var row in tile.Rows)
                {
                    if(row.HasPouch && !row.PouchTaken) row.Pouch.localRotation=Quaternion.Euler(0,reduced ? 0 : Mathf.Sin(clock*2)*15,0);
                    for(int c=0;c<row.Coins.Length;c++)
                    {
                        var coin=row.Coins[c]; if(!coin.gameObject.activeSelf) continue;
                        float z=row.Z-tile.Index*TileLength+(c-2)*2.2f;
                        float ahead=row.Z+(c-2)*2.2f-distance;
                        float pull=magnet && !reduced && ahead>=0 && ahead<6 && Mathf.Abs(RunRules.LaneX(row.Safe)-playerX)<3.2f ? 1-ahead/6 : 0;
                        coin.localPosition=new Vector3(Mathf.Lerp(RunRules.LaneX(row.Safe),playerX,pull),Mathf.Lerp(1.1f,playerHeight+.9f,pull),z);
                        coin.localRotation=coinRotation;
                    }
                }
            }
            Quaternion birdRotation = Quaternion.Euler(0, 0, reduced ? 0 : Mathf.Sin(clock * 4) * 12);
            foreach (var bird in birds)
                if (bird.position.z > -10 && bird.position.z < 125) bird.localRotation = birdRotation;
            foreach (var kite in kites)
            {
                if (kite.Root.position.z < -12 || kite.Root.position.z > 140) continue;
                float wind = reduced ? 0 : Mathf.Sin(clock * 1.1f + kite.Phase);
                kite.Root.localPosition = kite.BasePosition + new Vector3(wind * .4f, wind * .22f, 0);
                kite.Root.localRotation = Quaternion.Euler(0, 0, wind * 9);
            }
        }
        private static string LaneName(int lane) => lane==0 ? "LEFT" : lane==1 ? "MIDDLE":"RIGHT";
        private void ConsiderSound(int kind,float ahead,float pan,int key)
        {
            if(ahead < -10 || ahead > 52 || Mathf.Abs(ahead) >= Mathf.Abs(SoundAhead)) return;
            SoundKind=kind; SoundAhead=ahead; SoundPan=pan; SoundKey=key;
        }
        public int CollectAndCollide(Vector3 previousFoot, Vector3 foot, float playerHeight, out bool hit, bool magnet = false)
        {
            int collected = 0; PouchesCollected=SlidesCleared=0; hit = false; CamelContact = false; ContactMessage = "";
            foreach (var tile in tiles) foreach (var row in tile.Rows)
            {
                if (row.Z < previousFoot.z - 6 || row.Z > foot.z + 7) continue;
                for (int lane = 0; lane < 3; lane++)
                {
                    int kind = row.Kind[lane]; if (kind == 0) continue;
                    if (row.DynamicKind!=0 && kind==row.DynamicKind)
                    {
                        float before=row.Z-previousFoot.z, after=row.Z-foot.z;
                        var a=previousFoot-new Vector3(RunRules.HazardX(kind,lane,before),RunRules.HazardBaseY(kind,before),RunRules.HazardZ(kind,row.Z,previousFoot.z));
                        var b=foot-new Vector3(RunRules.HazardX(kind,lane,after),RunRules.HazardBaseY(kind,after),RunRules.HazardZ(kind,row.Z,foot.z));
                        var shape=FestivalObstacles.Shape(kind)*RajasthanObstacles.VisualScale(kind);
                        if(SegmentBox(a,b,new Vector3(-shape.x-RunRules.PlayerHalfWidth,-playerHeight+.02f,-shape.z-RunRules.PlayerHalfWidth),new Vector3(shape.x+RunRules.PlayerHalfWidth,shape.y,shape.z+RunRules.PlayerHalfWidth)))
                        { hit=true; ContactMessage=FestivalObstacles.Contact(kind); }
                        continue;
                    }
                    if (kind == RajasthanObstacles.Camel)
                    {
                        // Sweep in camel-relative coordinates, including motion over this
                        // frame. It cannot tunnel into the runner between render updates.
                        var a = new Vector3(previousFoot.x - RunRules.CamelX(row.Safe, row.Z - previousFoot.z), previousFoot.y, previousFoot.z - row.Z);
                        var b = new Vector3(foot.x - RunRules.CamelX(row.Safe, row.Z - foot.z), foot.y, foot.z - row.Z);
                        var min = new Vector3(-RunRules.CamelHalfWidth*RajasthanObstacles.VisualScale(kind) - RunRules.PlayerHalfWidth, -playerHeight + .02f, -RunRules.CamelHalfDepth*RajasthanObstacles.VisualScale(kind) - RunRules.PlayerHalfWidth);
                        var max = new Vector3(RunRules.CamelHalfWidth*RajasthanObstacles.VisualScale(kind) + RunRules.PlayerHalfWidth, 2.9f*RajasthanObstacles.VisualScale(kind), RunRules.CamelHalfDepth*RajasthanObstacles.VisualScale(kind) + RunRules.PlayerHalfWidth);
                        if (SegmentBox(a, b, min, max)) { hit = true; CamelContact = true; }
                        continue;
                    }
                    float x = RunRules.LaneX(lane), visualScale=RajasthanObstacles.VisualScale(kind);
                    if (kind == RajasthanObstacles.Awning || kind == RajasthanObstacles.Scaffold)
                    {
                        hit |= HitBox(previousFoot, foot, playerHeight, x, row.Z, .9f*visualScale, .75f*visualScale, 1.25f*visualScale, 2.32f*visualScale);
                        // The two visible posts are solid even during a slide; the centre
                        // opening stays clear. This avoids invisible/ghost support legs.
                        hit |= HitBox(previousFoot, foot, playerHeight, x - .82f*visualScale, row.Z, .06f*visualScale, .06f*visualScale, 0, 2.5f*visualScale);
                        hit |= HitBox(previousFoot, foot, playerHeight, x + .82f*visualScale, row.Z, .06f*visualScale, .06f*visualScale, 0, 2.5f*visualScale);
                        if(kind==RajasthanObstacles.Scaffold)
                        {
                            hit |= HitBox(previousFoot,foot,playerHeight,x-.82f,row.Z,.095f,.095f,2.475f,2.625f);
                            hit |= HitBox(previousFoot,foot,playerHeight,x+.82f,row.Z,.095f,.095f,2.475f,2.625f);
                        }
                    }
                    else if(kind==RajasthanObstacles.Roadworks)
                    {
                        var shape=FestivalObstacles.Shape(kind);
                        hit |= HitBox(previousFoot,foot,playerHeight,x,row.Z,shape.x,shape.z,0,shape.y);
                    }
                    else
                    {
                        float halfWidth = kind == RajasthanObstacles.Cart ? 1.05f : kind == RajasthanObstacles.Fabric ? .83f : .75f;
                        float halfDepth = kind == RajasthanObstacles.Cart ? .85f : .55f;
                        float top = kind == RajasthanObstacles.Cart ? 2.15f : .95f;
                        hit |= HitBox(previousFoot, foot, playerHeight, x, row.Z, halfWidth*visualScale, halfDepth*visualScale, 0, top*visualScale);
                    }
                }
                if(!hit && playerHeight<1 && !row.SlideTaken && previousFoot.z<row.Z && foot.z>=row.Z)
                {
                    for(int lane=0;lane<3;lane++)
                        if((row.Kind[lane]==RajasthanObstacles.Awning || row.Kind[lane]==RajasthanObstacles.Scaffold) && Mathf.Abs(foot.x-RunRules.LaneX(lane))<.40f)
                        { row.SlideTaken=true; SlidesCleared++; break; }
                }
                if(row.HasPouch && !row.PouchTaken && SegmentBox(previousFoot,foot,
                    new Vector3(RunRules.LaneX(row.Safe)-.75f,-.4f,row.Z-.6f),new Vector3(RunRules.LaneX(row.Safe)+.75f,1.8f,row.Z+.6f)))
                { row.PouchTaken=true; row.Pouch.gameObject.SetActive(false); PouchesCollected++; }
                for (int c = 0; c < 5; c++)
                {
                    if (row.Taken[c]) continue;
                    float z = row.Z + (c - 2) * 2.2f;
                    var min = new Vector3(RunRules.LaneX(row.Safe) - (magnet ? 3.2f : .7f), -.5f, z - .5f);
                    var max = new Vector3(RunRules.LaneX(row.Safe) + (magnet ? 3.2f : .7f), magnet ? 3f : 1.6f, z + .5f);
                    if (!SegmentBox(previousFoot, foot, min, max)) continue;
                    row.Taken[c] = true; row.Coins[c].gameObject.SetActive(false); collected++;
                }
            }
            return collected;
        }
        private static bool HitBox(Vector3 previous, Vector3 foot, float playerHeight, float x, float z, float halfWidth, float halfDepth, float bottom, float top)
        {
            return SegmentBox(previous, foot,
                new Vector3(x - halfWidth - RunRules.PlayerHalfWidth, bottom - playerHeight + .02f, z - halfDepth - RunRules.PlayerHalfWidth),
                new Vector3(x + halfWidth + RunRules.PlayerHalfWidth, top - .02f, z + halfDepth + RunRules.PlayerHalfWidth));
        }
        public void SetWeather(float cloudiness) => scenery.SetWeather(cloudiness);
        public void Dispose() => scenery.Dispose();
        public static bool SegmentBox(Vector3 a, Vector3 b, Vector3 min, Vector3 max)
        {
            float near = 0, far = 1;
            for (int axis = 0; axis < 3; axis++)
            {
                float delta = b[axis] - a[axis];
                if (Mathf.Abs(delta) < .00001f) { if (a[axis] < min[axis] || a[axis] > max[axis]) return false; }
                else
                {
                    float t0 = (min[axis] - a[axis]) / delta, t1 = (max[axis] - a[axis]) / delta;
                    near = Mathf.Max(near, Mathf.Min(t0, t1)); far = Mathf.Min(far, Mathf.Max(t0, t1));
                    if (near > far) return false;
                }
            }
            return true;
        }
    }
}
