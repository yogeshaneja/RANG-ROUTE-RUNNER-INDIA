using UnityEngine;

namespace IndianRunner
{
    public static class ExpandedWeather
    {
        public static bool Hills(int region) => region==RegionProfile.Shillong || region==RegionProfile.Himachal;
        public static WeatherController.Mode Auto(int region,float cycle)
        {
            if(region==RegionProfile.Shillong)return cycle<70 ? WeatherController.Mode.Rain:cycle<135 ? WeatherController.Mode.Cloudy:WeatherController.Mode.Sunny;
            if(region==RegionProfile.Kolkata)return cycle<75 ? WeatherController.Mode.Sunny:cycle<125 ? WeatherController.Mode.Cloudy:WeatherController.Mode.Rain;
            if(region==RegionProfile.MadhyaPradesh)return cycle<90 ? WeatherController.Mode.Sunny:cycle<145 ? WeatherController.Mode.Cloudy:WeatherController.Mode.Rain;
            return cycle<105 ? WeatherController.Mode.Sunny:cycle<165 ? WeatherController.Mode.Cloudy:WeatherController.Mode.Rain;
        }
        public static Color Sky(int region) => region==RegionProfile.Shillong ? new Color(.52f,.77f,.81f):region==RegionProfile.Kolkata ? new Color(.65f,.79f,.88f):region==RegionProfile.MadhyaPradesh ? new Color(.51f,.76f,.83f):new Color(.44f,.72f,.91f);
    }
}
