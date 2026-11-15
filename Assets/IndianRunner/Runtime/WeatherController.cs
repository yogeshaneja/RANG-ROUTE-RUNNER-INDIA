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
        private float rain, shade;
        public float Cloudiness => shade;
        public WeatherController(Transform parent, Camera cam, Light light)
        {
            camera = cam; sun = light;
            var rainMat = PrimitiveArt.Mat(new Color(.65f, .83f, .97f));
            var cloudMat = PrimitiveArt.Mat(new Color(.91f, .93f, .95f));
            for (int i = 0; i < drops.Length; i++)
            {
                drops[i] = PrimitiveArt.Box(parent, "Pooled rain streak", new Vector3((i * 7 % 19 - 9) * .65f, i % 11, i * 11 % 36), new Vector3(.025f, .5f, .025f), rainMat);
                drops[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < clouds.Length; i++)
                clouds[i] = PrimitiveArt.Shape(parent, "Cloud", PrimitiveType.Sphere, new Vector3((i % 2 == 0 ? -1 : 1) * (9 + i), 20 + i % 3 * 2, 20 + i * 17), new Vector3(12, 2, 5), cloudMat);
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 100; RenderSettings.fogEndDistance = 210;
        }
        public void Tick(float time, float dt, bool reduced)
        {
            // Mostly sunny Rajasthan cycle; long clear intervals, occasional light rain.
            float cycle = time % 180f;
            Mode target = Selection == Mode.Auto ? (cycle < 95 ? Mode.Sunny : cycle < 145 ? Mode.Cloudy : Mode.Rain) : Selection;
            Description = target == Mode.Rain ? "Light rain" : target == Mode.Cloudy ? "Cloudy" : "Sunny";
            shade = Mathf.MoveTowards(shade, target == Mode.Sunny ? 0 : .6f, dt * .08f);
            rain = Mathf.MoveTowards(rain, target == Mode.Rain ? 1 : 0, dt * .12f);
            camera.backgroundColor = Color.Lerp(new Color(.18f, .69f, .94f), new Color(.45f, .60f, .70f), shade);
            RenderSettings.fogColor = camera.backgroundColor;
            sun.intensity = 1.2f - shade * .55f;
            RenderSettings.ambientLight = Color.Lerp(new Color(.74f, .76f, .83f), new Color(.63f, .67f, .76f), shade);
            for (int i = 0; i < drops.Length; i++)
            {
                bool visible = rain > .08f && i < rain * (reduced ? 10 : drops.Length);
                drops[i].gameObject.SetActive(visible);
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
