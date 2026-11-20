using UnityEngine;

namespace IndianRunner
{
    public sealed class RunnerCompanion
    {
        private readonly Transform root,tail;
        private readonly Transform[] legs=new Transform[4];
        private float fetchAt=-100,fetchX,fetchZ;
        public RunnerCompanion(Transform parent)
        {
            root=new GameObject("Chutku - original helpful Indian street dog").transform;root.SetParent(parent,false);
            var body=new GameObject("Companion rounded body and bandana").transform;body.SetParent(root,false);
            var tan=AdventureArt.Colour(.77f,.47f,.23f);var cream=AdventureArt.Colour(.98f,.89f,.70f);
            var dark=AdventureArt.Colour(.075f,.08f,.06f);var teal=AdventureArt.Colour(.02f,.60f,.64f);var saffron=AdventureArt.Colour(1,.51f,.06f);
            NaturalMesh.Oval(body,"Soft dog torso",new Vector3(0,.39f,0),new Vector3(.35f,.44f,.78f),tan,14);
            NaturalMesh.Oval(body,"White chest",new Vector3(0,.43f,.28f),new Vector3(.29f,.39f,.23f),cream,12);
            NaturalMesh.Oval(body,"Expressive dog head",new Vector3(0,.67f,.38f),new Vector3(.32f,.34f,.35f),tan,14);
            NaturalMesh.Oval(body,"Cream muzzle",new Vector3(0,.60f,.57f),new Vector3(.23f,.16f,.22f),cream,12);
            NaturalMesh.Oval(body,"Button nose",new Vector3(0,.63f,.68f),Vector3.one*.085f,dark,10);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(body,"Warm bright eye",new Vector3(side*.115f,.72f,.50f),Vector3.one*.053f,dark,10);
                NaturalMesh.Oval(body,"Eye catchlight",new Vector3(side*.115f,.73f,.525f),Vector3.one*.014f,cream,8);
                var ear=NaturalMesh.Oval(body,"Floppy attentive ear",new Vector3(side*.17f,.77f,.34f),new Vector3(.13f,.26f,.17f),tan,12);ear.localRotation=Quaternion.Euler(0,0,-side*22);
            }
            NaturalMesh.Oval(body,"Turquoise collar",new Vector3(0,.53f,.33f),new Vector3(.36f,.10f,.30f),teal,12);
            var bandana=PrimitiveArt.Box(body,"Saffron companion bandana",new Vector3(0,.44f,.43f),new Vector3(.16f,.19f,.035f),saffron);bandana.localRotation=Quaternion.Euler(0,0,45);
            PrimitiveArt.CombineScenery(body,true);
            for(int i=0;i<4;i++)
            {
                legs[i]=new GameObject("Animated companion leg").transform;legs[i].SetParent(root,false);
                legs[i].localPosition=new Vector3(i%2==0 ? -.12f:.12f,.37f,i<2 ? .23f:-.24f);
                NaturalMesh.Branch(legs[i],"Little dog leg",Vector3.zero,new Vector3(0,-.32f,0),.045f,tan);
                NaturalMesh.Oval(legs[i],"Cream paw",new Vector3(0,-.32f,.035f),new Vector3(.11f,.08f,.16f),cream,10);PrimitiveArt.CombineScenery(legs[i],true);
            }
            tail=new GameObject("Wagging curled companion tail").transform;tail.SetParent(root,false);tail.localPosition=new Vector3(0,.49f,-.38f);
            NaturalMesh.Branch(tail,"Curved tail base",Vector3.zero,new Vector3(.05f,.28f,-.14f),.035f,tan);
            NaturalMesh.Branch(tail,"Cream tail tip",new Vector3(.05f,.28f,-.14f),new Vector3(.16f,.34f,-.05f),.025f,cream);PrimitiveArt.CombineScenery(tail,true);
        }
        public void Fetch(float targetX,float targetZ,float clock){fetchX=targetX;fetchZ=targetZ;fetchAt=clock;}
        public void Reset(){fetchAt=-100;}
        public void Tick(float playerX,float clock,bool running,bool reduced,bool riding,Vector3 seat)
        {
            float phase=Mathf.Clamp01((clock-fetchAt)/.85f),dash=riding ? 0:Mathf.Sin(phase*Mathf.PI);
            Vector3 follow=new Vector3(playerX+.92f,0,-1.15f);
            root.localPosition=riding ? new Vector3(playerX+seat.x,seat.y,seat.z):Vector3.Lerp(follow,new Vector3(fetchX,.05f,fetchZ),dash);
            root.localRotation=Quaternion.Euler(0,riding ? 0:!running ? 180:dash*(fetchX>playerX ? 30:-30),0);
            float gait=running && !riding ? Mathf.Sin(clock*13):0;
            for(int i=0;i<legs.Length;i++)legs[i].localRotation=Quaternion.Euler(gait*(i==0 || i==3 ? 1:-1)*(reduced ? 12:24),0,0);
            tail.localRotation=Quaternion.Euler(0,reduced ? 0:Mathf.Sin(clock*9)*18,0);
        }
    }
}
