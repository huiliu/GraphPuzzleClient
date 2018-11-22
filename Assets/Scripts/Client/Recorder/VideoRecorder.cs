using GraphGame.Logic;

namespace GraphGame.Client
{
    public class VideoRecorder
        : Recorder
    {
        public VideoRecorder(System.Version version, int levelID, int levelSeed)
            : base(version, levelID, levelSeed)
        {
        }

        protected override string GetArchiveFileName()
        {
            return ResourceMgr.Instance.GetVideoFileFullPath(base.GetArchiveFileName());
        }
    }
}
