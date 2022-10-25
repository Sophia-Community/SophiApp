using System;
using System.Collections.Generic;

namespace SophiApp.Dto
{
    internal class CPPRedistrCollection
    {
        public List<CPPRedistrLib> Supported = new List<CPPRedistrLib>();

        public List<CPPRedistrLib> Unsupported = new List<CPPRedistrLib>();
    }

    internal class CPPRedistrLib
    {
        public string Architecture { get; set; }
        public string Download { get; set; }
        public string Install { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public uint Release { get; set; }
        public string SilentInstall { get; set; }
        public string SilentUninstall { get; set; }
        public uint UninstallKey { get; set; }
        public string URL { get; set; }
        public Version Version { get; set; }
    }
}