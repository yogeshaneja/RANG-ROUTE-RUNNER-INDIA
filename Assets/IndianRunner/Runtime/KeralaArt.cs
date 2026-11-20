using UnityEngine;

namespace IndianRunner
{
    // All meshes are original, low-detail and shared by the obstacle catalogue.
    public static class KeralaArt
    {
        private static Material Colour(float r,float g,float b) => PrimitiveArt.Mat(new Color(r,g,b));
        public static void Build(Transform root,int kind)
        {
            var wood=Colour(.32f,.17f,.075f); var gold=Colour(1,.73f,.18f);
            var green=Colour(.16f,.51f,.20f); var dark=Colour(.09f,.10f,.075f);
            if(kind==RegionRules.Ducks)
            {
                for(int i=0;i<3;i++)
                {
                    var duck=new GameObject("Backwater duck").transform; duck.SetParent(root,false);
                    duck.localPosition=new Vector3((i-1)*.48f,0,(i%2)*.26f);
                    var cream=Colour(.97f,.93f,.78f);
                    NaturalMesh.Oval(duck,"Feathered body",new Vector3(0,.30f,0),new Vector3(.38f,.40f,.55f),cream,10);
                    NaturalMesh.Oval(duck,"Duck head",new Vector3(0,.54f,.19f),Vector3.one*.25f,cream,10);
                    NaturalMesh.Oval(duck,"Orange bill",new Vector3(0,.53f,.36f),new Vector3(.17f,.065f,.20f),gold,8);
                    foreach(float side in new[]{-1f,1f})
                    {
                        NaturalMesh.Oval(duck,"Bright eye",new Vector3(side*.10f,.58f,.24f),Vector3.one*.035f,dark,8);
                        PrimitiveArt.Box(duck,"Webbed foot",new Vector3(side*.085f,.045f,.03f),new Vector3(.12f,.04f,.15f),gold);
                    }
                }
            }
            else if(kind==RegionRules.Boar)
            {
                var hide=Colour(.25f,.20f,.16f);
                NaturalMesh.Oval(root,"Wild boar rounded body",new Vector3(0,.68f,0),new Vector3(.90f,.95f,1.40f),hide,14);
                NaturalMesh.Oval(root,"Boar head",new Vector3(0,.65f,.65f),new Vector3(.62f,.65f,.65f),hide,12);
                NaturalMesh.Oval(root,"Boar snout",new Vector3(0,.54f,.98f),new Vector3(.39f,.29f,.25f),wood,10);
                foreach(float side in new[]{-1f,1f})
                {
                    foreach(float z in new[]{-.43f,.43f}) NaturalMesh.Branch(root,"Short boar leg",new Vector3(side*.28f,.08f,z),new Vector3(side*.28f,.65f,z),.10f,hide);
                    NaturalMesh.Oval(root,"Boar ear",new Vector3(side*.23f,.97f,.56f),new Vector3(.17f,.28f,.14f),hide,10);
                    NaturalMesh.Oval(root,"Boar eye",new Vector3(side*.26f,.75f,.82f),Vector3.one*.055f,dark,8);
                }
            }
            else if(kind==RegionRules.Langur)
            {
                var fur=Colour(.39f,.40f,.32f); var face=Colour(.07f,.085f,.065f);
                NaturalMesh.Oval(root,"Seated langur body",new Vector3(0,.65f,0),new Vector3(.56f,.85f,.50f),fur,12);
                NaturalMesh.Oval(root,"Langur head",new Vector3(0,1.22f,.08f),new Vector3(.42f,.44f,.39f),fur,12);
                NaturalMesh.Oval(root,"Dark langur face",new Vector3(0,1.20f,.25f),new Vector3(.29f,.29f,.10f),face,10);
                foreach(float side in new[]{-1f,1f})
                {
                    NaturalMesh.Branch(root,"Langur arm",new Vector3(side*.30f,.16f,.18f),new Vector3(side*.23f,.92f,0),.065f,fur);
                    NaturalMesh.Oval(root,"Folded langur leg",new Vector3(side*.24f,.19f,.08f),new Vector3(.24f,.32f,.45f),fur,10);
                    NaturalMesh.Oval(root,"Langur eye",new Vector3(side*.075f,1.24f,.30f),Vector3.one*.035f,gold,8);
                }
                NaturalMesh.Branch(root,"Long curling tail base",new Vector3(0,.30f,-.17f),new Vector3(.45f,.37f,-.44f),.04f,fur);
                NaturalMesh.Branch(root,"Long curling tail tip",new Vector3(.45f,.37f,-.44f),new Vector3(.59f,.85f,-.39f),.03f,fur);
            }
            else if(kind==RegionRules.Coconuts || kind==RegionRules.Coir)
            {
                PrimitiveArt.Box(root,"Low coir market pallet",new Vector3(0,.09f,0),new Vector3(1.28f,.18f,.92f),wood);
                if(kind==RegionRules.Coconuts)
                    for(int i=0;i<6;i++) NaturalMesh.Oval(root,"Tender coconut",new Vector3((i%3-1)*.40f,.35f+(i/3)*.30f,(i/3)*.25f-.12f),Vector3.one*.43f,green,10);
                else for(int i=0;i<3;i++)
                {
                    NaturalMesh.Oval(root,"Rolled golden coir mat",new Vector3(0,.29f+i*.21f,0),new Vector3(1.42f,.25f,.73f),gold,12);
                    foreach(float side in new[]{-1f,1f}) PrimitiveArt.Box(root,"Coir binding",new Vector3(side*.44f,.30f+i*.21f,0),new Vector3(.05f,.27f,.75f),wood);
                }
            }
            else if(kind==RegionRules.Bamboo)
            {
                foreach(float side in new[]{-1f,1f}) NaturalMesh.Branch(root,"Bamboo shelter support",new Vector3(side*.86f,0,0),new Vector3(side*.86f,2.40f,0),.055f,wood);
                PrimitiveArt.Box(root,"Low woven palm shelter - slide",new Vector3(0,1.75f,0),new Vector3(1.92f,.86f,1.36f),green);
                for(int i=0;i<6;i++) PrimitiveArt.Box(root,"Woven shelter strip",new Vector3(-.80f+i*.32f,1.75f,-.69f),new Vector3(.04f,.86f,.035f),gold);
            }
            else if(kind==RegionRules.FishCart)
            {
                PrimitiveArt.Box(root,"Blue fish market cart",new Vector3(0,.92f,0),new Vector3(1.70f,1.22f,1.42f),Colour(.035f,.52f,.64f));
                foreach(float side in new[]{-1f,1f})
                {
                    var wheel=PrimitiveArt.Shape(root,"Cart wheel",PrimitiveType.Cylinder,new Vector3(side*.88f,.37f,0),new Vector3(.65f,.07f,.65f),dark); wheel.localRotation=Quaternion.Euler(0,0,90);
                    NaturalMesh.Branch(root,"Cart handle",new Vector3(side*.60f,1.1f,.60f),new Vector3(side*.60f,1.60f,.60f),.045f,wood);
                }
                for(int i=0;i<4;i++) NaturalMesh.Oval(root,"Silver fish in basket",new Vector3((i-1.5f)*.33f,1.58f,0),new Vector3(.21f,.12f,.69f),Colour(.70f,.85f,.82f),10);
            }
            else if(kind==RegionRules.Log)
            {
                var log=NaturalMesh.Lathe(root,"Rain-fallen palm trunk - jump",new[]{new Vector3(.38f,-.73f,.38f),new Vector3(.38f,.73f,.38f)},wood,12);
                log.localPosition=new Vector3(0,.39f,0); log.localRotation=Quaternion.Euler(0,0,90);
                foreach(float side in new[]{-1f,1f}) NaturalMesh.Oval(root,"Cut palm end",new Vector3(side*.75f,.39f,0),new Vector3(.025f,.64f,.64f),gold,10);
            }
        }
        public static Transform Token(Transform parent)
        {
            var root=new GameObject("Golden lotus token").transform; root.SetParent(parent,false);
            var gold=Colour(1,.75f,.12f); var coral=Colour(1,.38f,.34f);
            for(int i=0;i<6;i++)
            {
                float a=i*Mathf.PI/3;
                NaturalMesh.Oval(root,"Lotus petal",new Vector3(Mathf.Cos(a)*.17f,Mathf.Sin(a)*.17f,0),new Vector3(.23f,.23f,.06f),gold,8);
            }
            NaturalMesh.Oval(root,"Coral lotus centre",Vector3.zero,new Vector3(.24f,.24f,.08f),coral,10);
            PrimitiveArt.CombineScenery(root,true); return root;
        }
        public static Transform Garden(Transform parent)
        {
            var root=new GameObject("Backwater rest garden").transform; root.SetParent(parent,false);
            foreach(float side in new[]{-1f,1f})
            {
                PrimitiveArt.Box(root,"Garden planter",new Vector3(side*5.2f,.20f,0),new Vector3(.8f,.4f,3),Colour(.75f,.29f,.15f));
                for(int i=0;i<4;i++) NaturalMesh.Oval(root,"Bright hibiscus shrub",new Vector3(side*5.2f,.60f,-1+i*.65f),new Vector3(.7f,.8f,.7f),Colour(.14f,.50f,.19f),10);
            }
            PrimitiveArt.CombineScenery(root,true); return root;
        }
    }
}
