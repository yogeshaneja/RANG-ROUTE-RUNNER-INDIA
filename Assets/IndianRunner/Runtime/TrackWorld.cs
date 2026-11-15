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
            public bool CamelCrossing;
            public readonly Transform[] Coins = new Transform[5];
            public readonly bool[] Taken = new bool[5];
            public int Safe;
        }
        private sealed class Tile
        {
            public Transform Root;
            public int Index;
            public readonly Row[] Rows = { new Row(), new Row() };
        }
        private readonly Tile[] tiles = new Tile[TileCount];
        private readonly List<Transform> birds = new List<Transform>();
        private readonly Material yellow, coral, teal;
        private readonly RajasthanObstacles obstacleArt;
        private sealed class Kite
        {
            public Transform Root;
            public Vector3 BasePosition;
            public float Phase;
        }
        private readonly List<Kite> kites = new List<Kite>();
        public string WarningMessage { get; private set; } = "";
        public bool CamelContact { get; private set; }
        private int nextIndex;
        private readonly RajasthanScenery scenery;
        public TrackWorld(Transform parent)
        {
            scenery = new RajasthanScenery(parent);
            obstacleArt = new RajasthanObstacles();
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
                tile.Root.SetParent(parent, false); tiles[i] = tile;
                scenery.AddGround(tile.Root,i,TileLength);
                PrimitiveArt.Box(tile.Root, "Road", new Vector3(0, -.045f, TileLength / 2), new Vector3(8.5f, .12f, TileLength), road);
                for (int dash = 0; dash < 8; dash++) foreach (float x in new[] { -RunRules.LaneWidth / 2, RunRules.LaneWidth / 2 })
                    PrimitiveArt.Box(tile.Root, "Lane dash", new Vector3(x, .025f, dash * 6.5f + 2), new Vector3(.11f, .03f, 2.4f), white);
                if (i%3 == 1) PrimitiveArt.Camel(tile.Root,-7.7f,17+i*2,camel,RajasthanScenery.GroundY(i,-7.7f,17+i*2,TileLength));
                if (i%3 == 2) PrimitiveArt.Peacock(tile.Root,6.3f,29+i,blue,green,RajasthanScenery.GroundY(i,6.3f,29+i,TileLength));
                scenery.AddGeometry(tile.Root, i);
                PrimitiveArt.CombineScenery(tile.Root);
                scenery.AddPanels(tile.Root, i);
                for (int b = 0; b < 2; b++)
                {
                    var bird = new GameObject("Scenic bird").transform; bird.SetParent(tile.Root, false);
                    bird.localPosition = new Vector3(b == 0 ? -6 : 6, 8 + b, 12 + b * 23);
                    PrimitiveArt.Box(bird, "Left wing", new Vector3(-.3f, 0, 0), new Vector3(.7f, .06f, .16f), teal).localRotation = Quaternion.Euler(0, 0, -15);
                    PrimitiveArt.Box(bird, "Right wing", new Vector3(.3f, 0, 0), new Vector3(.7f, .06f, .16f), teal).localRotation = Quaternion.Euler(0, 0, 15);
                    birds.Add(bird);
                }
                for (int k = 0; k < (i%2 == 0 ? 2 : 0); k++)
                {
                    var kiteRoot = new GameObject("Flying Rajasthan festival kite").transform; kiteRoot.SetParent(tile.Root, false);
                    var position = new Vector3(k == 0 ? -3.9f : 3.9f, 6.6f + k, 8 + k * 28);
                    kiteRoot.localPosition = position;
                    var diamond = PrimitiveArt.Shape(kiteRoot, "Colourful diamond kite", PrimitiveType.Cube, Vector3.zero, new Vector3(.75f, 1.05f, .04f), k == 0 ? yellow : teal);
                    diamond.localRotation = Quaternion.Euler(0, 0, 45);
                    for (int tail = 0; tail < 3; tail++)
                        PrimitiveArt.Box(kiteRoot, "Short decorative ribbon", new Vector3(Mathf.Sin(tail) * .12f, -.85f - tail * .23f, 0), new Vector3(.13f, .3f, .035f), tail % 2 == 0 ? coral : yellow);
                    kites.Add(new Kite { Root = kiteRoot, BasePosition = position, Phase = i * .7f + k * 2 });
                }
                foreach (var row in tile.Rows)
                {
                    for (int lane = 0; lane < 3; lane++)
                        row.Obstacles[lane] = new RajasthanObstacles.Instance(tile.Root, obstacleArt);
                    for (int c = 0; c < 5; c++)
                        row.Coins[c] = PrimitiveArt.Shape(tile.Root, "Coin", PrimitiveType.Cylinder, Vector3.zero, new Vector3(.48f, .07f, .48f), yellow);
                }
            }
        }
        public void Reset(int seed)
        {
            nextIndex = TileCount; WarningMessage = ""; CamelContact = false;
            for (int i = 0; i < TileCount; i++) Configure(tiles[i], i, seed);
        }
        private static void Configure(Tile tile, int index, int seed)
        {
            tile.Index = index;
            for (int r = 0; r < 2; r++)
            {
                var row = tile.Rows[r]; int rowNumber = index * 2 + r;
                row.Z = index * TileLength + RunRules.RowSpacing * .5f + r * RunRules.RowSpacing;
                row.Safe = RunRules.SafeLane(rowNumber, seed);
                row.CamelCrossing = RunRules.HasCamel(rowNumber, seed);
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
                    row.Coins[c].localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }
        // Relative-world movement avoids unbounded runner/world coordinates.
        public void Position(float distance, int seed, float clock, bool reduced)
        {
            WarningMessage = ""; float nearestCamel = RunRules.CamelWarningDistance;
            Quaternion coinRotation = Quaternion.Euler(90, reduced ? 0 : clock * 100, 0);
            foreach (var tile in tiles)
            {
                if ((tile.Index + 1) * TileLength - distance < -18) Configure(tile, nextIndex++, seed);
                tile.Root.localPosition = new Vector3(0, 0, tile.Index * TileLength - distance);
                foreach (var row in tile.Rows)
                {
                    if (!row.CamelCrossing) continue;
                    float ahead = row.Z - distance;
                    var camel = row.Obstacles[1].Root;
                    camel.localPosition = new Vector3(RunRules.CamelX(row.Safe, ahead), 0, row.Z - tile.Index * TileLength);
                    camel.localRotation = Quaternion.Euler(0, row.Safe == 0 ? 180 : 0, 0);
                    if (ahead > 0 && ahead <= nearestCamel)
                    {
                        nearestCamel = ahead;
                        WarningMessage = row.Safe == 0 ? "CAMEL CROSSING - take LEFT lane" : "CAMEL CROSSING - take RIGHT lane";
                    }
                }
                // Do not update coin transforms on segments outside the playable view.
                if (tile.Root.localPosition.z < -52 || tile.Root.localPosition.z > 115) continue;
                foreach (var row in tile.Rows) foreach (var coin in row.Coins)
                    if (coin.gameObject.activeSelf) coin.localRotation = coinRotation;
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
        public int CollectAndCollide(Vector3 previousFoot, Vector3 foot, float playerHeight, out bool hit)
        {
            int collected = 0; hit = false; CamelContact = false;
            foreach (var tile in tiles) foreach (var row in tile.Rows)
            {
                if (row.Z < previousFoot.z - 6 || row.Z > foot.z + 7) continue;
                for (int lane = 0; lane < 3; lane++)
                {
                    int kind = row.Kind[lane]; if (kind == 0) continue;
                    if (kind == RajasthanObstacles.Camel)
                    {
                        // Sweep in camel-relative coordinates, including motion over this
                        // frame. It cannot tunnel into the runner between render updates.
                        var a = new Vector3(previousFoot.x - RunRules.CamelX(row.Safe, row.Z - previousFoot.z), previousFoot.y, previousFoot.z - row.Z);
                        var b = new Vector3(foot.x - RunRules.CamelX(row.Safe, row.Z - foot.z), foot.y, foot.z - row.Z);
                        var min = new Vector3(-RunRules.CamelHalfWidth - RunRules.PlayerHalfWidth, -playerHeight + .02f, -RunRules.CamelHalfDepth - RunRules.PlayerHalfWidth);
                        var max = new Vector3(RunRules.CamelHalfWidth + RunRules.PlayerHalfWidth, 2.8f, RunRules.CamelHalfDepth + RunRules.PlayerHalfWidth);
                        if (SegmentBox(a, b, min, max)) { hit = true; CamelContact = true; }
                        continue;
                    }
                    float x = RunRules.LaneX(lane);
                    if (kind == RajasthanObstacles.Awning)
                    {
                        hit |= HitBox(previousFoot, foot, playerHeight, x, row.Z, .9f, .75f, 1.25f, 2.32f);
                        // The two visible posts are solid even during a slide; the centre
                        // opening stays clear. This avoids invisible/ghost support legs.
                        hit |= HitBox(previousFoot, foot, playerHeight, x - .82f, row.Z, .06f, .06f, 0, 2.5f);
                        hit |= HitBox(previousFoot, foot, playerHeight, x + .82f, row.Z, .06f, .06f, 0, 2.5f);
                    }
                    else
                    {
                        float halfWidth = kind == RajasthanObstacles.Cart ? 1.05f : kind == RajasthanObstacles.Fabric ? .83f : .75f;
                        float halfDepth = kind == RajasthanObstacles.Cart ? .85f : .55f;
                        float top = kind == RajasthanObstacles.Cart ? 2.15f : .95f;
                        hit |= HitBox(previousFoot, foot, playerHeight, x, row.Z, halfWidth, halfDepth, 0, top);
                    }
                }
                for (int c = 0; c < 5; c++)
                {
                    if (row.Taken[c]) continue;
                    float z = row.Z + (c - 2) * 2.2f;
                    var min = new Vector3(RunRules.LaneX(row.Safe) - .7f, -.5f, z - .5f);
                    var max = new Vector3(RunRules.LaneX(row.Safe) + .7f, 1.6f, z + .5f);
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
