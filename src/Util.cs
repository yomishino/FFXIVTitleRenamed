using TitleRenamed.Strings;
using FFXIVClientStructs.FFXIV.Client.System.String;


namespace TitleRenamed
{
    public static class Util
    {
        public static void LogDebug(string msg)
        {
            Plugin.PluginLog.Debug(msg);
        }

        public unsafe static void LogDebug(Utf8String str)
        {
            LogDebug(ClientStringHelper.GetRawString(str));
        }

        public static void LogWarning(string msg)
        {
            Plugin.PluginLog.Warning(msg);
        }

        public static void LogError(string msg)
        {
            Plugin.PluginLog.Error(msg);
        }

        public static void PrintChat(string msg, bool prependPluginName = false)
        {
            if (prependPluginName)
                msg = $"[Title Renamed] " + msg;
            Plugin.ChatGui.Print(msg);
        }

        public static void PrintChatError(string msg, bool prependPluginName = false)
        {
            if (prependPluginName)
                msg = $"[Title Renamed] " + msg;
            Plugin.ChatGui.PrintError(msg);
        }
    }
}
