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
            public readonly Transform[] Obstacles = new Transform[3];
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
        private int nextIndex;
        private readonly RajasthanScenery scenery;
        public TrackWorld(Transform parent)
        {
            scenery = new RajasthanScenery(parent);
            yellow = PrimitiveArt.Mat(new Color(1f, .75f, .05f));
            coral = PrimitiveArt.Mat(new Color(.91f, .23f, .14f));
            teal = PrimitiveArt.Mat(new Color(.04f, .43f, .50f));
            var sand = PrimitiveArt.Mat(new Color(.94f, .67f, .34f));
            var road = PrimitiveArt.Mat(new Color(.29f, .25f, .30f));
            var white = PrimitiveArt.Mat(new Color(1f, .9f, .66f));
            var pink = PrimitiveArt.Mat(new Color(.94f, .40f, .42f));
            var camel = PrimitiveArt.Mat(new Color(.66f, .40f, .19f));
            var blue = PrimitiveArt.Mat(new Color(.03f, .27f, .8f));
            var green = PrimitiveArt.Mat(new Color(.08f, .56f, .28f));
            for (int i = 0; i < TileCount; i++)
            {
                var tile = new Tile { Root = new GameObject("Pooled Rajasthan segment " + i).transform };
                tile.Root.SetParent(parent, false); tiles[i] = tile;
                PrimitiveArt.Box(tile.Root, "Sand", new Vector3(0, -.18f, TileLength / 2), new Vector3(85, .25f, TileLength), sand);
                PrimitiveArt.Box(tile.Root, "Road", new Vector3(0, -.045f, TileLength / 2), new Vector3(8.5f, .12f, TileLength), road);
                foreach (float x in new[] { -4.45f, 4.45f })
                    PrimitiveArt.Box(tile.Root, "Road edge", new Vector3(x, .04f, TileLength / 2), new Vector3(.2f, .1f, TileLength), white);
                for (int dash = 0; dash < 8; dash++) foreach (float x in new[] { -RunRules.LaneWidth / 2, RunRules.LaneWidth / 2 })
                    PrimitiveArt.Box(tile.Root, "Lane dash", new Vector3(x, .025f, dash * 6.5f + 2), new Vector3(.11f, .03f, 2.4f), white);
                for (int house = 0; house < 4; house++)
                {
                    float side = house % 2 == 0 ? -1 : 1, z = house * 12 + 7;
                    float x = side * (9 + (i + house) % 3);
                    var walls = (i + house) % 2 == 0 ? pink : sand;
                    float h = 3 + (i + house) % 3;
                    PrimitiveArt.Box(tile.Root, "Sandstone village", new Vector3(x, h / 2, z), new Vector3(4.3f, h, 6), walls);
                    PrimitiveArt.Box(tile.Root, "Flat roof", new Vector3(x, h + .1f, z), new Vector3(4.7f, .25f, 6.4f), white);
                    PrimitiveArt.Box(tile.Root, "Door", new Vector3(x - side * 2.16f, 1, z), new Vector3(.04f, 2, 1), teal);
                    PrimitiveArt.Box(tile.Root, "Colourful market awning", new Vector3(side * 6.8f, 2.3f, z + 2), new Vector3(2.5f, .13f, 3), (house % 2 == 0 ? yellow : teal));
                    foreach (float dz in new[] { .8f, 3.2f })
                        PrimitiveArt.Box(tile.Root, "Market post", new Vector3(side * 5.7f, 1.1f, z + dz), new Vector3(.1f, 2.2f, .1f), camel);
                }
                PrimitiveArt.Camel(tile.Root, -6.4f, 22, camel);
                PrimitiveArt.Peacock(tile.Root, 5.9f, 41, blue, green);
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
                foreach (var row in tile.Rows)
                {
                    for (int lane = 0; lane < 3; lane++)
                        row.Obstacles[lane] = PrimitiveArt.Box(tile.Root, "Obstacle", Vector3.zero, Vector3.one, coral);
                    for (int c = 0; c < 5; c++)
                        row.Coins[c] = PrimitiveArt.Shape(tile.Root, "Coin", PrimitiveType.Cylinder, Vector3.zero, new Vector3(.48f, .07f, .48f), yellow);
                }
            }
        }
        public void Reset(int seed)
        {
            nextIndex = TileCount;
            for (int i = 0; i < TileCount; i++) Configure(tiles[i], i, seed);
        }
        private static void Configure(Tile tile, int index, int seed)
        {
            tile.Index = index;
            for (int r = 0; r < 2; r++)
            {
                var row = tile.Rows[r]; int rowNumber = index * 2 + r;
                row.Z = index * TileLength + 13 + r * RunRules.RowSpacing;
                row.Safe = RunRules.SafeLane(rowNumber, seed);
                for (int lane = 0; lane < 3; lane++)
                {
                    int kind = rowNumber < 2 || lane == row.Safe ? 0 : RunRules.Kind(rowNumber, lane, seed);
                    row.Kind[lane] = kind; var o = row.Obstacles[lane];
                    o.gameObject.SetActive(kind != 0);
                    float height = kind == 1 ? .95f : kind == 2 ? 1.5f : 3.2f;
                    float bottom = kind == 2 ? 1.25f : 0;
                    o.localPosition = new Vector3(RunRules.LaneX(lane), bottom + height / 2, 13 + r * RunRules.RowSpacing);
                    o.localScale = new Vector3(1.8f, height, 1.2f);
                    o.GetComponent<Renderer>().sharedMaterial = kind == 1
                        ? PrimitiveArt.Mat(new Color(1f, .75f, .05f))
                        : kind == 2 ? PrimitiveArt.Mat(new Color(.04f, .43f, .50f))
                        : PrimitiveArt.Mat(new Color(.91f, .23f, .14f));
                }
                for (int c = 0; c < 5; c++)
                {
                    row.Taken[c] = false; row.Coins[c].gameObject.SetActive(true);
                    row.Coins[c].localPosition = new Vector3(RunRules.LaneX(row.Safe), 1.1f, 13 + r * RunRules.RowSpacing + (c - 2) * 2.2f);
                    row.Coins[c].localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }
        // Relative-world movement avoids unbounded runner/world coordinates.
        public void Position(float distance, int seed, float clock, bool reduced)
        {
            Quaternion coinRotation = Quaternion.Euler(90, reduced ? 0 : clock * 100, 0);
            foreach (var tile in tiles)
            {
                if ((tile.Index + 1) * TileLength - distance < -18) Configure(tile, nextIndex++, seed);
                tile.Root.localPosition = new Vector3(0, 0, tile.Index * TileLength - distance);
                // Do not update coin transforms on segments outside the playable view.
                if (tile.Root.localPosition.z < -52 || tile.Root.localPosition.z > 115) continue;
                foreach (var row in tile.Rows) foreach (var coin in row.Coins)
                    if (coin.gameObject.activeSelf) coin.localRotation = coinRotation;
            }
            Quaternion birdRotation = Quaternion.Euler(0, 0, reduced ? 0 : Mathf.Sin(clock * 4) * 12);
            foreach (var bird in birds)
                if (bird.position.z > -10 && bird.position.z < 125) bird.localRotation = birdRotation;
        }
        public int CollectAndCollide(Vector3 previousFoot, Vector3 foot, float playerHeight, out bool hit)
        {
            int collected = 0; hit = false;
            foreach (var tile in tiles) foreach (var row in tile.Rows)
            {
                if (row.Z < previousFoot.z - 6 || row.Z > foot.z + 7) continue;
                for (int lane = 0; lane < 3; lane++)
                {
                    int kind = row.Kind[lane]; if (kind == 0) continue;
                    float bottom = kind == 2 ? 1.25f : 0;
                    float top = kind == 1 ? .95f : kind == 2 ? 2.75f : 3.2f;
                    var min = new Vector3(RunRules.LaneX(lane) - 1.24f, bottom - playerHeight + .02f, row.Z - .94f);
                    var max = new Vector3(RunRules.LaneX(lane) + 1.24f, top - .02f, row.Z + .94f);
                    if (SegmentBox(previousFoot, foot, min, max)) hit = true;
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
