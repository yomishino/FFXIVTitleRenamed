using Dalamud.Configuration;
using TitleRenamed.Entries;
using System;


namespace TitleRenamed
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool AutoEnableOnStart { get; set; } = false;
        public TitleRenameSaveEntry[]? TitleRenameArray = null;
    }
}
