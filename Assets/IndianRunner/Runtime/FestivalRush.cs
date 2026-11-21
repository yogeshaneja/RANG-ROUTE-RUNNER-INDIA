using UnityEngine;

namespace IndianRunner
{
    // Original procedural scenery and one fixed petal pool; no textures or colliders.
    public sealed class FestivalRush
    {
        private readonly Transform root;
        private readonly Transform[] petals=new Transform[24];
        public FestivalRush(Transform parent,int region)
        {
            root=new GameObject("Reusable Festival Rush petal shower").transform;root.SetParent(parent,false);
            for(int i=0;i<petals.Length;i++)
                petals[i]=NaturalMesh.Oval(root,"Lightweight festival flower petal",Vector3.zero,
                    new Vector3(.11f,.025f,.19f),Palette(region,i%3),6);
            Reset();
        }
        private static Material Palette(int region,int index)
        {
            if(ExpandedRegionRules.IsExpanded(region))return PrimitiveArt.Mat(index==1 ? ExpandedRegionArt.Secondary(region):index==2 ? ExpandedRegionArt.Accent(region):new Color(1,.73f,.15f));
            if(index==0)return AdventureArt.Colour(1,.68f,.08f);
            if(index==1)return region==RegionProfile.Kerala ? AdventureArt.Colour(.99f,.94f,.73f):AdventureArt.Colour(.98f,.16f,.47f);
            return region==RegionProfile.Punjab ? AdventureArt.Colour(.66f,.16f,.79f):AdventureArt.Colour(.04f,.72f,.76f);
        }
        public void Reset() => root.gameObject.SetActive(false);
        public void Tick(float clock,bool active,bool reduced)
        {
            if(root.gameObject.activeSelf!=active)root.gameObject.SetActive(active);
            if(!active)return;
            for(int i=0;i<petals.Length;i++)
            {
                bool visible=i<(reduced ? 6:petals.Length);
                if(petals[i].gameObject.activeSelf!=visible)petals[i].gameObject.SetActive(visible);
                if(!visible)continue;
                float t=Mathf.Repeat(clock*(reduced ? .12f:.28f)+i*.618034f,1);
                // Keep the road silhouette clear: petals stay above the runner's jump.
                petals[i].localPosition=new Vector3((i%2==0 ? -1:1)*(2.1f+1.5f*Mathf.Sin(i*2.4f+clock*.7f)),7.6f-t*3.4f,3+i%7*2.3f);
                petals[i].localRotation=Quaternion.Euler(reduced ? 0:clock*52+i*23,i*47,reduced ? 12:Mathf.Sin(clock*2+i)*35);
            }
        }
        public static Transform Tile(Transform parent,int region,float length)
        {
            var root=new GameObject("Pooled colourful regional Festival Rush avenue").transform;root.SetParent(parent,false);
            var gold=Palette(region,0);var pink=Palette(region,1);var aqua=Palette(region,2);
            var cream=AdventureArt.Colour(1,.92f,.70f);var wood=AdventureArt.Colour(.43f,.22f,.12f);
            foreach(float side in new[]{-1f,1f})for(int n=0;n<2;n++)
            {
                float z=7+n*28;
                NaturalMesh.Branch(root,"Tall festival gateway pole",new Vector3(side*5.1f,0,z),new Vector3(side*5.1f,6.4f,z),.09f,wood);
                NaturalMesh.Oval(root,"Golden gateway finial",new Vector3(side*5.1f,6.5f,z),Vector3.one*.36f,gold,8);
                for(int ribbon=0;ribbon<3;ribbon++)
                    PrimitiveArt.Box(root,"Colourful side ribbon",new Vector3(side*(5.2f+ribbon*.14f),4.4f,z),new Vector3(.10f,2.4f,.035f),ribbon==0 ? gold:ribbon==1 ? pink:aqua);
                // Flower rosettes remain beyond the outer lane's collision envelope.
                for(int petal=0;petal<6;petal++)
                {
                    float a=petal*Mathf.PI/3;
                    NaturalMesh.Oval(root,"Festival roadside flower rosette",new Vector3(side*5.2f+Mathf.Cos(a)*.30f,2.35f+Mathf.Sin(a)*.30f,z-.12f),new Vector3(.39f,.39f,.10f),pink,8);
                }
                NaturalMesh.Oval(root,"Flower golden centre",new Vector3(side*5.2f,2.35f,z-.21f),new Vector3(.30f,.30f,.14f),gold,8);
                if(ExpandedRegionRules.IsExpanded(region))
                {
                    ExpandedRegionArt.FestivalEmblem(root,region,new Vector3(side*6.6f,3.7f,z+5),aqua,gold);
                    PrimitiveArt.Box(root,"Regional festival colour ribbon",new Vector3(side*6.6f,2.3f,z+5),new Vector3(.8f,1.7f,.05f),pink);
                }
                else if(region==RegionProfile.Rajasthan)
                {
                    var kite=PrimitiveArt.Box(root,"Bright desert diamond kite",new Vector3(side*7,6.7f,z+5),new Vector3(1.1f,1.1f,.055f),aqua);
                    kite.localRotation=Quaternion.Euler(0,side*20,45);
                    NaturalMesh.Branch(root,"Kite ribbon tail",new Vector3(side*7,5.9f,z+5),new Vector3(side*7.4f,4.9f,z+5),.025f,pink);
                }
                else if(region==RegionProfile.Kerala)
                {
                    NaturalMesh.Oval(root,"Flower festival ceremonial parasol",new Vector3(side*7,3.65f,z+5),new Vector3(2.2f,.48f,2.2f),gold,12);
                    NaturalMesh.Branch(root,"Parasol stem",new Vector3(side*7,0,z+5),new Vector3(side*7,3.6f,z+5),.055f,wood);
                }
                else
                {
                    PrimitiveArt.Box(root,region==RegionProfile.Punjab ? "Phulkari inspired colour panel":"Coastal carnival colour panel",new Vector3(side*6.5f,2.6f,z+5),new Vector3(1.5f,2,.08f),aqua);
                    for(int stitch=0;stitch<3;stitch++)
                    {
                        var diamond=PrimitiveArt.Box(root,"Golden festival diamond motif",new Vector3(side*6.5f,2+stitch*.60f,z+4.94f),new Vector3(.32f,.32f,.04f),stitch%2==0 ? gold:pink);
                        diamond.localRotation=Quaternion.Euler(0,0,45);
                    }
                }
            }
            for(int n=0;n<2;n++)for(int bead=0;bead<13;bead++)
            {
                float x=(bead-6)*.85f,y=6.35f-.8f*(1-x*x/26.01f),z=7+n*28;
                if(bead>0)
                {
                    float previousX=x-.85f,previousY=6.35f-.8f*(1-previousX*previousX/26.01f);
                    NaturalMesh.Branch(root,"Continuous high garland string",new Vector3(previousX,previousY,z),new Vector3(x,y,z),.025f,cream);
                }
                NaturalMesh.Oval(root,"High marigold garland",new Vector3(x,y,z),new Vector3(.42f,.35f,.35f),bead%3==0 ? pink:gold,8);
                if(bead%3==0)NaturalMesh.Oval(root,"Hanging jewel lantern",new Vector3(x,y-.50f,z),new Vector3(.30f,.48f,.30f),aqua,8);
            }
            PrimitiveArt.CombineScenery(root,true);root.gameObject.SetActive(false);return root;
        }
    }
}
