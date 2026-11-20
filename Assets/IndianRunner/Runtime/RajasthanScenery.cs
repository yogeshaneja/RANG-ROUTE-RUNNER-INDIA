using UnityEngine;
using UnityEngine.Rendering;

namespace IndianRunner
{
    // Original bundled illustrations on reusable world geometry. No downloads at runtime.
    public sealed class RajasthanScenery
    {
        private readonly Material horizonMaterial;
        private readonly Material sandstone, paleStone, leafy, terracotta;
        public Material RoadSurface { get; }
        private readonly Material sandSurface;
        private readonly Texture2D roadGrain, sandGrain;
        private float appliedCloudiness=-1;
        public RajasthanScenery(Transform parent)
        {
            var shader = Resources.Load<Shader>("Scenery/RunnerScenery");
            var horizon = Resources.Load<Texture2D>("Scenery/RajasthanHorizonReal");
            if (shader == null || horizon == null)
                throw new System.InvalidOperationException("Bundled Rajasthan scenery is missing. Pull the artwork, shader and their metadata, then let Unity import them.");
            horizonMaterial = new Material(shader) { mainTexture = horizon, name = "Rajasthan distant dunes and fort", renderQueue = 1000 };
            horizonMaterial.SetFloat("_ZWrite", 0);
            horizonMaterial.SetColor("_Tint", Color.white);
            horizonMaterial.SetFloat("_Saturation",1);
            Panel(parent, "Distant Rajasthan panorama", new Vector3(0, 22, 185), new Vector2(420, 280), 0, horizonMaterial);
            sandstone = PrimitiveArt.Mat(new Color(.79f, .43f, .23f));
            paleStone = PrimitiveArt.Mat(new Color(1, .83f, .53f));
            leafy = PrimitiveArt.Mat(new Color(.28f, .48f, .17f));
            terracotta = PrimitiveArt.Mat(new Color(.65f, .29f, .16f));
            roadGrain = Grain("Original road grit",new Color(.34f,.30f,.255f),31);
            sandGrain = Grain("Original sand grain",new Color(.82f,.66f,.44f),79);
            RoadSurface = Surface(roadGrain,new Vector2(3,18));
            sandSurface = Surface(sandGrain,Vector2.one);
        }
        public void AddGround(Transform tile, int variant, float length)
        {
            float[] columns = { -42,-28,-18,-12,-8,-5.4f,-4.5f,0,4.5f,5.4f,8,12,18,28,42 };
            const int rows = 15; var vertices = new Vector3[columns.Length*rows];
            var uv = new Vector2[vertices.Length]; var triangles = new int[(columns.Length-1)*(rows-1)*6];
            for (int z = 0; z < rows; z++) for (int x = 0; x < columns.Length; x++)
            {
                float forward = z*length/(rows-1);
                float elevation = GroundY(variant,columns[x],forward,length);
                int at = z*columns.Length+x; vertices[at] = new Vector3(columns[x],elevation,forward);
                uv[at] = new Vector2(columns[x]*.16f,forward*.18f);
            }
            int cursor = 0;
            for (int z = 0; z < rows-1; z++) for (int x = 0; x < columns.Length-1; x++)
            {
                int a = z*columns.Length+x,b = a+1,c = a+columns.Length,d = c+1;
                triangles[cursor++] = a; triangles[cursor++] = c; triangles[cursor++] = b;
                triangles[cursor++] = b; triangles[cursor++] = c; triangles[cursor++] = d;
            }
            NaturalMesh.Make(tile,"Gently uneven desert shoulder",vertices,uv,triangles,sandSurface);
        }
        public static float GroundY(int variant, float x, float z, float length)
        {
            float offRoad = Mathf.Clamp01((Mathf.Abs(x)-4.6f)/8);
            float edgeFade = Mathf.Pow(Mathf.Sin(Mathf.PI*z/length),2);
            return -.14f+offRoad*edgeFade*(.35f+.48f*Mathf.PerlinNoise(x*.08f+variant*.37f,z*.055f));
        }
        private static float Variation(int seed, int n)
        {
            uint x = (uint)(seed*7477+n*1327+91); x ^= x >> 13; x *= 1274126177;
            return (x & 65535)/65535f;
        }
        public void AddGeometry(Transform tile, int variant)
        {
            bool openRoad = variant%3 == 0;
            foreach (float side in new[] { -1f, 1f })
            {
                int houses = openRoad ? 1 : 2;
                for (int h = 0; h < houses; h++)
                {
                    int seed = variant*17+h*7+(side > 0 ? 9 : 2);
                    float z = 9+h*28+Variation(seed,1)*7;
                    float setback = 9.8f+Variation(seed,2)*4;
                    float height = 3+Variation(seed,3)*2.7f;
                    Haveli(tile,new Vector3(side*setback,GroundY(variant,side*setback,z,RunRules.RowSpacing*2),z),side,height,6+Variation(seed,4)*3,seed);
                }
                int plants = openRoad ? 4 : 2;
                for (int n = 0; n < plants; n++)
                {
                    float z = 4+n*(46f/plants)+Variation(variant+n,3)*5;
                    float x = side*(5.4f+Variation(variant+n,4)*2.2f);
                    float ground = GroundY(variant,x,z,RunRules.RowSpacing*2);
                    if ((n+variant)%2 == 0) Acacia(tile,new Vector3(x,ground,z),variant+n);
                    else
                    {
                        NaturalMesh.Oval(tile,"Uneven desert shrub",new Vector3(x,ground+.34f,z),new Vector3(.8f+Variation(n,1),.55f,.7f),leafy,10);
                        NaturalMesh.Oval(tile,"Shrub side growth",new Vector3(x+.3f,ground+.24f,z+.3f),new Vector3(.65f,.4f,.65f),leafy,9);
                    }
                    for (int rock = 0; rock < 2; rock++)
                    {
                        var stone = NaturalMesh.Oval(tile,"Scattered sandstone",new Vector3(side*(5.15f+Variation(n+variant,rock+5)*1.9f),ground+.07f,z+rock*1.8f),new Vector3(.36f,.17f,.26f),paleStone,7);
                        stone.localRotation = Quaternion.Euler(7,Variation(n,rock)*180,12);
                    }
                }
                if (!openRoad)
                {
                    float z = 18+Variation(variant,6)*15;
                    NaturalMesh.Lathe(tile,"Handmade roadside matka",new[] {new Vector3(.18f,0,.18f),new Vector3(.31f,.28f,.31f),new Vector3(.21f,.53f,.21f),new Vector3(.13f,.68f,.13f)},terracotta,14).localPosition = new Vector3(side*5.3f,GroundY(variant,side*5.3f,z,RunRules.RowSpacing*2),z);
                }
                if (variant == 2 || variant == 4) Pavilion(tile,new Vector3(side*15,GroundY(variant,side*15,28,RunRules.RowSpacing*2),28));
            }
        }
        private void Haveli(Transform tile, Vector3 position, float side, float height, float length, int seed)
        {
            var root = new GameObject("Varied Rajasthan courtyard house").transform; root.SetParent(tile,false); root.localPosition = position;
            root.localRotation = Quaternion.Euler(0,(Variation(seed,5)-.5f)*7,0);
            var wall = PrimitiveArt.TexturedMat("Scenery/SandstoneReal",Color.Lerp(new Color(.91f,.85f,.76f),new Color(1,.98f,.93f),Variation(seed,7)),new Vector2(2,2));
            var recess = PrimitiveArt.Mat(new Color(.20f,.24f,.22f));
            const float depth = 4.6f;
            PrimitiveArt.Box(root,"Solid sandstone building",new Vector3(0,height/2,0),new Vector3(depth,height,length),wall);
            PrimitiveArt.Box(root,"Projecting roof cornice",new Vector3(0,height+.03f,0),new Vector3(depth+.3f,.20f,length+.3f),paleStone);
            PrimitiveArt.Box(root,"Uneven parapet",new Vector3(0,height+.30f,0),new Vector3(depth+.12f,.40f,length+.12f),wall);
            var face = new GameObject("Recessed arched facade").transform; face.SetParent(root,false);
            face.localPosition = new Vector3(-side*(depth/2+.03f),0,0); face.localRotation = Quaternion.Euler(0,side*90,0);
            PrimitiveArt.Box(face,"Recessed wooden doorway",new Vector3(0,.85f,-.015f),new Vector3(1.02f,1.70f,.05f),recess);
            var round = PrimitiveArt.Shape(face,"Arched doorway top",PrimitiveType.Cylinder,new Vector3(0,1.70f,-.02f),new Vector3(1.02f,.025f,1.02f),recess);
            round.localRotation = Quaternion.Euler(90,0,0);
            NaturalMesh.Arch(face,new Vector3(0,1.70f,0),.51f,paleStone);
            foreach (float x in new[] { -.57f,.57f }) PrimitiveArt.Box(face,"Door jamb",new Vector3(x,.85f,-.04f),new Vector3(.12f,1.70f,.12f),paleStone);
            for (int w = 0; w < 2; w++)
            {
                float x = (w == 0 ? -1 : 1)*length*.29f, y = Mathf.Max(2.0f,height*.65f);
                PrimitiveArt.Box(face,"Recessed jharokha window",new Vector3(x,y,-.045f),new Vector3(.9f,1.1f,.07f),recess);
                PrimitiveArt.Box(face,"Jharokha sill",new Vector3(x,y-.58f,-.16f),new Vector3(1.13f,.16f,.35f),paleStone);
                PrimitiveArt.Box(face,"Window canopy",new Vector3(x,y+.62f,-.17f),new Vector3(1.15f,.13f,.42f),paleStone);
                for (int bar = -1; bar <= 1; bar++) PrimitiveArt.Box(face,"Lattice vertical",new Vector3(x+bar*.22f,y,-.10f),new Vector3(.035f,1.05f,.035f),paleStone);
                foreach (float dy in new[] { -.25f,.25f }) PrimitiveArt.Box(face,"Lattice horizontal",new Vector3(x,y+dy,-.10f),new Vector3(.85f,.035f,.035f),paleStone);
            }
            if (seed%2 == 0)
            {
                var cloth = PrimitiveArt.Mat(new Color(.12f,.43f,.43f));
                var awning = PrimitiveArt.Box(face,"Angled fabric shop shade",new Vector3(-length*.28f,1.9f,-.65f),new Vector3(2.6f,.07f,1.25f),cloth);
                awning.localRotation = Quaternion.Euler(-10,0,0);
            }
        }
        private void Acacia(Transform tile, Vector3 position, int seed)
        {
            var bark = PrimitiveArt.Mat(new Color(.34f,.25f,.15f));
            var crown = PrimitiveArt.Mat(new Color(.32f,.42f,.20f));
            Vector3 fork = position+new Vector3(.12f,1.6f,0);
            NaturalMesh.Branch(tile,"Crooked desert tree trunk",position,fork,.13f,bark);
            for (int b = 0; b < 3; b++)
            {
                float angle = (b*120+seed*23)*Mathf.Deg2Rad;
                Vector3 end = fork+new Vector3(Mathf.Cos(angle)*.9f,.65f+Variation(seed,b)*.4f,Mathf.Sin(angle)*.9f);
                NaturalMesh.Branch(tile,"Irregular acacia branch",fork,end,.075f,bark);
                var leaves = NaturalMesh.Oval(tile,"Broad acacia crown",end+Vector3.up*.18f,new Vector3(2.0f,.65f,1.6f),crown,11);
                leaves.localRotation = Quaternion.Euler(5,seed*17+b*32,7);
            }
        }
        private void Pavilion(Transform tile, Vector3 pos)
        {
            PrimitiveArt.Box(tile,"Chhatri plinth",pos+new Vector3(0,.18f,0),new Vector3(3.5f,.36f,3.5f),sandstone);
            foreach (float x in new[] {-1.2f,1.2f}) foreach (float z in new[] {-1.2f,1.2f})
                NaturalMesh.Branch(tile,"Chhatri pillar",pos+new Vector3(x,.3f,z),pos+new Vector3(x,3,z),.14f,paleStone);
            PrimitiveArt.Box(tile,"Chhatri cornice",pos+new Vector3(0,3,0),new Vector3(3.7f,.2f,3.7f),sandstone);
            NaturalMesh.Oval(tile,"Rounded chhatri dome",pos+new Vector3(0,3.35f,0),new Vector3(3.4f,1.2f,3.4f),paleStone,16);
        }
        public void AddPanels(Transform tile, int variant)
        {
            // Architecture now uses actual textured 3D walls; omit the old
            // flat illustrated storefront cards from the active scene.
        }
        private static Texture2D Grain(string name, Color colour, int seed)
        {
            const int size = 128; var texture = new Texture2D(size,size,TextureFormat.RGB24,true) { name = name,wrapMode = TextureWrapMode.Repeat,filterMode = FilterMode.Trilinear,anisoLevel = 2 };
            var pixels = new Color32[size*size];
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
            {
                float fine = Variation(seed+x,y), broad = .5f+.5f*Mathf.Sin(x*Mathf.PI*2/size)*Mathf.Cos(y*Mathf.PI*4/size);
                pixels[y*size+x] = colour*(.82f+.13f*fine+.05f*broad);
            }
            texture.SetPixels32(pixels); texture.Apply(true,true); return texture;
        }
        private static Material Surface(Texture2D texture, Vector2 tiling)
        {
            var material = new Material(PrimitiveArt.Mat(Color.white)) { name = texture.name };
            if (material.HasProperty("_MainTex")) { material.SetTexture("_MainTex",texture); material.SetTextureScale("_MainTex",tiling); }
            if (material.HasProperty("_BaseMap")) { material.SetTexture("_BaseMap",texture); material.SetTextureScale("_BaseMap",tiling); }
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness",.035f);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness",.035f);
            return material;
        }
        public void SetWeather(float cloudiness)
        {
            if(Mathf.Abs(cloudiness-appliedCloudiness)<.0001f) return;
            appliedCloudiness=cloudiness; horizonMaterial.SetFloat("_Cloudiness", cloudiness);
        }
        private static void Panel(Transform parent, string name, Vector3 pos, Vector2 size, float yaw, Material material)
        {
            var panel = GameObject.CreatePrimitive(PrimitiveType.Quad); panel.name = name;
            panel.transform.SetParent(parent, false); panel.transform.localPosition = pos;
            panel.transform.localRotation = Quaternion.Euler(0, yaw, 0);
            panel.transform.localScale = new Vector3(size.x, size.y, 1);
            Object.Destroy(panel.GetComponent<Collider>());
            var renderer = panel.GetComponent<MeshRenderer>(); renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off; renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }
        public void Dispose()
        {
            Object.Destroy(horizonMaterial);
            Object.Destroy(RoadSurface); Object.Destroy(sandSurface); Object.Destroy(roadGrain); Object.Destroy(sandGrain);
        }
    }
}
