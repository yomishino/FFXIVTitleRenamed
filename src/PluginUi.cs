using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace TitleRenamed
{
    public partial class Plugin
    {
        private const uint titleInputMaxCharCount = 39;
        private string newEntryTitleInput = string.Empty;
        private IEnumerable<(string Title, bool IsPrefix)> newEntryTitleInputCurrentMatches = null;
        private bool newEntryTitleInputMatchesToDraw = false;
        private string newEntryRenamedInput = string.Empty;
        private int newEntryPrefixComboSelected = 0;
        private bool newEntryDisplayed = true;
        private string? titleToRemove = null;

        private int lastSavedEpoch = 0;
        private static int CurrentEpoch => (int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;


        //private static Vector2 UiTextBaseSize => ImGui.CalcTextSize("A");
        private static Vector2 UiBaseBoxSize => new(ImGui.GetTextLineHeightWithSpacing(), ImGui.GetTextLineHeightWithSpacing());

        private void DrawConfigUi()
        {
            // Config Ui Main
            ImGui.Begin("Title Renamed: Configuration", ref inConfig);

            bool enabled = this.enabled;
            ImGui.Checkbox("Enable plugin", ref enabled);
            Enabled = enabled;

            ImGui.NewLine();

            bool autoEnable = config.AutoEnableOnStart;
            ImGui.Checkbox("Auto enable plugin on start", ref autoEnable);
            if (ImGui.IsItemHovered()) 
                ImGui.SetTooltip("Auto enable the plugin on start of the game in future, "
                    + "regardless of whether the plugin is currently enabled or not.");
            config.AutoEnableOnStart = autoEnable;
            
            ImGui.NewLine();

            DrawConfigUiTitleRenameList();

            if (ImGui.Button("Save Title Rename List"))
            {
                SaveConfig();
                lastSavedEpoch = CurrentEpoch;
            }
            int sinceLastSaved = CurrentEpoch - lastSavedEpoch;
            if (sinceLastSaved > 0 && sinceLastSaved < 4)
            {
                ImGui.SameLine();
                ImGui.Text("List saved!");
            }

            ImGui.NewLine();
            ImGui.Checkbox("Show Sponsor/Support button", ref config.ShowSponsor);
            if (config.ShowSponsor)
            {
                ImGui.Indent();
                ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
                var sponsorButton = ImGui.Button("Buy Yomishino a Coffee");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(
                        "You can support me and buy me a coffee if you want.\n" +
                        "(Will open external link to Ko-fi in your browser)");
                if (sponsorButton)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://ko-fi.com/yomishino",
                        UseShellExecute = true
                    });
                }
                ImGui.PopStyleColor(3);
                ImGui.Unindent();
            }

            // Config Ui Popups
            DrawConfgUiPopups();

            ImGui.End();
        }

        private void DrawConfigUiTitleRenameList()
        {
            // |Enable |Title |Renamed |Prefix |Display |Add/Remove |
            if (ImGui.CollapsingHeader("Title Rename List", ImGuiTreeNodeFlags.DefaultOpen))
            {
                int tblColCount = 6;
                var flags = ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit;
                if (ImGui.BeginTable("Table_TitleRenameList", tblColCount, flags))
                {
                    // header
                    ImGui.TableSetupColumn("##Enable", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Original Title", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("Renamed to ...", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("##Prefix/Suffix", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("##Display", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("##Add/Remove", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableHeadersRow();
                    // body: list added
                    foreach (var (title, entry) in renameMap)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        bool enabled = entry.RenameEnabled;
                        ImGui.PushItemWidth(-1);
                        ImGui.Checkbox($"##Enable_{title}", ref enabled);
                        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Enable rename for this title");
                        ImGui.PopItemWidth();
                        if (entry.RenameEnabled != enabled) entry.RenameEnabled = enabled;
                        ImGui.TableNextColumn();
                        if (Titles.Exists(title))
                        {
                            ImGui.Text(title);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(title);
                        }
                        else
                        {
                            ImGui.TextDisabled(title);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip($"{title}\n\nThis title is not found in game data. \nYou may want to remove this entry.");
                        }
                        ImGui.TableNextColumn();
                        // TODO: edit renamed title
                        ImGui.Text(entry.RenamedTitle);
                        if (ImGui.IsItemHovered() && entry.RenamedTitle != string.Empty) ImGui.SetTooltip(entry.RenamedTitle);
                        ImGui.TableNextColumn();
                        int prefixSelected = entry.IsPrefixTitle ? 0 : 1;
                        ImGui.PushItemWidth(-1);
                        DrawConfigUiTitleRenameListPrefixCombo($"##Prefix_{title}", ref prefixSelected);
                        ImGui.PopItemWidth();
                        if ((entry.IsPrefixTitle && prefixSelected != 0) || (!entry.IsPrefixTitle && prefixSelected != 1))
                            entry.IsPrefixTitle = prefixSelected == 0;
                        ImGui.TableNextColumn();
                        bool display = entry.ToDisplay;
                        ImGui.Checkbox($"Display##{title}", ref display);
                        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Display or hide the title from the nameplate");
                        if (entry.ToDisplay != display) entry.ToDisplay = display;
                        ImGui.TableNextColumn();
                        ImGui.PushItemWidth(-1);
                        if (ImGui.Button($"-##{title}", UiBaseBoxSize))
                            titleToRemove = title;
                        ImGui.PopItemWidth();
                    }
                    // Check remove after enumeration
                    CheckIfRemoveTitleRenameEntry();
                    // Use colour & tooltip to indicate if title not found
                    ImGui.TableNextRow();
                    // body: last row - add new
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Dummy(UiBaseBoxSize);
                    ImGui.TableNextColumn();
                    // TODO: search instead of just match first few chars?
                    ImGui.PushItemWidth(-1);
                    ImGui.InputText("##Title_New", ref newEntryTitleInput, titleInputMaxCharCount);
                    ImGui.PopItemWidth();
                    if (ImGui.IsItemActive())
                        newEntryTitleInputMatchesToDraw = true;
                    if (newEntryTitleInputMatchesToDraw)
                        DrawConfigUiTitleRenameListNewEntryMatchedTitles();
                    ImGui.TableNextColumn();
                    ImGui.PushItemWidth(-1);
                    ImGui.InputText("##Renamed_New", ref newEntryRenamedInput, titleInputMaxCharCount);
                    ImGui.PopItemWidth();
                    ImGui.TableNextColumn();
                    DrawConfigUiTitleRenameListPrefixCombo("##Prefix_New", ref newEntryPrefixComboSelected);
                    ImGui.TableNextColumn();
                    ImGui.PushItemWidth(-1);
                    ImGui.Checkbox("Display##New", ref newEntryDisplayed);
                    ImGui.PopItemWidth();
                    ImGui.TableNextColumn();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.Button("+", UiBaseBoxSize))
                        CheckIfAddNewTitleRenameEntry();
                    ImGui.PopItemWidth();
                    ImGui.EndTable();
                }

            }
        }

        private static void DrawConfigUiTitleRenameListPrefixCombo(string label, ref int selected)
        {
            ImGui.PushItemWidth(UiBaseBoxSize.X * 3);
            ImGui.Combo(label, ref selected, "Prefix\0Suffix\0");
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("Whether the renamed title is to be displayed as Prefix (before the name) or Suffix (after the name)");
            ImGui.PopItemWidth();
        }

        private void DrawConfigUiTitleRenameListNewEntryMatchedTitles()
        {
            newEntryTitleInputCurrentMatches = Titles.GetCurrentInputMatchedTitles(newEntryTitleInput);
            if (newEntryTitleInputCurrentMatches != null && newEntryTitleInputCurrentMatches.Any())
            {
                string titles = "";
                foreach (var (title, _) in newEntryTitleInputCurrentMatches)
                    titles += title + "\n";
                ImGui.PushItemWidth(-1);
                ImGui.BeginListBox("##TitleMatched_New");
                for (int i = 0; i < newEntryTitleInputCurrentMatches.Count(); i++)
                {
                    var (Title, IsPrefix) = newEntryTitleInputCurrentMatches.ElementAt(i);
                    if (ImGui.Selectable(Title, false))
                    {
                        newEntryTitleInput = Title;
                        newEntryPrefixComboSelected = IsPrefix ? 0 : 1;
                        newEntryTitleInputMatchesToDraw = false;
                    }
                }
                ImGui.EndListBox();
                // use ImGui.IsAnyItemActive() instead of ImGui.IsItemActive() works good
                if (ImGui.IsAnyItemActive())
                    newEntryTitleInputMatchesToDraw = true;
                else 
                    newEntryTitleInputMatchesToDraw = false;
                ImGui.PopItemWidth();
            }
        }

        #region Pop-ups
        // Pop-ups

        private static Vector2 PopupDefaultPos => new(ImGui.GetWindowPos().X + ImGui.GetWindowSize().X / 2, ImGui.GetWindowPos().Y + ImGui.GetWindowSize().Y / 2);
        private static Vector2 PopupButtonDefaultSize => new(ImGui.GetColumnWidth() - UiBaseBoxSize.X, ImGui.GetTextLineHeightWithSpacing());

        private void DrawConfgUiPopups()
        {
            DrawConfigUiPopupWarningNonexistingTitleToAdd();
            DrawConfigUiPopupWarnFailedAdd();
            DrawConfigUiPopupRemoveEntry();
        }

        // Have to use bool vars for open/close popup because of the imgui id stack thing
        private bool configUiPopupWarnNonexistingTitleToAddToOpen = false; 

        private void DrawConfigUiPopupWarningNonexistingTitleToAdd()
        {
            string popupName = "Title Renamed##WarningNonexistingTitleToAdd";
            if (configUiPopupWarnNonexistingTitleToAddToOpen)
                ImGui.OpenPopup(popupName);
            ImGui.SetNextWindowPos(PopupDefaultPos, ImGuiCond.Always);
            if (ImGui.BeginPopupModal(popupName, ref configUiPopupWarnNonexistingTitleToAddToOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text($"Cannot find \"{newEntryTitleInput}\" as an existing title.");
                ImGui.Text("Do you still want to add this entry?");
                ImGui.NewLine();
                ImGui.Columns(2);
                if (ImGui.Button("Yes", PopupButtonDefaultSize))
                {
                    configUiPopupWarnNonexistingTitleToAddToOpen = false;
                    ImGui.CloseCurrentPopup();
                    AddNewTitleRenameEntry();
                }
                ImGui.NextColumn();
                if (ImGui.Button("No", PopupButtonDefaultSize))
                {
                    configUiPopupWarnNonexistingTitleToAddToOpen = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemDefaultFocus();
                ImGui.Columns(1);
                ImGui.EndPopup();
            }
        }

        private bool configUiPopupWarnFailedAddToOpen = false;
        private void DrawConfigUiPopupWarnFailedAdd()
        {
            string popupName = "Title Renamed: Failed to Add Entry";
            if (configUiPopupWarnFailedAddToOpen)
                ImGui.OpenPopup(popupName);
            ImGui.SetNextWindowPos(PopupDefaultPos, ImGuiCond.Always);
            if (ImGui.BeginPopupModal(popupName, ref configUiPopupWarnFailedAddToOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text($"Unable to add rename entry for title \"{newEntryTitleInput}\":");
                ImGui.Text("An entry for this title already exists");
                ImGui.NewLine();
                ImGui.Dummy(UiBaseBoxSize);
                ImGui.SameLine(ImGui.GetColumnWidth() / 8);
                if (ImGui.Button("Yes", PopupButtonDefaultSize))
                {
                    configUiPopupWarnFailedAddToOpen = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }

        private bool configUiPopupRemoveEntryToOpen = false;
        private void DrawConfigUiPopupRemoveEntry()
        {
            string popupName = "Title Renamed: Remove Entry?";
            if (configUiPopupRemoveEntryToOpen)
                ImGui.OpenPopup(popupName);
            ImGui.SetNextWindowPos(PopupDefaultPos, ImGuiCond.Always);
            if (ImGui.BeginPopupModal(popupName, ref configUiPopupRemoveEntryToOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text($"Remove the rename entry for title \"{titleToRemove}\"?");
                ImGui.NewLine();
                ImGui.Columns(2);
                if (ImGui.Button("Yes", PopupButtonDefaultSize))
                {
                    RemoveTitleRenameEntry();
                    configUiPopupRemoveEntryToOpen = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.NextColumn();
                if (ImGui.Button("No", PopupButtonDefaultSize))
                {
                    titleToRemove = null;
                    configUiPopupRemoveEntryToOpen = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SetItemDefaultFocus();
                ImGui.Columns(1);
                ImGui.EndPopup();
            }
        }

        #endregion


        #region Entry actions

        private void CheckIfAddNewTitleRenameEntry()
        {
            if (Titles.Exists(newEntryTitleInput))
                AddNewTitleRenameEntry();
            else
                configUiPopupWarnNonexistingTitleToAddToOpen = true;
        }

        private void AddNewTitleRenameEntry()
        {
            if (string.IsNullOrEmpty(newEntryTitleInput))
                return;
            if (renameMap.TryAdd(newEntryTitleInput, newEntryRenamedInput, newEntryPrefixComboSelected == 0, newEntryDisplayed))
            {
                newEntryTitleInput = string.Empty;
                newEntryRenamedInput = string.Empty;
            }
            else
                configUiPopupWarnFailedAddToOpen = true;
        }

        private void CheckIfRemoveTitleRenameEntry()
        {
            if (titleToRemove != null)
                configUiPopupRemoveEntryToOpen = true;
        }

        private void RemoveTitleRenameEntry()
        {
            if (titleToRemove == null) return;
            renameMap.Remove(titleToRemove);
            titleToRemove = null;
        }

        #endregion
    }
}
