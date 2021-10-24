using TitleRenamed.Entries;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;

namespace TitleRenamed
{
    public partial class Plugin : IDalamudPlugin
    {
        [PluginService]
        [RequiredVersion("1.0")]
        internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static SigScanner SigScanner { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static CommandManager CommandMgr { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static DataManager DataMgr { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static GameGui GameGui { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static ChatGui ChatGui { get; private set; } = null!;

        public string Name =>
#if DEBUG
            "Title Renamed [DEV]";
#elif TEST
            "Title Renamed [TEST]";
#else
            "Title Renamed";
#endif

        private readonly NameplateHelper npHelper = null!;
        private readonly TitleRenameMap renameMap = new();
        private readonly Configuration config = null!;

        private bool enabled = false;
        internal bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (value)
                    npHelper?.EnableHook();
                else
                    npHelper?.DisableHook();
            }
        }
        private bool inConfig = false;

        public Plugin()
        {
            this.config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            npHelper = new(renameMap);

            AddCommands();

            PluginInterface.UiBuilder.Draw += OnDrawUi;
            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

            LoadConfig(true);
        }

        public void LoadConfig(bool firstRunSinceStart = false)
        {
            renameMap.FromSaveEntryArray(config.TitleRenameArray);

            if (firstRunSinceStart)
                Enabled = config.AutoEnableOnStart;
        }

        public void SaveConfig()
        {
            config.TitleRenameArray = renameMap.ToSaveEntryArray();

            PluginInterface.SavePluginConfig(config);
        }

        private void OnDrawUi()
        {
            if (inConfig)
                DrawConfigUi();
        }

        private void OnOpenConfigUi()
        {
            inConfig = true;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            // Auto-save config
            SaveConfig();

            PluginInterface.UiBuilder.Draw -= OnDrawUi;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;

            RemoveCommands();

            npHelper.Dispose();
            renameMap.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
