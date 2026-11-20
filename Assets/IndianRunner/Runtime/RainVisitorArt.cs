using UnityEngine;

namespace IndianRunner
{
    // Original adult woman in a bright lehenga/dupatta, holding a panelled umbrella.
    public static class RainVisitorArt
    {
        public static void Build(Transform root)
        {
            var pink=PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.94f,.08f,.40f),new Vector2(2,2));
            var teal=PrimitiveArt.Mat(new Color(.015f,.70f,.66f));
            var gold=PrimitiveArt.Mat(new Color(1,.72f,.11f));
            var skin=PrimitiveArt.Mat(new Color(.62f,.37f,.25f));
            var dark=PrimitiveArt.Mat(new Color(.055f,.032f,.025f));
            var ivory=PrimitiveArt.Mat(new Color(1,.94f,.79f));
            NaturalMesh.Lathe(root,"Flowing pink lehenga",new[]{new Vector3(.34f,.13f,.25f),new Vector3(.32f,.29f,.24f),new Vector3(.28f,.57f,.22f),new Vector3(.23f,.82f,.18f),new Vector3(.20f,.96f,.16f)},pink,24);
            NaturalMesh.Lathe(root,"Embroidered skirt border",new[]{new Vector3(.344f,.14f,.254f),new Vector3(.338f,.19f,.251f)},gold,24);
            NaturalMesh.Lathe(root,"Tailored turquoise blouse",new[]{new Vector3(.20f,.94f,.16f),new Vector3(.18f,1.12f,.135f),new Vector3(.22f,1.34f,.15f),new Vector3(.20f,1.43f,.13f)},teal,20);
            NaturalMesh.Oval(root,"Neck",new Vector3(0,1.48f,0),new Vector3(.14f,.17f,.14f),skin,14);
            NaturalMesh.Oval(root,"Gentle adult face",new Vector3(0,1.72f,-.015f),new Vector3(.25f,.33f,.235f),skin,20);
            NaturalMesh.Oval(root,"Swept dark hair",new Vector3(0,1.82f,.045f),new Vector3(.27f,.20f,.23f),dark,18);
            NaturalMesh.Oval(root,"Hair bun",new Vector3(0,1.85f,.145f),Vector3.one*.16f,dark,14);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(root,"Small eye",new Vector3(side*.052f,1.74f,-.124f),new Vector3(.031f,.014f,.012f),dark,10);
                NaturalMesh.Oval(root,"Gold earring",new Vector3(side*.136f,1.66f,-.01f),new Vector3(.035f,.061f,.024f),gold,10);
                NaturalMesh.Oval(root,"Flat embroidered shoe",new Vector3(side*.12f,.07f,-.025f),new Vector3(.15f,.12f,.26f),dark,14);
            }
            NaturalMesh.Oval(root,"Bindi",new Vector3(0,1.80f,-.118f),Vector3.one*.017f,pink,8);
            NaturalMesh.Oval(root,"Nose",new Vector3(0,1.70f,-.135f),new Vector3(.034f,.065f,.041f),skin,12);
            NaturalMesh.Oval(root,"Soft smile",new Vector3(0,1.64f,-.116f),new Vector3(.053f,.009f,.009f),dark,10);
            NaturalMesh.Branch(root,"Left relaxed arm",new Vector3(-.20f,1.33f,0),new Vector3(-.29f,.99f,-.03f),.060f,skin);
            NaturalMesh.Branch(root,"Umbrella holding arm",new Vector3(.20f,1.33f,0),new Vector3(.14f,1.17f,-.01f),.060f,skin);
            NaturalMesh.Oval(root,"Umbrella holding hand",new Vector3(.14f,1.17f,-.015f),new Vector3(.085f,.10f,.075f),skin,12);
            var drape=NaturalMesh.Oval(root,"Pink dupatta drape",new Vector3(-.12f,1.17f,.10f),new Vector3(.29f,.75f,.15f),pink,16);
            drape.localRotation=Quaternion.Euler(0,0,-12);
            NaturalMesh.Branch(root,"Gold dupatta edge",new Vector3(-.24f,1.42f,.07f),new Vector3(-.21f,.83f,.09f),.014f,gold);
            for(int i=0;i<8;i++)
            {
                float a=i*Mathf.PI/4;
                NaturalMesh.Oval(root,"Skirt mirror ornament",new Vector3(Mathf.Cos(a)*.295f,.39f,Mathf.Sin(a)*.225f),Vector3.one*.045f,ivory,8);
            }
            PrimitiveArt.Box(root,"Umbrella shaft",new Vector3(.14f,1.85f,0),new Vector3(.035f,1.50f,.035f),gold);
            NaturalMesh.Oval(root,"Umbrella top finial",new Vector3(.14f,2.62f,0),Vector3.one*.09f,gold,12);
            const int panels=12, rings=6;
            for(int panel=0;panel<panels;panel++)
            {
                // Duplicate underside vertices keep the canopy visible from below,
                // with proper opposing normals rather than cancelling shared normals.
                int layer=rings*2; var vertices=new Vector3[layer*2]; var uv=new Vector2[vertices.Length]; var triangles=new int[(rings-1)*12];
                for(int r=0;r<rings;r++) for(int edge=0;edge<2;edge++)
                {
                    float radius=Mathf.Lerp(.02f,1,(float)r/(rings-1)),angle=(panel+edge)*Mathf.PI*2/panels;
                    int i=r*2+edge;
                    vertices[i]=new Vector3(.14f+Mathf.Cos(angle)*radius,2.30f+.27f*(1-radius*radius),Mathf.Sin(angle)*radius);
                    vertices[i+layer]=vertices[i]-Vector3.up*.012f;
                    uv[i]=uv[i+layer]=new Vector2(edge,radius);
                }
                int cursor=0;
                for(int r=0;r<rings-1;r++)
                {
                    int a=r*2,b=a+1,c=a+2,d=a+3;
                    triangles[cursor++]=a; triangles[cursor++]=b; triangles[cursor++]=c;
                    triangles[cursor++]=b; triangles[cursor++]=d; triangles[cursor++]=c;
                    triangles[cursor++]=a+layer; triangles[cursor++]=c+layer; triangles[cursor++]=b+layer;
                    triangles[cursor++]=b+layer; triangles[cursor++]=c+layer; triangles[cursor++]=d+layer;
                }
                NaturalMesh.Make(root,"Two-sided bright umbrella panel",vertices,uv,triangles,panel%2==0 ? teal:pink);
                float rib=panel*Mathf.PI*2/panels;
                NaturalMesh.Branch(root,"Gold umbrella rib",new Vector3(.14f,2.56f,0),new Vector3(.14f+Mathf.Cos(rib),2.29f,Mathf.Sin(rib)),.012f,gold);
            }
        }
    }
}
