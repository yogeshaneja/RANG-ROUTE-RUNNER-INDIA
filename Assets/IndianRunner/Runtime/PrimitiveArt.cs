using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    public static class PrimitiveArt
    {
        private static readonly HashSet<Mesh> OwnedMeshes = new HashSet<Mesh>();
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
        public static void OwnMesh(Mesh mesh) { OwnedMeshes.Add(mesh); }
        public static void Clear()
        {
            foreach (var mat in Materials.Values) if (mat != null) Object.Destroy(mat);
            Materials.Clear();
            foreach (var material in TexturedMaterials.Values) if (material != null) Object.Destroy(material);
            TexturedMaterials.Clear();
            foreach (var mesh in OwnedMeshes) if (mesh != null) Object.Destroy(mesh);
            OwnedMeshes.Clear();
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
        // finalizeStatic consumes uniquely owned construction meshes and releases the
        // result's CPU copy. Finalized results may be rendered/shared, never merged again.
        // Leave false for builders that still need to read/reuse their source geometry.
        public static void CombineScenery(Transform root, bool finalizeStatic = false)
        {
            var groups = new Dictionary<Material, List<CombineInstance>>();
            var consumed = new HashSet<Mesh>();
            var filters = root.GetComponentsInChildren<MeshFilter>();
            // Validate before disabling any renderer, so an invalid future builder
            // fails clearly instead of silently losing finalized/unreadable geometry.
            foreach (var filter in filters)
            {
                var renderer=filter.GetComponent<MeshRenderer>();
                if(renderer!=null && renderer.enabled && filter.sharedMesh!=null && !filter.sharedMesh.isReadable)
                    throw new System.InvalidOperationException("Cannot merge a finalized mesh. Reuse its sharedMesh for rendering, or keep construction data readable until the final merge.");
            }
            foreach (var filter in filters)
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (renderer == null || !renderer.enabled || filter.sharedMesh == null) continue;
                var mat = renderer.sharedMaterial;
                if (!groups.TryGetValue(mat, out var entries)) { entries = new List<CombineInstance>(); groups.Add(mat, entries); }
                entries.Add(new CombineInstance { mesh = filter.sharedMesh, transform = root.worldToLocalMatrix * filter.transform.localToWorldMatrix });
                consumed.Add(filter.sharedMesh); renderer.enabled = false;
            }
            foreach (var group in groups)
            {
                long vertices=0;
                foreach(var part in group.Value) vertices+=part.mesh.vertexCount;
                var mesh = new Mesh { name = "Combined runner scenery", indexFormat = vertices>65535 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16 };
                mesh.CombineMeshes(group.Value.ToArray(), true, true); OwnMesh(mesh);
                if(finalizeStatic) mesh.UploadMeshData(true);
                var obj = new GameObject("Batched scenery"); obj.transform.SetParent(root, false);
                obj.AddComponent<MeshFilter>().sharedMesh = mesh;
                var renderer=obj.AddComponent<MeshRenderer>(); renderer.sharedMaterial=group.Key;
                renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off; renderer.receiveShadows=false;
                renderer.lightProbeUsage=UnityEngine.Rendering.LightProbeUsage.Off; renderer.reflectionProbeUsage=UnityEngine.Rendering.ReflectionProbeUsage.Off;
            }
            // Imported/built-in shared primitive meshes are never owned here, so are
            // never destroyed. Callers opt in only for uniquely owned source meshes.
            if(finalizeStatic) foreach(var mesh in consumed)
                if(OwnedMeshes.Remove(mesh)) Object.Destroy(mesh);
            foreach (var filter in filters) Object.Destroy(filter.gameObject);
        }
        public static void GetOwnedMeshStats(out int count,out int readable,out long bytes)
        {
            count=readable=0; bytes=0;
            foreach(var mesh in OwnedMeshes) if(mesh!=null)
            {
                count++; if(mesh.isReadable) readable++;
                bytes+=UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(mesh);
            }
        }
        public static void Camel(Transform parent, float x, float z, Material sand, float groundHeight = 0)
        {
            var root = new GameObject("Scenic camel").transform; root.SetParent(parent, false); root.localPosition = new Vector3(x, groundHeight, z); root.localScale = Vector3.one*1.16f;
            NaturalMesh.Oval(root,"Rounded camel body",new Vector3(0,1.35f,0),new Vector3(.85f,.72f,1.65f),sand,18);
            NaturalMesh.Oval(root,"Single camel hump",new Vector3(0,1.83f,-.12f),new Vector3(.75f,.86f,.8f),sand,18);
            NaturalMesh.Oval(root,"Long camel neck",new Vector3(0,1.94f,.80f),new Vector3(.34f,1.30f,.37f),sand,18);
            NaturalMesh.Oval(root,"Camel head",new Vector3(0,2.57f,1.04f),new Vector3(.39f,.34f,.62f),sand,18);
            var dark=Mat(new Color(.20f,.13f,.08f));
            foreach(float side in new[]{-.29f,.29f}) foreach(float end in new[]{-.5f,.5f})
            {
                NaturalMesh.Branch(root,"Tapered leg",new Vector3(side,.08f,end),new Vector3(side,1.15f,end),.085f,sand);
                NaturalMesh.Oval(root,"Padded foot",new Vector3(side,.07f,end+.03f),new Vector3(.20f,.12f,.25f),dark);
            }
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(root,"Camel ear",new Vector3(side*.17f,2.76f,.86f),new Vector3(.10f,.22f,.12f),sand);
                NaturalMesh.Oval(root,"Camel eye",new Vector3(side*.19f,2.63f,1.12f),Vector3.one*.05f,dark,8);
            }
            NaturalMesh.Oval(root,"Woven camel saddle",new Vector3(0,1.57f,-.2f),new Vector3(.98f,.20f,.95f),Mat(new Color(.04f,.52f,.53f)),16);
        }
        public static void Peacock(Transform parent, float x, float z, Material blue, Material green, float groundHeight = 0)
        {
            var root = new GameObject("Scenic peacock").transform; root.SetParent(parent, false); root.localPosition = new Vector3(x, groundHeight, z); root.localScale = Vector3.one*1.30f;
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
