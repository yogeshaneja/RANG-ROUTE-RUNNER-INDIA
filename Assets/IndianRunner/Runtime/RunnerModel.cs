using UnityEngine;

namespace IndianRunner
{
    public sealed class RunnerModel
    {
        public Transform Root { get; }
        private readonly Transform torso, head, scarf, scarfMiddle, scarfTip, leftArm, rightArm, leftElbow, rightElbow;
        private readonly Transform[] thighs=new Transform[2], calves=new Transform[2], feet=new Transform[2], wrists=new Transform[2], eyes=new Transform[2];
        private readonly Transform shadow;
        private readonly Material shadowMaterial, outfitMaterial;
        private float lastClock, gaitPhase, slideBlend, jumpBlend, locomotionBlend, scarfPitch, scarfBend;
        private const float UpperLeg=.49f, LowerLeg=.47f, StandingHip=1.035f;
        private static Transform Pivot(Transform parent,string name,Vector3 position)
        {
            var t=new GameObject(name).transform; t.SetParent(parent,false); t.localPosition=position; return t;
        }
        public RunnerModel(Transform parent)
        {
            Root=Pivot(parent,"Original sculpted Indian runner",Vector3.zero);
            var skin=PrimitiveArt.Mat(new Color(.57f,.345f,.225f));
            var skinWarm=PrimitiveArt.Mat(new Color(.61f,.365f,.245f));
            outfitMaterial=new Material(PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.08f,.55f,.61f),new Vector2(2,3)));
            var trim=PrimitiveArt.Mat(new Color(.89f,.68f,.29f));
            var trousers=PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.97f,.94f,.86f),new Vector2(2,3));
            var hair=PrimitiveArt.Mat(new Color(.040f,.029f,.023f));
            var shoe=PrimitiveArt.Mat(new Color(.24f,.105f,.047f));
            var orange=PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.98f,.39f,.10f),new Vector2(1,2));
            var white=PrimitiveArt.Mat(new Color(.91f,.88f,.79f));
            var lip=PrimitiveArt.Mat(new Color(.38f,.195f,.145f));
            torso=Pivot(Root,"Runner torso",new Vector3(0,StandingHip,0));
            var shirt=Pivot(torso,"Tailored kurta details",Vector3.zero);
            RunnerSculpt.Kurta(shirt,outfitMaterial);
            NaturalMesh.Lathe(shirt,"Subtle embroidered hem",new[]{new Vector3(.235f,-.159f,.148f),new Vector3(.234f,-.144f,.147f)},trim,28);
            NaturalMesh.Lathe(shirt,"Nehru band collar",new[]{new Vector3(.104f,.582f,.088f),new Vector3(.102f,.645f,.086f)},outfitMaterial,20);
            PrimitiveArt.Box(shirt,"Fine collar edge",new Vector3(0,.615f,.090f),new Vector3(.014f,.055f,.009f),trim);
            PrimitiveArt.Box(shirt,"Stitched kurta placket",new Vector3(0,.444f,.148f),new Vector3(.022f,.24f,.009f),trim);
            for(int i=0;i<3;i++) NaturalMesh.Oval(shirt,"Small ivory button",new Vector3(0,.525f-i*.064f,.157f),Vector3.one*.016f,white,8);
            // Cloth seams are shallow and follow the silhouette, rather than bulky props.
            foreach(float side in new[]{-1f,1f})
                NaturalMesh.Branch(shirt,"Kurta side seam",new Vector3(side*.213f,-.12f,0),new Vector3(side*.201f,.20f,0),.003f,trim);
            PrimitiveArt.CombineScenery(shirt);
            NaturalMesh.Oval(torso,"Anatomical neck",new Vector3(0,.669f,-.006f),new Vector3(.145f,.155f,.139f),skin,18);
            head=Pivot(torso,"Stabilised sculpted head",new Vector3(0,.83f,.018f));
            var faceRigid=Pivot(head,"Sculpted head surfaces",Vector3.zero);
            RunnerSculpt.Face(faceRigid,skinWarm); RunnerSculpt.Hair(faceRigid,hair);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(faceRigid,"Shaped ear",new Vector3(side*.124f,-.008f,-.003f),new Vector3(.044f,.079f,.038f),skin,12);
                NaturalMesh.Oval(faceRigid,"Inner ear contour",new Vector3(side*.143f,-.008f,.003f),new Vector3(.012f,.040f,.020f),skinWarm,10);
                RunnerSculpt.FaceLine(faceRigid,"Tapered natural eyebrow",side*.052f,.054f,.054f,.006f,.003f,hair);
                RunnerSculpt.FaceLine(faceRigid,"Upper eyelid",side*.052f,.029f,.047f,.0025f,.005f,skin);
                float nx=side*.017f,ny=-.049f;
                NaturalMesh.Oval(faceRigid,"Subtle nostril",new Vector3(nx,ny,RunnerSculpt.FaceZ(nx,ny)+.001f),new Vector3(.008f,.0045f,.004f),lip,8);
            }
            RunnerSculpt.FaceLine(faceRigid,"Relaxed closed mouth",0,-.091f,.062f,.0035f,-.0015f,lip);
            RunnerSculpt.FaceLine(faceRigid,"Soft lower lip",0,-.098f,.054f,.004f,-.002f,skin);
            PrimitiveArt.CombineScenery(faceRigid);
            eyes[0]=RunnerSculpt.Eye(head,-1,white,hair,white); eyes[1]=RunnerSculpt.Eye(head,1,white,hair,white);
            NaturalMesh.Lathe(torso,"Saffron scarf neck wrap",new[]{new Vector3(.111f,.607f,.100f),new Vector3(.115f,.646f,.102f),new Vector3(.100f,.676f,.089f)},orange,20);
            scarf=Pivot(torso,"Soft scarf root",new Vector3(.145f,.58f,-.128f));
            scarfMiddle=Pivot(scarf,"Scarf middle",new Vector3(0,-.27f,0)); scarfTip=Pivot(scarfMiddle,"Scarf tip",new Vector3(0,-.27f,0));
            RunnerSculpt.Scarf(scarf,scarfMiddle,scarfTip,orange);
            for(int i=0;i<4;i++) NaturalMesh.Branch(scarfTip,"Fine scarf fringe",new Vector3(-.058f+i*.039f,0,0),new Vector3(-.058f+i*.039f,-.05f,0),.004f,trim);
            PrimitiveArt.CombineScenery(scarfTip);
            leftArm=Arm(torso,-1,outfitMaterial,skinWarm,trim,out leftElbow,out wrists[0]);
            rightArm=Arm(torso,1,outfitMaterial,skinWarm,trim,out rightElbow,out wrists[1]);
            for(int i=0;i<2;i++)
            {
                thighs[i]=Pivot(Root,"Hip "+i,new Vector3(i==0 ? -.128f:.128f,StandingHip,0));
                calves[i]=Pivot(thighs[i],"Knee "+i,new Vector3(0,-UpperLeg,0));
                RunnerSculpt.Limb(thighs[i],calves[i],"Continuous skinned trouser leg",new[]{
                    new Vector3(.092f,0,.094f),new Vector3(.100f,-.14f,.099f),new Vector3(.087f,-.32f,.088f),
                    new Vector3(.073f,-.43f,.075f),new Vector3(.072f,-.49f,.076f),new Vector3(.073f,-.55f,.075f),
                    new Vector3(.077f,-.68f,.077f),new Vector3(.058f,-.86f,.060f),new Vector3(.052f,-.96f,.053f)
                },UpperLeg,trousers,trousers);
                feet[i]=Pivot(calves[i],"Ankle "+i,new Vector3(0,-LowerLeg,0));
                NaturalMesh.Oval(feet[i],"Shaped leather running shoe",new Vector3(0,-.014f,.058f),new Vector3(.166f,.118f,.30f),shoe,18);
                NaturalMesh.Oval(feet[i],"Thin dark shoe sole",new Vector3(0,-.064f,.058f),new Vector3(.169f,.023f,.304f),hair,16);
                NaturalMesh.Oval(feet[i],"Leather vamp stitching",new Vector3(0,.027f,.088f),new Vector3(.113f,.009f,.145f),trim,12);
                PrimitiveArt.CombineScenery(feet[i]);
            }
            var shader=Resources.Load<Shader>("Scenery/RunnerGroundShadow");
            if(shader!=null)
            {
                shadowMaterial=new Material(shader);
                shadow=PrimitiveArt.Shape(parent,"Soft runner contact shadow",PrimitiveType.Quad,new Vector3(0,.024f,0),new Vector3(.82f,1.16f,1),shadowMaterial);
                shadow.localRotation=Quaternion.Euler(90,0,0); shadow.GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            Animate(0,false,false,0,false);
        }
        private static Transform Arm(Transform parent,float side,Material sleeve,Material skin,Material trim,out Transform elbow,out Transform wrist)
        {
            var shoulder=Pivot(parent,"Soft shoulder",new Vector3(side*.245f,.49f,0));
            elbow=Pivot(shoulder,"Elbow",new Vector3(0,-.28f,0));
            RunnerSculpt.Limb(shoulder,elbow,"Continuous skinned sleeve and forearm",new[]{
                new Vector3(.079f,0,.079f),new Vector3(.079f,-.09f,.079f),new Vector3(.070f,-.20f,.072f),
                new Vector3(.063f,-.26f,.064f),new Vector3(.061f,-.28f,.062f),new Vector3(.059f,-.32f,.061f),
                new Vector3(.057f,-.40f,.058f),new Vector3(.038f,-.53f,.040f)
            },.28f,sleeve,skin);
            var cuff=NaturalMesh.Lathe(shoulder,"Fine sleeve cuff",new[]{new Vector3(.065f,-.262f,.066f),new Vector3(.067f,-.244f,.068f)},trim,16);
            // Cuff follows the upper arm; it never spans the deforming joint.
            cuff.name="Embroidered upper sleeve cuff";
            wrist=Pivot(elbow,"Relaxed wrist",new Vector3(0,-.25f,0)); RunnerSculpt.Hand(wrist,skin,-side);
            return shoulder;
        }
        public void Animate(float clock,bool running,bool sliding,float height,bool reduced,float speed=9)
        {
            float dt=Mathf.Clamp(clock-lastClock,0,.05f);
            bool reset=clock<lastClock || clock==0;
            if(reset) { gaitPhase=slideBlend=jumpBlend=locomotionBlend=0; scarfPitch=scarfBend=0; }
            lastClock=clock;
            float blend=reset ? 1:1-Mathf.Exp(-22*dt),speedT=Mathf.InverseLerp(9,18,speed);
            locomotionBlend=Mathf.Lerp(locomotionBlend,running ? 1:0,reset ? 1:1-Mathf.Exp(-14*dt));
            slideBlend=Mathf.Lerp(slideBlend,sliding ? 1:0,blend); jumpBlend=Mathf.Lerp(jumpBlend,height>.04f ? 1:0,blend);
            if(running) gaitPhase+=dt*Mathf.Lerp(10.5f,14,speedT);
            Root.localPosition=new Vector3(Root.localPosition.x,height,0);
            float activity=locomotionBlend*(1-slideBlend)*(1-jumpBlend);
            float bob=reduced ? 0:(1-Mathf.Cos(gaitPhase*2))*.010f*activity;
            float idleBreath=running || reduced ? 0:Mathf.Sin(clock*1.8f)*.005f;
            float hipY=Mathf.Lerp(Mathf.Lerp(StandingHip,Mathf.Lerp(.93f,.89f,speedT),locomotionBlend),.34f,slideBlend)+bob+idleBreath;
            torso.localPosition=new Vector3(0,hipY,0);
            float twist=reduced ? 0:Mathf.Sin(gaitPhase)*3.5f*activity;
            float roll=reduced ? 0:Mathf.Cos(gaitPhase)*1.7f*activity;
            float torsoPitch=Mathf.Lerp(7*locomotionBlend,-72,slideBlend);
            torso.localRotation=Quaternion.Euler(torsoPitch,twist,roll);
            head.localRotation=Quaternion.Euler(Mathf.Lerp(-torsoPitch-2,50,slideBlend),-twist*.75f,-roll*.65f);
            // A gentle blink, independent of frame rate; no moving eyes in reduced effects.
            float blinkPhase=Mathf.Repeat(clock,3.7f);
            float blink=reduced ? 0:Mathf.Clamp01(1-Mathf.Abs(blinkPhase-3.45f)/.10f);
            foreach(var eye in eyes) eye.localScale=new Vector3(1,Mathf.Max(.06f,1-blink),1);
            for(int i=0;i<2;i++)
            {
                float phase=Mathf.Repeat(gaitPhase/(Mathf.PI*2)+i*.5f,1),stance=Mathf.Lerp(.30f,.24f,speedT);
                float reach=Mathf.Lerp(.43f,.52f,speedT),footZ=0,footY=.10f,toePitch=0;
                if(running)
                {
                    if(phase<stance)
                    {
                        float contact=phase/stance;
                        footZ=Mathf.Lerp(reach,-reach,contact); toePitch=Mathf.Lerp(-7,10,Mathf.SmoothStep(0,1,contact));
                        footY+=Mathf.Max(0,Mathf.Sin(toePitch*Mathf.Deg2Rad))*.16f;
                    }
                    else
                    {
                        float recovery=(phase-stance)/(1-stance);
                        footZ=Mathf.Lerp(-reach,reach,Mathf.SmoothStep(0,1,recovery)); toePitch=Mathf.Lerp(10,-7,Mathf.SmoothStep(0,1,recovery))+Mathf.Sin(recovery*Mathf.PI)*10;
                        footY+=Mathf.Sin(recovery*Mathf.PI)*.31f+Mathf.Max(0,Mathf.Sin(toePitch*Mathf.Deg2Rad))*.16f;
                    }
                }
                footZ*=locomotionBlend; footY=Mathf.Lerp(.10f,footY,locomotionBlend);
                footY=Mathf.Lerp(footY,.23f,jumpBlend); footZ=Mathf.Lerp(footZ,i==0 ? .32f:-.12f,jumpBlend);
                footZ=Mathf.Lerp(footZ,i==0 ? .48f:.29f,slideBlend); footY=Mathf.Lerp(footY,.10f,slideBlend);
                toePitch*=activity;
                // Project targets inside reach instead of allowing a stretched/popped knee.
                float reachLimit=Mathf.Sqrt(Mathf.Max(.001f,Mathf.Pow(UpperLeg+LowerLeg-.004f,2)-Mathf.Pow(hipY-footY,2)));
                footZ=Mathf.Clamp(footZ,-reachLimit,reachLimit);
                SolveLeg(i,hipY,footY,footZ,toePitch);
            }
            float armSwing=Mathf.Cos(gaitPhase+.12f)*29*locomotionBlend;
            leftArm.localRotation=Quaternion.Euler(Mathf.Lerp(armSwing-10,25,slideBlend),-twist*.30f,-6);
            rightArm.localRotation=Quaternion.Euler(Mathf.Lerp(-armSwing-10,25,slideBlend),-twist*.30f,6);
            float bend=Mathf.Lerp(-14,-77,locomotionBlend);
            leftElbow.localRotation=Quaternion.Euler(Mathf.Lerp(bend+Mathf.Sin(gaitPhase+.7f)*9*locomotionBlend,-65,slideBlend),0,0);
            rightElbow.localRotation=Quaternion.Euler(Mathf.Lerp(bend-Mathf.Sin(gaitPhase+.7f)*9*locomotionBlend,-65,slideBlend),0,0);
            for(int i=0;i<wrists.Length;i++) wrists[i].localRotation=Quaternion.Euler(-7,reduced ? 0:Mathf.Sin(gaitPhase+i*Mathf.PI)*4*activity,i==0 ? 6:-6);
            float scarfTarget=reduced ? 7:Mathf.Lerp(4,32,locomotionBlend)+Mathf.Sin(clock*6)*4*activity;
            scarfPitch=Mathf.Lerp(scarfPitch,scarfTarget,reset ? 1:1-Mathf.Exp(-9*dt));
            scarfBend=Mathf.Lerp(scarfBend,reduced ? 0:Mathf.Sin(clock*5.5f-.5f)*8*activity,reset ? 1:1-Mathf.Exp(-7*dt));
            scarf.localRotation=Quaternion.Euler(Mathf.Lerp(scarfPitch,6,slideBlend),reduced ? 0:Mathf.Sin(clock*3)*3*activity,0);
            scarfMiddle.localRotation=Quaternion.Euler(scarfBend,0,reduced ? 0:Mathf.Sin(clock*4-.6f)*3*activity);
            scarfTip.localRotation=Quaternion.Euler(scarfBend*.65f,0,0);
            if(shadow!=null)
            {
                shadow.localPosition=new Vector3(Root.localPosition.x,.024f,0); float size=Mathf.Lerp(1,.68f,Mathf.Clamp01(height/1.7f));
                shadow.localScale=new Vector3(.82f*size,1.16f*size,1); shadowMaterial.SetColor("_Tint",new Color(.14f,.10f,.06f,Mathf.Lerp(.30f,.11f,Mathf.Clamp01(height/1.7f))));
            }
        }
        private void SolveLeg(int i,float hipY,float footY,float footZ,float toePitch)
        {
            thighs[i].localPosition=new Vector3(i==0 ? -.128f:.128f,hipY,0);
            float dy=footY-hipY,d=Mathf.Clamp(Mathf.Sqrt(dy*dy+footZ*footZ),.08f,UpperLeg+LowerLeg-.001f);
            float aim=Mathf.Atan2(footZ,-dy)*Mathf.Rad2Deg;
            float hip=Mathf.Acos(Mathf.Clamp((UpperLeg*UpperLeg+d*d-LowerLeg*LowerLeg)/(2*UpperLeg*d),-1,1))*Mathf.Rad2Deg;
            float knee=180-Mathf.Acos(Mathf.Clamp((UpperLeg*UpperLeg+LowerLeg*LowerLeg-d*d)/(2*UpperLeg*LowerLeg),-1,1))*Mathf.Rad2Deg;
            float upper=-aim-hip; thighs[i].localRotation=Quaternion.Euler(upper,0,0); calves[i].localRotation=Quaternion.Euler(knee,0,0);
            feet[i].localRotation=Quaternion.Euler(-upper-knee+toePitch,0,0);
        }
        public void ValidateRig()
        {
            var skins=Root.GetComponentsInChildren<SkinnedMeshRenderer>();
            if(skins.Length!=5) throw new System.InvalidOperationException("Runner needs four soft limbs and one scarf renderer.");
            foreach(var renderer in skins)
            {
                var mesh=renderer.sharedMesh; var bones=renderer.bones;
                if(mesh==null || renderer.rootBone==null || mesh.bindposes.Length!=bones.Length || mesh.boneWeights.Length!=mesh.vertexCount)
                    throw new System.InvalidOperationException("Runner skin mesh/bind poses are incomplete: "+renderer.name);
                foreach(var bone in bones) if(bone==null) throw new System.InvalidOperationException("Runner skin references a missing joint.");
                foreach(var weight in mesh.boneWeights)
                    if(Mathf.Abs(weight.weight0+weight.weight1-1)>.001f || weight.boneIndex0>=bones.Length || weight.boneIndex1>=bones.Length)
                        throw new System.InvalidOperationException("Runner skin has an invalid joint weight.");
                if(renderer.sharedMaterials.Length!=mesh.subMeshCount)
                    throw new System.InvalidOperationException("Runner skin material groups do not match mesh groups.");
            }
        }
        public void SetOutfit(int index)
        {
            Color colour=index==1 ? new Color(1,.57f,.055f):index==2 ? new Color(.96f,.12f,.43f):index==3 ? new Color(.12f,.27f,.94f):new Color(.08f,.55f,.61f);
            if(outfitMaterial.HasProperty("_BaseColor")) outfitMaterial.SetColor("_BaseColor",colour);
            if(outfitMaterial.HasProperty("_Color")) outfitMaterial.SetColor("_Color",colour);
        }
        public void Dispose() { if(shadowMaterial!=null) Object.Destroy(shadowMaterial); Object.Destroy(outfitMaterial); }
    }
}
