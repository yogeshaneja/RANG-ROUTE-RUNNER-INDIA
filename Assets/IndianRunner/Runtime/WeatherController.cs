using UnityEngine;

namespace IndianRunner
{
    public sealed class WeatherController
    {
        public enum Mode { Auto, Sunny, Cloudy, Rain }
        public Mode Selection { get; set; }
        public string Description { get; private set; } = "Sunny";
        private readonly Camera camera;
        private readonly Light sun;
        private readonly Transform[] drops = new Transform[48];
        private readonly Transform[] clouds = new Transform[8];
        private readonly bool[] dropVisible;
        private float appliedShade=-1;
        private float rain, shade;
        private readonly bool kerala;
        public float Cloudiness => shade;
        public float RainIntensity => rain;
        public WeatherController(Transform parent, Camera cam, Light light,bool kerala=false)
        {
            this.kerala=kerala; camera = cam; sun = light;
            shade=kerala ? .35f:0; rain=kerala ? .35f:0; dropVisible=new bool[drops.Length];
            var rainMat = PrimitiveArt.Mat(new Color(.65f, .83f, .97f));
            var cloudMat = PrimitiveArt.Mat(new Color(.91f, .93f, .95f));
            for (int i = 0; i < drops.Length; i++)
            {
                drops[i] = PrimitiveArt.Box(parent, "Pooled rain streak", new Vector3((i * 7 % 19 - 9) * .65f, i % 11, i * 11 % 36), new Vector3(.025f, .5f, .025f), rainMat);
                drops[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < clouds.Length; i++)
            {
                var cloud = new GameObject("Irregular layered cloud").transform; cloud.SetParent(parent,false);
                cloud.localPosition = new Vector3((i%2 == 0 ? -1 : 1)*(9+i),20+i%3*2,20+i*17);
                NaturalMesh.Oval(cloud,"Cloud main lobe",Vector3.zero,new Vector3(9+i%3,2.1f,4.8f),cloudMat,11);
                NaturalMesh.Oval(cloud,"Cloud upper lobe",new Vector3(-1.5f,.8f,.3f),new Vector3(5.4f,2.7f,4.1f),cloudMat,11);
                NaturalMesh.Oval(cloud,"Cloud trailing lobe",new Vector3(3,-.2f,-.2f),new Vector3(5.6f,1.6f,3.5f),cloudMat,11);
                PrimitiveArt.CombineScenery(cloud,true); clouds[i] = cloud;
            }
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = kerala ? 85:100; RenderSettings.fogEndDistance = kerala ? 185:210;
        }
        public void Tick(float time, float dt, bool reduced)
        {
            // Region-specific cosmetic cycles: Rajasthan mostly clear; Kerala begins
            // in rain, clears through cloud, then gets a bright interval.
            float cycle = time % 180f;
            Mode target = Selection == Mode.Auto ? (kerala ? (cycle<75 ? Mode.Rain:cycle<125 ? Mode.Cloudy:Mode.Sunny):(cycle < 95 ? Mode.Sunny : cycle < 145 ? Mode.Cloudy : Mode.Rain)) : Selection;
            Description = target == Mode.Rain ? (kerala ? "Monsoon rain":"Light rain") : target == Mode.Cloudy ? "Cloudy" : "Sunny";
            shade = Mathf.MoveTowards(shade, target == Mode.Sunny ? 0 : .6f, dt * .08f);
            rain = Mathf.MoveTowards(rain, target == Mode.Rain ? 1 : 0, dt * .12f);
            if(Mathf.Abs(shade-appliedShade)>.0001f)
            {
                appliedShade=shade;
                camera.backgroundColor = Color.Lerp(kerala ? new Color(.49f,.77f,.79f):new Color(.48f,.72f,.86f), new Color(.61f,.67f,.71f), shade);
                RenderSettings.fogColor = camera.backgroundColor;
                sun.intensity = 1.2f - shade * .55f;
                sun.color = Color.Lerp(new Color(1,.91f,.79f),new Color(.93f,.96f,1),shade);
                RenderSettings.ambientLight = Color.Lerp(new Color(.76f,.72f,.65f), new Color(.64f,.67f,.72f), shade);
            }
            for (int i = 0; i < drops.Length; i++)
            {
                bool visible = rain > .08f && i < rain * (reduced ? 10 : drops.Length);
                if(dropVisible[i]!=visible)
                {
                    drops[i].gameObject.SetActive(visible); dropVisible[i]=visible;
                }
                if (visible) drops[i].localPosition = new Vector3((i * 7 % 19 - 9) * .65f, 11 - Mathf.Repeat(time * 11 + i * .73f, 11), 3 + i * 11 % 36);
            }
            for (int i = 0; i < clouds.Length; i++)
            {
                var p = clouds[i].localPosition;
                p.x = (i % 2 == 0 ? -1 : 1) * (9 + i) + (reduced ? 0 : Mathf.Sin(time * .025f + i) * 3);
                clouds[i].localPosition = p;
            }
        }
    }
}
