using TitleRenamed.Entries;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using Dalamud.Plugin.Services;

namespace TitleRenamed
{
    public partial class Plugin : IDalamudPlugin
    {
        internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
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

        public Plugin(DalamudPluginInterface _dalamudPluginInterface,
            ICommandManager _commandManager,
            IDataManager _dataManager,
            IGameGui _gameGui,
            IChatGui _chatGui,
            IPluginLog _pluginLog,
            IGameInteropProvider _gameInteropProvider)
        {
            PluginInterface = _dalamudPluginInterface;
            CommandMgr = _commandManager;
            DataMgr = _dataManager;
            GameGui = _gameGui;
            ChatGui = _chatGui;
            PluginLog = _pluginLog;

            this.config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();


            npHelper = new(renameMap);
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
