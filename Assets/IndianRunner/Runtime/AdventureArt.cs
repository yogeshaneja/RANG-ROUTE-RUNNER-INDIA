using UnityEngine;

namespace IndianRunner
{
    // Region-only decorative geometry is baked into seven reusable tile overlays.
    public static class AdventureArt
    {
        public static Material Colour(float r,float g,float b) => PrimitiveArt.Mat(new Color(r,g,b));
        public static Vector3 RideObstacleShape(int region) => region==RegionProfile.Rajasthan ? new Vector3(1.176f,2.408f,.952f):FestivalObstacles.Shape(AdventureRules.RideObstacle(region));
        public static void BuildBuoy(Transform root)
        {
            var coral=Colour(.96f,.25f,.25f);var gold=Colour(1,.76f,.15f);var teal=Colour(.025f,.57f,.61f);
            NaturalMesh.Oval(root,"Floating turquoise canal marker base",new Vector3(0,.19f,0),new Vector3(.96f,.31f,.96f),teal,14);
            NaturalMesh.Lathe(root,"Coral tapered navigation buoy",new[]{new Vector3(.29f,.22f,.29f),new Vector3(.25f,1.22f,.25f),new Vector3(.12f,1.55f,.12f)},coral,14);
            NaturalMesh.Oval(root,"Golden buoy band",new Vector3(0,.80f,0),new Vector3(.55f,.20f,.55f),gold,12);
            NaturalMesh.Oval(root,"Buoy top reflector",new Vector3(0,1.64f,0),Vector3.one*.23f,gold,12);
        }
        public static Transform Tile(Transform parent,int region,float length,bool ride)
        {
            var root=new GameObject(ride ? "Reusable regional vehicle stretch":"Reusable regional adventure passage").transform;root.SetParent(parent,false);
            var gold=Colour(1,.75f,.12f);var coral=Colour(.96f,.23f,.32f);var teal=Colour(.035f,.58f,.60f);
            if(ExpandedRegionRules.IsExpanded(region)){teal=PrimitiveArt.Mat(ExpandedRegionArt.Accent(region));coral=PrimitiveArt.Mat(ExpandedRegionArt.Secondary(region));}
            var cream=Colour(.99f,.89f,.65f);var wood=Colour(.39f,.21f,.10f);var leaf=Colour(.13f,.48f,.24f);
            if(region==RegionProfile.Kerala)
            {
                PrimitiveArt.Box(root,"Turquoise channel below the adventure",new Vector3(0,ride ? .05f:.003f,length*.5f),new Vector3(28,.012f,length),teal);
                if(!ride)
                {
                    PrimitiveArt.Box(root,"Canopy bridge continuous deck",new Vector3(0,.016f,length*.5f),new Vector3(8.7f,.004f,length),wood);
                    for(int i=0;i<14;i++)PrimitiveArt.Box(root,"Bridge plank seam",new Vector3(0,.019f,i*4),new Vector3(8.7f,.002f,.06f),cream);
                }
            }
            else if(region==RegionProfile.Goa || region==RegionProfile.Shillong)
            {
                PrimitiveArt.Box(root,"Seaside boardwalk planks",new Vector3(0,.016f,length*.5f),new Vector3(8.7f,.004f,length),cream);
                for(int i=0;i<14;i++)PrimitiveArt.Box(root,"Boardwalk plank joints",new Vector3(0,.019f,i*4),new Vector3(8.7f,.002f,.055f),wood);
            }
            foreach(float side in new[]{-1f,1f})
            {
                if(region==RegionProfile.Rajasthan && !ride)
                    PrimitiveArt.Box(root,"Sandstone fort wall",new Vector3(side*5.15f,2.4f,length*.5f),new Vector3(.65f,4.8f,length),Colour(.83f,.43f,.23f));
                for(int i=0;i<4;i++)
                {
                    float z=3+i*14;
                    NaturalMesh.Branch(root,"Colourful roadside adventure post",new Vector3(side*4.8f,0,z),new Vector3(side*4.8f,4.8f,z),.07f,wood);
                    NaturalMesh.Oval(root,"Warm hanging lantern",new Vector3(side*4.6f,3.45f,z),new Vector3(.43f,.65f,.43f),i%2==0 ? gold:coral,10);
                    if(region==RegionProfile.Kerala || region==RegionProfile.Goa || ExpandedRegionRules.IsExpanded(region))
                        PrimitiveArt.Box(root,"Continuous bridge handrail",new Vector3(side*4.8f,1.25f,z+3.5f),new Vector3(.14f,.14f,14),ride ? cream:leaf);
                }
            }
            for(int arch=0;arch<2;arch++)
            {
                float z=5+arch*28;
                if(region==RegionProfile.Rajasthan && !ride)NaturalMesh.Arch(root,new Vector3(0,.18f,z),5.4f,gold);
                else
                {
                    NaturalMesh.Branch(root,"High festival rope",new Vector3(-4.8f,4.8f,z),new Vector3(4.8f,4.8f,z),.035f,cream);
                    for(int flag=0;flag<9;flag++)
                    {
                        var pennant=PrimitiveArt.Box(root,"Bright high festival pennant",new Vector3((flag-4)*.95f,4.45f,z),new Vector3(.47f,.61f,.035f),flag%3==0 ? coral:flag%3==1 ? gold:teal);
                        pennant.localRotation=Quaternion.Euler(0,0,flag%2==0 ? 12:-12);
                    }
                }
            }
            PrimitiveArt.CombineScenery(root,true);root.gameObject.SetActive(false);return root;
        }
    }
}
