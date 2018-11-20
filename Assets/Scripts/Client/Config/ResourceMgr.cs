using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GraphGame.Client
{
    public class ResourceMgr
    {
        public static ResourceMgr Instance { get { return instance; } }
        private static ResourceMgr instance = new ResourceMgr();

        private ResourceMgr() { }
        public void Init()
        {
            this.RefreshUpdateList();
        }

        private Dictionary<string, bool> UpdateList = new Dictionary<string, bool>();
        private void RefreshUpdateList()
        {
            var UpdateListFile = Application.persistentDataPath + "/updatelist.xml";
            if (!File.Exists(UpdateListFile))
            {
                return;
            }

            using (var sr = new StreamReader(UpdateListFile))
            {
                this.UpdateList.Add(sr.ReadLine().Trim(), true);
            }
        }

        private bool HasUpdated(string file)
        {
            return this.UpdateList.ContainsKey(file);
        }

        #region ConfigFile
        public const string kConfigFilePath = "Config/";
        public string GetConfigFileFullPath(string name)
        {
            var path = "";
            if (this.HasUpdated(kConfigFilePath + name))
                path = Path.Combine(Application.persistentDataPath, kConfigFilePath + name);
            else
                path = Path.Combine(Application.streamingAssetsPath, kConfigFilePath + name);
            //return Application.streamingAssetsPath + kConfigFilePath + name; 
            return path;
        }

        public void LoadConfig(string name, Action<StreamReader> callback)
        {
            var path = this.GetConfigFileFullPath(name);
            if (!File.Exists(path))
            {
                callback(null);
                return;
            }

            using (var sr = new StreamReader(path))
            {
                callback.SafeInvoke(sr);
            }
        }
        #endregion
    }
}
