using Lumina.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

using TitleSheet = Lumina.Excel.GeneratedSheets.Title;

namespace TitleRenamed
{
    public class Titles
    {
        public static ExcelSheet<TitleSheet>? TitleSheet => Plugin.DataMgr.GetExcelSheet<TitleSheet>();
        
        public static Dictionary<string, bool> GetAllUniqueTitles()
        {
            var dict = new Dictionary<string, bool>();
            var sheet = TitleSheet;
            if (sheet == null) return dict;
            for (uint i = 0; i < sheet.RowCount; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;
                if (!string.IsNullOrEmpty(row.Masculine))
                    dict.TryAdd(row.Masculine, row.IsPrefix);
                if (!string.IsNullOrEmpty(row.Feminine))
                    dict.TryAdd(row.Feminine, row.IsPrefix);
            }
            return dict;
        }

        public static SortedSet<(string Title, bool IsPrefix)> GetAllUniqueTitleSorted()
        {
            var set = new SortedSet<(string, bool)>();
            var sheet = TitleSheet;
            if (sheet == null) return set;
            for (uint i = 0; i < sheet.RowCount; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;
                if (!string.IsNullOrEmpty(row.Masculine))
                    set.Add((row.Masculine, row.IsPrefix));
                if (!string.IsNullOrEmpty(row.Feminine))
                    set.Add((row.Feminine, row.IsPrefix));
            }
            return set;
        }

        public static IEnumerable<(string, bool)> GetCurrentInputMatchedTitles(
            string input, bool ignoreCase = true, int maxResults = 10, SortedSet<(string, bool)>? titles = null)
        {
            if (input == null) throw new ArgumentNullException(paramName: nameof(input));
            titles ??= GetAllUniqueTitleSorted();
            StringComparison rule = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return titles.Where(item => item.Item1.StartsWith(input, rule)).Take(maxResults);
        }
        
        public static (string, bool) GetCurrentInputFirstMatchedTitle(
            string input, bool ignoreCase = true, IEnumerable<(string, bool)>? matched = null, SortedSet<(string, bool)>? titles = null)
        {
            if (input == null) throw new ArgumentNullException(paramName: nameof(input));
            if (input == string.Empty) return ("", true);
            matched ??= GetCurrentInputMatchedTitles(input, ignoreCase, 1, titles);
            return matched.Any() ? matched.First() : ("", true);
        }

        public static bool Exists(string title)
            => TitleSheet != null && !string.IsNullOrEmpty(title) && TitleSheet.Any(row => row.Masculine == title || row.Feminine == title);
    }
}
