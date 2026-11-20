#if UNITY_ANDROID
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Android;

namespace IndianRunner.Editor
{
    // Add only service discovery to Unity's generated manifest. Preserve the
    // selected Activity/GameActivity, permissions and all existing manifest data.
    public sealed class RunnerSpeechManifest : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 50;
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifest=Path.Combine(path,"src/main/AndroidManifest.xml");
            var document=XDocument.Load(manifest); var root=document.Root;
            if(root==null) throw new System.InvalidOperationException("Generated Android manifest has no root.");
            XNamespace android="http://schemas.android.com/apk/res/android";
            var queries=root.Element("queries");
            if(queries==null) { queries=new XElement("queries"); root.Add(queries); }
            bool exists=queries.Elements("intent").Elements("action").Any(a=>(string)a.Attribute(android+"name")=="android.intent.action.TTS_SERVICE");
            if(!exists) queries.Add(new XElement("intent",new XElement("action",new XAttribute(android+"name","android.intent.action.TTS_SERVICE"))));
            document.Save(manifest);
        }
    }
}
#endif
