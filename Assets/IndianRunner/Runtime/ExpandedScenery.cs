using UnityEngine;

namespace IndianRunner
{
    // Only the selected destination is constructed, in the existing seven-tile pool.
    public sealed class ExpandedScenery
    {
        private readonly int region;
        private readonly Material green,deepGreen,cream,wood,water,stone,accent,gold,red,glass;
        public Material RoadSurface { get; }
        public ExpandedScenery(Transform parent,int region)
        {
            this.region=region;
            green=ExpandedRegionArt.Colour(.23f,.51f,.25f);deepGreen=ExpandedRegionArt.Colour(.08f,.29f,.23f);
            cream=ExpandedRegionArt.Colour(.98f,.91f,.74f);wood=ExpandedRegionArt.Colour(.37f,.24f,.15f);
            water=ExpandedRegionArt.Colour(.06f,.56f,.66f);stone=ExpandedRegionArt.Colour(.59f,.64f,.62f);
            accent=PrimitiveArt.Mat(ExpandedRegionArt.Accent(region));gold=ExpandedRegionArt.Colour(1,.75f,.15f);
            red=ExpandedRegionArt.Colour(.83f,.14f,.22f);glass=ExpandedRegionArt.Colour(.10f,.26f,.32f);
            RoadSurface=ExpandedRegionArt.Colour(region==RegionProfile.MadhyaPradesh ? .52f:.32f,region==RegionProfile.MadhyaPradesh ? .36f:.36f,region==RegionProfile.MadhyaPradesh ? .22f:.37f);
            var horizon=new GameObject("Distinct "+RegionProfile.Name(region)+" horizon").transform;horizon.SetParent(parent,false);
            if(region==RegionProfile.Kolkata)Bridge(horizon,new Vector3(0,1,177),true);
            else for(int i=0;i<5;i++)
            {
                var p=new Vector3((i-2)*46,-5,175+i%2*12);
                if(region==RegionProfile.Himachal)
                {
                    var peak=NaturalMesh.Lathe(horizon,"Distant Himalayan peak",new[]{new Vector3(34,0,24),new Vector3(22,22,15),new Vector3(1,47-i%2*8,1)},stone,10);peak.localPosition=p;
                    var snow=NaturalMesh.Lathe(horizon,"Distant snow cap",new[]{new Vector3(17,29,11),new Vector3(1,47-i%2*8+.1f,1)},cream,10);snow.localPosition=p;
                }
                else NaturalMesh.Oval(horizon,region==RegionProfile.Shillong ? "Pine-covered Khasi hill":"Distant forest ridge",p,new Vector3(90,region==RegionProfile.Shillong ? 48:31,44),deepGreen,12);
            }
            PrimitiveArt.CombineScenery(horizon,true);
        }
        public void AddGround(Transform root,int variant,float length)
        {
            PrimitiveArt.Box(root,"Regional terrain",new Vector3(0,-.19f,length*.5f),new Vector3(92,.28f,length),region==RegionProfile.Kolkata ? stone:green);
            if(region==RegionProfile.Shillong || region==RegionProfile.Kolkata || region==RegionProfile.MadhyaPradesh)
            {
                PrimitiveArt.Box(root,region==RegionProfile.Kolkata ? "Hooghly inspired riverfront":region==RegionProfile.Shillong ? "Hill lake beside route":"Forest river beside route",new Vector3(-28,-.12f,length*.5f),new Vector3(40,.05f,length),water);
                for(int i=0;i<5;i++)PrimitiveArt.Box(root,"Soft river ripple",new Vector3(-11-i%3*3,-.085f,4+i*10),new Vector3(2.4f,.012f,.08f),cream);
            }
            foreach(float side in new[]{-1f,1f})
            {
                if(region==RegionProfile.Kolkata)PrimitiveArt.Box(root,"Wide riverfront pavement",new Vector3(side*6,.03f,length*.5f),new Vector3(3.2f,.06f,length),cream);
                for(int i=0;i<4;i++)
                {
                    float z=3+i*14;
                    NaturalMesh.Branch(root,"Visible roadside guardrail post",new Vector3(side*4.9f,0,z),new Vector3(side*4.9f,1.1f,z),.065f,wood);
                    PrimitiveArt.Box(root,"Continuous scenic roadside guardrail",new Vector3(side*4.9f,.84f,z+6),new Vector3(.12f,.12f,14),region==RegionProfile.Kolkata ? accent:wood);
                }
            }
            if(region==RegionProfile.Himachal && variant%2==0)for(int i=0;i<5;i++)NaturalMesh.Oval(root,"Rocky orchard verge",new Vector3(7+i%2*2,.35f,i*12),new Vector3(2,.8f,1.4f),stone,9);
        }
        public void AddGeometry(Transform root,int variant)
        {
            if(region==RegionProfile.Kolkata)
            {
                HeritageHouse(root,new Vector3(10,0,12),variant);
                HeritageHouse(root,new Vector3(11,0,40),variant+1);
                Tram(root,new Vector3(8.2f,0,29));
                for(int n=0;n<3;n++)
                {
                    float z=5+n*20;
                    NaturalMesh.Branch(root,"Riverfront lamp post",new Vector3(-5.7f,0,z),new Vector3(-5.7f,3.9f,z),.065f,deepGreen);
                    NaturalMesh.Oval(root,"Warm riverfront lantern",new Vector3(-5.7f,4,z),new Vector3(.38f,.55f,.38f),gold,10);
                }
                var ferry=new GameObject("Distant river ferry").transform;ferry.SetParent(root,false);ferry.localPosition=new Vector3(-19,0,28);
                NaturalMesh.Oval(ferry,"Teal ferry hull",Vector3.zero,new Vector3(2.1f,.60f,6),accent,12);
                PrimitiveArt.Box(ferry,"Cream ferry cabin",new Vector3(0,.65f,0),new Vector3(1.6f,1.1f,3.9f),cream);
                var dog=new GameObject("Off-road Kolkata street dog").transform;dog.SetParent(root,false);dog.localPosition=new Vector3(6,0,7);RegionalArt.Build(dog,RegionRules.Dog);
                return;
            }
            for(int sideIndex=0;sideIndex<2;sideIndex++)
            {
                float side=sideIndex==0 ? -1:1;
                for(int i=0;i<3;i++)
                {
                    Vector3 p=new Vector3(side*(region==RegionProfile.MadhyaPradesh ? 9:10)+side*(i%2)*3,0,4+i*19);
                    Tree(root,p,region!=RegionProfile.MadhyaPradesh);
                }
            }
            if(region==RegionProfile.Shillong)
            {
                Cottage(root,new Vector3(10,0,26),variant,false);
                if(variant%3==0)
                {
                    NaturalMesh.Oval(root,"Green waterfall cliff",new Vector3(-28,3,32),new Vector3(12,12,9),deepGreen,10);
                    PrimitiveArt.Box(root,"Distant waterfall ribbon",new Vector3(-23,3.4f,28),new Vector3(2.1f,7.4f,.15f),cream);
                    Bridge(root,new Vector3(-17,.3f,17),false);
                }
                var goat=new GameObject("Roadside hill goat").transform;goat.SetParent(root,false);goat.localPosition=new Vector3(6.6f,0,44);RegionalArt.Build(goat,RegionRules.Goat);
            }
            else if(region==RegionProfile.MadhyaPradesh)
            {
                Cottage(root,new Vector3(10.5f,0,25),variant,true);
                for(int i=0;i<4;i++)NaturalMesh.Oval(root,"Pale riverside rock formation",new Vector3(-8-i*3,1.5f+i%2,7+i*12),new Vector3(3.1f,3+i%2,3.8f),cream,10);
                var deer=new GameObject("Off-road spotted deer").transform;deer.SetParent(root,false);deer.localPosition=new Vector3(6.8f,0,40);ExpandedRegionArt.Animal(deer,RegionRules.Deer);
                if(variant%2==0)PrimitiveArt.Peacock(root,7,12,accent,deepGreen);
            }
            else
            {
                Cottage(root,new Vector3(10.5f,0,21),variant,false);
                for(int i=0;i<3;i++)AppleTree(root,new Vector3(7.1f+i%2*3,0,7+i*20));
                var sheep=new GameObject("Off-road orchard sheep").transform;sheep.SetParent(root,false);sheep.localPosition=new Vector3(-6.8f,0,35);ExpandedRegionArt.Animal(sheep,RegionRules.Sheep);
            }
        }
        private void Tree(Transform root,Vector3 p,bool pine)
        {
            NaturalMesh.Branch(root,pine ? "Conifer trunk":"Sal inspired tree trunk",p,p+Vector3.up*6,.18f,wood);
            if(pine)for(int tier=0;tier<3;tier++)
            {
                float radius=2.1f-tier*.40f;
                var crown=NaturalMesh.Lathe(root,"Layered conifer crown",new[]{new Vector3(radius,2.4f+tier*1.35f,radius),new Vector3(.06f,5.5f+tier*1.1f,.06f)},tier%2==0 ? deepGreen:green,10);crown.localPosition=p;
            }
            else for(int lobe=0;lobe<3;lobe++)NaturalMesh.Oval(root,"Broad sal leaf crown",p+new Vector3((lobe-1)*1.3f,5.7f+lobe%2,0),new Vector3(4.3f,3.4f,4),lobe%2==0 ? green:deepGreen,12);
        }
        private void AppleTree(Transform root,Vector3 p)
        {
            NaturalMesh.Branch(root,"Orchard apple tree trunk",p,p+Vector3.up*2.9f,.14f,wood);
            NaturalMesh.Oval(root,"Rounded apple tree crown",p+Vector3.up*3.1f,new Vector3(3.2f,2.7f,3),green,12);
            for(int i=0;i<7;i++)NaturalMesh.Oval(root,"Bright red apples",p+new Vector3(Mathf.Sin(i*2.4f)*1.14f,2.9f+(i%3)*.30f,Mathf.Cos(i*2.4f)*1.05f),Vector3.one*.27f,red,8);
        }
        private void Cottage(Transform root,Vector3 p,int variant,bool forest)
        {
            PrimitiveArt.Box(root,"Colourful regional cottage",p+new Vector3(0,1.4f,0),new Vector3(4.6f,2.8f,4.8f),forest ? accent:cream);
            foreach(float side in new[]{-1f,1f})
            {
                var roof=PrimitiveArt.Box(root,"Sloping hill or village roof",p+new Vector3(side*1.2f,3.25f,0),new Vector3(2.9f,.18f,5.4f),region==RegionProfile.Himachal ? stone:red);roof.localRotation=Quaternion.Euler(0,0,-side*25);
                PrimitiveArt.Box(root,"Dark framed cottage window",p+new Vector3(-2.34f,1.7f,side*1.35f),new Vector3(.06f,.88f,.84f),glass);
            }
            PrimitiveArt.Box(root,"Timber cottage door",p+new Vector3(-2.36f,.95f,0),new Vector3(.08f,1.9f,.95f),wood);
            if(forest)for(int i=0;i<6;i++)NaturalMesh.Oval(root,"Original dotted leaf wall motif",p+new Vector3(-2.40f,2.3f+Mathf.Sin(i)*.18f,-1.8f+i*.66f),new Vector3(.035f,.15f,.25f),cream,6);
        }
        private void HeritageHouse(Transform root,Vector3 p,int variant)
        {
            PrimitiveArt.Box(root,"Kolkata inspired heritage facade",p+new Vector3(0,3,0),new Vector3(5.4f,6,8.5f),variant%2==0 ? accent:gold);
            for(int floor=0;floor<2;floor++)for(int w=0;w<3;w++)
            {
                var q=p+new Vector3(-2.75f,1.7f+floor*2.6f,-2.7f+w*2.7f);
                PrimitiveArt.Box(root,"Cream window arch frame",q,new Vector3(.12f,1.7f,1.15f),cream);
                PrimitiveArt.Box(root,"Teal heritage shutter",q+Vector3.left*.08f,new Vector3(.07f,1.4f,.9f),deepGreen);
            }
            PrimitiveArt.Box(root,"Cream heritage cornice",p+new Vector3(0,5.95f,0),new Vector3(5.8f,.25f,8.8f),cream);
        }
        private void Tram(Transform root,Vector3 p)
        {
            PrimitiveArt.Box(root,"Decorative heritage tram on separate side track",p+new Vector3(0,1.25f,0),new Vector3(1.95f,2.20f,6.6f),cream);
            PrimitiveArt.Box(root,"Green tram lower stripe",p+new Vector3(0,.70f,0),new Vector3(2,.65f,6.65f),deepGreen);
            for(int i=0;i<5;i++)PrimitiveArt.Box(root,"Tram side window",p+new Vector3(-1,1.65f,-2.4f+i*1.2f),new Vector3(.035f,.80f,.85f),glass);
            foreach(float side in new[]{-1f,1f})PrimitiveArt.Box(root,"Separate heritage tram rail",p+new Vector3(side*.62f,.03f,0),new Vector3(.06f,.05f,11),stone);
            NaturalMesh.Branch(root,"Tram overhead collector",p+new Vector3(0,2.4f,0),p+new Vector3(0,3.7f,1),.035f,wood);
        }
        private void Bridge(Transform root,Vector3 p,bool steel)
        {
            float width=steel ? 56:14,height=steel ? 19:2.5f;
            PrimitiveArt.Box(root,"Scenic bridge deck",p+new Vector3(0,height*.34f,0),new Vector3(width,.36f,steel ? 7:1.8f),steel ? stone:wood);
            foreach(float side in new[]{-1f,1f})
            {
                var start=p+new Vector3(side*width*.45f,height*.34f,0);var top=p+new Vector3(side*width*.20f,height,0);
                NaturalMesh.Branch(root,"Scenic bridge diagonal truss",start,top,steel ? .25f:.06f,steel ? stone:wood);
                NaturalMesh.Branch(root,"Scenic bridge central span",top,p+new Vector3(0,height*.70f,0),steel ? .25f:.06f,steel ? stone:wood);
            }
        }
        public void SetWeather(float shade) { }
    }
}
