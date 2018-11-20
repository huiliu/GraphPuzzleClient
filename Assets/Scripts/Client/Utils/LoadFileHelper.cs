using System.IO;

namespace GraphGame.Client
{
    /// <summary>
    /// 貌似在android上读取文件有坑
    /// </summary>
    public static class LoadFileHelper
    {
        public static string ReadAllText(string path)
        {
            if (path.Contains("file://"))
            {
#if UNITY_ANDROID
                Log.Print("Loader", "begin www load");
                UnityEngine.WWW www = new UnityEngine.WWW(path);
                while (!www.isDone && www.error == null)
                {

                }
                if (www.error != null)
                {
                    Log.Error("load file error + " + www.error);
                    return string.Empty;
                }
                return www.text;
#endif
            }
            return File.ReadAllText(path);

        }

        public static byte[] ReadAllBytes(string path)
        {
            if (path.Contains("://"))
            {
#if UNITY_ANDROID
                UnityEngine.WWW www = new UnityEngine.WWW(path);

                while (!www.isDone && www.error == null) { }
                if (www.error != null)
                {
                    Log.Error("load file error + " + www.error);
                    return null;
                }
                return www.bytes;
#endif
            }
            return File.ReadAllBytes(path);
        }
    }
}
