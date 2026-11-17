using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    public static class PrimitiveArt
    {
        private static readonly List<Mesh> CombinedMeshes = new List<Mesh>();
        private static readonly Dictionary<(string,Color,Vector2),Material> TexturedMaterials = new Dictionary<(string,Color,Vector2),Material>();
        private static readonly Dictionary<Color, Material> Materials = new Dictionary<Color, Material>();
        public static Material Mat(Color color)
        {
            if (Materials.TryGetValue(color, out var existing)) return existing;
            var template = Resources.Load<Material>("RunnerBase");
            // A saved template can still use Standard after import into URP.
            // Always bind the selected pipeline's shader on the runtime clone.
            var shader = ResolveShader();
            var mat = template != null ? new Material(template) : new Material(shader);
            ConfigureMaterial(mat, shader, color);
            mat.name = "Runner colour " + color;
            Materials[color] = mat; return mat;
        }
        public static Material TexturedMat(string resource, Color tint, Vector2 tiling)
        {
            var key = (resource,tint,tiling);
            if (TexturedMaterials.TryGetValue(key,out var existing)) return existing;
            var texture = Resources.Load<Texture2D>(resource);
            if (texture == null) return Mat(tint);
            var material = new Material(Mat(tint)) { name = "Original surface " + resource };
            foreach (var property in new[] { "_MainTex", "_BaseMap" })
                if (material.HasProperty(property)) { material.SetTexture(property,texture); material.SetTextureScale(property,tiling); }
            TexturedMaterials.Add(key,material); return material;
        }
        public static Shader ResolveShader()
        {
            // currentRenderPipeline may not be initialized during the first Awake.
            // Quality overrides take precedence over the project-wide pipeline.
            var pipeline = UnityEngine.QualitySettings.renderPipeline;
            if (pipeline == null) pipeline = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            if (pipeline == null) pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            Shader shader = pipeline != null ? pipeline.defaultShader : Shader.Find("Standard");
            if (shader == null || !shader.isSupported)
                throw new System.InvalidOperationException("No supported runner shader for the selected render pipeline. Check Graphics/Quality settings and shader errors in the Console.");
            return shader;
        }
        public static void ConfigureMaterial(Material material, Shader shader, Color color)
        {
            material.shader = shader;
            material.shaderKeywords = System.Array.Empty<string>();
            material.renderQueue = -1;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", .12f);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", .12f);
        }
        public static void OwnMesh(Mesh mesh) { CombinedMeshes.Add(mesh); }
        public static void Clear()
        {
            foreach (var mat in Materials.Values) if (mat != null) Object.Destroy(mat);
            Materials.Clear();
            foreach (var material in TexturedMaterials.Values) if (material != null) Object.Destroy(material);
            TexturedMaterials.Clear();
            foreach (var mesh in CombinedMeshes) if (mesh != null) Object.Destroy(mesh);
            CombinedMeshes.Clear();
        }
        public static Transform Box(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
            => Shape(parent, name, PrimitiveType.Cube, pos, scale, mat);
        public static Transform Shape(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat)
        {
            var o = GameObject.CreatePrimitive(type); o.name = name;
            o.transform.SetParent(parent, false); o.transform.localPosition = pos; o.transform.localScale = scale;
            o.GetComponent<Renderer>().sharedMaterial = mat;
            var collider = o.GetComponent<Collider>(); if (collider != null) Object.Destroy(collider);
            return o.transform;
        }
        // One combined scenery mesh per material per segment. Moving the pooled root
        // does not require recreating meshes; animated birds/coins are added afterwards.
        public static void CombineScenery(Transform root)
        {
            var groups = new Dictionary<Material, List<CombineInstance>>();
            var filters = root.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in filters)
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (renderer == null || filter.sharedMesh == null) continue;
                var mat = renderer.sharedMaterial;
                if (!groups.TryGetValue(mat, out var entries)) { entries = new List<CombineInstance>(); groups.Add(mat, entries); }
                entries.Add(new CombineInstance { mesh = filter.sharedMesh, transform = root.worldToLocalMatrix * filter.transform.localToWorldMatrix });
                renderer.enabled = false;
            }
            foreach (var group in groups)
            {
                var mesh = new Mesh { name = "Combined Rajasthan scenery", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
                mesh.CombineMeshes(group.Value.ToArray(), true, true); CombinedMeshes.Add(mesh);
                var obj = new GameObject("Batched scenery"); obj.transform.SetParent(root, false);
                obj.AddComponent<MeshFilter>().sharedMesh = mesh;
                obj.AddComponent<MeshRenderer>().sharedMaterial = group.Key;
            }
            foreach (var filter in filters) Object.Destroy(filter.gameObject);
        }
        public static void Camel(Transform parent, float x, float z, Material sand, float groundHeight = 0)
        {
            var root = new GameObject("Scenic camel").transform; root.SetParent(parent, false); root.localPosition = new Vector3(x, groundHeight, z);
            Box(root, "Body", new Vector3(0, 1.35f, 0), new Vector3(.85f, .7f, 1.6f), sand);
            Shape(root, "Hump", PrimitiveType.Sphere, new Vector3(0, 1.82f, -.15f), new Vector3(.75f, .85f, .8f), sand);
            Box(root, "Neck", new Vector3(0, 1.9f, .85f), new Vector3(.32f, 1.35f, .35f), sand);
            Box(root, "Head", new Vector3(0, 2.57f, 1.05f), new Vector3(.4f, .33f, .65f), sand);
            foreach (float side in new[] { -.29f, .29f }) foreach (float end in new[] { -.5f, .5f })
                Box(root, "Leg", new Vector3(side, .56f, end), new Vector3(.16f, 1.1f, .17f), sand);
        }
        public static void Peacock(Transform parent, float x, float z, Material blue, Material green, float groundHeight = 0)
        {
            var root = new GameObject("Scenic peacock").transform; root.SetParent(parent, false); root.localPosition = new Vector3(x, groundHeight, z);
            Shape(root, "Body", PrimitiveType.Sphere, new Vector3(0, .55f, 0), new Vector3(.42f, .55f, .65f), blue);
            Shape(root, "Neck", PrimitiveType.Capsule, new Vector3(0, .95f, .22f), new Vector3(.16f, .35f, .16f), blue);
            for (int i = -3; i <= 3; i++)
            {
                var f = Shape(root, "Tail feather", PrimitiveType.Sphere, new Vector3(i * .14f, .95f - Mathf.Abs(i) * .08f, -.42f), new Vector3(.23f, 1.1f, .08f), green);
                f.localRotation = Quaternion.Euler(0, 0, -i * 16);
            }
        }
    }
}
