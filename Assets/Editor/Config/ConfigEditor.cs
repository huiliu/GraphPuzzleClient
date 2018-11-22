using System.IO;
using UnityEditor;
using UnityEngine;
using GraphGame.Logic;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace GraphGame.Client.Editor
{
    public class ConfigEditor
        : EditorWindow
    {
        [MenuItem("Tool/关卡编辑器")]
        public static void ShowWindows()
        {
            EditorWindow.GetWindow(typeof(ConfigEditor));
        }

        protected void Awake()
        {
            this.titleContent = new GUIContent("关卡编辑器");
        }

        private ConfigData ConfigData;
        private void LoadFromFile()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Config/" + ConfigMgr.kLevelDataFile);
            if (!File.Exists(path))
            {
                this.ConfigData = new ConfigData();
                return;
            }

            using (var sr = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                this.ConfigData = serializer.Deserialize(sr) as ConfigData;
            }
        }
    }
}
