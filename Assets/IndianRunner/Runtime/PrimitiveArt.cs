using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    public static class PrimitiveArt
    {
        private static readonly List<Mesh> CombinedMeshes = new List<Mesh>();
        private static readonly Dictionary<Color, Material> Materials = new Dictionary<Color, Material>();
        public static Material Mat(Color color)
        {
            if (Materials.TryGetValue(color, out var existing)) return existing;
            var template = Resources.Load<Material>("RunnerBase");
            Shader shader = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null
                ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
            if (template == null && shader == null) throw new System.InvalidOperationException("Run Indian Runner > Create or open Rajasthan scene to prepare the render material.");
            var mat = template != null ? new Material(template) : new Material(shader);
            mat.color = color; mat.name = "Runner colour " + color;
            Materials[color] = mat; return mat;
        }
        public static void Clear()
        {
            foreach (var mat in Materials.Values) if (mat != null) Object.Destroy(mat);
            Materials.Clear();
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
        public static void Camel(Transform parent, float x, float z, Material sand)
        {
            var root = new GameObject("Scenic camel").transform; root.SetParent(parent, false); root.localPosition = new Vector3(x, 0, z);
            Box(root, "Body", new Vector3(0, 1.35f, 0), new Vector3(.85f, .7f, 1.6f), sand);
            Shape(root, "Hump", PrimitiveType.Sphere, new Vector3(0, 1.82f, -.15f), new Vector3(.75f, .85f, .8f), sand);
            Box(root, "Neck", new Vector3(0, 1.9f, .85f), new Vector3(.32f, 1.35f, .35f), sand);
            Box(root, "Head", new Vector3(0, 2.57f, 1.05f), new Vector3(.4f, .33f, .65f), sand);
            foreach (float side in new[] { -.29f, .29f }) foreach (float end in new[] { -.5f, .5f })
                Box(root, "Leg", new Vector3(side, .56f, end), new Vector3(.16f, 1.1f, .17f), sand);
        }
        public static void Peacock(Transform parent, float x, float z, Material blue, Material green)
        {
            var root = new GameObject("Scenic peacock").transform; root.SetParent(parent, false); root.localPosition = new Vector3(x, 0, z);
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
