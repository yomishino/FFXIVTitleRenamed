using Dalamud.Game.Command;

namespace TitleRenamed
{
    public partial class Plugin
    {
        private const string cmdMainToggle = "/titlerenamed";

        private void AddCommands()
        {
            CommandMgr.AddHandler(cmdMainToggle, new CommandInfo(ProcessCmdMainToggle)
            {
                HelpMessage = 
                    $"\n\t{cmdMainToggle}: Toggle whether to enable Title Renamed\n" +
                    $"\t{cmdMainToggle} [on|off]: Enable/Disable Title Renamed",
                ShowInHelp = true
            });
        }

        private static void RemoveCommands()
        {
            CommandMgr.RemoveHandler(cmdMainToggle);
        }

        private void ProcessCmdMainToggle(string command, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                Enabled = !Enabled;
            else if (args.Trim() == "on")
                Enabled = true;
            else if (args.Trim() == "off")
                Enabled = false;
            else
                Util.PrintChatError($"{command}: Unknown command args: {args}");
        }
    }
}
