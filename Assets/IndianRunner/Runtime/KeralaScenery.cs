using UnityEngine;

namespace IndianRunner
{
    public sealed class KeralaScenery
    {
        public Material RoadSurface { get; }
        private readonly Material water, grass, leaf, youngLeaf, bark, cream, roof, mistHill;
        private float appliedShade=-1;
        public KeralaScenery(Transform parent)
        {
            RoadSurface=PrimitiveArt.Mat(new Color(.23f,.29f,.28f));
            water=PrimitiveArt.Mat(new Color(.055f,.43f,.47f));
            grass=PrimitiveArt.Mat(new Color(.25f,.53f,.16f));
            leaf=PrimitiveArt.Mat(new Color(.045f,.32f,.15f));
            youngLeaf=PrimitiveArt.Mat(new Color(.29f,.64f,.18f));
            bark=PrimitiveArt.Mat(new Color(.38f,.27f,.15f));
            cream=PrimitiveArt.Mat(new Color(.97f,.86f,.60f));
            roof=PrimitiveArt.Mat(new Color(.62f,.20f,.11f));
            mistHill=PrimitiveArt.Mat(new Color(.21f,.43f,.35f));
            var distant=new GameObject("Layered Western Ghats silhouette").transform; distant.SetParent(parent,false);
            for(int i=0;i<7;i++) NaturalMesh.Oval(distant,"Misty green hill",new Vector3((i-3)*35,6+i%3*4,167+i%2*12),new Vector3(76,30+i%3*11,43),mistHill,14);
            PrimitiveArt.CombineScenery(distant,true);
        }
        public void AddGround(Transform root,int variant,float length)
        {
            bool bridge=RegionRules.BridgeTile(variant),forest=variant%7>=4;
            PrimitiveArt.Box(root,"Backwater channel",new Vector3(0,-.63f,length*.5f),new Vector3(90,.16f,length),water);
            foreach(float side in new[]{-1f,1f})
            {
                if(!bridge) PrimitiveArt.Box(root,"Lush river bank",new Vector3(side*17,-.35f,length*.5f),new Vector3(25,.50f,length),grass);
                else PrimitiveArt.Box(root,"Palm island",new Vector3(side*26,-.36f,length*.5f),new Vector3(16,.45f,length),grass);
                for(int i=0;i<8;i++)
                {
                    float z=3+i*6.5f;
                    PrimitiveArt.Box(root,"Soft backwater glint",new Vector3(side*(bridge ? 10:4.8f),-.535f,z),new Vector3(bridge ? 3:1,.012f,.045f),cream);
                }
                if(!bridge) for(int i=0;i<5;i++) NaturalMesh.Oval(root,forest ? "Forest floor hummock":"Paddy field",new Vector3(side*(10+i%2*5),-.1f,5+i*11),new Vector3(7,.6f,9),youngLeaf,10);
            }
            if(bridge)
            {
                // The deck meets adjoining road at the same gameplay height.
                PrimitiveArt.Box(root,"Bridge stone deck",new Vector3(0,-.09f,length*.5f),new Vector3(9.4f,.18f,length),cream);
                for(int i=0;i<14;i++) PrimitiveArt.Box(root,"Bridge expansion seam",new Vector3(0,.018f,i*4),new Vector3(8.5f,.018f,.05f),bark);
                foreach(float side in new[]{-1f,1f})
                {
                    for(int i=0;i<15;i++) PrimitiveArt.Box(root,"Cream bridge baluster",new Vector3(side*4.65f,.61f,i*4),new Vector3(.22f,1.22f,.22f),cream);
                    PrimitiveArt.Box(root,"Teal bridge handrail",new Vector3(side*4.65f,1.22f,length*.5f),new Vector3(.27f,.17f,length),leaf);
                }
            }
        }
        public void AddGeometry(Transform root,int variant)
        {
            bool forest=variant%7>=4,bridge=RegionRules.BridgeTile(variant);
            foreach(float side in new[]{-1f,1f})
            {
                for(int i=0;i<(forest ? 6:4);i++)
                {
                    float x=side*(bridge ? 23+i%2*4:6.8f+i%2*3),z=4+i*(forest ? 9:14);
                    if(forest) Tree(root,new Vector3(x,0,z),i+variant); else Palm(root,new Vector3(x,0,z),i+variant);
                    if(!bridge) Banana(root,new Vector3(side*(5.4f+i%2*7),0,z+3),i);
                }
                if(!forest && !bridge) House(root,new Vector3(side*13,0,18+variant*2),side);
                if(bridge) Boat(root,new Vector3(side*13,-.48f,16+(side>0 ? 22:0)),side);
                if(forest)
                {
                    var monkey=new GameObject("Forest langur resting off the route").transform; monkey.SetParent(root,false);
                    monkey.localPosition=new Vector3(side*7.4f,0,25); KeralaArt.Build(monkey,RegionRules.Langur);
                }
            }
            if(forest && variant==6)
            {
                var elephant=new GameObject("Wild Asian elephant in forest clearing").transform; elephant.SetParent(root,false);
                elephant.localPosition=new Vector3(13,0,38); elephant.localRotation=Quaternion.Euler(0,-40,0);
                RajasthanDecor.BuildElephant(elephant,true,false);
            }
        }
        private void Palm(Transform parent,Vector3 position,int variant)
        {
            var root=new GameObject("Leaning coconut palm").transform;root.SetParent(parent,false);root.localPosition=position;
            float height=6.7f+variant%3*.7f,lean=(variant%2==0 ? 1:-1)*.55f;
            for(int i=0;i<5;i++) NaturalMesh.Branch(root,"Ringed palm trunk",new Vector3(lean*i/5f,height*i/5f,0),new Vector3(lean*(i+1)/5f,height*(i+1)/5f,0),.19f-i*.019f,bark);
            for(int i=0;i<7;i++) Frond(root,new Vector3(lean,height,0),i*Mathf.PI*2/7+variant,.0f,2.8f);
            for(int i=0;i<3;i++) NaturalMesh.Oval(root,"Coconut cluster",new Vector3(lean+(i-1)*.22f,height-.25f,.15f),Vector3.one*.38f,youngLeaf,8);
        }
        private void Banana(Transform root,Vector3 pos,int variant)
        {
            NaturalMesh.Branch(root,"Banana stem",pos,pos+Vector3.up*2.5f,.10f,youngLeaf);
            for(int i=0;i<4;i++) Frond(root,pos+Vector3.up*2.5f,i*Mathf.PI*.5f+variant,.20f,1.6f);
        }
        private void Frond(Transform root,Vector3 start,float angle,float extraWidth,float length)
        {
            const int rings=7;var vertices=new Vector3[rings*3];var uv=new Vector2[vertices.Length];var triangles=new int[(rings-1)*24];int cursor=0;
            Vector3 direction=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle)),across=new Vector3(-direction.z,0,direction.x);
            for(int i=0;i<rings;i++)
            {
                float t=i/(float)(rings-1);Vector3 centre=start+direction*(length*t)+Vector3.up*(Mathf.Sin(t*Mathf.PI)*.5f-t*t*.75f);
                float width=Mathf.Sin(t*Mathf.PI)*(.35f+extraWidth);
                for(int j=0;j<3;j++){vertices[i*3+j]=centre+across*((j-1)*width)-Vector3.up*(j==1 ? 0:.07f);uv[i*3+j]=new Vector2(j*.5f,t);}
                if(i==0)continue;
                for(int j=0;j<2;j++)
                {
                    int a=(i-1)*3+j,b=a+1,c=i*3+j,d=c+1;
                    foreach(int k in new[]{a,c,b,b,c,d,a,b,c,b,d,c})triangles[cursor++]=k;
                }
            }
            NaturalMesh.Make(root,"Curving tropical frond",vertices,uv,triangles,youngLeaf);
        }
        private void Tree(Transform root,Vector3 position,int variant)
        {
            NaturalMesh.Branch(root,"Rainforest trunk",position,position+Vector3.up*6.5f,.29f,bark);
            for(int i=0;i<3;i++) NaturalMesh.Oval(root,"Dense broadleaf canopy",position+new Vector3((i-1)*1.5f,6.5f+i%2,0),new Vector3(4.2f,3.3f,4.2f),i%2==0 ? leaf:youngLeaf,12);
        }
        private void House(Transform root,Vector3 position,float side)
        {
            var house=new GameObject("Verandah house with terracotta tiled roof").transform; house.SetParent(root,false);house.localPosition=position;
            PrimitiveArt.Box(house,"Cream plaster house",new Vector3(0,1.3f,0),new Vector3(4,2.6f,4),cream);
            foreach(float pitch in new[]{-1f,1f})
            {
                var slope=PrimitiveArt.Box(house,"Pitched terracotta roof",new Vector3(pitch*1.15f,3,0),new Vector3(2.8f,.16f,4.7f),roof);slope.localRotation=Quaternion.Euler(0,0,-pitch*25);
            }
            PrimitiveArt.Box(house,"Dark timber door",new Vector3(-side*2.02f,.95f,0),new Vector3(.04f,1.9f,.85f),bark);
            foreach(float z in new[]{-1.5f,1.5f}) PrimitiveArt.Box(house,"Verandah column",new Vector3(-side*2.6f,1.1f,z),new Vector3(.13f,2.2f,.13f),bark);
        }
        private void Boat(Transform root,Vector3 position,float side)
        {
            var boat=new GameObject("Kettuvallam inspired houseboat").transform;boat.SetParent(root,false);boat.localPosition=position;boat.localRotation=Quaternion.Euler(0,side*17,0);
            NaturalMesh.Oval(boat,"Long jackwood hull",new Vector3(0,.22f,0),new Vector3(3.4f,.82f,11),bark,16);
            PrimitiveArt.Box(boat,"Houseboat deck",new Vector3(0,.53f,0),new Vector3(2.9f,.15f,8.4f),cream);
            NaturalMesh.Oval(boat,"Arched woven coir cabin",new Vector3(0,1.35f,0),new Vector3(2.7f,2.8f,6.2f),cream,16);
            for(int i=0;i<4;i++) foreach(float s in new[]{-1f,1f}) PrimitiveArt.Box(boat,"Dark houseboat window",new Vector3(s*1.30f,1.3f,-2.1f+i*1.4f),new Vector3(.05f,.62f,.85f),leaf);
        }
        public static void Bird(Transform root,bool forest)
        {
            var dark=PrimitiveArt.Mat(new Color(.06f,.13f,.14f));var teal=PrimitiveArt.Mat(new Color(.015f,.52f,.74f));var gold=PrimitiveArt.Mat(new Color(1,.67f,.09f));
            NaturalMesh.Oval(root,forest ? "Hornbill body":"Kingfisher body",Vector3.zero,new Vector3(.30f,.30f,.70f),forest ? dark:teal,10);
            foreach(float side in new[]{-1f,1f}) NaturalMesh.Oval(root,"Outstretched bird wing",new Vector3(side*.38f,0,0),new Vector3(.72f,.06f,.30f),forest ? dark:teal,10);
            NaturalMesh.Oval(root,"Bird head",new Vector3(0,.10f,.29f),Vector3.one*.25f,dark,10);
            NaturalMesh.Oval(root,"Long golden bill",new Vector3(0,.07f,.49f),new Vector3(.12f,.10f,.30f),gold,8);
            if(forest) NaturalMesh.Oval(root,"Hornbill casque",new Vector3(0,.23f,.39f),new Vector3(.13f,.12f,.23f),gold,8);
        }
        public void SetWeather(float shade)
        {
            if(Mathf.Abs(shade-appliedShade)<.002f)return;appliedShade=shade;
            Color waterTint=Color.Lerp(new Color(.055f,.43f,.47f),new Color(.075f,.33f,.38f),shade);
            if(water.HasProperty("_Color"))water.SetColor("_Color",waterTint);
            if(water.HasProperty("_BaseColor"))water.SetColor("_BaseColor",waterTint);
        }
    }
}
