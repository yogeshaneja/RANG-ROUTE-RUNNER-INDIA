using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Shared geometry is built once. Every pooled slot reuses these meshes/materials.
    public sealed class RajasthanObstacles
    {
        public const int Matkas = 1, Fabric = 2, Cart = 3, Awning = 4, Camel = 5, Motorcycle = 6, Spaceship = 7, Wheel = 8, Procession = 9, FallingCloth = 10, Roadworks = 11, Peacock = 12, Scaffold = 13, UmbrellaLady = 14;
        public const int MaxKind=14, ElephantSound=100;
        public static float VisualScale(int kind) => kind == Camel ? 1.12f : kind == Motorcycle ? 1.22f : kind == Spaceship ? 1.30f : kind == Awning ? 1.05f : kind == 0 || kind >= Wheel ? 1 : 1.12f;
        private sealed class Part
        {
            public Mesh Mesh;
            public Material Material;
        }
        private readonly Part[][] models = new Part[MaxKind+1][];
        private Part[] camelLeg;
        public int MaxParts { get; private set; }

        public RajasthanObstacles()
        {
            var legRoot = new GameObject("Shared sculpted camel leg").transform;
            var legHide = PrimitiveArt.Mat(new Color(.66f,.40f,.19f));
            NaturalMesh.Lathe(legRoot,"Tapered camel leg",new[] {new Vector3(.07f,-1.05f,.065f),new Vector3(.055f,-.65f,.055f),new Vector3(.085f,-.48f,.08f),new Vector3(.10f,0,.09f)},legHide,10);
            NaturalMesh.Oval(legRoot,"Soft camel foot",new Vector3(.03f,-1.04f,0),new Vector3(.25f,.12f,.20f),PrimitiveArt.Mat(new Color(.29f,.19f,.12f)),10);
            PrimitiveArt.CombineScenery(legRoot,true);
            var legParts = new List<Part>();
            foreach(var r in legRoot.GetComponentsInChildren<MeshRenderer>()) if(r.enabled)
                legParts.Add(new Part {Mesh=r.GetComponent<MeshFilter>().sharedMesh,Material=r.sharedMaterial});
            camelLeg = legParts.ToArray(); Object.Destroy(legRoot.gameObject);
            for (int kind = 1; kind <= MaxKind; kind++)
            {
                var root = new GameObject("Shared Rajasthan obstacle " + kind).transform;
                Build(root, kind);
                PrimitiveArt.CombineScenery(root,true);
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
            private readonly Transform[] legs;
            public Instance(Transform parent, RajasthanObstacles source, bool camelSlot = false)
            {
                catalog = source;
                Root = new GameObject("Pooled themed obstacle").transform; Root.SetParent(parent, false);
                if (camelSlot)
                {
                    legs = new Transform[4];
                    for(int l=0;l<4;l++)
                    {
                        legs[l]=new GameObject("Walking camel leg").transform; legs[l].SetParent(Root,false);
                        foreach(var part in source.camelLeg)
                        {
                            var go=new GameObject("Shared leg mesh"); go.transform.SetParent(legs[l],false);
                            go.AddComponent<MeshFilter>().sharedMesh=part.Mesh;
                            var renderer=go.AddComponent<MeshRenderer>(); renderer.sharedMaterial=part.Material;
                            renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off; renderer.receiveShadows=false;
                        }
                    }
                }
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
                Root.gameObject.SetActive(kind != 0); Root.localScale = Vector3.one*VisualScale(kind);
                Root.localRotation = Quaternion.identity;
                if(legs != null) foreach(var leg in legs) leg.gameObject.SetActive(kind == Camel);
                if (kind == 0) return;
                var parts = catalog.models[kind];
                for (int i = 0; i < filters.Length; i++)
                {
                    bool active = i < parts.Length; renderers[i].enabled = active;
                    if (!active) continue;
                    filters[i].sharedMesh = parts[i].Mesh; renderers[i].sharedMaterial = parts[i].Material;
                    filters[i].transform.localPosition = kind==Wheel ? new Vector3(0,-.62f,0) : Vector3.zero;
                }
                Root.name = kind == Matkas ? "Matka pottery stack - jump" : kind == Fabric ? "Fabric bundles - jump" : kind == Cart ? "Wooden bazaar cart - dodge" : kind == Awning ? "Market awning - slide" : kind == Camel ? "Decorated camel crossing - dodge" : kind == Motorcycle ? "Oncoming motorcycle - dodge" : kind==Spaceship ? "Descending spaceship - dodge" : FestivalObstacles.Warning(kind);
            }
            public void Walk(float distance)
            {
                if(legs == null) return;
                for(int i=0;i<4;i++)
                {
                    legs[i].localPosition=new Vector3(i<2 ? -.52f : .52f,1.10f,i%2==0 ? -.29f : .29f);
                    legs[i].localRotation=Quaternion.Euler(0,0,Mathf.Sin(distance*.45f+i%2*Mathf.PI+(i<2 ? .5f : 0))*13);
                }
            }
        }
        private static void Build(Transform root, int kind)
        {
            if(kind>=Wheel) { FestivalObstacles.Build(root,kind); return; }
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
            else if (kind == Motorcycle)
            {
                var metal=PrimitiveArt.Mat(new Color(.48f,.53f,.56f));
                var paint=PrimitiveArt.Mat(new Color(.76f,.08f,.08f));
                foreach(float z in new[]{-.72f,.72f})
                {
                    var wheel=PrimitiveArt.Shape(root,"Rubber motorcycle tyre",PrimitiveType.Cylinder,new Vector3(0,.36f,z),new Vector3(.68f,.10f,.68f),dark);
                    wheel.localRotation=Quaternion.Euler(0,0,90);
                    var hub=PrimitiveArt.Shape(root,"Metal wheel hub",PrimitiveType.Cylinder,new Vector3(0,.36f,z),new Vector3(.40f,.105f,.40f),metal);
                    hub.localRotation=Quaternion.Euler(0,0,90);
                }
                NaturalMesh.Branch(root,"Frame",new Vector3(0,.40f,.65f),new Vector3(0,.85f,-.5f),.08f,metal);
                NaturalMesh.Branch(root,"Front fork",new Vector3(0,.35f,-.72f),new Vector3(0,1.25f,-.52f),.06f,metal);
                NaturalMesh.Oval(root,"Red fuel tank",new Vector3(0,.92f,-.16f),new Vector3(.52f,.40f,.66f),paint,18);
                NaturalMesh.Oval(root,"Saddle",new Vector3(0,.98f,.39f),new Vector3(.44f,.18f,.62f),dark);
                PrimitiveArt.Box(root,"Engine",new Vector3(0,.55f,0),new Vector3(.38f,.38f,.45f),metal);
                PrimitiveArt.Box(root,"Handlebars",new Vector3(0,1.24f,-.57f),new Vector3(.94f,.07f,.08f),metal);
                NaturalMesh.Oval(root,"Bright headlamp",new Vector3(0,1.06f,-.74f),new Vector3(.27f,.27f,.12f),gold);
                // Helmeted rider, facing towards the player (-Z).
                NaturalMesh.Oval(root,"Rider jacket",new Vector3(0,1.32f,.12f),new Vector3(.51f,.62f,.36f),teal);
                NaturalMesh.Oval(root,"Helmet",new Vector3(0,1.78f,-.02f),new Vector3(.36f,.38f,.35f),gold,18);
                NaturalMesh.Oval(root,"Dark visor",new Vector3(0,1.78f,-.17f),new Vector3(.30f,.12f,.08f),dark);
                foreach(float x in new[]{-.22f,.22f})
                {
                    NaturalMesh.Branch(root,"Rider arm",new Vector3(x,1.5f,.08f),new Vector3(x*1.6f,1.24f,-.54f),.075f,teal);
                    NaturalMesh.Branch(root,"Rider leg",new Vector3(x,1,.35f),new Vector3(x,.45f,.08f),.10f,dark);
                }
            }
            else if (kind == Spaceship)
            {
                var metal=PrimitiveArt.Mat(new Color(.48f,.59f,.67f));
                NaturalMesh.Lathe(root,"Sculpted flying saucer",new[]{new Vector3(.25f,0,.25f),new Vector3(.87f,.16f,.87f),new Vector3(.92f,.26f,.92f),new Vector3(.55f,.42f,.55f),new Vector3(.25f,.48f,.25f)},metal,24);
                NaturalMesh.Oval(root,"Turquoise cockpit",new Vector3(0,.49f,0),new Vector3(.80f,.50f,.80f),teal,20);
                for(int i=0;i<10;i++)
                {
                    float a=i*Mathf.PI/5;
                    NaturalMesh.Oval(root,"Amber rim light",new Vector3(Mathf.Cos(a)*.82f,.27f,Mathf.Sin(a)*.82f),Vector3.one*.13f,gold,8);
                }
            }
            else
            {
                // Side-on camel, with its head facing +X. The broad collision envelope
                // stays inside the two blocked lanes; the head never enters the safe lane.
                NaturalMesh.Oval(root, "Camel body", new Vector3(0, 1.35f, 0), new Vector3(1.6f, .7f, .85f), hide);
                NaturalMesh.Oval(root, "Camel hump", new Vector3(-.1f, 1.83f, 0), new Vector3(.8f, .8f, .75f), hide);
                NaturalMesh.Oval(root, "Camel neck", new Vector3(.80f, 1.93f, 0), new Vector3(.35f, 1.3f, .32f), hide);
                NaturalMesh.Oval(root, "Camel head", new Vector3(1.02f, 2.57f, 0), new Vector3(.61f, .34f, .39f), hide);
                NaturalMesh.Oval(root,"Soft camel muzzle",new Vector3(1.22f,2.49f,0),new Vector3(.30f,.22f,.35f),hide);
                foreach(float z in new[]{-.17f,.17f})
                {
                    NaturalMesh.Oval(root,"Upright camel ear",new Vector3(.85f,2.76f,z),new Vector3(.12f,.22f,.10f),hide);
                    NaturalMesh.Oval(root,"Muzzle nostril",new Vector3(1.32f,2.53f,z*.8f),new Vector3(.045f,.03f,.025f),dark,8);
                }
                NaturalMesh.Branch(root,"Camel tail",new Vector3(-.75f,1.5f,0),new Vector3(-1.05f,.8f,0),.045f,hide);
                NaturalMesh.Oval(root,"Tail tuft",new Vector3(-1.05f,.76f,0),new Vector3(.13f,.25f,.13f),dark);
                NaturalMesh.Branch(root,"Colourful bridle",new Vector3(.81f,2.55f,-.2f),new Vector3(1.30f,2.44f,-.2f),.025f,magenta);
                PrimitiveArt.Box(root, "Decorated saddle cloth", new Vector3(-.2f, 1.57f, 0), new Vector3(.95f, .15f, .98f), teal);
                foreach (float z in new[] { -.51f, .51f })
                {
                    for(int stitch=0;stitch<5;stitch++)
                    {
                        var diamond=PrimitiveArt.Box(root,"Mirror-work saddle diamond",new Vector3(-.56f+stitch*.18f,1.38f,z*1.08f),new Vector3(.10f,.10f,.025f),gold);
                        diamond.localRotation=Quaternion.Euler(0,0,45);
                    }
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
