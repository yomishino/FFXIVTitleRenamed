using Dalamud.Utility;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace TitleRenamed.Entries
{
    public class TitleRenameEntry
    {
        private string renamedTitle = null!;
        public string RenamedTitle
        {
            get => renamedTitle;
            internal set
            {
                renamedTitle = value ?? string.Empty;
                TitleString = !renamedTitle.IsNullOrWhitespace()
                    ? new SeString(new TextPayload(renamedTitle))
                    : SeString.Empty;
            }
        }
        public bool IsPrefixTitle { get; internal set; }
        public bool ToDisplay { get; internal set; }
        public bool RenameEnabled { get; internal set; }
        internal SeString TitleString { get; private set; } = SeString.Empty;


        public TitleRenameEntry(string renamed, bool isPrefix, bool toDisplay, bool enabled)
        {
            RenamedTitle = renamed ?? string.Empty;
            IsPrefixTitle = isPrefix;
            ToDisplay = toDisplay;
            RenameEnabled = enabled;
        }

        public TitleRenameEntry(TitleRenameSaveEntry entry)
            : this(entry.RenamedTitle, entry.IsPrefixTitle, entry.ToDisplay, entry.RenameEnabled) { }

        public override string ToString()
            => $"RenamedTo:{RenamedTitle},IsPrefix:{IsPrefixTitle},ToDisplay:{ToDisplay},Enabled:{RenameEnabled}";
    }
}
