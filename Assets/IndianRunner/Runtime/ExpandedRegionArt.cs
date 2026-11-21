using UnityEngine;

namespace IndianRunner
{
    // Original low-poly regional props. Every mesh is built once then shared/batched.
    public static class ExpandedRegionArt
    {
        public static Material Colour(float r,float g,float b) => AdventureArt.Colour(r,g,b);
        public static Color Accent(int region) => region==RegionProfile.Shillong ? new Color(.13f,.62f,.67f):region==RegionProfile.Kolkata ? new Color(.91f,.38f,.22f):region==RegionProfile.MadhyaPradesh ? new Color(.28f,.56f,.24f):new Color(.72f,.16f,.28f);
        public static Color Secondary(int region) => region==RegionProfile.Shillong ? new Color(.97f,.58f,.26f):region==RegionProfile.Kolkata ? new Color(.98f,.79f,.19f):region==RegionProfile.MadhyaPradesh ? new Color(.94f,.62f,.18f):new Color(.10f,.55f,.65f);
        public static Vector3 Shape(int kind)
        {
            if(kind==RegionRules.HillBus)return new Vector3(1.08f,2.45f,1.55f);
            if(kind==RegionRules.YellowTaxi)return new Vector3(1.02f,1.60f,1.50f);
            if(kind==RegionRules.SafariJeep)return new Vector3(1.04f,1.95f,1.45f);
            if(kind==RegionRules.Deer)return new Vector3(.63f,1.78f,1.10f);
            if(kind==RegionRules.Sheep)return new Vector3(.57f,1.06f,.92f);
            if(RegionRules.IsSlide(kind))return new Vector3(.96f,2.40f,.68f);
            return new Vector3(.72f,.90f,.47f);
        }
        public static string Warning(int kind)
        {
            switch(kind)
            {
                case RegionRules.PineappleBasket:return "FRUIT BASKETS - jump";
                case RegionRules.HillBus:return "HILL BUS COMING - change lane";
                case RegionRules.SweetCrates:return "SWEET SHOP CRATES - jump";
                case RegionRules.FlowerAwning:return "LOW FLOWER STALL - slide";
                case RegionRules.YellowTaxi:return "YELLOW TAXI - change lane";
                case RegionRules.ForestBaskets:return "FOREST CRAFT BASKETS - jump";
                case RegionRules.ForestBranch:return "LOW LEAF CANOPY - slide";
                case RegionRules.Deer:return "DEER AHEAD - give it room";
                case RegionRules.SafariJeep:return "SUPPLY JEEP - change lane";
                case RegionRules.AppleCrates:return "APPLE CRATES - jump";
                case RegionRules.WoolAwning:return "LOW WOOL STALL - slide";
                default:return "SHEEP AHEAD - give it room";
            }
        }
        public static void Obstacle(Transform root,int kind)
        {
            var gold=Colour(1,.73f,.15f);var cream=Colour(.98f,.91f,.74f);var wood=Colour(.40f,.24f,.12f);
            var teal=Colour(.035f,.52f,.59f);var dark=Colour(.055f,.085f,.095f);var coral=Colour(.93f,.20f,.25f);
            if(kind==RegionRules.Deer || kind==RegionRules.Sheep){Animal(root,kind,false);return;}
            if(RegionRules.IsTraffic(kind))
            {
                bool bus=kind==RegionRules.HillBus,taxi=kind==RegionRules.YellowTaxi;
                var paint=bus ? teal:taxi ? gold:Colour(.26f,.48f,.27f);
                PrimitiveArt.Box(root,"Rounded regional vehicle chassis",new Vector3(0,.56f,0),new Vector3(1.72f,.40f,2.78f),paint);
                PrimitiveArt.Box(root,"Bonnet and lower body",new Vector3(0,.88f,.55f),new Vector3(1.68f,.38f,1.55f),paint);
                float roof=bus ? 2.34f:taxi ? 1.50f:1.85f;
                PrimitiveArt.Box(root,"Cream cabin roof",new Vector3(0,roof, bus ? 0:-.18f),new Vector3(1.75f,.14f,bus ? 2.75f:1.68f),cream);
                foreach(float side in new[]{-1f,1f})
                {
                    PrimitiveArt.Box(root,"Cabin door panel",new Vector3(side*.82f,1.05f,-.35f),new Vector3(.10f,.72f,1.4f),paint);
                    PrimitiveArt.Box(root,"Blue side window",new Vector3(side*.84f,(roof+1.20f)*.5f,-.22f),new Vector3(.045f,roof-1.22f,1.31f),dark);
                    foreach(float z in new[]{-.93f,.93f})
                    {
                        var wheel=PrimitiveArt.Shape(root,"Rubber vehicle wheel",PrimitiveType.Cylinder,new Vector3(side*.87f,.32f,z),new Vector3(.64f,.095f,.64f),dark);wheel.localRotation=Quaternion.Euler(0,0,90);
                        NaturalMesh.Oval(root,"Golden wheel hub",new Vector3(side*.98f,.32f,z),new Vector3(.025f,.27f,.27f),gold,8);
                    }
                    NaturalMesh.Oval(root,"Warm vehicle headlamp",new Vector3(side*.58f,.83f,1.37f),new Vector3(.24f,.20f,.035f),cream,10);
                }
                PrimitiveArt.Box(root,"Cabin front windshield",new Vector3(0,(roof+1.20f)*.5f,bus ? 1.28f:.61f),new Vector3(1.48f,roof-1.22f,.05f),dark);
                PrimitiveArt.Box(root,"Visible front bumper",new Vector3(0,.46f,1.43f),new Vector3(1.76f,.12f,.12f),cream);
                if(taxi)PrimitiveArt.Box(root,"Taxi rooftop lamp",new Vector3(0,1.56f,-.1f),new Vector3(.40f,.08f,.22f),gold);
                if(bus)for(int i=0;i<3;i++)PrimitiveArt.Box(root,"Hill bus window division",new Vector3(0,1.70f,1.32f),new Vector3(.08f,.92f,.045f),cream).localPosition+=Vector3.right*((i-1)*.48f);
                return;
            }
            if(RegionRules.IsSlide(kind))
            {
                foreach(float side in new[]{-1f,1f})PrimitiveArt.Box(root,"Visible slide-gate support",new Vector3(side*.86f,1.20f,0),new Vector3(.11f,2.40f,.11f),wood);
                PrimitiveArt.Box(root,"Clearly low regional canopy",new Vector3(0,1.75f,0),new Vector3(1.92f,.86f,1.36f),kind==RegionRules.ForestBranch ? teal:coral);
                for(int i=0;i<6;i++)
                {
                    float x=-.78f+i*.31f;
                    NaturalMesh.Oval(root,kind==RegionRules.FlowerAwning ? "Marigold flower fringe":kind==RegionRules.WoolAwning ? "Colourful wool fringe":"Bright canopy leaf",new Vector3(x,1.50f,-.70f),new Vector3(.22f,.25f,.055f),i%2==0 ? gold:cream,8);
                }
                return;
            }
            PrimitiveArt.Box(root,"Low regional goods crate",new Vector3(0,.28f,0),new Vector3(1.38f,.56f,.88f),wood);
            for(int rail=0;rail<3;rail++)PrimitiveArt.Box(root,"Crate golden slat",new Vector3(0,.12f+rail*.18f,-.45f),new Vector3(1.40f,.055f,.035f),gold);
            for(int i=0;i<6;i++)
            {
                var p=new Vector3((i%3-1)*.38f,.64f,(i/3-.5f)*.35f);
                Material fruit=kind==RegionRules.AppleCrates ? coral:kind==RegionRules.ForestBaskets ? teal:kind==RegionRules.SweetCrates ? cream:gold;
                NaturalMesh.Oval(root,"Regional produce or craft bundle",p,new Vector3(.27f,.30f,.27f),fruit,10);
                if(kind==RegionRules.PineappleBasket || kind==RegionRules.AppleCrates)NaturalMesh.Branch(root,"Small fruit leaf",p+Vector3.up*.10f,p+new Vector3(.05f,.23f,0),.025f,teal);
            }
        }
        public static void Animal(Transform root,int kind,bool withLegs=true)
        {
            bool deer=kind==RegionRules.Deer;
            var hide=deer ? Colour(.68f,.39f,.19f):Colour(.94f,.89f,.72f);var dark=Colour(.11f,.09f,.07f);var cream=Colour(1,.95f,.77f);
            NaturalMesh.Oval(root,deer ? "Slender spotted deer body":"Soft wool sheep body",new Vector3(0,.72f,0),new Vector3(deer ? .78f:.98f,.66f,1.40f),hide,14);
            if(withLegs)foreach(float side in new[]{-1f,1f})foreach(float z in new[]{-.48f,.46f})
            {
                NaturalMesh.Branch(root,"Tapered animal leg",new Vector3(side*.29f,.62f,z),new Vector3(side*.30f,.08f,z+.04f),deer ? .045f:.065f,hide);
                NaturalMesh.Oval(root,"Small animal hoof",new Vector3(side*.30f,.07f,z+.08f),new Vector3(.14f,.12f,.20f),dark,8);
            }
            NaturalMesh.Oval(root,"Raised animal head",new Vector3(0,deer ? 1.24f:.78f,.74f),new Vector3(.35f,.47f,.43f),hide,12);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(root,"Expressive animal ear",new Vector3(side*.24f,deer ? 1.38f:.87f,.73f),new Vector3(.27f,.14f,.13f),hide,8);
                NaturalMesh.Oval(root,"Bright animal eye",new Vector3(side*.13f,deer ? 1.28f:.82f,.93f),Vector3.one*.045f,dark,8);
                if(deer)
                {
                    NaturalMesh.Branch(root,"Deer antler",new Vector3(side*.11f,1.43f,.70f),new Vector3(side*.28f,1.72f,.56f),.025f,cream);
                    NaturalMesh.Branch(root,"Deer antler tine",new Vector3(side*.20f,1.58f,.64f),new Vector3(side*.40f,1.68f,.70f),.018f,cream);
                    for(int dot=0;dot<5;dot++)NaturalMesh.Oval(root,"Cream deer coat spot",new Vector3(side*.38f,.83f+(dot%2)*.08f,-.43f+dot*.20f),Vector3.one*.075f,cream,6);
                }
            }
        }
        public static Transform Token(Transform parent,int region)
        {
            var root=new GameObject("Regional "+RegionProfile.TokenName(region)+" token").transform;root.SetParent(parent,false);
            var gold=Colour(1,.76f,.16f);var accent=PrimitiveArt.Mat(Accent(region));var cream=Colour(1,.94f,.75f);
            if(region==RegionProfile.Shillong)
            {
                for(int ring=0;ring<3;ring++)for(int scale=0;scale<5;scale++)
                {
                    float a=scale*Mathf.PI*2/5+ring*.45f;
                    NaturalMesh.Oval(root,"Golden pine-cone scale",new Vector3(Mathf.Cos(a)*(.12f-ring*.025f),-.14f+ring*.14f,Mathf.Sin(a)*(.08f-ring*.01f)),new Vector3(.17f,.18f,.10f),gold,6);
                }
            }
            else if(region==RegionProfile.Kolkata)
            {
                NaturalMesh.Lathe(root,"Golden terracotta tea cup",new[]{new Vector3(.13f,-.24f,.13f),new Vector3(.21f,.19f,.21f)},gold,12);
                NaturalMesh.Oval(root,"Warm tea surface",new Vector3(0,.18f,0),new Vector3(.36f,.04f,.36f),accent,10);
                NaturalMesh.Oval(root,"Tea cup gleam",new Vector3(-.09f,0,-.18f),new Vector3(.04f,.22f,.025f),cream,8);
            }
            else if(region==RegionProfile.MadhyaPradesh)
            {
                var leaf=NaturalMesh.Oval(root,"Golden forest leaf",Vector3.zero,new Vector3(.32f,.60f,.07f),gold,12);leaf.localRotation=Quaternion.Euler(0,0,-25);
                NaturalMesh.Branch(root,"Leaf vein",new Vector3(-.12f,-.24f,-.06f),new Vector3(.12f,.24f,-.06f),.018f,accent);
            }
            else
            {
                foreach(float side in new[]{-1f,1f})NaturalMesh.Oval(root,"Golden apple lobe",new Vector3(side*.10f,0,0),new Vector3(.33f,.44f,.27f),gold,12);
                NaturalMesh.Branch(root,"Apple stem",new Vector3(0,.16f,0),new Vector3(.03f,.31f,0),.025f,accent);
                NaturalMesh.Oval(root,"Apple leaf",new Vector3(.14f,.24f,0),new Vector3(.20f,.10f,.04f),accent,8);
            }
            PrimitiveArt.CombineScenery(root,true);return root;
        }
        public static void FestivalEmblem(Transform parent,int region,Vector3 p,Material accent,Material gold)
        {
            for(int i=0;i<5;i++)
            {
                float a=i*Mathf.PI*2/5;
                NaturalMesh.Oval(parent,region==RegionProfile.Himachal ? "Apple blossom festival petal":"Regional flower emblem",p+new Vector3(Mathf.Cos(a)*.37f,Mathf.Sin(a)*.37f,0),new Vector3(.42f,.42f,.10f),accent,8);
            }
            NaturalMesh.Oval(parent,"Golden festival centre",p-new Vector3(0,0,.07f),Vector3.one*.27f,gold,8);
        }
        public static void GuardianHead(Transform head,int region,Material accent,Material secondary,Material gold)
        {
            NaturalMesh.Oval(head,region==RegionProfile.Himachal ? "Knitted valley guide cap":"Original regional fantasy explorer cap",new Vector3(0,3.5f,0),new Vector3(8.7f,2.2f,6.1f),accent,14);
            NaturalMesh.Oval(head,"Colourful cap band",new Vector3(0,3,-.2f),new Vector3(9.2f,.65f,6.3f),secondary,14);
            if(region==RegionProfile.Kolkata)
            {
                foreach(float side in new[]{-1f,1f})NaturalMesh.Branch(head,"Fantasy bridge crown support",new Vector3(side*3.4f,3.3f,0),new Vector3(side*2,6,0),.15f,gold);
                NaturalMesh.Branch(head,"Fantasy bridge crown span",new Vector3(-2,6,0),new Vector3(2,6,0),.12f,gold);
            }
            else if(region==RegionProfile.Shillong)NaturalMesh.Oval(head,"Broad rain explorer brim",new Vector3(0,3.1f,0),new Vector3(11,.40f,7.8f),secondary,14);
            else if(region==RegionProfile.MadhyaPradesh)for(int i=0;i<3;i++)NaturalMesh.Oval(head,"Forest leaf crest",new Vector3((i-1)*1.3f,5,0),new Vector3(1.8f,2.7f,.7f),secondary,10);
            else FestivalEmblem(head,region,new Vector3(0,3.2f,-3.4f),secondary,gold);
        }
    }
}
