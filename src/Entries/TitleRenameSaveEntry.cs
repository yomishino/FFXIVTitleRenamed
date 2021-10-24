using Newtonsoft.Json;
using System;

namespace TitleRenamed.Entries
{
    [Serializable]
    public class TitleRenameSaveEntry
    {
        public string Title = null!;
        public string RenamedTitle = null!;
        public bool IsPrefixTitle;
        public bool ToDisplay;
        public bool RenameEnabled;

        [JsonConstructor]
        public TitleRenameSaveEntry(string title, string renamed, bool isPrefix, bool toDisplay, bool enabled)
        {
            Title = title ?? throw new ArgumentNullException(paramName: nameof(title));
            RenamedTitle = renamed;
            IsPrefixTitle = isPrefix;
            ToDisplay = toDisplay;
            RenameEnabled = enabled;
        }

        public TitleRenameSaveEntry(string title, TitleRenameEntry entry)
        {
            Title = title ?? throw new ArgumentNullException(paramName: nameof(title));
            if (entry == null) throw new ArgumentNullException(paramName: nameof(entry));
            RenamedTitle = entry.RenamedTitle;
            IsPrefixTitle = entry.IsPrefixTitle;
            ToDisplay = entry.ToDisplay;
            RenameEnabled = entry.RenameEnabled;
        }

        public override string ToString()
            => $"<Title:{Title},Renamed:{RenamedTitle},IsPrefix:{IsPrefixTitle},ToDisplay:{ToDisplay},Enabled:{RenamedTitle}>";
    }
}
