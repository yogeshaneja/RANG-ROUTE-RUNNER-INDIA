using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Original mirror-work-inspired festival ornaments; shared pooled geometry.
    public sealed class RajasthanDecor
    {
        private readonly Mesh[] tokenMeshes;
        private readonly Material[] tokenMaterials;
        public RajasthanDecor()
        {
            var root=new GameObject("Shared Rajasthan mirror-work token").transform;
            var gold=PrimitiveArt.Mat(new Color(1,.68f,.13f));
            var turquoise=PrimitiveArt.Mat(new Color(.015f,.63f,.65f));
            var mirror=PrimitiveArt.Mat(new Color(.84f,.96f,1));
            var rim=PrimitiveArt.Shape(root,"Gold embroidered rim",PrimitiveType.Cylinder,Vector3.zero,new Vector3(.67f,.035f,.67f),gold);
            rim.localRotation=Quaternion.Euler(90,0,0);
            var cloth=PrimitiveArt.Shape(root,"Turquoise fabric medallion",PrimitiveType.Cylinder,Vector3.zero,new Vector3(.56f,.041f,.56f),turquoise);
            cloth.localRotation=Quaternion.Euler(90,0,0);
            foreach(float side in new[]{-1f,1f})
            {
                var centre=PrimitiveArt.Box(root,"Central diamond mirror",new Vector3(0,0,side*.05f),new Vector3(.25f,.25f,.035f),mirror);
                centre.localRotation=Quaternion.Euler(0,0,45);
                for(int i=0;i<8;i++)
                {
                    float a=i*Mathf.PI/4;
                    NaturalMesh.Oval(root,"Embroidered gold petal",new Vector3(Mathf.Cos(a)*.235f,Mathf.Sin(a)*.235f,side*.05f),new Vector3(.08f,.08f,.035f),gold,8);
                }
            }
            PrimitiveArt.CombineScenery(root);
            var meshes=new List<Mesh>(); var materials=new List<Material>();
            foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>()) if(renderer.enabled)
            {
                meshes.Add(renderer.GetComponent<MeshFilter>().sharedMesh); materials.Add(renderer.sharedMaterial);
            }
            tokenMeshes=meshes.ToArray(); tokenMaterials=materials.ToArray(); Object.Destroy(root.gameObject);
        }
        public Transform Token(Transform parent)
        {
            var root=new GameObject("Pooled Rajasthan mirror-work token").transform; root.SetParent(parent,false);
            for(int i=0;i<tokenMeshes.Length;i++)
            {
                var part=new GameObject("Shared ornament mesh"); part.transform.SetParent(root,false);
                part.AddComponent<MeshFilter>().sharedMesh=tokenMeshes[i];
                var renderer=part.AddComponent<MeshRenderer>(); renderer.sharedMaterial=tokenMaterials[i];
                renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off; renderer.receiveShadows=false;
            }
            return root;
        }
        public static void Elephant(Transform parent,Vector3 position)
        {
            var root=new GameObject("Decorated roadside elephant - scenery").transform; root.SetParent(parent,false);
            root.localPosition=position; root.localRotation=Quaternion.Euler(0,-35,0);
            var grey=PrimitiveArt.Mat(new Color(.43f,.46f,.47f));
            var soft=PrimitiveArt.Mat(new Color(.54f,.53f,.50f));
            var dark=PrimitiveArt.Mat(new Color(.09f,.075f,.06f));
            var cloth=PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.72f,.09f,.30f),new Vector2(2,2));
            var gold=PrimitiveArt.Mat(new Color(1,.68f,.13f));
            NaturalMesh.Oval(root,"Large rounded elephant body",new Vector3(0,1.8f,0),new Vector3(1.8f,1.9f,2.8f),grey,20);
            NaturalMesh.Oval(root,"Domed elephant head",new Vector3(0,2.15f,1.30f),new Vector3(1.25f,1.55f,1.12f),grey,20);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(root,"Asian elephant ear",new Vector3(side*.67f,2.11f,1.08f),new Vector3(.68f,1.22f,.28f),soft,16);
                NaturalMesh.Oval(root,"Gentle elephant eye",new Vector3(side*.49f,2.36f,1.68f),Vector3.one*.075f,dark,10);
                foreach(float z in new[]{-.8f,.85f})
                {
                    NaturalMesh.Lathe(root,"Rounded elephant leg",new[]{new Vector3(.24f,.08f,.25f),new Vector3(.22f,.65f,.22f),new Vector3(.29f,1.55f,.28f)},grey,14).localPosition=new Vector3(side*.53f,0,z);
                    for(int n=0;n<3;n++) NaturalMesh.Oval(root,"Elephant toenail",new Vector3(side*.53f-.13f+n*.13f,.15f,z+.23f),new Vector3(.10f,.14f,.04f),soft,8);
                }
                PrimitiveArt.Box(root,"Embroidered saddle border",new Vector3(side*.84f,1.92f,-.1f),new Vector3(.06f,.11f,1.46f),gold);
                for(int i=0;i<5;i++) NaturalMesh.Oval(root,"Saddle tassel",new Vector3(side*.87f,1.64f,-.65f+i*.28f),new Vector3(.09f,.18f,.09f),gold,8);
            }
            var trunk=NaturalMesh.Lathe(root,"Long softly curved trunk",new[]{new Vector3(.12f,0,.12f),new Vector3(.15f,.45f,.15f),new Vector3(.20f,.95f,.20f),new Vector3(.27f,1.6f,.27f)},grey,16);
            trunk.localPosition=new Vector3(0,.53f,1.78f); trunk.localRotation=Quaternion.Euler(12,0,0);
            NaturalMesh.Oval(root,"Trunk tip",new Vector3(0,.61f,1.83f),new Vector3(.28f,.25f,.30f),soft,14);
            NaturalMesh.Oval(root,"Woven pink saddle cloth",new Vector3(0,2.18f,-.15f),new Vector3(1.87f,1.1f,1.55f),cloth,18);
            NaturalMesh.Branch(root,"Elephant tail",new Vector3(0,1.86f,-1.3f),new Vector3(.15f,.65f,-1.56f),.055f,grey);
            NaturalMesh.Oval(root,"Tail tuft",new Vector3(.15f,.63f,-1.56f),new Vector3(.14f,.25f,.13f),dark,10);
        }
    }
}
