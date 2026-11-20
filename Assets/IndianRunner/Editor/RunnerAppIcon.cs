using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace IndianRunner.Editor
{
    public static class RunnerAppIcon
    {
        public const string AssetPath = "Assets/IndianRunner/Editor/Branding/RangRouteIcon.png";

        [MenuItem("Indian Runner/Apply Rajasthan app icon")]
        public static void Apply()
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetPath);
            if (texture == null)
                throw new InvalidOperationException("Rajasthan app icon is missing: " + AssetPath);

            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { texture }, IconKind.Any);
            // Query the installed Android module instead of referencing its assembly
            // while the project is compiling for desktop without Android support.
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android)) return;
            foreach (var kind in PlayerSettings.GetSupportedIconKinds(NamedBuildTarget.Android))
            {
                var slots = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
                foreach (var slot in slots)
                {
                    var layers = new Texture2D[slot.maxLayerCount];
                    // This full-bleed illustration intentionally fills both adaptive
                    // layers. Android supplies its own circle/squircle mask.
                    for (int layer = 0; layer < layers.Length; layer++) layers[layer] = texture;
                    slot.SetTextures(layers);
                }
                PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, slots);
            }
        }
    }
}
