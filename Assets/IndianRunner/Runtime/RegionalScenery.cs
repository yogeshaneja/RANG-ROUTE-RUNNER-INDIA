using UnityEngine;

namespace IndianRunner
{
    // Punjab and Goa share construction tools, with separate layouts and palettes.
    public sealed class RegionalScenery
    {
        private readonly int region;
        private readonly Material green,gold,cream,wood,sea,sand,coral,blue;
        public Material RoadSurface { get; }
        public RegionalScenery(Transform parent,int region)
        {
            this.region=region;
            green=PrimitiveArt.Mat(new Color(.23f,.50f,.16f));gold=PrimitiveArt.Mat(new Color(1,.78f,.11f));
            cream=PrimitiveArt.Mat(new Color(1,.89f,.66f));wood=PrimitiveArt.Mat(new Color(.40f,.22f,.12f));
            sea=PrimitiveArt.Mat(new Color(.035f,.57f,.68f));sand=PrimitiveArt.Mat(new Color(.91f,.76f,.49f));
            coral=PrimitiveArt.Mat(new Color(.91f,.26f,.18f));blue=PrimitiveArt.Mat(new Color(.055f,.40f,.77f));
            RoadSurface=PrimitiveArt.Mat(region==RegionProfile.Punjab ? new Color(.43f,.35f,.25f):new Color(.34f,.36f,.34f));
            var horizon=new GameObject("Regional distant landscape").transform;horizon.SetParent(parent,false);
            if(region==RegionProfile.Punjab)
            {
                for(int i=0;i<5;i++) NaturalMesh.Oval(horizon,"Soft distant countryside",new Vector3((i-2)*45,1,172+i%2*12),new Vector3(95,13+i%2*6,45),green,12);
            }
            else
            {
                PrimitiveArt.Box(horizon,"Arabian Sea horizon",new Vector3(-105,-.48f,120),new Vector3(198,.12f,220),sea);
                NaturalMesh.Oval(horizon,"Coastal headland",new Vector3(37,3,169),new Vector3(74,27,52),green,14);
            }
            PrimitiveArt.CombineScenery(horizon,true);
        }
        public void AddGround(Transform root,int variant,float length)
        {
            bool punjab=region==RegionProfile.Punjab;
            PrimitiveArt.Box(root,punjab ? "Cultivated green fields":"Golden sand coastal strip",new Vector3(0,-.19f,length*.5f),new Vector3(88,.28f,length),punjab ? green:sand);
            if(punjab)
            {
                foreach(float side in new[]{-1f,1f})
                {
                    PrimitiveArt.Box(root,"Irrigation channel",new Vector3(side*5.5f,-.035f,length*.5f),new Vector3(.65f,.05f,length),sea);
                    if(variant%2==0) PrimitiveArt.Box(root,"Dense golden mustard field bed",new Vector3(side*20,-.055f,length*.5f),new Vector3(26,.07f,length-4),gold);
                    CropField(root,side,variant,length);
                    for(int i=0;i<8;i++) PrimitiveArt.Box(root,"Field boundary",new Vector3(side*20,-.025f,i*7),new Vector3(28,.05f,.12f),wood);
                }
            }
            else
            {
                PrimitiveArt.Box(root,"Arabian Sea beside the route",new Vector3(-38,-.30f,length*.5f),new Vector3(58,.10f,length),sea);
                for(int i=0;i<10;i++) PrimitiveArt.Box(root,"White surf ribbon",new Vector3(-9.5f-i%3*.6f,-.19f,2+i*5.4f),new Vector3(.14f,.025f,3.4f),cream);
                foreach(float side in new[]{-1f,1f}) for(int i=0;i<8;i++) PrimitiveArt.Box(root,"Promenade edge stone",new Vector3(side*4.75f,.10f,i*7),new Vector3(.45f,.20f,3.8f),cream);
            }
        }
        private void CropField(Transform root,float side,int variant,float length)
        {
            // One mesh per field: crossed golden heads and green stalk silhouettes.
            const int count=96;var vertices=new Vector3[count*8];var uv=new Vector2[vertices.Length];var indices=new int[count*24];
            for(int i=0;i<count;i++)
            {
                float x=side*(7+(i%8)*3.1f),z=1+(i/8)*length/12f,height=.75f+(i*7+variant)%5*.08f;
                for(int plane=0;plane<2;plane++)
                {
                    int at=i*8+plane*4;Vector3 across=plane==0 ? Vector3.right:Vector3.forward;
                    Vector3 centre=new Vector3(x,height*.65f,z);
                    vertices[at]=centre-across*.24f;vertices[at+1]=centre+across*.24f;
                    vertices[at+2]=centre+across*.17f+Vector3.up*.39f;vertices[at+3]=centre-across*.17f+Vector3.up*.39f;
                    int t=i*24+plane*12;indices[t]=at;indices[t+1]=at+1;indices[t+2]=at+2;indices[t+3]=at;indices[t+4]=at+2;indices[t+5]=at+3;indices[t+6]=at;indices[t+7]=at+2;indices[t+8]=at+1;indices[t+9]=at;indices[t+10]=at+3;indices[t+11]=at+2;
                }
            }
            var field=NaturalMesh.Make(root,variant%2==0 ? "Mustard flower rows":"Golden harvest rows",vertices,uv,indices,gold);
            // Double-sided foliage shares vertices; assign soft upward normals
            // instead of allowing reversed triangle normals to cancel out.
            var normals=new Vector3[vertices.Length];for(int i=0;i<normals.Length;i++)normals[i]=Vector3.up;
            field.GetComponent<MeshFilter>().sharedMesh.normals=normals;
        }
        public void AddGeometry(Transform root,int variant)
        {
            if(region==RegionProfile.Punjab)
            {
                foreach(float side in new[]{-1f,1f})
                {
                    Farmhouse(root,new Vector3(side*10.5f,0,12+variant*3),side);
                    for(int i=0;i<3;i++) Tree(root,new Vector3(side*(6.8f+i%2*9),0,5+i*20));
                    var animal=new GameObject("Off-road farm animal").transform;animal.SetParent(root,false);animal.localPosition=new Vector3(side*7.6f,0,33);
                    RegionalArt.Build(animal,variant%2==0 ? RegionRules.Buffalo:RegionRules.Goat);
                    for(int i=0;i<3;i++) NaturalMesh.Oval(root,"Haystack beside fields",new Vector3(side*(15+i*2),.75f,42),new Vector3(1.9f,1.5f,2.1f),gold,12);
                }
            }
            else
            {
                for(int i=0;i<5;i++) Palm(root,new Vector3(i%2==0 ? -6.8f:7.3f,0,4+i*11),variant+i);
                if(variant%3!=2) CoastalHouse(root,new Vector3(11,0,15),variant);
                if(variant%3==2) Shack(root,new Vector3(9,0,24));
                var boat=new GameObject("Colourful fishing boat beside the beach").transform;boat.SetParent(root,false);boat.localPosition=new Vector3(-7.2f,.12f,23);boat.localRotation=Quaternion.Euler(0,25,0);
                NaturalMesh.Oval(boat,"Blue fishing boat hull",Vector3.zero,new Vector3(2,.85f,5.5f),blue,14);
                PrimitiveArt.Box(boat,"Cream boat gunwale",new Vector3(0,.25f,0),new Vector3(1.7f,.1f,4.4f),cream);
                if(variant%2==0) Dolphin(root,new Vector3(-18,.05f,38));
                // Protected turtle is background decoration only, outside gameplay.
                if(variant==3) Turtle(root,new Vector3(-7.2f,.12f,42));
            }
        }
        private void Farmhouse(Transform root,Vector3 p,float side)
        {
            PrimitiveArt.Box(root,"Village farmhouse",p+new Vector3(0,1.5f,0),new Vector3(5,3,4.6f),cream);
            PrimitiveArt.Box(root,"Flat parapet roof",p+new Vector3(0,3.05f,0),new Vector3(5.4f,.25f,5),coral);
            PrimitiveArt.Box(root,"Indigo courtyard door",p+new Vector3(-side*2.52f,1,0),new Vector3(.08f,2,1.35f),blue);
            for(int i=0;i<5;i++)
            {
                var diamond=PrimitiveArt.Box(root,"Phulkari-inspired wall decoration",p+new Vector3(-side*2.57f,2.5f,-1.7f+i*.83f),new Vector3(.05f,.27f,.27f),gold);diamond.localRotation=Quaternion.Euler(45,0,0);
            }
            PrimitiveArt.Box(root,"Charpoy woven cot",p+new Vector3(-side*3.2f,.42f,2),new Vector3(1.6f,.12f,1),wood);
        }
        private void Tree(Transform root,Vector3 p)
        {
            NaturalMesh.Branch(root,"Village shade tree trunk",p,p+Vector3.up*4.9f,.22f,wood);
            NaturalMesh.Oval(root,"Round leafy village canopy",p+Vector3.up*5.3f,new Vector3(5.4f,3.6f,4.8f),green,14);
        }
        private void Palm(Transform root,Vector3 p,int variant)
        {
            Vector3 crown=p+new Vector3(.45f,6.5f+variant%3*.4f,0);
            NaturalMesh.Branch(root,"Leaning coastal coconut trunk",p,crown,.17f,wood);
            for(int i=0;i<7;i++)
            {
                float a=i*Mathf.PI*2/7;Vector3 direction=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
                // Two tapered curved leaf sections, all merged into the tile batch.
                var frond=NaturalMesh.Oval(root,"Coconut frond",crown+direction*1.25f,new Vector3(2.7f,.16f,.65f),green,10);frond.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,12);
                var tip=NaturalMesh.Oval(root,"Drooping palm leaf tip",crown+direction*2.5f-Vector3.up*.4f,new Vector3(1.4f,.12f,.40f),green,10);tip.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,30);
            }
            for(int i=0;i<3;i++) NaturalMesh.Oval(root,"Coconuts",crown+new Vector3((i-1)*.19f,-.23f,0),Vector3.one*.34f,gold,8);
        }
        private void CoastalHouse(Transform root,Vector3 p,int variant)
        {
            Material wall=variant%3==0 ? coral:variant%3==1 ? blue:gold;
            PrimitiveArt.Box(root,"Colourful Fontainhas-inspired house",p+new Vector3(0,1.5f,0),new Vector3(5,3,5),wall);
            foreach(float side in new[]{-1f,1f})
            {
                var slope=PrimitiveArt.Box(root,"Sloping terracotta tile roof",p+new Vector3(side*1.35f,3.25f,0),new Vector3(3.4f,.16f,5.7f),coral);slope.localRotation=Quaternion.Euler(0,0,-side*25);
                PrimitiveArt.Box(root,"Cream window frame",p+new Vector3(-2.54f,1.8f,side*1.45f),new Vector3(.08f,1.15f,.95f),cream);
                PrimitiveArt.Box(root,"Blue window shutter",p+new Vector3(-2.60f,1.8f,side*1.45f),new Vector3(.06f,.92f,.72f),blue);
            }
            PrimitiveArt.Box(root,"Arched doorway base",p+new Vector3(-2.56f,1,0),new Vector3(.1f,2,1.05f),wood);
        }
        private void Shack(Transform root,Vector3 p)
        {
            foreach(float x in new[]{-1.7f,1.7f}) foreach(float z in new[]{-1.8f,1.8f}) NaturalMesh.Branch(root,"Beach shack bamboo post",p+new Vector3(x,0,z),p+new Vector3(x,2.8f,z),.07f,wood);
            PrimitiveArt.Box(root,"Palm-roof beach shack",p+new Vector3(0,2.8f,0),new Vector3(4,.25f,4.4f),gold);
            PrimitiveArt.Box(root,"Colourful shack counter",p+new Vector3(0,.70f,0),new Vector3(3.3f,1.4f,1),blue);
        }
        private void Dolphin(Transform root,Vector3 p)
        {
            var body=NaturalMesh.Oval(root,"Distant dolphin above sea",p,new Vector3(.6f,.7f,2.1f),blue,12);body.localRotation=Quaternion.Euler(-20,0,0);
            NaturalMesh.Branch(root,"Dolphin dorsal fin",p,p+new Vector3(0,.80f,-.2f),.17f,blue);
            NaturalMesh.Oval(root,"Dolphin tail flukes",p+new Vector3(0,-.25f,-1),new Vector3(.85f,.08f,.4f),blue,10);
        }
        private void Turtle(Transform root,Vector3 p)
        {
            NaturalMesh.Oval(root,"Protected turtle shell",p+Vector3.up*.2f,new Vector3(.8f,.45f,1.05f),green,12);
            NaturalMesh.Oval(root,"Turtle head",p+new Vector3(0,.1f,.63f),Vector3.one*.23f,green,10);
            foreach(float side in new[]{-1f,1f}) NaturalMesh.Oval(root,"Turtle flipper",p+new Vector3(side*.46f,.02f,.1f),new Vector3(.55f,.06f,.20f),green,10);
        }
        public void SetWeather(float shade) { /* Shared fog/light provides the weather tint. */ }
    }
}
