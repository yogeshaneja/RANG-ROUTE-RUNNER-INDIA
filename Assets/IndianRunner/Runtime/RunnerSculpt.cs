using UnityEngine;

namespace IndianRunner
{
    // Original compact mesh authoring. No downloaded character, rig or runtime package.
    public static class RunnerSculpt
    {
        private static readonly Vector3[] FaceProfile = {
            new Vector3(.025f,-.168f,.045f),new Vector3(.067f,-.152f,.082f),
            new Vector3(.093f,-.123f,.100f),new Vector3(.114f,-.082f,.108f),
            new Vector3(.124f,-.035f,.115f),new Vector3(.123f,.025f,.112f),
            new Vector3(.119f,.080f,.108f),new Vector3(.107f,.125f,.098f),
            new Vector3(.074f,.156f,.070f),new Vector3(.014f,.174f,.020f)
        };
        private static Vector2 FaceRadius(float y)
        {
            for(int i=1;i<FaceProfile.Length;i++) if(y<=FaceProfile[i].y)
            {
                float t=Mathf.InverseLerp(FaceProfile[i-1].y,FaceProfile[i].y,y);
                return Vector2.Lerp(new Vector2(FaceProfile[i-1].x,FaceProfile[i-1].z),new Vector2(FaceProfile[i].x,FaceProfile[i].z),t);
            }
            return new Vector2(FaceProfile[FaceProfile.Length-1].x,FaceProfile[FaceProfile.Length-1].z);
        }
        private static float Bell(float x,float y,float cx,float cy,float wx,float wy)
            => Mathf.Exp(-Mathf.Pow((x-cx)/wx,2)-Mathf.Pow((y-cy)/wy,2));
        private static float Relief(float x,float y)
        {
            float nose=.028f*Bell(x,y,0,.004f,.020f,.047f)+.033f*Bell(x,y,0,-.040f,.024f,.019f);
            float wings=.007f*(Bell(x,y,-.023f,-.045f,.011f,.014f)+Bell(x,y,.023f,-.045f,.011f,.014f));
            float cheeks=.004f*(Bell(x,y,-.074f,-.038f,.031f,.034f)+Bell(x,y,.074f,-.038f,.031f,.034f));
            float sockets=.006f*(Bell(x,y,-.052f,.027f,.029f,.016f)+Bell(x,y,.052f,.027f,.029f,.016f));
            return nose+wings+cheeks-sockets;
        }
        public static float FaceZ(float x,float y)
        {
            var radius=FaceRadius(y);
            return radius.y*Mathf.Sqrt(Mathf.Max(0,1-x*x/(radius.x*radius.x)))+Relief(x,y);
        }
        private static int[] SurfaceTriangles(int rings,int sides,bool descending)
        {
            int stride=sides+1; var indices=new int[(rings-1)*sides*6]; int cursor=0;
            for(int r=0;r<rings-1;r++) for(int s=0;s<sides;s++)
            {
                int a=r*stride+s,b=a+1,c=a+stride,d=c+1;
                indices[cursor++]=a; indices[cursor++]=descending ? b:c; indices[cursor++]=descending ? c:b;
                indices[cursor++]=b; indices[cursor++]=descending ? d:c; indices[cursor++]=descending ? c:d;
            }
            return indices;
        }
        private static Mesh Surface(string name,Vector3[] vertices,Vector2[] uv,int[] indices,int rings,int sides)
        {
            var mesh=new Mesh {name=name,vertices=vertices,uv=uv,triangles=indices}; mesh.RecalculateNormals();
            var normals=mesh.normals; int stride=sides+1;
            for(int r=0;r<rings;r++)
            {
                int a=r*stride,b=a+sides; Vector3 n=(normals[a]+normals[b]).normalized; normals[a]=normals[b]=n;
            }
            mesh.normals=normals; mesh.RecalculateBounds(); PrimitiveArt.OwnMesh(mesh); return mesh;
        }
        private static Transform StaticSurface(Transform parent,string name,Mesh mesh,Material material)
        {
            var go=new GameObject(name); go.transform.SetParent(parent,false);
            go.AddComponent<MeshFilter>().sharedMesh=mesh; var renderer=go.AddComponent<MeshRenderer>(); renderer.sharedMaterial=material;
            renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off; return go.transform;
        }
        public static void Face(Transform parent,Material skin)
        {
            const int rings=33,sides=56; var vertices=new Vector3[rings*(sides+1)]; var uv=new Vector2[vertices.Length];
            for(int r=0;r<rings;r++)
            {
                float y=Mathf.Lerp(-.168f,.174f,(float)r/(rings-1)); var radius=FaceRadius(y);
                for(int s=0;s<=sides;s++)
                {
                    float a=s*Mathf.PI*2/sides,x=Mathf.Cos(a)*radius.x;
                    float front=Mathf.Pow(Mathf.Max(0,Mathf.Sin(a)),6);
                    int i=r*(sides+1)+s;
                    vertices[i]=new Vector3(x,y,Mathf.Sin(a)*radius.y+Relief(x,y)*front);
                    uv[i]=new Vector2((float)s/sides,(y+.168f)/.342f);
                }
            }
            StaticSurface(parent,"Continuous sculpted face - jaw cheeks nose and sockets",Surface("Original young adult face",vertices,uv,SurfaceTriangles(rings,sides,false),rings,sides),skin);
        }
        public static void Hair(Transform parent,Material material)
        {
            const int rings=15,sides=36; var vertices=new Vector3[rings*(sides+1)]; var uv=new Vector2[vertices.Length];
            for(int r=0;r<rings;r++) for(int s=0;s<=sides;s++)
            {
                float v=(float)r/(rings-1),a=s*Mathf.PI*2/sides,front=Mathf.Max(0,Mathf.Sin(a));
                float radial=Mathf.Sin(Mathf.Lerp(.025f,Mathf.PI*.5f,v));
                float line=-.025f+.115f*Mathf.Pow(front,4);
                float comb=.0015f*Mathf.Sin(a*9+v*4)*Mathf.Sin(v*Mathf.PI);
                int i=r*(sides+1)+s;
                vertices[i]=new Vector3(Mathf.Cos(a)*(.132f+comb)*radial-.017f*(1-v),
                    Mathf.Lerp(.195f,line,1-Mathf.Cos(v*Mathf.PI*.5f))+.022f*front*Mathf.Sin(v*Mathf.PI),
                    Mathf.Sin(a)*(.124f+comb)*radial-.010f);
                uv[i]=new Vector2((float)s/sides,v);
            }
            StaticSurface(parent,"Swept short hair with shaped hairline",Surface("Original swept hair surface",vertices,uv,SurfaceTriangles(rings,sides,true),rings,sides),material);
        }
        // A curved, tapered strip laid onto the face rather than floating oval features.
        public static void FaceLine(Transform parent,string name,float centreX,float centreY,float width,float thickness,float arch,Material material)
        {
            const int steps=14; var vertices=new Vector3[(steps+1)*2]; var uv=new Vector2[vertices.Length]; var tris=new int[steps*6];
            for(int i=0;i<=steps;i++) for(int edge=0;edge<2;edge++)
            {
                float t=(float)i/steps,x=centreX+(t-.5f)*width;
                float y=centreY+arch*Mathf.Sin(t*Mathf.PI)+(edge==0 ? -1:1)*thickness*Mathf.Sin(t*Mathf.PI)*.5f;
                vertices[i*2+edge]=new Vector3(x,y,FaceZ(x,y)+.0025f); uv[i*2+edge]=new Vector2(t,edge);
            }
            for(int i=0;i<steps;i++) { int a=i*2,c=i*6; tris[c]=a; tris[c+1]=a+2; tris[c+2]=a+1; tris[c+3]=a+1; tris[c+4]=a+2; tris[c+5]=a+3; }
            NaturalMesh.Make(parent,name,vertices,uv,tris,material);
        }
        public static Transform Eye(Transform parent,float side,Material white,Material iris,Material glint)
        {
            var root=new GameObject("Blinking almond eye").transform; root.SetParent(parent,false);
            float cx=side*.052f,cy=.026f; root.localPosition=new Vector3(cx,cy,FaceZ(cx,cy));
            const int steps=20; var vertices=new Vector3[steps+1]; var uv=new Vector2[vertices.Length]; var tris=new int[steps*3];
            vertices[0]=new Vector3(0,0,.005f);
            for(int i=0;i<steps;i++)
            {
                float a=i*Mathf.PI*2/steps,x=.022f*Mathf.Cos(a),y=.0075f*Mathf.Sin(a);
                vertices[i+1]=new Vector3(x,y,FaceZ(cx+x,cy+y)-root.localPosition.z+.002f);
                tris[i*3]=0; tris[i*3+1]=i+1; tris[i*3+2]=(i+1)%steps+1;
            }
            NaturalMesh.Make(root,"Curved almond sclera",vertices,uv,tris,white);
            NaturalMesh.Oval(root,"Warm dark iris",new Vector3(0,0,.007f),new Vector3(.012f,.013f,.007f),iris,12);
            NaturalMesh.Oval(root,"Eye catchlight",new Vector3(-.002f,.0025f,.011f),Vector3.one*.0035f,glint,8);
            PrimitiveArt.CombineScenery(root); return root;
        }
        public static void Kurta(Transform parent,Material material)
        {
            const int sides=28; var profile=new[]{new Vector3(.234f,-.16f,.146f),new Vector3(.231f,-.10f,.144f),new Vector3(.211f,.04f,.133f),new Vector3(.200f,.18f,.127f),new Vector3(.219f,.34f,.143f),new Vector3(.244f,.48f,.151f),new Vector3(.226f,.55f,.135f),new Vector3(.103f,.60f,.086f)};
            var vertices=new Vector3[profile.Length*(sides+1)]; var uv=new Vector2[vertices.Length];
            for(int r=0;r<profile.Length;r++) for(int s=0;s<=sides;s++)
            {
                float a=s*Mathf.PI*2/sides;
                float fold=.0032f*Mathf.Sin(a*8+profile[r].y*3)*(1-Mathf.InverseLerp(-.16f,.48f,profile[r].y));
                int i=r*(sides+1)+s;
                vertices[i]=new Vector3(Mathf.Cos(a)*(profile[r].x+fold),profile[r].y,Mathf.Sin(a)*(profile[r].z+fold));
                uv[i]=new Vector2((float)s/sides,(profile[r].y+.16f)/.76f);
            }
            StaticSurface(parent,"Tailored continuous kurta with gentle cloth folds",Surface("Original tailored kurta",vertices,uv,SurfaceTriangles(profile.Length,sides,false),profile.Length,sides),material);
        }
        public static void Limb(Transform upper,Transform lower,string name,Vector3[] profile,float joint,Material upperMaterial,Material lowerMaterial)
        {
            const int sides=16; int stride=sides+1; var vertices=new Vector3[profile.Length*stride]; var uv=new Vector2[vertices.Length]; var weights=new BoneWeight[vertices.Length];
            for(int r=0;r<profile.Length;r++) for(int s=0;s<=sides;s++)
            {
                float a=s*Mathf.PI*2/sides,fold=.0015f*Mathf.Sin(a*5+profile[r].y*13);
                int i=r*stride+s; vertices[i]=new Vector3(Mathf.Cos(a)*(profile[r].x+fold),profile[r].y,Mathf.Sin(a)*(profile[r].z+fold));
                uv[i]=new Vector2((float)s/sides,-profile[r].y/-profile[profile.Length-1].y);
                float blend=Mathf.SmoothStep(0,1,Mathf.InverseLerp(joint-.075f,joint+.075f,-profile[r].y));
                weights[i]=new BoneWeight {boneIndex0=0,weight0=1-blend,boneIndex1=1,weight1=blend};
            }
            var mesh=Surface(name,vertices,uv,SurfaceTriangles(profile.Length,sides,true),profile.Length,sides);
            if(upperMaterial!=lowerMaterial)
            {
                var top=new System.Collections.Generic.List<int>(); var bottom=new System.Collections.Generic.List<int>();
                var all=mesh.triangles;
                for(int i=0;i<all.Length;i+=3)
                {
                    float depth=-(vertices[all[i]].y+vertices[all[i+1]].y+vertices[all[i+2]].y)/3;
                    var target=depth<=joint ? top:bottom; target.Add(all[i]); target.Add(all[i+1]); target.Add(all[i+2]);
                }
                mesh.subMeshCount=2; mesh.SetTriangles(top,0); mesh.SetTriangles(bottom,1);
            }
            mesh.boneWeights=weights;
            mesh.bindposes=new[]{upper.worldToLocalMatrix*upper.localToWorldMatrix,lower.worldToLocalMatrix*upper.localToWorldMatrix};
            var go=new GameObject(name); go.transform.SetParent(upper,false); var skin=go.AddComponent<SkinnedMeshRenderer>();
            skin.sharedMesh=mesh; skin.bones=new[]{upper,lower}; skin.rootBone=upper; skin.quality=SkinQuality.Bone2;
            skin.sharedMaterials=upperMaterial==lowerMaterial ? new[]{upperMaterial}:new[]{upperMaterial,lowerMaterial};
            skin.localBounds=new Bounds(new Vector3(0,-joint,.05f),new Vector3(.70f,2.1f,1.8f));
            skin.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        public static void Hand(Transform parent,Material skin,float thumbSide)
        {
            NaturalMesh.Lathe(parent,"Sculpted relaxed palm",new[]{new Vector3(.028f,-.105f,.022f),new Vector3(.041f,-.055f,.029f),new Vector3(.035f,0,.026f)},skin,14);
            for(int i=0;i<4;i++)
            {
                float x=(-.027f+i*.018f)*thumbSide,length=i==0 || i==3 ? .053f:.067f;
                var finger=NaturalMesh.Oval(parent,"Relaxed curled finger",new Vector3(x,-.108f,.014f),new Vector3(.019f,length,.026f),skin,10);
                finger.localRotation=Quaternion.Euler(-25,0,0);
            }
            NaturalMesh.Oval(parent,"Natural thumb",new Vector3(.044f*thumbSide,-.052f,.016f),new Vector3(.028f,.071f,.033f),skin,12).localRotation=Quaternion.Euler(-15,0,-24*thumbSide);
            PrimitiveArt.CombineScenery(parent);
        }
        public static void Scarf(Transform root,Transform middle,Transform tip,Material material)
        {
            const int rings=13,stride=5; var vertices=new Vector3[rings*stride]; var uv=new Vector2[vertices.Length]; var weights=new BoneWeight[vertices.Length];
            for(int r=0;r<rings;r++) for(int s=0;s<stride;s++)
            {
                float depth=r*.045f,angle=s*Mathf.PI*.5f; int i=r*stride+s;
                vertices[i]=new Vector3(Mathf.Cos(angle)*(.068f+depth*.018f),-depth,Mathf.Sin(angle)*.012f);
                uv[i]=new Vector2((float)s/(stride-1),depth*2);
                float span=Mathf.Clamp01(depth<.27f ? depth/.27f:(depth-.27f)/.27f);
                weights[i]=new BoneWeight {boneIndex0=depth<.27f ? 0:1,boneIndex1=depth<.27f ? 1:2,weight0=1-span,weight1=span};
            }
            var mesh=Surface("Continuous woven scarf",vertices,uv,SurfaceTriangles(rings,4,true),rings,4); mesh.boneWeights=weights;
            mesh.bindposes=new[]{root.worldToLocalMatrix*root.localToWorldMatrix,middle.worldToLocalMatrix*root.localToWorldMatrix,tip.worldToLocalMatrix*root.localToWorldMatrix};
            var go=new GameObject("Three-bone soft scarf"); go.transform.SetParent(root,false); var renderer=go.AddComponent<SkinnedMeshRenderer>(); renderer.sharedMesh=mesh;
            renderer.bones=new[]{root,middle,tip}; renderer.rootBone=root; renderer.sharedMaterial=material; renderer.quality=SkinQuality.Bone2;
            renderer.localBounds=new Bounds(new Vector3(0,-.27f,-.15f),new Vector3(.6f,1.5f,1.5f)); renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
