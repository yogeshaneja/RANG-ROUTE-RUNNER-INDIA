using UnityEngine;

namespace IndianRunner
{
    // Original Punjab/Goa meshes. The active catalogue shares each recipe across slots.
    public static class RegionalArt
    {
        private static Material C(float r,float g,float b) => PrimitiveArt.Mat(new Color(r,g,b));
        public static void Build(Transform root,int kind)
        {
            var wood=C(.40f,.22f,.10f);var gold=C(1,.74f,.12f);var dark=C(.085f,.10f,.09f);
            var cream=C(.97f,.91f,.73f);var pink=C(.94f,.15f,.42f);var blue=C(.04f,.42f,.77f);
            if(kind==RegionRules.Buffalo || kind==RegionRules.Goat || kind==RegionRules.Dog)
            {
                bool buffalo=kind==RegionRules.Buffalo,goat=kind==RegionRules.Goat;
                var hide=buffalo ? C(.19f,.21f,.20f):goat ? cream:C(.72f,.39f,.17f);
                float scale=buffalo ? 1:goat ? .67f:.62f;
                var animal=new GameObject(buffalo ? "Village water buffalo":goat ? "Farm goat":"Friendly coastal street dog").transform;
                animal.SetParent(root,false); animal.localScale=Vector3.one*scale;
                NaturalMesh.Oval(animal,"Rounded animal body",new Vector3(0,.84f,0),new Vector3(1.02f,.88f,1.65f),hide,14);
                NaturalMesh.Oval(animal,"Sculpted animal head",new Vector3(0,1.04f,.91f),new Vector3(.64f,.66f,.72f),hide,12);
                NaturalMesh.Oval(animal,"Animal muzzle",new Vector3(0,.87f,1.22f),new Vector3(.49f,.29f,.38f),buffalo ? dark:wood,10);
                foreach(float side in new[]{-1f,1f})
                {
                    foreach(float z in new[]{-.52f,.48f}) NaturalMesh.Branch(animal,"Tapered animal leg",new Vector3(side*.33f,.06f,z),new Vector3(side*.33f,.80f,z),buffalo ? .10f:.08f,hide);
                    NaturalMesh.Oval(animal,"Soft animal ear",new Vector3(side*.39f,1.14f,.83f),new Vector3(.35f,.19f,.15f),hide,10);
                    NaturalMesh.Oval(animal,"Dark bright eye",new Vector3(side*.26f,1.16f,1.17f),Vector3.one*.062f,dark,8);
                    if(buffalo || goat)
                    {
                        NaturalMesh.Branch(animal,"Sweeping horn",new Vector3(side*.25f,1.32f,.85f),new Vector3(side*.67f,1.37f,.92f),.055f,wood);
                        NaturalMesh.Branch(animal,"Upturned horn tip",new Vector3(side*.67f,1.37f,.92f),new Vector3(side*.73f,1.57f,.98f),.034f,wood);
                    }
                }
                NaturalMesh.Branch(animal,"Animal tail",new Vector3(0,.95f,-.78f),new Vector3(.20f,goat ? 1.23f:.50f,-1.12f),.045f,hide);
                if(goat) NaturalMesh.Oval(animal,"Goat beard",new Vector3(0,.65f,1.10f),new Vector3(.14f,.30f,.18f),wood,10);
            }
            else if(kind==RegionRules.Tractor)
            {
                var green=C(.12f,.56f,.30f);
                PrimitiveArt.Box(root,"Green tractor chassis",new Vector3(0,.70f,0),new Vector3(1.35f,.32f,2.25f),green);
                PrimitiveArt.Box(root,"Rounded tractor hood",new Vector3(0,1.15f,.55f),new Vector3(1.25f,.65f,1.20f),green);
                PrimitiveArt.Box(root,"Cream radiator grille",new Vector3(0,1.12f,1.17f),new Vector3(.96f,.43f,.06f),cream);
                for(int i=0;i<5;i++) PrimitiveArt.Box(root,"Radiator stripe",new Vector3(-.36f+i*.18f,1.12f,1.205f),new Vector3(.055f,.39f,.02f),dark);
                foreach(float side in new[]{-1f,1f}) foreach(float z in new[]{-.70f,.80f})
                {
                    float size=z<0 ? 1.18f:.78f;
                    var wheel=PrimitiveArt.Shape(root,"Tractor rubber tyre",PrimitiveType.Cylinder,new Vector3(side*.84f,size*.5f,z),new Vector3(size,.12f,size),dark);wheel.localRotation=Quaternion.Euler(0,0,90);
                    NaturalMesh.Oval(root,"Bright wheel hub",new Vector3(side*.975f,size*.5f,z),new Vector3(.045f,size*.48f,size*.48f),gold,10);
                }
                NaturalMesh.Oval(root,"Farmer seated torso",new Vector3(0,1.48f,-.55f),new Vector3(.49f,.67f,.40f),cream,12);
                NaturalMesh.Oval(root,"Farmer face",new Vector3(0,1.97f,-.52f),new Vector3(.28f,.33f,.27f),wood,10);
                NaturalMesh.Oval(root,"Farmer blue turban",new Vector3(0,2.12f,-.54f),new Vector3(.35f,.18f,.31f),blue,10);
                PrimitiveArt.Box(root,"Driver seat",new Vector3(0,1.10f,-.55f),new Vector3(.72f,.20f,.54f),dark);
                foreach(float side in new[]{-1f,1f}) NaturalMesh.Branch(root,"Sunshade support",new Vector3(side*.54f,.85f,-.85f),new Vector3(side*.54f,2.35f,-.85f),.04f,dark);
                PrimitiveArt.Box(root,"Tractor cream sunshade",new Vector3(0,2.35f,-.48f),new Vector3(1.7f,.12f,1.25f),cream);
            }
            else if(kind==RegionRules.Hay || kind==RegionRules.Grain || kind==RegionRules.FishCrates)
            {
                var colour=kind==RegionRules.Hay ? gold:kind==RegionRules.Grain ? cream:blue;
                for(int i=0;i<2;i++)
                {
                    float x=(i-.5f)*.68f;
                    NaturalMesh.Oval(root,kind==RegionRules.Hay ? "Bound hay bale":kind==RegionRules.Grain ? "Grain sack":"Fishing basket",new Vector3(x,.43f,0),new Vector3(.70f,.80f,.85f),colour,12);
                    for(int stripe=0;stripe<3;stripe++) PrimitiveArt.Box(root,"Basket binding",new Vector3(x,.20f+stripe*.21f,-.43f),new Vector3(.62f,.035f,.035f),wood);
                }
            }
            else if(kind==RegionRules.PhulkariGate || kind==RegionRules.NetGate)
            {
                bool punjab=kind==RegionRules.PhulkariGate;
                foreach(float side in new[]{-1f,1f}) NaturalMesh.Branch(root,"Low stall support",new Vector3(side*.86f,0,0),new Vector3(side*.86f,2.4f,0),.055f,wood);
                PrimitiveArt.Box(root,punjab ? "Low Phulkari cloth - slide":"Low drying fishing net - slide",new Vector3(0,1.75f,0),new Vector3(1.92f,.86f,1.36f),punjab ? pink:blue);
                for(int i=0;i<6;i++)
                {
                    var stripe=PrimitiveArt.Box(root,punjab ? "Gold embroidery diamond":"Net vertical weave",new Vector3(-.78f+i*.31f,1.75f,-.69f),new Vector3(punjab ? .14f:.025f,punjab ? .25f:.85f,.03f),punjab ? gold:cream);
                    if(punjab)stripe.localRotation=Quaternion.Euler(0,0,45);
                }
                if(!punjab) for(int i=0;i<4;i++) PrimitiveArt.Box(root,"Net horizontal weave",new Vector3(0,1.42f+i*.22f,-.70f),new Vector3(1.90f,.025f,.03f),cream);
            }
            else if(kind==RegionRules.Crab)
            {
                var coral=C(.97f,.29f,.14f);
                NaturalMesh.Oval(root,"Stylised shore crab shell",new Vector3(0,.22f,0),new Vector3(.85f,.35f,.64f),coral,12);
                foreach(float side in new[]{-1f,1f})
                {
                    for(int leg=0;leg<3;leg++) NaturalMesh.Branch(root,"Crab scuttling leg",new Vector3(side*.3f,.19f,(leg-1)*.19f),new Vector3(side*.83f,.045f,(leg-1)*.31f),.035f,coral);
                    NaturalMesh.Branch(root,"Raised crab claw arm",new Vector3(side*.27f,.24f,.24f),new Vector3(side*.69f,.42f,.43f),.065f,coral);
                    NaturalMesh.Oval(root,"Crab rounded claw",new Vector3(side*.75f,.45f,.45f),new Vector3(.30f,.26f,.30f),coral,10);
                    NaturalMesh.Branch(root,"Crab eye stalk",new Vector3(side*.14f,.29f,.28f),new Vector3(side*.17f,.48f,.35f),.025f,coral);
                    NaturalMesh.Oval(root,"Crab eye",new Vector3(side*.17f,.49f,.35f),Vector3.one*.065f,dark,8);
                }
            }
            else if(kind==RegionRules.BoatCart)
            {
                PrimitiveArt.Box(root,"Fishing canoe handcart",new Vector3(0,.40f,0),new Vector3(1.5f,.22f,1.80f),wood);
                NaturalMesh.Oval(root,"Blue fishing canoe",new Vector3(0,.90f,0),new Vector3(1.60f,.90f,2.50f),blue,14);
                PrimitiveArt.Box(root,"Canoe cream gunwale",new Vector3(0,1.18f,0),new Vector3(1.42f,.10f,1.92f),cream);
                foreach(float side in new[]{-1f,1f})
                {
                    var wheel=PrimitiveArt.Shape(root,"Handcart wheel",PrimitiveType.Cylinder,new Vector3(side*.85f,.32f,0),new Vector3(.61f,.08f,.61f),dark);wheel.localRotation=Quaternion.Euler(0,0,90);
                }
            }
        }
        public static Transform Token(Transform parent,int region)
        {
            var root=new GameObject(region==RegionProfile.Punjab ? "Golden Phulkari flower token":"Golden shell token").transform;root.SetParent(parent,false);
            var gold=C(1,.75f,.15f);var coral=C(.98f,.26f,.36f);
            if(region==RegionProfile.Punjab)
            {
                for(int i=0;i<4;i++)
                {
                    float a=i*Mathf.PI*.5f;
                    var diamond=PrimitiveArt.Box(root,"Phulkari stitch petal",new Vector3(Mathf.Cos(a)*.16f,Mathf.Sin(a)*.16f,0),new Vector3(.20f,.20f,.045f),gold);diamond.localRotation=Quaternion.Euler(0,0,45);
                }
                NaturalMesh.Oval(root,"Pink embroidered centre",Vector3.zero,Vector3.one*.15f,coral,8);
            }
            else
            {
                for(int i=0;i<7;i++)
                {
                    float a=Mathf.Lerp(-1.1f,1.1f,i/6f);
                    NaturalMesh.Branch(root,"Scallop shell gold rib",new Vector3(0,-.21f,0),new Vector3(Mathf.Sin(a)*.29f,Mathf.Cos(a)*.29f,0),.042f,gold);
                }
            }
            PrimitiveArt.CombineScenery(root,true);return root;
        }
    }
}
