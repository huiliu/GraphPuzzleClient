using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphGame.Client
{
    public class Version
    {
        public static int Major = 0;
        public static int Minor = 1;
        public static int Patch = 0;

        private string version;
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.version))
            {
                this.version = string.Format("{0}.{1}.{2}", Major, Minor, Patch);
            }

            return this.version;
        }
    }
}
