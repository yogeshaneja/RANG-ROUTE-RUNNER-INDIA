using UnityEngine;
using UnityEngine.Rendering;

namespace IndianRunner
{
    // Original bundled illustrations on reusable world geometry. No downloads at runtime.
    public sealed class RajasthanScenery
    {
        private readonly Material horizonMaterial, facadeMaterial;
        private readonly Material sandstone, paleStone, leafy, terracotta;
        public RajasthanScenery(Transform parent)
        {
            var shader = Resources.Load<Shader>("Scenery/RunnerScenery");
            var horizon = Resources.Load<Texture2D>("Scenery/RajasthanHorizon");
            var bazaar = Resources.Load<Texture2D>("Scenery/RajasthanBazaar");
            if (shader == null || horizon == null || bazaar == null)
                throw new System.InvalidOperationException("Bundled Rajasthan scenery is missing. Pull the artwork, shader and their metadata, then let Unity import them.");
            horizonMaterial = new Material(shader) { mainTexture = horizon, name = "Rajasthan distant dunes and fort", renderQueue = 1000 };
            horizonMaterial.SetFloat("_ZWrite", 0);
            facadeMaterial = new Material(shader) { mainTexture = bazaar, name = "Rajasthan haveli and bazaar", enableInstancing = true };
            facadeMaterial.SetFloat("_CutSky", 1);
            facadeMaterial.SetFloat("_ZWrite", 1);
            Panel(parent, "Distant Rajasthan panorama", new Vector3(0, 22, 185), new Vector2(420, 280), 0, horizonMaterial);
            sandstone = PrimitiveArt.Mat(new Color(.79f, .43f, .23f));
            paleStone = PrimitiveArt.Mat(new Color(1, .83f, .53f));
            leafy = PrimitiveArt.Mat(new Color(.28f, .48f, .17f));
            terracotta = PrimitiveArt.Mat(new Color(.78f, .27f, .12f));
        }
        public void AddGeometry(Transform tile, int variant)
        {
            // Roadside details are combined into the segment's static material batches.
            foreach (float side in new[] { -1f, 1f })
            {
                for (int n = 0; n < 3; n++)
                {
                    float z = 4 + n * 18;
                    PrimitiveArt.Shape(tile, "Terracotta pot", PrimitiveType.Sphere, new Vector3(side * 5.25f, .34f, z), new Vector3(.65f, .72f, .65f), terracotta);
                    PrimitiveArt.Shape(tile, "Desert shrub", PrimitiveType.Sphere, new Vector3(side * 5.25f, .85f, z), new Vector3(.9f, .75f, .8f), leafy);
                    PrimitiveArt.Box(tile, "Stone path edging", new Vector3(side * 4.7f, .09f, z + 4), new Vector3(.22f, .18f, 8), paleStone);
                }
                // An ornamental chhatri pavilion outside the playable lanes.
                float px = side * (13 + variant % 2), pz = 26;
                PrimitiveArt.Box(tile, "Pavilion plinth", new Vector3(px, .22f, pz), new Vector3(4, .44f, 4), sandstone);
                foreach (float dx in new[] { -1.4f, 1.4f }) foreach (float dz in new[] { -1.4f, 1.4f })
                    PrimitiveArt.Shape(tile, "Chhatri column", PrimitiveType.Cylinder, new Vector3(px + dx, 1.75f, pz + dz), new Vector3(.25f, 1.5f, .25f), paleStone);
                PrimitiveArt.Box(tile, "Carved pavilion cornice", new Vector3(px, 3.35f, pz), new Vector3(4, .35f, 4), sandstone);
                PrimitiveArt.Shape(tile, "Pavilion dome", PrimitiveType.Sphere, new Vector3(px, 3.65f, pz), new Vector3(3.6f, 1.35f, 3.6f), paleStone);
                PrimitiveArt.Shape(tile, "Dome finial", PrimitiveType.Sphere, new Vector3(px, 4.45f, pz), Vector3.one * .35f, sandstone);
                // A low crenellated wall beyond the roadside shops.
                PrimitiveArt.Box(tile, "Sandstone boundary wall", new Vector3(side * 18, 1.1f, 26), new Vector3(1, 2.2f, 52), sandstone);
                for (int b = 0; b < 9; b++)
                    PrimitiveArt.Box(tile, "Wall merlon", new Vector3(side * 18, 2.5f, 2 + b * 6), new Vector3(1.15f, .65f, 2), paleStone);
            }
            // Bunting runs beside the road, leaving the lane silhouette unobstructed.
            for (int n = 0; n < 8; n++)
            {
                float z = 2 + n * 6.5f;
                var flag = PrimitiveArt.Box(tile, "Roadside festival pennant", new Vector3(-5.8f, 3.3f, z), new Vector3(.08f, .65f, .65f), n % 2 == 0 ? paleStone : terracotta);
                flag.localRotation = Quaternion.Euler(0, 0, 12);
            }
        }
        public void AddPanels(Transform tile, int variant)
        {
            for (int n = 0; n < 2; n++) foreach (float side in new[] { -1f, 1f })
            {
                float z = 11 + n * 26 + (side > 0 ? 3 : 0);
                float h = 8.8f + (variant + n) % 2 * .7f;
                Panel(tile, "Illustrated haveli bazaar frontage", new Vector3(side * 6.7f, h / 2, z), new Vector2(h * 1.5f, h), side * 90, facadeMaterial);
            }
        }
        public void SetWeather(float cloudiness)
        {
            horizonMaterial.SetFloat("_Cloudiness", cloudiness);
            facadeMaterial.SetFloat("_Cloudiness", cloudiness);
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
        public void Dispose() { Object.Destroy(horizonMaterial); Object.Destroy(facadeMaterial); }
    }
}
