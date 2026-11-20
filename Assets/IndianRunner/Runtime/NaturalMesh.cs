using UnityEngine;

namespace IndianRunner
{
    // Small original smooth meshes; no model importer or external package is required.
    public static class NaturalMesh
    {
        // Each ring is (X radius, Y height, Z radius).
        public static Transform Lathe(Transform parent, string name, Vector3[] rings, Material material, int sides = 14)
        {
            int stride = sides+1; // Duplicate UV seam vertices for woven/textured surfaces.
            var vertices = new Vector3[rings.Length * stride + 2];
            var uv = new Vector2[vertices.Length];
            var triangles = new int[((rings.Length - 1) * sides * 2 + sides * 2) * 3];
            for (int r = 0; r < rings.Length; r++) for (int s = 0; s <= sides; s++)
            {
                float angle = s * Mathf.PI * 2 / sides;
                vertices[r * stride + s] = new Vector3(Mathf.Cos(angle) * rings[r].x, rings[r].y, Mathf.Sin(angle) * rings[r].z);
                uv[r * stride + s] = new Vector2((float)s / sides, (float)r / (rings.Length - 1));
            }
            int cursor = 0;
            for (int r = 0; r < rings.Length - 1; r++) for (int s = 0; s < sides; s++)
            {
                int a = r * stride + s,b = a+1,c = a+stride,d = b+stride;
                triangles[cursor++] = a; triangles[cursor++] = c; triangles[cursor++] = b;
                triangles[cursor++] = b; triangles[cursor++] = c; triangles[cursor++] = d;
            }
            int bottom = vertices.Length - 2, top = vertices.Length - 1;
            vertices[bottom] = new Vector3(0, rings[0].y, 0); vertices[top] = new Vector3(0, rings[rings.Length - 1].y, 0);
            for (int s = 0; s < sides; s++)
            {
                int next = s+1;
                triangles[cursor++] = bottom; triangles[cursor++] = s; triangles[cursor++] = next;
                triangles[cursor++] = top; triangles[cursor++] = (rings.Length - 1) * stride + next; triangles[cursor++] = (rings.Length - 1) * stride + s;
            }
            var shape = Make(parent,name,vertices,uv,triangles,material);
            var mesh = shape.GetComponent<MeshFilter>().sharedMesh;
            var normals = mesh.normals;
            for (int r = 0; r < rings.Length; r++)
            {
                int first = r*stride,last = first+sides;
                var normal = (normals[first]+normals[last]).normalized;
                normals[first] = normals[last] = normal;
            }
            mesh.normals = normals;
            return shape;
        }
        public static Transform Oval(Transform parent, string name, Vector3 center, Vector3 size, Material material, int sides = 14)
        {
            var rings = new Vector3[9];
            for (int r = 0; r < rings.Length; r++)
            {
                float a = Mathf.Lerp(-Mathf.PI / 2 + .025f, Mathf.PI / 2 - .025f, r / 8f);
                rings[r] = new Vector3(Mathf.Cos(a) * size.x * .5f, Mathf.Sin(a) * size.y * .5f, Mathf.Cos(a) * size.z * .5f);
            }
            var shape = Lathe(parent, name, rings, material, sides); shape.localPosition = center; return shape;
        }
        public static Transform Branch(Transform parent, string name, Vector3 start, Vector3 end, float radius, Material material)
        {
            var branch = Lathe(parent, name, new[] { new Vector3(radius, 0, radius), new Vector3(radius * .72f, (end - start).magnitude * .6f, radius * .72f), new Vector3(radius * .28f, (end - start).magnitude, radius * .28f) }, material, 8);
            branch.localPosition = start; branch.localRotation = Quaternion.FromToRotation(Vector3.up, end - start); return branch;
        }
        public static Transform Arch(Transform parent, Vector3 center, float radius, Material material)
        {
            const int steps = 12; var v = new Vector3[(steps + 1) * 4]; var uv = new Vector2[v.Length]; var t = new int[steps * 24];
            for (int s = 0; s <= steps; s++) for (int edge = 0; edge < 4; edge++)
            {
                float a = s * Mathf.PI / steps, r = radius + (edge % 2 == 0 ? 0 : .11f);
                v[s * 4 + edge] = center + new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, edge < 2 ? -.08f : .03f);
            }
            int index = 0;
            for (int s = 0; s < steps; s++) for (int edge = 0; edge < 4; edge++)
            {
                int next = edge == 0 ? 1 : edge == 1 ? 3 : edge == 3 ? 2 : 0;
                int a = s * 4 + edge, b = s * 4 + next, c = a + 4, d = b + 4;
                t[index++] = a; t[index++] = c; t[index++] = b; t[index++] = b; t[index++] = c; t[index++] = d;
            }
            return Make(parent, "Carved arch frame", v, uv, t, material);
        }
        public static Transform Make(Transform parent, string name, Vector3[] vertices, Vector2[] uv, int[] triangles, Material material)
        {
            var mesh = new Mesh { name = name, vertices = vertices, uv = uv, triangles = triangles };
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); PrimitiveArt.OwnMesh(mesh);
            var obj = new GameObject(name); obj.transform.SetParent(parent, false);
            obj.AddComponent<MeshFilter>().sharedMesh = mesh; obj.AddComponent<MeshRenderer>().sharedMaterial = material;
            return obj.transform;
        }
    }
}
