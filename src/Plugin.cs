using System;

using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using TitleRenamed.Entries;

namespace TitleRenamed
{
    public partial class Plugin : IDalamudPlugin
    {
        internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        internal static ICommandManager CommandMgr { get; private set; } = null!;
        internal static IDataManager DataMgr { get; private set; } = null!;
        internal static IGameGui GameGui { get; private set; } = null!;
        internal static IChatGui ChatGui { get; private set; } = null!;
        internal static IPluginLog PluginLog { get; private set; } = null!;

        public string Name =>
#if DEBUG
            "Title Renamed [DEV]";
#elif TEST
            "Title Renamed [TEST]";
#else
            "Title Renamed";
#endif

        private readonly NameplateHelper npHelper;
        private readonly TitleRenameMap renameMap = [];
        private readonly Configuration config = null!;

        private bool enabled = false;
        internal bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                npHelper.ignoreUpdates = !enabled;
            }
        }
        private bool inConfig = false;

        public Plugin(IDalamudPluginInterface _dalamudPluginInterface,
            ICommandManager _commandManager,
            IDataManager _dataManager,
            IGameGui _gameGui,
            IChatGui _chatGui,
            IPluginLog _pluginLog,
            INamePlateGui _nameplateGui,
            IClientState _clientState,
            IGameInteropProvider _gameInteropProvider)
        {
            PluginInterface = _dalamudPluginInterface;
            CommandMgr = _commandManager;
            DataMgr = _dataManager;
            GameGui = _gameGui;
            ChatGui = _chatGui;
            PluginLog = _pluginLog;

            this.config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            npHelper = new(renameMap, _nameplateGui, _clientState);
            _gameInteropProvider.InitializeFromAttributes(npHelper);

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
