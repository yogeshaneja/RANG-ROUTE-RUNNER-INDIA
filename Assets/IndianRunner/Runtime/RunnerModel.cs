using UnityEngine;

namespace IndianRunner
{
    public sealed class RunnerModel
    {
        public Transform Root { get; }
        private readonly Transform torso, head, scarf, leftArm, rightArm, leftElbow, rightElbow;
        private readonly Transform[] thighs = new Transform[2], calves = new Transform[2], feet = new Transform[2];
        private readonly Transform shadow;
        private readonly Material shadowMaterial, outfitMaterial;
        private float lastClock, gaitPhase, slideBlend, jumpBlend, locomotionBlend;
        private const float UpperLeg = .45f, LowerLeg = .43f;
        private static Transform Pivot(Transform parent, string name, Vector3 position)
        {
            var t = new GameObject(name).transform; t.SetParent(parent, false); t.localPosition = position; return t;
        }
        public RunnerModel(Transform parent)
        {
            Root = Pivot(parent, "Original articulated Indian runner", Vector3.zero);
            var skin = PrimitiveArt.Mat(new Color(.55f, .32f, .20f));
            var skinLight = PrimitiveArt.Mat(new Color(.64f, .39f, .25f));
            outfitMaterial = new Material(PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.08f,.55f,.61f),new Vector2(2,3)));
            var kurta = outfitMaterial;
            var trim = PrimitiveArt.Mat(new Color(.86f, .66f, .29f));
            var trousers = PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(1,.98f,.91f),new Vector2(2,3));
            var hair = PrimitiveArt.Mat(new Color(.055f, .038f, .028f));
            var shoe = PrimitiveArt.Mat(new Color(.27f, .13f, .065f));
            var orange = PrimitiveArt.TexturedMat("Scenery/CottonReal",new Color(.98f,.44f,.15f),new Vector2(1,2));
            var white = PrimitiveArt.Mat(new Color(.96f, .94f, .86f));
            torso = Pivot(Root, "Pelvis and torso articulation", new Vector3(0, .94f, 0));
            var shirt = Pivot(torso, "Tailored kurta body", Vector3.zero);
            NaturalMesh.Lathe(shirt, "Shaped kurta", new[] { new Vector3(.25f,-.12f,.16f), new Vector3(.23f,.10f,.14f), new Vector3(.22f,.29f,.145f), new Vector3(.29f,.52f,.17f), new Vector3(.27f,.63f,.145f) }, kurta, 18);
            NaturalMesh.Lathe(shirt, "Kurta hem piping", new[] { new Vector3(.252f,-.125f,.163f), new Vector3(.25f,-.10f,.16f) }, trim, 18);
            NaturalMesh.Lathe(shirt, "Band collar", new[] { new Vector3(.105f,.61f,.09f), new Vector3(.104f,.69f,.087f) }, trim, 14);
            PrimitiveArt.Box(shirt, "Front stitched placket", new Vector3(0,.48f,.166f), new Vector3(.035f,.28f,.016f), trim);
            for (int i = 0; i < 3; i++) NaturalMesh.Oval(shirt, "Kurta button", new Vector3(0,.58f-i*.07f,.184f), Vector3.one*.025f, white, 8);
            PrimitiveArt.CombineScenery(shirt);
            NaturalMesh.Oval(torso, "Neck", new Vector3(0,.72f,0), new Vector3(.17f,.20f,.16f), skin);
            head = Pivot(torso, "Stable head", new Vector3(0,.91f,.005f));
            NaturalMesh.Oval(head, "Face and jaw", Vector3.zero, new Vector3(.295f,.39f,.285f), skinLight, 20);
            NaturalMesh.Oval(head, "Defined jaw", new Vector3(0,-.095f,.015f), new Vector3(.245f,.16f,.245f), skin, 18);
            NaturalMesh.Oval(head, "Sculpted black hair", new Vector3(0,.14f,-.025f), new Vector3(.32f,.16f,.30f), hair, 18);
            NaturalMesh.Oval(head, "Swept hair quiff", new Vector3(-.035f,.20f,.035f), new Vector3(.22f,.105f,.22f), hair, 16).localRotation = Quaternion.Euler(0,0,-12);
            foreach (float side in new[] { -1f, 1f })
            {
                NaturalMesh.Oval(head, "Ear", new Vector3(side*.153f,-.005f,0), new Vector3(.06f,.10f,.055f), skin, 10);
                NaturalMesh.Oval(head, "Eye white", new Vector3(side*.066f,.025f,.132f), new Vector3(.053f,.022f,.018f), white, 12);
                NaturalMesh.Oval(head, "Dark eye", new Vector3(side*.066f,.025f,.145f), new Vector3(.021f,.020f,.010f), hair, 10);
                NaturalMesh.Oval(head, "Eyebrow", new Vector3(side*.065f,.063f,.135f), new Vector3(.072f,.014f,.015f), hair, 10);
            }
            NaturalMesh.Oval(head, "Nose bridge", new Vector3(0,-.006f,.146f), new Vector3(.043f,.082f,.052f), skinLight, 12);
            NaturalMesh.Oval(head, "Mouth", new Vector3(0,-.092f,.129f), new Vector3(.078f,.015f,.012f), skin, 12);
            NaturalMesh.Oval(head,"Shaped nose tip",new Vector3(0,-.037f,.174f),new Vector3(.052f,.038f,.045f),skinLight,14);
            NaturalMesh.Oval(head,"Lower lip",new Vector3(0,-.101f,.134f),new Vector3(.063f,.010f,.012f),PrimitiveArt.Mat(new Color(.40f,.22f,.16f)),12);
            foreach(float side in new[]{-1f,1f})
            {
                NaturalMesh.Oval(head,"Hair sideburn",new Vector3(side*.135f,.045f,-.01f),new Vector3(.025f,.105f,.10f),hair,12);
                NaturalMesh.Oval(head,"Ear inner contour",new Vector3(side*.174f,-.005f,.008f),new Vector3(.015f,.05f,.028f),skinLight,10);
                NaturalMesh.Oval(head,"Small eye highlight",new Vector3(side*.066f-.004f,.029f,.152f),Vector3.one*.007f,white,8);
            }
            PrimitiveArt.CombineScenery(head);
            NaturalMesh.Lathe(torso,"Saffron scarf neck wrap",new[]{new Vector3(.12f,.64f,.105f),new Vector3(.125f,.69f,.11f),new Vector3(.105f,.72f,.095f)},orange,18);
            scarf = Pivot(torso, "Free scarf tail", new Vector3(.17f,.60f,-.16f));
            NaturalMesh.Lathe(scarf, "Saffron woven scarf", new[] {new Vector3(.085f,-.57f,.026f),new Vector3(.08f,-.29f,.025f),new Vector3(.072f,0,.026f)}, orange, 10);
            for (int i = 0; i < 4; i++) PrimitiveArt.Box(scarf, "Scarf fringe", new Vector3(-.06f+i*.04f,-.61f,0), new Vector3(.012f,.10f,.018f), trim);
            NaturalMesh.Lathe(scarf,"Second offset scarf fold",new[]{new Vector3(.035f,-.46f,.018f),new Vector3(.045f,-.20f,.02f),new Vector3(.04f,0,.02f)},orange,10).localPosition=new Vector3(-.10f,0,-.02f);
            PrimitiveArt.CombineScenery(scarf);
            leftArm = Arm(torso, -1, kurta, skinLight, trim, out leftElbow);
            rightArm = Arm(torso, 1, kurta, skinLight, trim, out rightElbow);
            for (int i = 0; i < 2; i++)
            {
                float x = i == 0 ? -.145f : .145f;
                thighs[i] = Pivot(Root, "Hip " + i, new Vector3(x,.94f,0));
                NaturalMesh.Lathe(thighs[i], "Tapered trouser thigh", new[] { new Vector3(.08f,-UpperLeg,.08f),new Vector3(.10f,-.18f,.10f),new Vector3(.105f,0,.105f)}, trousers, 14);
                calves[i] = Pivot(thighs[i], "Knee " + i, new Vector3(0,-UpperLeg,0));
                NaturalMesh.Lathe(calves[i], "Trouser calf", new[] {new Vector3(.062f,-LowerLeg,.062f),new Vector3(.085f,-.17f,.085f),new Vector3(.08f,0,.08f)}, trousers, 14);
                feet[i] = Pivot(calves[i], "Ankle " + i, new Vector3(0,-LowerLeg,0));
                NaturalMesh.Oval(feet[i], "Brown running jutti", new Vector3(0,-.015f,.065f), new Vector3(.19f,.13f,.34f), shoe, 16);
                NaturalMesh.Oval(feet[i], "Shoe sole", new Vector3(0,-.065f,.065f), new Vector3(.195f,.025f,.345f), hair, 14);
                NaturalMesh.Oval(feet[i],"Stitched leather vamp",new Vector3(0,.028f,.11f),new Vector3(.14f,.025f,.19f),trim,12);
                PrimitiveArt.CombineScenery(feet[i]);
            }
            var shader = Resources.Load<Shader>("Scenery/RunnerGroundShadow");
            if (shader != null)
            {
                shadowMaterial = new Material(shader);
                shadow = PrimitiveArt.Shape(parent, "Soft runner contact shadow", PrimitiveType.Quad, new Vector3(0,.024f,0), new Vector3(.9f,1.25f,1), shadowMaterial);
                shadow.localRotation = Quaternion.Euler(90,0,0);
                shadow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            Animate(0,false,false,0,false);
        }
        private static Transform Arm(Transform torso, float side, Material sleeve, Material skin, Material trim, out Transform elbow)
        {
            var shoulder = Pivot(torso, "Shoulder", new Vector3(side*.31f,.55f,0));
            NaturalMesh.Lathe(shoulder, "Kurta sleeve", new[] {new Vector3(.075f,-.29f,.075f),new Vector3(.09f,-.12f,.09f),new Vector3(.092f,0,.092f)}, sleeve, 14);
            NaturalMesh.Lathe(shoulder,"Neat embroidered sleeve cuff",new[]{new Vector3(.077f,-.29f,.077f),new Vector3(.079f,-.265f,.079f)},trim,14);
            elbow = Pivot(shoulder, "Elbow", new Vector3(0,-.29f,0));
            NaturalMesh.Lathe(elbow, "Forearm", new[] {new Vector3(.05f,-.25f,.05f),new Vector3(.067f,-.10f,.067f),new Vector3(.068f,0,.068f)}, skin, 14);
            NaturalMesh.Oval(elbow, "Relaxed hand", new Vector3(0,-.29f,.012f), new Vector3(.105f,.15f,.095f), skin, 12);
            NaturalMesh.Oval(elbow,"Natural thumb",new Vector3(side*.045f,-.275f,.035f),new Vector3(.035f,.075f,.045f),skin,10);
            return shoulder;
        }
        public void Animate(float clock, bool running, bool sliding, float height, bool reduced, float speed = 9)
        {
            float dt = Mathf.Clamp(clock-lastClock,0,.05f);
            if (clock < lastClock || clock == 0) { gaitPhase = 0; slideBlend = jumpBlend = locomotionBlend = 0; }
            lastClock = clock;
            float blend = clock == 0 ? 1 : 1-Mathf.Exp(-28*dt);
            locomotionBlend = Mathf.Lerp(locomotionBlend,running ? 1 : 0,clock == 0 ? 1 : 1-Mathf.Exp(-18*dt));
            slideBlend = Mathf.Lerp(slideBlend,sliding ? 1 : 0,blend);
            jumpBlend = Mathf.Lerp(jumpBlend,height > .04f ? 1 : 0,blend);
            if (running) gaitPhase += dt*Mathf.Lerp(11,16,Mathf.InverseLerp(9,18,speed));
            Root.localPosition = new Vector3(Root.localPosition.x,height,0);
            float bob = running && !reduced ? (1-Mathf.Cos(gaitPhase*2))*.009f*(1-slideBlend)*(1-jumpBlend) : 0;
            float idleBreath = running || reduced ? 0 : Mathf.Sin(clock*1.7f)*.006f;
            float hipHeight = Mathf.Lerp(Mathf.Lerp(.94f,.80f,locomotionBlend),.34f,slideBlend)+bob+idleBreath;
            torso.localPosition = new Vector3(0,hipHeight,0);
            float twist = reduced ? 0 : Mathf.Sin(gaitPhase)*3*(1-slideBlend);
            torso.localRotation = Quaternion.Euler(Mathf.Lerp(7*locomotionBlend,-72,slideBlend),twist,0);
            head.localRotation = Quaternion.Euler(-3-Mathf.Lerp(7*locomotionBlend,0,slideBlend),-twist*.65f,0);
            for (int i = 0; i < 2; i++)
            {
                float phase = Mathf.Repeat(gaitPhase/(Mathf.PI*2)+i*.5f,1);
                float footZ = 0, footY = .085f;
                if (running)
                {
                    float reach = Mathf.Lerp(.38f,.48f,Mathf.InverseLerp(9,18,speed));
                    float rate = Mathf.Lerp(11,16,Mathf.InverseLerp(9,18,speed));
                    // During the short sprint stance, the foot travels backwards at
                    // ground speed. The longer recovery arc gives a natural knee lift.
                    float stance = Mathf.Clamp(2*reach*rate/(Mathf.Max(1,speed)*Mathf.PI*2),.10f,.25f);
                    if (phase < stance) footZ = Mathf.Lerp(reach,-reach,phase/stance);
                    else { float swing = (phase-stance)/(1-stance); footZ = Mathf.Lerp(-reach,reach,Mathf.SmoothStep(0,1,swing)); footY += Mathf.Sin(swing*Mathf.PI)*.35f; }
                }
                footZ *= locomotionBlend;
                footY = Mathf.Lerp(.085f,footY,locomotionBlend);
                footY = Mathf.Lerp(footY,.22f,jumpBlend);
                footZ = Mathf.Lerp(footZ,i == 0 ? .30f : -.12f,jumpBlend);
                footZ = Mathf.Lerp(footZ,i == 0 ? .47f : .30f,slideBlend);
                footY = Mathf.Lerp(footY,.085f,slideBlend);
                SolveLeg(i,hipHeight,footY,footZ);
            }
            float swingArm = Mathf.Cos(gaitPhase)*25*locomotionBlend;
            leftArm.localRotation = Quaternion.Euler(Mathf.Lerp(swingArm-8,-52,slideBlend),0,-5);
            rightArm.localRotation = Quaternion.Euler(Mathf.Lerp(-swingArm-8,-52,slideBlend),0,5);
            float elbowBase = Mathf.Lerp(-12,-75,locomotionBlend);
            leftElbow.localRotation = Quaternion.Euler(Mathf.Lerp(elbowBase+Mathf.Sin(gaitPhase+.6f)*10*locomotionBlend,-45,slideBlend),0,0);
            rightElbow.localRotation = Quaternion.Euler(Mathf.Lerp(elbowBase-Mathf.Sin(gaitPhase+.6f)*10*locomotionBlend,-45,slideBlend),0,0);
            scarf.localRotation = Quaternion.Euler(reduced ? 0 : -10+Mathf.Sin(clock*7)*7,reduced ? 0 : Mathf.Sin(clock*4)*3,0);
            if (shadow != null)
            {
                shadow.localPosition = new Vector3(Root.localPosition.x,.024f,0);
                float size = Mathf.Lerp(1,.68f,Mathf.Clamp01(height/1.7f)); shadow.localScale = new Vector3(.9f*size,1.25f*size,1);
                shadowMaterial.SetColor("_Tint",new Color(.14f,.10f,.06f,Mathf.Lerp(.30f,.11f,Mathf.Clamp01(height/1.7f))));
            }
        }
        private void SolveLeg(int i, float hipY, float footY, float footZ)
        {
            thighs[i].localPosition = new Vector3(i == 0 ? -.145f : .145f,hipY,0);
            float dy = footY-hipY, d = Mathf.Clamp(Mathf.Sqrt(dy*dy+footZ*footZ),.08f,UpperLeg+LowerLeg-.001f);
            float aim = Mathf.Atan2(footZ,-dy)*Mathf.Rad2Deg;
            float hip = Mathf.Acos(Mathf.Clamp((UpperLeg*UpperLeg+d*d-LowerLeg*LowerLeg)/(2*UpperLeg*d),-1,1))*Mathf.Rad2Deg;
            float knee = 180-Mathf.Acos(Mathf.Clamp((UpperLeg*UpperLeg+LowerLeg*LowerLeg-d*d)/(2*UpperLeg*LowerLeg),-1,1))*Mathf.Rad2Deg;
            float upper = -aim-hip;
            thighs[i].localRotation = Quaternion.Euler(upper,0,0);
            calves[i].localRotation = Quaternion.Euler(knee,0,0);
            feet[i].localRotation = Quaternion.Euler(-upper-knee,0,0);
        }
        public void SetOutfit(int index)
        {
            Color colour=index==1 ? new Color(1,.57f,.055f) : index==2 ? new Color(.96f,.12f,.43f) : index==3 ? new Color(.12f,.27f,.94f) : new Color(.08f,.55f,.61f);
            if(outfitMaterial.HasProperty("_BaseColor")) outfitMaterial.SetColor("_BaseColor",colour);
            if(outfitMaterial.HasProperty("_Color")) outfitMaterial.SetColor("_Color",colour);
        }
        public void Dispose() { if (shadowMaterial != null) Object.Destroy(shadowMaterial); Object.Destroy(outfitMaterial); }
    }
}
