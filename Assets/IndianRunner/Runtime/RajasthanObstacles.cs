using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Shared geometry is built once. Every pooled slot reuses these meshes/materials.
    public sealed class RajasthanObstacles
    {
        public const int Matkas = 1, Fabric = 2, Cart = 3, Awning = 4, Camel = 5;
        private sealed class Part
        {
            public Mesh Mesh;
            public Material Material;
        }
        private readonly Part[][] models = new Part[6][];
        public int MaxParts { get; private set; }

        public RajasthanObstacles()
        {
            for (int kind = 1; kind <= 5; kind++)
            {
                var root = new GameObject("Shared Rajasthan obstacle " + kind).transform;
                Build(root, kind);
                PrimitiveArt.CombineScenery(root);
                var parts = new List<Part>();
                foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>())
                {
                    // CombineScenery disables original meshes until Destroy executes.
                    if (!renderer.enabled) continue;
                    parts.Add(new Part { Mesh = renderer.GetComponent<MeshFilter>().sharedMesh, Material = renderer.sharedMaterial });
                }
                models[kind] = parts.ToArray();
                MaxParts = Mathf.Max(MaxParts, parts.Count);
                Object.Destroy(root.gameObject);
            }
        }
        public sealed class Instance
        {
            public Transform Root { get; }
            private readonly MeshFilter[] filters;
            private readonly MeshRenderer[] renderers;
            private readonly RajasthanObstacles catalog;
            public Instance(Transform parent, RajasthanObstacles source)
            {
                catalog = source;
                Root = new GameObject("Pooled themed obstacle").transform; Root.SetParent(parent, false);
                filters = new MeshFilter[source.MaxParts]; renderers = new MeshRenderer[source.MaxParts];
                for (int i = 0; i < filters.Length; i++)
                {
                    var mesh = new GameObject("Shared artwork part"); mesh.transform.SetParent(Root, false);
                    filters[i] = mesh.AddComponent<MeshFilter>(); renderers[i] = mesh.AddComponent<MeshRenderer>();
                    renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderers[i].receiveShadows = false;
                }
            }
            public void Configure(int kind)
            {
                Root.gameObject.SetActive(kind != 0); Root.localScale = Vector3.one;
                Root.localRotation = Quaternion.identity;
                if (kind == 0) return;
                var parts = catalog.models[kind];
                for (int i = 0; i < filters.Length; i++)
                {
                    bool active = i < parts.Length; renderers[i].enabled = active;
                    if (!active) continue;
                    filters[i].sharedMesh = parts[i].Mesh; renderers[i].sharedMaterial = parts[i].Material;
                }
                Root.name = kind == Matkas ? "Matka pottery stack - jump" : kind == Fabric ? "Fabric bundles - jump" : kind == Cart ? "Wooden bazaar cart - dodge" : kind == Awning ? "Market awning - slide" : "Decorated camel crossing - dodge";
            }
        }
        private static void Build(Transform root, int kind)
        {
            var clay = PrimitiveArt.Mat(new Color(.73f, .26f, .12f));
            var gold = PrimitiveArt.Mat(new Color(1, .71f, .13f));
            var teal = PrimitiveArt.Mat(new Color(.02f, .60f, .61f));
            var magenta = PrimitiveArt.Mat(new Color(.85f, .12f, .43f));
            var wood = PrimitiveArt.Mat(new Color(.45f, .23f, .10f));
            var dark = PrimitiveArt.Mat(new Color(.17f, .12f, .09f));
            var hide = PrimitiveArt.Mat(new Color(.66f, .40f, .19f));
            if (kind == Matkas)
            {
                Pot(root, new Vector3(-.43f, 0, -.1f), .67f, clay, gold);
                Pot(root, new Vector3(.38f, 0, .15f), .82f, clay, gold);
                Pot(root, new Vector3(0, .45f, -.15f), .47f, clay, gold);
            }
            else if (kind == Fabric)
            {
                PrimitiveArt.Box(root, "Low cloth pallet", new Vector3(0, .09f, 0), new Vector3(1.65f, .18f, 1.1f), wood);
                for (int layer = 0; layer < 3; layer++)
                {
                    var mat = layer == 0 ? magenta : layer == 1 ? gold : teal;
                    PrimitiveArt.Box(root, "Folded woven fabric", new Vector3(layer % 2 == 0 ? -.06f : .08f, .27f + layer * .24f, 0), new Vector3(1.55f - layer * .1f, .23f, 1), mat);
                    PrimitiveArt.Box(root, "Gold cloth border", new Vector3(0, .28f + layer * .24f, -.51f), new Vector3(1.45f - layer * .1f, .06f, .035f), gold);
                }
            }
            else if (kind == Cart)
            {
                PrimitiveArt.Box(root, "Wooden cart bed", new Vector3(0, .68f, 0), new Vector3(1.6f, .2f, 1.7f), wood);
                foreach (float x in new[] { -.76f, .76f })
                {
                    PrimitiveArt.Box(root, "Cart side rail", new Vector3(x, 1.05f, 0), new Vector3(.1f, .66f, 1.65f), wood);
                    foreach (float z in new[] { -.55f, .55f })
                    {
                        var wheel = PrimitiveArt.Shape(root, "Wooden cart wheel", PrimitiveType.Cylinder, new Vector3(x * 1.12f, .42f, z), new Vector3(.68f, .075f, .68f), dark);
                        wheel.localRotation = Quaternion.Euler(0, 0, 90);
                        PrimitiveArt.Shape(root, "Wheel hub", PrimitiveType.Sphere, new Vector3(x * 1.24f, .42f, z), Vector3.one * .19f, gold);
                    }
                }
                foreach (float z in new[] { -.78f, .78f }) PrimitiveArt.Box(root, "Cart end rail", new Vector3(0, 1.03f, z), new Vector3(1.5f, .6f, .1f), wood);
                PrimitiveArt.Box(root, "Textile cargo", new Vector3(-.3f, 1.25f, 0), new Vector3(.65f, .68f, 1.15f), teal);
                PrimitiveArt.Box(root, "Colourful cargo", new Vector3(.35f, 1.43f, 0), new Vector3(.6f, 1.05f, 1.1f), magenta);
                Pot(root, new Vector3(-.25f, 1.6f, .2f), .55f, clay, gold);
            }
            else if (kind == Awning)
            {
                foreach (float x in new[] { -.82f, .82f })
                    PrimitiveArt.Box(root, "Awning support", new Vector3(x, 1.25f, 0), new Vector3(.12f, 2.5f, .12f), wood);
                PrimitiveArt.Box(root, "Bazaar cloth canopy", new Vector3(0, 2.22f, 0), new Vector3(1.8f, .16f, 1.5f), teal);
                PrimitiveArt.Box(root, "Low hanging fabric - slide beneath", new Vector3(0, 1.61f, -.55f), new Vector3(1.8f, .72f, .14f), gold);
                for (int stripe = 0; stripe < 5; stripe++)
                    PrimitiveArt.Box(root, "Fabric canopy stripe", new Vector3(-.72f + stripe * .36f, 2.31f, 0), new Vector3(.16f, .035f, 1.5f), gold);
            }
            else
            {
                // Side-on camel, with its head facing +X. The broad collision envelope
                // stays inside the two blocked lanes; the head never enters the safe lane.
                PrimitiveArt.Box(root, "Camel body", new Vector3(0, 1.35f, 0), new Vector3(1.6f, .7f, .85f), hide);
                PrimitiveArt.Shape(root, "Camel hump", PrimitiveType.Sphere, new Vector3(-.1f, 1.83f, 0), new Vector3(.8f, .8f, .75f), hide);
                PrimitiveArt.Box(root, "Camel neck", new Vector3(.80f, 1.93f, 0), new Vector3(.35f, 1.3f, .32f), hide);
                PrimitiveArt.Box(root, "Camel head", new Vector3(1.02f, 2.57f, 0), new Vector3(.61f, .34f, .39f), hide);
                foreach (float x in new[] { -.5f, .5f }) foreach (float z in new[] { -.29f, .29f })
                    PrimitiveArt.Box(root, "Camel leg", new Vector3(x, .56f, z), new Vector3(.17f, 1.1f, .16f), hide);
                PrimitiveArt.Box(root, "Decorated saddle cloth", new Vector3(-.2f, 1.57f, 0), new Vector3(.95f, .15f, .98f), teal);
                foreach (float z in new[] { -.51f, .51f })
                {
                    PrimitiveArt.Box(root, "Saddle cloth tassels", new Vector3(-.2f, 1.35f, z), new Vector3(.95f, .33f, .05f), magenta);
                    for (int tassel = 0; tassel < 4; tassel++)
                        PrimitiveArt.Shape(root, "Gold tassel", PrimitiveType.Sphere, new Vector3(-.55f + tassel * .23f, 1.12f, z), Vector3.one * .12f, gold);
                    PrimitiveArt.Shape(root, "Camel eye", PrimitiveType.Sphere, new Vector3(1.11f, 2.63f, z * .4f), Vector3.one * .065f, dark);
                }
            }
        }
        private static void Pot(Transform root, Vector3 basePosition, float size, Material clay, Material gold)
        {
            PrimitiveArt.Shape(root, "Round matka", PrimitiveType.Sphere, basePosition + new Vector3(0, size * .38f, 0), new Vector3(size * .72f, size * .76f, size * .72f), clay);
            PrimitiveArt.Shape(root, "Matka neck", PrimitiveType.Cylinder, basePosition + new Vector3(0, size * .79f, 0), new Vector3(size * .33f, size * .10f, size * .33f), clay);
            PrimitiveArt.Shape(root, "Painted matka rim", PrimitiveType.Cylinder, basePosition + new Vector3(0, size * .90f, 0), new Vector3(size * .44f, size * .035f, size * .44f), gold);
        }
    }
}
