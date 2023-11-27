using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodStain
{
    public class WoodStainConfig
    {
        public static WoodStainConfig Loaded { get; set; } = new WoodStainConfig();

        public bool DummySettingBool { get; set; } = true;
    }
}