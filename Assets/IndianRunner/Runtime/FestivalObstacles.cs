using UnityEngine;

namespace IndianRunner
{
    // Original themed meshes and shared collision dimensions. Built once per catalogue.
    public static class FestivalObstacles
    {
        // X/Z are half extents; Y is height above the moving object's base.
        public static Vector3 Shape(int kind)
        {
            if(kind==RegionRules.Ducks) return new Vector3(.70f,.68f,.73f);
            if(kind==RegionRules.Boar) return new Vector3(.49f,1.16f,1.12f);
            if(kind==RegionRules.Langur) return new Vector3(.64f,1.48f,.48f);
            if(kind==RegionRules.Coconuts) return new Vector3(.70f,.89f,.52f);
            if(kind==RegionRules.Coir) return new Vector3(.75f,.87f,.42f);
            if(kind==RegionRules.FishCart) return new Vector3(1.0f,1.72f,.76f);
            if(kind==RegionRules.Bamboo) return new Vector3(.96f,2.40f,.68f);
            if(kind==RegionRules.Log) return new Vector3(.78f,.78f,.40f);
            if(kind==RajasthanObstacles.OncomingElephant) return new Vector3(1.05f,3f,2.45f);
            if(kind==RajasthanObstacles.Motorcycle) return new Vector3(.50f,1.97f,1.06f);
            if(kind==RajasthanObstacles.Spaceship) return new Vector3(.92f,.74f,.92f);
            if(kind==RajasthanObstacles.Wheel) return new Vector3(.23f,1.24f,.62f);
            if(kind==RajasthanObstacles.Procession) return new Vector3(1.04f,3.35f,1.05f);
            if(kind==RajasthanObstacles.FallingCloth) return new Vector3(.82f,.94f,.60f);
            if(kind==RajasthanObstacles.Peacock) return new Vector3(1.12f,1.66f,.62f);
            if(kind==RajasthanObstacles.UmbrellaLady) return new Vector3(1.16f,2.68f,1.05f);
            return new Vector3(.80f,1.18f,.72f);
        }
        public static string Warning(int kind)
        {
            if(kind==RegionRules.Ducks) return "DUCKS ON THE PATH";
            if(kind==RegionRules.Boar) return "FOREST BOAR AHEAD";
            if(kind==RegionRules.Langur) return "LANGUR ON THE PATH";
            if(kind==RegionRules.Log) return "FALLEN PALM - jump or dodge";
            if(kind==RegionRules.Bamboo) return "LOW PALM SHELTER - slide";
            if(kind==RegionRules.Coconuts) return "COCONUT BASKETS - jump";
            if(kind==RegionRules.Coir) return "COIR MATS - jump";
            if(kind==RegionRules.FishCart) return "FISH CART - change lane";
            if(kind==RajasthanObstacles.OncomingElephant) return "ELEPHANT COMING";
            if(kind==RajasthanObstacles.UmbrellaLady) return "LADY WITH UMBRELLA";
            if(kind==RajasthanObstacles.Wheel) return "ROLLING WHEEL";
            if(kind==RajasthanObstacles.Procession) return "FESTIVAL PROCESSION";
            if(kind==RajasthanObstacles.FallingCloth) return "FABRIC FALLING - marked lane";
            if(kind==RajasthanObstacles.Peacock) return "PEACOCK DISPLAY";
            if(kind==RajasthanObstacles.Roadworks) return "ROAD REPAIRS - jump, slide or dodge";
            return kind==RajasthanObstacles.Motorcycle ? "MOTORCYCLE AHEAD" : "SPACESHIP DESCENDING";
        }
        public static string Contact(int kind)
        {
            if(kind==RegionRules.Ducks || kind==RegionRules.Boar || kind==RegionRules.Langur) return "The animal is safe. Give it room and follow the open lane.";
            if(kind>=RegionRules.Coconuts) return "Watch the warning. Jump, slide or follow the lotus lane.";
            if(kind==RajasthanObstacles.OncomingElephant) return "Gentle elephant stumble. Give the big fellow his own lane.";
            if(kind==RajasthanObstacles.UmbrellaLady) return "Gentle umbrella stumble. Give her marked lane some room.";
            if(kind==RajasthanObstacles.Peacock) return "Peacock is unharmed. Give its marked lane some space.";
            if(kind==RajasthanObstacles.Wheel) return "Rolling wheel contact. Jump early or take the open lane.";
            if(kind==RajasthanObstacles.Procession) return "Leave room for the procession. Follow the token lane.";
            if(kind==RajasthanObstacles.FallingCloth) return "Fabric bundle contact. Watch its landing marker.";
            return kind==RajasthanObstacles.Motorcycle ? "Motorcycle contact. Follow the warning and change lane." : "Spaceship contact. Avoid its marked landing lane.";
        }
        private static void Ring(Transform parent,string name,Vector3 centre,float radius,float thickness,Material material)
        {
            const int around=24, tube=8;
            var vertices=new Vector3[around*tube]; var uv=new Vector2[vertices.Length]; var triangles=new int[around*tube*6];
            int cursor=0;
            for(int a=0;a<around;a++) for(int b=0;b<tube;b++)
            {
                float angle=a*Mathf.PI*2/around, cross=b*Mathf.PI*2/tube;
                float r=radius+thickness*Mathf.Sin(cross); int i=a*tube+b;
                vertices[i]=centre+new Vector3(thickness*Mathf.Cos(cross),r*Mathf.Cos(angle),r*Mathf.Sin(angle));
                uv[i]=new Vector2((float)a/around,(float)b/tube);
                int nextB=a*tube+(b+1)%tube, nextA=((a+1)%around)*tube+b, corner=((a+1)%around)*tube+(b+1)%tube;
                triangles[cursor++]=i; triangles[cursor++]=nextB; triangles[cursor++]=nextA;
                triangles[cursor++]=nextB; triangles[cursor++]=corner; triangles[cursor++]=nextA;
            }
            NaturalMesh.Make(parent,name,vertices,uv,triangles,material);
        }
        public static void Build(Transform root,int kind)
        {
            if(kind==RajasthanObstacles.UmbrellaLady) { RainVisitorArt.Build(root); return; }
            var gold=PrimitiveArt.Mat(new Color(1,.70f,.08f));
            var pink=PrimitiveArt.Mat(new Color(.94f,.06f,.38f));
            var teal=PrimitiveArt.Mat(new Color(.015f,.64f,.65f));
            var blue=PrimitiveArt.Mat(new Color(.055f,.20f,.78f));
            var wood=PrimitiveArt.Mat(new Color(.38f,.18f,.065f));
            var dark=PrimitiveArt.Mat(new Color(.065f,.065f,.055f));
            var cream=PrimitiveArt.Mat(new Color(1,.95f,.75f));
            if(kind==RajasthanObstacles.Wheel)
            {
                Ring(root,"Iron-bound rolling wheel rim",new Vector3(0,.62f,0),.57f,.05f,dark);
                foreach(float x in new[]{-.075f,.075f})
                {
                    Ring(root,"Carved wooden wheel rim",new Vector3(x,.62f,0),.52f,.04f,wood);
                    for(int i=0;i<8;i++)
                    {
                        float a=i*Mathf.PI/4;
                        NaturalMesh.Branch(root,"Bright cart wheel spoke",new Vector3(x,.62f,0),new Vector3(x,.62f+Mathf.Sin(a)*.50f,Mathf.Cos(a)*.50f),.035f,gold);
                    }
                    NaturalMesh.Oval(root,"Turquoise wheel hub",new Vector3(x*1.20f,.62f,0),Vector3.one*.21f,teal,12);
                }
            }
            else if(kind==RajasthanObstacles.Procession)
            {
                PrimitiveArt.Box(root,"Decorated procession cart",new Vector3(0,.80f,0),new Vector3(1.72f,.64f,1.75f),pink);
                PrimitiveArt.Box(root,"Gold cart border",new Vector3(0,1.11f,0),new Vector3(1.85f,.12f,1.88f),gold);
                foreach(float x in new[]{-.87f,.87f}) foreach(float z in new[]{-.55f,.55f})
                {
                    var wheel=PrimitiveArt.Shape(root,"Procession wheel",PrimitiveType.Cylinder,new Vector3(x,.39f,z),new Vector3(.72f,.07f,.72f),wood);
                    wheel.localRotation=Quaternion.Euler(0,0,90);
                    NaturalMesh.Oval(root,"Wheel gold hub",new Vector3(x*1.08f,.39f,z),Vector3.one*.18f,gold,10);
                }
                foreach(float x in new[]{-.68f,.68f})
                {
                    NaturalMesh.Branch(root,"Festival flag pole",new Vector3(x,.95f,.45f),new Vector3(x,3.30f,.45f),.045f,gold);
                    var flag=PrimitiveArt.Box(root,"Bright festival pennant",new Vector3(x,2.92f,.45f),new Vector3(.40f,.50f,.035f),teal);
                    flag.localRotation=Quaternion.Euler(0,0,12);
                }
                var drum=PrimitiveArt.Shape(root,"Decorative festival drum",PrimitiveType.Cylinder,new Vector3(0,1.50f,-.25f),new Vector3(.85f,.40f,.85f),teal);
                drum.localRotation=Quaternion.Euler(90,0,0);
                NaturalMesh.Oval(root,"Cream drum skin",new Vector3(0,1.50f,-.66f),new Vector3(.78f,.78f,.045f),cream,18);
                for(int i=0;i<7;i++)
                {
                    float x=-.72f+i*.24f;
                    NaturalMesh.Oval(root,"Marigold garland",new Vector3(x,1.01f-Mathf.Sin(i*Mathf.PI/6)*.19f,-.91f),Vector3.one*.16f,gold,10);
                    var mirror=PrimitiveArt.Box(root,"Cart mirror diamond",new Vector3(x,.73f,-.89f),new Vector3(.11f,.11f,.025f),cream); mirror.localRotation=Quaternion.Euler(0,0,45);
                }
            }
            else if(kind==RajasthanObstacles.FallingCloth)
            {
                for(int i=0;i<3;i++)
                {
                    PrimitiveArt.Box(root,"Falling folded textile",new Vector3(i%2==0 ? -.04f:.04f,.17f+i*.29f,0),new Vector3(1.48f,.28f,1.10f),i==0 ? teal:i==1 ? pink:blue);
                    PrimitiveArt.Box(root,"Fabric gold selvedge",new Vector3(0,.17f+i*.29f,-.565f),new Vector3(1.48f,.065f,.035f),gold);
                }
                PrimitiveArt.Box(root,"Bundle rope",new Vector3(0,.47f,-.59f),new Vector3(.065f,.93f,.025f),cream);
            }
            else if(kind==RajasthanObstacles.Peacock)
            {
                var green=PrimitiveArt.Mat(new Color(.025f,.42f,.19f));
                NaturalMesh.Oval(root,"Peacock rounded body",new Vector3(0,.57f,.02f),new Vector3(.48f,.66f,.72f),blue,18);
                NaturalMesh.Oval(root,"Curved blue neck",new Vector3(0,.98f,-.20f),new Vector3(.20f,.72f,.22f),teal,16);
                NaturalMesh.Oval(root,"Peacock head",new Vector3(0,1.32f,-.22f),new Vector3(.23f,.23f,.27f),blue,16);
                NaturalMesh.Branch(root,"Peacock beak",new Vector3(0,1.32f,-.32f),new Vector3(0,1.29f,-.48f),.045f,gold);
                foreach(float x in new[]{-.095f,.095f})
                {
                    NaturalMesh.Oval(root,"Peacock eye",new Vector3(x,1.35f,-.27f),Vector3.one*.035f,cream,8);
                    NaturalMesh.Branch(root,"Peacock leg",new Vector3(x,.05f,.02f),new Vector3(x,.38f,.02f),.025f,gold);
                }
                for(int i=-5;i<=5;i++)
                {
                    var feather=NaturalMesh.Oval(root,"Fan feather",new Vector3(i*.135f,1.00f-Mathf.Abs(i)*.025f,.35f),new Vector3(.22f,1.15f,.09f),green,12);
                    feather.localRotation=Quaternion.Euler(0,0,-i*9);
                    NaturalMesh.Oval(root,"Gold feather eye",new Vector3(i*.135f,1.42f-Mathf.Abs(i)*.06f,.28f),new Vector3(.125f,.18f,.04f),gold,10);
                    NaturalMesh.Oval(root,"Turquoise feather jewel",new Vector3(i*.135f,1.42f-Mathf.Abs(i)*.06f,.255f),new Vector3(.066f,.11f,.025f),teal,8);
                }
                for(int i=-1;i<=1;i++) NaturalMesh.Branch(root,"Peacock crest",new Vector3(i*.04f,1.40f,-.22f),new Vector3(i*.07f,1.62f,-.20f),.018f,teal);
            }
            else if(kind==RajasthanObstacles.Roadworks)
            {
                var sand=PrimitiveArt.Mat(new Color(.83f,.49f,.15f));
                NaturalMesh.Oval(root,"Road repair sand pile",new Vector3(0,.45f,.10f),new Vector3(1.50f,1.12f,1.20f),sand,18);
                foreach(float x in new[]{-.67f,.67f}) PrimitiveArt.Box(root,"Repair barricade post",new Vector3(x,.43f,-.58f),new Vector3(.12f,.86f,.12f),dark);
                for(int i=0;i<7;i++)
                {
                    var stripe=PrimitiveArt.Box(root,"High contrast repair stripe",new Vector3(-.60f+i*.20f,.67f,-.64f),new Vector3(.20f,.22f,.07f),i%2==0 ? gold:dark);
                    stripe.localRotation=Quaternion.Euler(0,0,-12);
                }
            }
            else // Repair scaffolding: same clear slide opening as an awning.
            {
                foreach(float x in new[]{-.82f,.82f})
                {
                    PrimitiveArt.Box(root,"Repair scaffold upright",new Vector3(x,1.25f,0),new Vector3(.12f,2.50f,.12f),blue);
                    PrimitiveArt.Box(root,"Amber scaffold cap",new Vector3(x,2.55f,0),new Vector3(.19f,.15f,.19f),gold);
                }
                PrimitiveArt.Box(root,"Scaffold overhead beam",new Vector3(0,2.22f,0),new Vector3(1.80f,.20f,1.50f),blue);
                for(int i=0;i<9;i++) PrimitiveArt.Box(root,"Low caution crossbar",new Vector3(-.8f+i*.20f,1.61f,-.55f),new Vector3(.20f,.72f,.14f),i%2==0 ? gold:dark);
            }
        }
    }
}
