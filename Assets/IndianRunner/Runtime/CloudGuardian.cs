using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndianRunner
{
    // Original fantasy scenery, not a collision obstacle or a religious figure.
    // Built once, shown only inside an obstacle-free garden buffer.
    public sealed class CloudGuardian : IDisposable
    {
        public const float Duration=4;
        private readonly Transform root, head;
        private readonly Transform[] arms=new Transform[2],banks=new Transform[3],sparkles=new Transform[8];
        private readonly List<Material> materials=new List<Material>();
        private readonly bool available;
        private float appearedAt;
        private int lastGarden=-1;
        public bool Active { get; private set; }
        public CloudGuardian(Transform parent)
        {
            root=Pivot(parent,"Original Thar Cloud Guardian",new Vector3(0,2.5f,70));
            var shader=Resources.Load<Shader>("Scenery/CloudGuardian");
            if(shader==null || !shader.isSupported)
            { root.gameObject.SetActive(false); Debug.LogWarning("Cloud Guardian disabled: its sky shader is unavailable."); return; }
            var skin=Tint(shader,"Warm sandstone face",new Color(.83f,.55f,.35f,1));
            var dark=Tint(shader,"Indigo moustache",new Color(.075f,.055f,.13f,1));
            var teal=Tint(shader,"Turquoise guardian coat",new Color(.04f,.56f,.58f,.93f));
            var pink=Tint(shader,"Bright rose turban",new Color(.96f,.16f,.44f,1));
            var gold=Tint(shader,"Warm golden embroidery",new Color(1,.76f,.25f,1));
            var eyeGlow=Tint(shader,"Soft glowing eyes",new Color(.87f,.98f,1,.95f));
            var cloudWhite=Tint(shader,"Pearl cloud",new Color(.91f,.95f,1,.66f));
            var cloudRose=Tint(shader,"Rose cloud",new Color(.96f,.66f,.76f,.47f));
            var cloudViolet=Tint(shader,"Lavender cloud",new Color(.65f,.62f,.89f,.53f));
            var body=Pivot(root,"Broad embroidered cloud coat",Vector3.zero);
            NaturalMesh.Oval(body,"Giant shoulders",new Vector3(0,9,1),new Vector3(20,10,7),teal,14);
            NaturalMesh.Oval(body,"Cloud-borne torso",new Vector3(0,4,1.5f),new Vector3(15,11,6),teal,14);
            NaturalMesh.Oval(body,"Gold collar",new Vector3(0,12,-1.4f),new Vector3(8,1.5f,3),gold,12);
            for(int i=0;i<7;i++)
            {
                float side=i-3;
                NaturalMesh.Oval(body,"Necklace bead",new Vector3(side*1.1f,10.6f+side*side*.11f,-2.5f),new Vector3(.65f,.75f,.35f),gold,8);
            }
            PrimitiveArt.CombineScenery(body,true);
            head=Pivot(root,"Guardian face and turban",new Vector3(0,17,0));
            NaturalMesh.Oval(head,"Rounded giant face",Vector3.zero,new Vector3(7.6f,8.4f,5),skin,16);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(head,"Ear",new Vector3(side*3.6f,-.2f,0),new Vector3(1.3f,2.3f,1),skin,10);
                NaturalMesh.Oval(head,"Gold earring",new Vector3(side*3.8f,-1.7f,-.35f),new Vector3(.7f,1,.3f),gold,10);
                NaturalMesh.Oval(head,"Glowing eye",new Vector3(side*1.5f,.8f,-2.38f),new Vector3(1.3f,.7f,.45f),eyeGlow,12);
                NaturalMesh.Oval(head,"Indigo pupil",new Vector3(side*1.5f,.76f,-2.63f),new Vector3(.30f,.42f,.12f),dark,10);
                NaturalMesh.Branch(head,"Expressive eyebrow",new Vector3(side*.85f,1.55f,-2.1f),new Vector3(side*2.2f,1.7f,-1.9f),.18f,dark);
                NaturalMesh.Branch(head,"Curled moustache",new Vector3(0,-.8f,-2.65f),new Vector3(side*1.4f,-1.05f,-2.9f),.22f,dark);
                NaturalMesh.Branch(head,"Upturned moustache tip",new Vector3(side*1.4f,-1.05f,-2.9f),new Vector3(side*2.15f,-.55f,-2.6f),.17f,dark);
            }
            NaturalMesh.Oval(head,"Sculpted nose",new Vector3(0,-.1f,-2.65f),new Vector3(.85f,1.4f,1.1f),skin,12);
            NaturalMesh.Branch(head,"Small warm smile",new Vector3(-.8f,-1.75f,-2.37f),new Vector3(.8f,-1.75f,-2.37f),.12f,dark);
            for(int i=0;i<4;i++)
            {
                var fold=NaturalMesh.Oval(head,"Layered colourful turban",new Vector3((i-1.5f)*.2f,3.7f+i*.62f,.1f),new Vector3(10.1f-i*.55f,2,6.1f),i%2==0 ? pink:teal,14);
                fold.localRotation=Quaternion.Euler(0,0,-12+i*7);
            }
            var jewel=PrimitiveArt.Box(head,"Turban diamond",new Vector3(0,3.1f,-3.02f),new Vector3(1.3f,1.3f,.3f),gold);
            jewel.localRotation=Quaternion.Euler(0,0,45);
            NaturalMesh.Oval(head,"Turban crest",new Vector3(.4f,6.6f,.2f),new Vector3(2,3,2),pink,12);
            PrimitiveArt.CombineScenery(head,true);
            for(int i=0;i<arms.Length;i++)
            {
                float side=i==0 ? -1:1;
                arms[i]=Pivot(root,"Welcoming giant arm",new Vector3(side*7,10,1));
                NaturalMesh.Branch(arms[i],"Wide sleeve",Vector3.zero,new Vector3(side*3,-1,0),1.5f,teal);
                NaturalMesh.Branch(arms[i],"Raised forearm",new Vector3(side*3,-1,0),new Vector3(side*4.1f,2,-.5f),1.1f,skin);
                NaturalMesh.Oval(arms[i],"Golden cuff",new Vector3(side*3.3f,-.4f,-.1f),new Vector3(2.6f,1,2.5f),gold,10);
                NaturalMesh.Oval(arms[i],"Open palm",new Vector3(side*4.2f,3,-.6f),new Vector3(2.4f,3.2f,1),skin,12);
                PrimitiveArt.CombineScenery(arms[i],true);
            }
            for(int i=0;i<banks.Length;i++)
            {
                banks[i]=Pivot(root,"Pooled sweeping cloud bank",new Vector3((i-1)*23,5+i*3,3+i));
                Material cloud=i==0 ? cloudRose:i==1 ? cloudWhite:cloudViolet;
                for(int lobe=0;lobe<5;lobe++)
                    NaturalMesh.Oval(banks[i],"Irregular cloud lobe",new Vector3((lobe-2)*5,lobe%2*1.6f,Mathf.Sin(lobe)*1.4f),new Vector3(17-lobe%2*3,7+lobe%3,7),cloud,10);
                PrimitiveArt.CombineScenery(banks[i],true);
            }
            for(int i=0;i<sparkles.Length;i++)
            {
                sparkles[i]=PrimitiveArt.Box(root,"Pooled sky mirror glint",new Vector3((i%2==0 ? -1:1)*(10+i),12+i%4*3,-5),new Vector3(.4f,.75f,.2f),i%2==0 ? gold:eyeGlow);
                sparkles[i].localRotation=Quaternion.Euler(0,0,45);
            }
            foreach(var renderer in root.GetComponentsInChildren<Renderer>())
            { renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off; renderer.receiveShadows=false; }
            available=true; root.gameObject.SetActive(false);
        }
        private static Transform Pivot(Transform parent,string name,Vector3 position)
        { var t=new GameObject(name).transform; t.SetParent(parent,false); t.localPosition=position; return t; }
        private Material Tint(Shader shader,string name,Color color)
        {
            var material=new Material(shader) { name=name };
            material.SetColor("_Color",color); material.SetFloat("_Opacity",0); materials.Add(material); return material;
        }
        public void Reset()
        { Active=false; lastGarden=-1; root.gameObject.SetActive(false); }
        public bool Tick(float distance,float clock,bool reduced)
        {
            if(!available) return false;
            bool started=false;
            int row=(int)Mathf.Max(0,distance/RunRules.RowSpacing),garden=row/36;
            if(!Active && RunRules.GuardianWindow(distance) && garden!=lastGarden)
            {
                lastGarden=garden; appearedAt=clock; Active=true; started=true; root.gameObject.SetActive(true);
                foreach(var sparkle in sparkles) sparkle.gameObject.SetActive(!reduced);
            }
            if(!Active) return false;
            float elapsed=clock-appearedAt;
            if(elapsed>=Duration) { Active=false; root.gameObject.SetActive(false); return false; }
            float fade=Mathf.SmoothStep(0,1,Mathf.Min(Mathf.Clamp01(elapsed/.55f),Mathf.Clamp01((Duration-elapsed)/.7f)));
            foreach(var material in materials) material.SetFloat("_Opacity",fade*(reduced ? .72f:1));
            root.localScale=Vector3.one*(reduced ? 1:Mathf.Lerp(.96f,1,Mathf.Clamp01(elapsed/.8f)));
            head.localRotation=Quaternion.Euler(0,reduced ? 0:Mathf.Sin(elapsed*1.5f)*4,0);
            for(int i=0;i<arms.Length;i++) arms[i].localRotation=Quaternion.Euler(0,0,reduced ? 0:Mathf.Sin(elapsed*2+i)*5);
            for(int i=0;i<banks.Length;i++)
                banks[i].localPosition=new Vector3((i-1)*23+(reduced ? 0:Mathf.Sin(elapsed*.6f+i)*1.8f),5+i*3,3+i);
            if(!reduced) for(int i=0;i<sparkles.Length;i++) sparkles[i].localRotation=Quaternion.Euler(0,0,45+elapsed*(i%2==0 ? 14:-14));
            return started;
        }
        public void Dispose()
        { foreach(var material in materials) UnityEngine.Object.Destroy(material); UnityEngine.Object.Destroy(root.gameObject); }
    }
}
