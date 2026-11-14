using UnityEngine;

namespace IndianRunner
{
    public sealed class RunnerModel
    {
        public Transform Root { get; }
        private readonly Transform body, leftArm, rightArm, leftLeg, rightLeg, scarf;
        public RunnerModel(Transform parent)
        {
            Root = new GameObject("Original placeholder runner").transform;
            Root.SetParent(parent, false);
            body = new GameObject("Animated body").transform; body.SetParent(Root, false);
            var skin = PrimitiveArt.Mat(new Color(0.58f, 0.33f, 0.19f));
            var kurta = PrimitiveArt.Mat(new Color(0.02f, 0.65f, 0.72f));
            var trousers = PrimitiveArt.Mat(new Color(0.97f, 0.91f, 0.72f));
            var hair = PrimitiveArt.Mat(new Color(0.09f, 0.06f, 0.04f));
            var orange = PrimitiveArt.Mat(new Color(1f, 0.32f, 0.08f));
            PrimitiveArt.Box(body, "Teal kurta", new Vector3(0, 1.18f, 0), new Vector3(.60f, .73f, .34f), kurta);
            PrimitiveArt.Shape(body, "Head", PrimitiveType.Sphere, new Vector3(0, 1.82f, 0), Vector3.one * .43f, skin);
            PrimitiveArt.Shape(body, "Hair", PrimitiveType.Sphere, new Vector3(0, 1.99f, -.01f), new Vector3(.45f, .17f, .43f), hair);
            PrimitiveArt.Box(body, "Scarf collar", new Vector3(0, 1.52f, .03f), new Vector3(.65f, .13f, .38f), orange);
            scarf = PrimitiveArt.Box(body, "Scarf tail", new Vector3(.20f, 1.20f, -.24f), new Vector3(.16f, .63f, .06f), orange);
            leftArm = Limb(body, "Left arm", new Vector3(-.40f, 1.45f, 0), new Vector3(.20f, .62f, .23f), kurta);
            rightArm = Limb(body, "Right arm", new Vector3(.40f, 1.45f, 0), new Vector3(.20f, .62f, .23f), kurta);
            leftLeg = Limb(body, "Left leg", new Vector3(-.17f, .85f, 0), new Vector3(.23f, .74f, .24f), trousers);
            rightLeg = Limb(body, "Right leg", new Vector3(.17f, .85f, 0), new Vector3(.23f, .74f, .24f), trousers);
        }
        private static Transform Limb(Transform parent, string name, Vector3 position, Vector3 size, Material mat)
        {
            var pivot = new GameObject(name).transform; pivot.SetParent(parent, false); pivot.localPosition = position;
            PrimitiveArt.Box(pivot, name + " mesh", new Vector3(0, -size.y / 2, 0), size, mat);
            return pivot;
        }
        public void Animate(float clock, bool running, bool sliding, float height, bool reduced)
        {
            Root.localPosition = new Vector3(Root.localPosition.x, height, 0);
            body.localScale = sliding ? new Vector3(1, .42f, 1.35f) : Vector3.one;
            body.localPosition = new Vector3(0, running && !reduced && height <= 0 ? Mathf.Abs(Mathf.Sin(clock * 12)) * .045f : 0, 0);
            float swing = running ? Mathf.Sin(clock * 12) * (reduced ? 15 : 38) : 0;
            leftArm.localRotation = Quaternion.Euler(swing, 0, 0); rightArm.localRotation = Quaternion.Euler(-swing, 0, 0);
            leftLeg.localRotation = Quaternion.Euler(-swing, 0, 0); rightLeg.localRotation = Quaternion.Euler(swing, 0, 0);
            scarf.localRotation = Quaternion.Euler(reduced ? 0 : Mathf.Sin(clock * 7) * 12, 0, 0);
        }
    }
}
