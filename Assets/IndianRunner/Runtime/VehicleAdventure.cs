using UnityEngine;

namespace IndianRunner
{
    public sealed class VehicleAdventure
    {
        private readonly Transform root;
        private readonly Transform[] wheels;
        private readonly int region;
        private bool mounted;
        private float boardedAt,departUntil=-1;
        public bool Visible => root.gameObject.activeSelf;
        public float SeatHeight => region==RegionProfile.Kerala ? .82f:region==RegionProfile.Punjab ? .86f:region==RegionProfile.Goa ? 1.0f:.90f;
        public Vector3 BuddySeat => region==RegionProfile.Kerala ? new Vector3(.45f,.37f,.80f):region==RegionProfile.Goa ? new Vector3(.52f,.24f,-.22f):new Vector3(.48f,.45f,-.65f);
        public VehicleAdventure(Transform parent,int region)
        {
            this.region=region;root=new GameObject(AdventureRules.RideName(region)).transform;root.SetParent(parent,false);
            var body=new GameObject("Batched original adventure vehicle").transform;body.SetParent(root,false);
            var teal=AdventureArt.Colour(.025f,.58f,.62f);var gold=AdventureArt.Colour(1,.74f,.12f);
            var pink=AdventureArt.Colour(.96f,.19f,.37f);var cream=AdventureArt.Colour(.98f,.91f,.74f);
            if(ExpandedRegionRules.IsExpanded(region)){teal=PrimitiveArt.Mat(ExpandedRegionArt.Accent(region));pink=PrimitiveArt.Mat(ExpandedRegionArt.Secondary(region));}
            var wood=AdventureArt.Colour(.38f,.20f,.09f);var dark=AdventureArt.Colour(.07f,.10f,.10f);
            if(region==RegionProfile.Kerala)
            {
                NaturalMesh.Oval(body,"Curved colourful canoe hull",new Vector3(0,.20f,.12f),new Vector3(1.48f,.58f,2.60f),teal,20);
                NaturalMesh.Oval(body,"Golden canoe interior",new Vector3(0,.36f,.12f),new Vector3(1.22f,.12f,2.25f),gold,16);
                PrimitiveArt.Box(body,"Canoe seat",new Vector3(0,.53f,-.16f),new Vector3(1.14f,.16f,.48f),wood);
                foreach(float side in new[]{-1f,1f})
                {
                    NaturalMesh.Branch(body,"Decorative paddle shaft",new Vector3(side*.5f,.95f,.5f),new Vector3(side*.68f,.20f,-.7f),.025f,wood);
                    NaturalMesh.Oval(body,"Paddle blade",new Vector3(side*.67f,.18f,-.72f),new Vector3(.12f,.12f,.47f),gold,10);
                }
                wheels=new Transform[0];
            }
            else if(region==RegionProfile.Goa)
            {
                NaturalMesh.Oval(body,"Coral scooter front fairing",new Vector3(0,.73f,.58f),new Vector3(.62f,.92f,.39f),pink,16);
                PrimitiveArt.Box(body,"Teal scooter footboard",new Vector3(0,.32f,-.10f),new Vector3(.56f,.14f,1.13f),teal);
                NaturalMesh.Oval(body,"Cream scooter saddle",new Vector3(0,.76f,-.35f),new Vector3(.62f,.23f,.76f),cream,14);
                NaturalMesh.Branch(body,"Steering column",new Vector3(0,.65f,.56f),new Vector3(0,1.18f,.60f),.045f,teal);
                NaturalMesh.Branch(body,"Scooter handlebars",new Vector3(-.38f,1.18f,.60f),new Vector3(.38f,1.18f,.60f),.04f,dark);
                NaturalMesh.Oval(body,"Golden headlamp",new Vector3(0,1.10f,.71f),new Vector3(.22f,.21f,.08f),gold,12);
                // Small companion platform is visual only, within the riding envelope.
                PrimitiveArt.Box(body,"Companion scooter platform",new Vector3(.48f,.25f,-.25f),new Vector3(.42f,.10f,.60f),teal);
                wheels=new[]{Wheel(root,new Vector3(0,.23f,.60f),.23f,dark,gold),Wheel(root,new Vector3(0,.23f,-.63f),.23f,dark,gold)};
            }
            else
            {
                bool farm=region==RegionProfile.Punjab || ExpandedRegionRules.IsExpanded(region);
                PrimitiveArt.Box(body,"Bright cart platform",new Vector3(0,.38f,0),new Vector3(1.36f,.22f,2.1f),farm ? teal:pink);
                PrimitiveArt.Box(body,"Comfortable cart bench",new Vector3(0,.64f,-.12f),new Vector3(1.16f,.19f,.65f),cream);
                foreach(float side in new[]{-1f,1f})
                {
                    PrimitiveArt.Box(body,"Gold cart side trim",new Vector3(side*.62f,.64f,0),new Vector3(.09f,.30f,1.95f),gold);
                    NaturalMesh.Branch(body,"Cart handle support",new Vector3(side*.46f,.45f,.70f),new Vector3(side*.46f,1.12f,.70f),.035f,wood);
                }
                NaturalMesh.Branch(body,"Steering bar",new Vector3(-.46f,1.12f,.70f),new Vector3(.46f,1.12f,.70f),.035f,teal);
                if(farm)for(int i=0;i<3;i++)NaturalMesh.Oval(body,"Harvest flowers on trolley",new Vector3(-.42f+i*.42f,.62f,-.80f),Vector3.one*.27f,gold,10);
                else
                {
                    NaturalMesh.Branch(body,"Desert windcart pennant mast",new Vector3(-.58f,.55f,-.88f),new Vector3(-.58f,2.2f,-.88f),.025f,wood);
                    PrimitiveArt.Box(body,"Small saffron wind pennant",new Vector3(-.34f,2,-.88f),new Vector3(.43f,.37f,.035f),gold);
                }
                wheels=new Transform[4];int at=0;
                foreach(float side in new[]{-1f,1f})foreach(float z in new[]{-.72f,.72f})wheels[at++]=Wheel(root,new Vector3(side*.58f,.23f,z),.23f,dark,gold);
            }
            if(ExpandedRegionRules.IsExpanded(region))
            {
                if(region==RegionProfile.MadhyaPradesh)
                {
                    PrimitiveArt.Box(body,"Forest buggy nose",new Vector3(0,.62f,.90f),new Vector3(1.1f,.35f,.42f),teal);
                    foreach(float side in new[]{-1f,1f})NaturalMesh.Oval(body,"Buggy headlamp",new Vector3(side*.40f,.75f,1.12f),new Vector3(.18f,.16f,.035f),cream,10);
                }
                if(region==RegionProfile.Himachal)for(int i=0;i<3;i++)NaturalMesh.Oval(body,"Harvest trolley red apple",new Vector3((i-1)*.30f,.65f,-.82f),Vector3.one*.25f,pink,10);
                if(region==RegionProfile.Shillong)NaturalMesh.Oval(body,"Pine explorer trolley badge",new Vector3(0,.52f,1.07f),new Vector3(.30f,.38f,.04f),gold,10);
            }
            PrimitiveArt.CombineScenery(body,true);root.gameObject.SetActive(false);
        }
        private static Transform Wheel(Transform parent,Vector3 position,float radius,Material rubber,Material gold)
        {
            var pivot=new GameObject("Animated shared vehicle wheel").transform;pivot.SetParent(parent,false);pivot.localPosition=position;
            var tyre=PrimitiveArt.Shape(pivot,"Soft tyre",PrimitiveType.Cylinder,Vector3.zero,new Vector3(radius*2,.065f,radius*2),rubber);tyre.localRotation=Quaternion.Euler(0,0,90);
            for(int spoke=0;spoke<4;spoke++)
            {
                float a=spoke*Mathf.PI*.5f;
                NaturalMesh.Branch(pivot,"Gold wheel spoke",Vector3.zero,new Vector3(0,Mathf.Cos(a)*radius*.82f,Mathf.Sin(a)*radius*.82f),.018f,gold);
            }
            PrimitiveArt.CombineScenery(pivot,true);return pivot;
        }
        public void Mount(float clock){mounted=true;boardedAt=clock;departUntil=-1;root.gameObject.SetActive(true);}
        public void Dismount(float clock){mounted=false;departUntil=clock+.65f;}
        public void Reset(){mounted=false;departUntil=-1;root.gameObject.SetActive(false);}
        public void Tick(float x,float clock,float distance,bool reduced)
        {
            if(!mounted && clock>=departUntil){root.gameObject.SetActive(false);return;}
            float departure=mounted ? 0:Mathf.Clamp01(1-(departUntil-clock)/.65f);
            root.localPosition=new Vector3(x+departure*departure*4,region==RegionProfile.Kerala && !reduced ? Mathf.Sin(clock*3)*.012f:0,departure*7);
            root.localRotation=Quaternion.Euler(0,departure*18,reduced ? 0:Mathf.Sin(clock*2.5f)*.7f);
            root.localScale=Vector3.one*(reduced ? 1:Mathf.Lerp(.92f,1,Mathf.Clamp01((clock-boardedAt)/.3f)));
            foreach(var wheel in wheels)wheel.localRotation=Quaternion.Euler(distance/.23f*Mathf.Rad2Deg,0,0);
        }
    }
}
