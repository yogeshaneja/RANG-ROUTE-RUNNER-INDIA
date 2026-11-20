using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace IndianRunner.Editor
{
    // Let this installed Editor serialize its own shader reference. Never hand-code
    // the native built-in shader GUID/fileID in a cross-version material asset.
    public sealed class RunnerBuildPreparation : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;

        [InitializeOnLoadMethod]
        private static void ScheduleMaterialPreparation()
        {
            EditorApplication.delayCall += PrepareAfterImport;
        }
        private static void PrepareAfterImport()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += PrepareAfterImport;
                return;
            }
            try
            {
                RunnerAppIcon.Apply();
                var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/IndianRunner/Resources/RunnerBase.mat");
                var shader = PrimitiveArt.ResolveShader();
                if (material == null || material.shader == null || material.shader != shader)
                    RunnerSetup.RepairMaterials();
            }
            catch (System.Exception error)
            {
                Debug.LogError("Runner build preparation failed: " + error.Message + " Check the icon asset and use Indian Runner > Repair runner materials before building.");
            }
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            // Also covers a build started without first opening the Rajasthan scene
            // or entering Play Mode; validation fails before packaging if repair fails.
            RunnerSetup.RepairMaterials();
            RunnerAppIcon.Apply();
        }
    }
}
