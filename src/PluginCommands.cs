using Dalamud.Game.Command;

namespace TitleRenamed
{
    public partial class Plugin
    {
        private const string cmdTitleRenamedMain = "/titlerenamed";

        private void AddCommands()
        {
            CommandMgr.AddHandler(cmdTitleRenamedMain, new CommandInfo(ProcessCmdMainToggle)
            {
                HelpMessage = 
                    $"\n\t{cmdTitleRenamedMain}: Toggle whether to enable Title Renamed\n" +
                    $"\t{cmdTitleRenamedMain} [on|off]: Enable/Disable Title Renamed\n" +
                    $"\t{cmdTitleRenamedMain} config: Open Title Renamed configuration window",
                ShowInHelp = true
            });
        }

        private static void RemoveCommands()
        {
            CommandMgr.RemoveHandler(cmdTitleRenamedMain);
        }

        private void ProcessCmdMainToggle(string command, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Enabled = !Enabled;
                Util.PrintChat("Plugin " + (Enabled ? "enabled" : "disabled"), true);
            }
            else if (args.Trim() == "on")
            {
                Enabled = true;
                Util.PrintChat("Plugin " + (Enabled ? "enabled" : "disabled"), true);
            }
            else if (args.Trim() == "off")
            {
                Enabled = false;
                Util.PrintChat("Plugin " + (Enabled ? "enabled" : "disabled"), true);
            }
            else if (args.Trim() == "config")
            {
                inConfig = true;
            }
            else
                Util.PrintChatError($"{command}: Unknown command args: {args}");
        }
    }
}
