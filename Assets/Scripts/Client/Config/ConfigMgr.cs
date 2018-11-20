using System.IO;
using System.Xml.Serialization;
using GraphGame.Logic;

namespace GraphGame.Client
{
    public class ConfigMgr
    {
        public static ConfigMgr Instance { get { return instance; } }
        private static ConfigMgr instance = new ConfigMgr();

        private ConfigMgr() { }
        public void Init()
        {
            this.LoadConfig();
        }

        public const string kLevelDataFile = "level.xml";
        public ConfigData LevelData { get; private set; }
        private void ParseLevelConfig(StreamReader sr)
        {
            if (sr == null)
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(ConfigData));
            this.LevelData = serializer.Deserialize(sr) as ConfigData;
        }

        private void LoadConfig()
        {
            ResourceMgr.Instance.LoadConfig(kLevelDataFile, this.ParseLevelConfig);
        }

        public int LevelCount { get { return this.LevelData.Levels.Count; } }
        public LevelData GetLevelConfig(int idx)
        {
            return this.LevelData.Levels[idx];
        }

        public void SaveConfig(ConfigData data)
        {
            var path = ResourceMgr.Instance.GetConfigFileFullPath(kLevelDataFile);
            using (var sw = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                serializer.Serialize(sw, data);
            }
        }

        #region Test
        public void TestConfig()
        {
            var level = new LevelData();
            level.Seed = 10;
            level.BoardHeight = 5;
            level.BoardWidth = 5;
            level.Squares.Add(new SquareData()
            {
                Type = Logic.SquareType.A,
                ColorWeights = new System.Collections.Generic.List<SquareData.ColorWeightTuple>
                {
                    new SquareData.ColorWeightTuple
                    {
                        Color = Logic.Color.Red,
                        Weight = 100
                    },
                    new SquareData.ColorWeightTuple
                    {
                        Color = Logic.Color.Green,
                        Weight = 100
                    }
                }
            });
            level.unUsedSquareID.Add(0);
            var data = new ConfigData();
            data.Levels.Add(level);
            this.SaveConfig(data);

            this.LoadConfig();
        }
        #endregion
    }
}
