# Total Commander internal commands against SynapseCommander scope

Source: https://www.ghisler.ch/wiki/index.php?title=List_of_internal_commands (raw wikitext, Total Commander 11.57).

**Row counts.** The page's command table has 441 rows. This file's table has 441 rows. They match: every page row is placed. The page lists no em_ commands or other command types. Its other tables only explain parameters. A row that the page shows as a range (for example cm_GotoDrive[A,C-F,Z], cm_UserMenu[1-10], cm_SrcActivateTab[1-10]) stays one row here, as on the page.

**Reading the table.**
- Default key: a key shown without a mark comes from the wiki description. "(mem)" means not verified against a source. "-" means the page gives none and no other source gives one.
- PRODUCT.md status: a requirement id (B.. or A..), "missing" (not mentioned), "excluded (...)" or "near" (an id covers part of the need).
- Decision values: Implement (with its stage), Covered (the same need is met another way, named in Reason), Later (useful, not for the first release), Not (useless here: obsolete, DOS-era, duplicate, replaced by Windows, or out of scope). Every decision was confirmed by the owner on 2026-11-01, except cm_LeftOpenDrives and cm_RightOpenDrives, which the lead changed from Not to Covered the same day to agree with the owner's key map.
- Rows in group "Panel side copies" are the cm_Left*/cm_Right*/cm_Trg* copies of active-panel commands. They are Not as duplicates; the feature itself is decided on its cm_Src* or unprefixed row. The exceptions are cm_LeftOpenDrives and cm_RightOpenDrives, whose keys Alt+F1 and Alt+F2 the key map keeps.
- Rulings applied: TC is the interaction reference, not a 1:1 copy; WinUI 3 and Fluent 2 decide controls; Search, Multi-Rename, File operations and Settings are pages; only viewers and editors are other windows; F4 uses an external editor; no Synchronize preview page; FTP is not planned; archives behave like folders through format modules; search reads names from the index only; delete is permanent only, with no Recycle Bin for now.

| Group | Command | TC description (short) | Default key | PRODUCT.md status | Proposed decision | Reason |
| --- | --- | --- | --- | --- | --- | --- |
| Panel view | cm_SrcThumbs | Active panel: Thumbnail view | Ctrl+Shift+F1 | missing | Later | Needs shell thumbnails off the UI thread; after the core panels |
| Panel view | cm_SrcCustomViewMenu | Active panel: Show custom columns menu | - | missing | Later | Named column sets; B05 only covers column order and visibility |
| Panel view | cm_SrcCustomView[1-9] | Active panel: Activate custom columns view 1-29 | - | missing | Later | Named column sets |
| Panel view | cm_SrcComments | Active panel: Enable/disable showing comments instead of file details | Ctrl+Shift+F2 | missing | Not | File comments (descript.ion) are DOS-era; Windows shows no comments |
| Panel view | cm_SrcShort | Active panel: Show only filenames | Ctrl+F1 | missing | Later | Brief multi-column view is a TC signature; needs a second layout beside the B05 table |
| Panel view | cm_SrcLong | Active panel: Show all file details | Ctrl+F2 | B05 | Implement (Stage 2) | The normal detail table with columns |
| Panel view | cm_SrcTree | Active panel: Show folder tree | Ctrl+F8 | missing | Later | Folder tree inside a panel; panels, history and tabs come first |
| Panel view | cm_SrcViewModeList | Active panel: Show custom views menu or switch to specified custom view | - | missing | Later | View-mode presets; follows named column sets |
| Panel view | cm_RereadSource | Active panel: Refresh contents | Ctrl+R (mem) | B03 | Implement (Stage 2) | Refresh |
| Panel view | cm_SwitchLongNames | Turn long names on and off | - | missing | Not | DOS 8.3 names |
| Panel view | cm_Switch83Names | Turn 8.3 names lowercase on/off | - | missing | Not | DOS 8.3 names |
| Panel view | cm_EditComment | Edit file comment | Ctrl+Z (mem) | missing | Not | File comments are DOS-era |
| Panel view | cm_ShowHint | Show full filename of file under cursor in a tooltip (and place mouse cursor on it) | - | B05 | Covered | Full-value tooltips |
| Panel view | cm_ReloadSelThumbs | Reload selected thumbnails | - | missing | Later | Follows thumbnail view |
| Panel view | cm_CustomColumnDlg | Change current custom columns | - | B05 | Covered | Columns dialog for order and visibility (B05) |
| Panel view | cm_SwitchWatchDirs | Enable/disable auto-refresh file list (setting is not saved) | - | B03 | Covered | Live watching is always on (B03) |
| Panel view | cm_ToggleAutoViewModeSwitch | Turn automatic view mode switching on/off | - | missing | Later | Follows view modes |
| Panel view | cm_ZoomIn | Enlarge thumbnails or set the size to specified percentage (10-200) | - | missing | Later | Follows thumbnail view |
| Panel view | cm_ZoomOut | Make thumbnails smaller or set the size to specified percentage (10-200) | - | missing | Later | Follows thumbnail view |
| Panel view | cm_ReloadFileIcons | Reload all file list icons and overlays | - | B03 | Covered | Refresh reloads icons |
| Panel view | cm_SwitchFileTipWindows | Enable/disable file tooltips | - | B05 | Covered | Tooltips (B05) |
| Panel view | cm_SrcNextCustomView | Active panel: Next custom columns view | - | missing | Later | Cycle named column sets |
| Panel view | cm_SrcPrevCustomView | Active panel: Previous custom columns view | - | missing | Later | Cycle named column sets |
| Panel view | cm_SrcViewMode0 | Active panel: Activate default view mode | - | missing | Later | Default view mode |
| Panel view | cm_SrcViewMode[1-9] | Active panel: Activate custom view 1-249 | - | missing | Later | View-mode presets |
| Panel filter | cm_SrcExecs | Active panel: Show only programs | Ctrl+F11 | missing | Later | Programs-only filter; belongs to the persistent panel filter |
| Panel filter | cm_SrcAllFiles | Active panel: Show all files | Ctrl+F10 | missing | Later | Clears the persistent panel filter |
| Panel filter | cm_SrcUserSpec | Active panel: Enable last chosen custom filter, or temporarily save current filter (parameter 8), or load temporarily... | - | missing | Later | Re-apply or store a persistent panel filter |
| Panel filter | cm_SrcUserDef | Active panel: Set custom filter (show only certain files/folders) | Ctrl+F12 | missing (A05 near) | Later | Persistent per-panel mask filter; A05 quick filter covers the temporary case |
| Panel filter | cm_SwitchHidSys | Turn hidden/system files on and off | - | A19 | Implement (Stage 2) | Show or hide hidden and system files (A19) |
| Panel filter | cm_ShowOnlySelected | Hide files which aren't selected | - | missing | Later | Hide unmarked files; fits A05 |
| Panel filter | cm_SwitchIgnoreList | Enable/disable ignore list file to not show filenames | - | missing | Later | Follows the ignore list |
| Panel filter | cm_SwitchHid | Turn hidden files on and off | - | missing | Not | Covered by the combined hidden and system switch |
| Panel filter | cm_SwitchSys | Turn system files on and off | - | missing | Not | Covered by the combined hidden and system switch |
| Sort | cm_SrcByName | Active panel: Sort by name | Ctrl+F3 (mem) | B05 | Implement (Stage 2) | Sort by column |
| Sort | cm_SrcByExt | Active panel: Sort by extension | Ctrl+F4 (mem) | B05 | Implement (Stage 2) | Sort by column |
| Sort | cm_SrcBySize | Active panel: Sort by size | Ctrl+F6 (mem) | B05 | Implement (Stage 2) | Sort by column |
| Sort | cm_SrcByDateTime | Active panel: Sort by date-time | Ctrl+F5 (mem) | B05 | Implement (Stage 2) | Sort by column |
| Sort | cm_SrcUnsorted | Active panel: Change sorting to unsorted | Ctrl+F7 (mem) | missing | Not | Disk order has no meaning with async, virtualized listings |
| Sort | cm_SrcNegOrder | Active panel: Reverse sort order | - | B05 | Implement (Stage 2) | Reverse sort direction |
| Sort | cm_SwitchDirSort | Turn on/off sorting of folders by name | - | B05 | Later | Folders-first toggle; default is folders first |
| Sort | cm_SrcSortByCol[1-10,99] | Active panel: Sort by column 1-99 | - | B05 | Covered | Header click or sort menu |
| Panel layout | cm_VerticalPanels | File windows above each other | - | missing | Later | Stack panels vertically; A16 only covers the splitter |
| Panel layout | cm_Exchange | Swap left and right file list | Ctrl+U | A10 | Implement (Stage 2) | Swap panels |
| Panel layout | cm_MatchSrc | Inactive panel: Go to the same folder as active panel | - | A10 | Implement (Stage 2) | Show the active folder on the other side |
| Panel layout | cm_ExchangeWithTabs | Swap left and right panel (all folder tabs) | Ctrl+Shift+U | A10 | Implement (Stage 2) | Swap panels including tabs (the tab part lands at Stage 6) |
| Panel layout | cm_50Percent | Active panel: Resize panel to 50% of the available width (or increase/decrease panel width with specified percentage) | - | A16 | Covered | Splitter with remembered position |
| Panel layout | cm_100Percent | Active panel: Resize panel to 100% (or specified percentage) of the available width | - | A16 | Covered | Splitter with remembered position |
| Panel layout | cm_VisCurDir | Show/hide current folder | - | B02 | Covered | Path box |
| Panel layout | cm_VisTabHeader | Show/hide tab header (sorting) | - | B05 | Covered | Sortable column headers |
| Panel layout | cm_VisStatusbar | Show/hide status bar | - | B04 | Implement (Stage 2) | Counts and sizes shown for marks and folder |
| Panel layout | cm_VisCmdLine | Show/hide Command line | - | A01 | Covered | The command line is always part of the Commander page |
| Panel layout | cm_VisKeyButtons | Show/hide function key buttons | - | A22 | Implement (Stage 2) | The key bar (A22) |
| Panel layout | cm_VisBreadCrumbs | Show/hide Breadcrumb bar | - | B02 near | Later | WinUI BreadcrumbBar beside the editable path |
| Navigation | cm_SrcPathFocus | Focus the path of the active panel | - | B02 | Implement (Stage 2) | Focus the editable path |
| Navigation | cm_CdTree | Open the directory tree dialog to navigate to a folder | Alt+F10 | missing | Later | Folder tree dialog; follows the tree panel |
| Navigation | cm_GoToRoot | Go to root folder | Ctrl+\ | B02 | Implement (Stage 2) | Root of drive |
| Navigation | cm_GoToParent | Go to parent folder | Ctrl+PgUp | B02 | Implement (Stage 2) | Parent folder |
| Navigation | cm_GoToDir | Open folder or (executable) archive under cursor | Ctrl+PgDn | A09 | Implement (Stage 6) | Enter opens a folder, or an archive as a folder (owner ruling: archives behave like folders) |
| Navigation | cm_TransferLeft | Show folder under cursor in left panel | Ctrl+Left | missing | Later | Show folder under cursor in the left panel |
| Navigation | cm_TransferRight | Show folder under cursor in right panel | Ctrl+Right | missing | Later | Show folder under cursor in the right panel |
| Navigation | cm_SyncChangeDir | Synchronous folder changing (try to go to the same subfolder in the other panel too) | - | missing | Later | Both panels follow folder changes; small, after A04/A10 |
| Navigation | cm_EditPath | Edit path field above file list | - | B02 | Implement (Stage 2) | Edit the path |
| List navigation | cm_GoToFirstEntry | Place cursor on first folder or file and move down the specified number of times | - | B22 | Covered | Home and arrow keys in the table |
| List navigation | cm_GoToFirstFile | Place cursor on first file and move down the specified number of times | - | B22 | Covered | Keyboard navigation in the table |
| List navigation | cm_GotoNextSelected | Go to next selected file | - | missing | Later | Jump between marks |
| List navigation | cm_GotoPrevSelected | Go to previous selected file | - | missing | Later | Jump between marks |
| List navigation | cm_GotoNext | Go to next file | - | B22 | Covered | Arrow keys |
| List navigation | cm_GotoPrev | Go to previous file | - | B22 | Covered | Arrow keys |
| List navigation | cm_GoToLast | Go to last file | - | B22 | Covered | End key |
| History | cm_GotoPreviousDir | Go to previous folder in folder history | - | A04 | Implement (Stage 2) | Back |
| History | cm_GotoNextDir | Go to next folder in folder history | - | A04 | Implement (Stage 2) | Forward |
| History | cm_DirectoryHistory | Show folder history (filter folders where you didn't stay long) | Alt+Down | A04 | Implement (Stage 2) | History list |
| History | cm_GotoPreviousLocalDir | Go to previous folder in folder history, no ftp | - | A04 | Not | Variant that skips FTP entries |
| History | cm_GotoNextLocalDir | Go to next folder in folder history, no ftp | - | A04 | Not | Variant that skips FTP entries |
| History | cm_DirectoryHistoryNoThinning | Show folder history (unfiltered) | Alt+Shift+Down | A04 | Not | Variant of the history list without filtering |
| Favorites | cm_DirectoryHotlist | Open Directory Hotlist | Ctrl+D | A04 | Implement (Stage 2) | Saved favorites (hotlist submenus are not in A04) |
| Favorites | cm_VisHistHotButtons | Show/hide Directory Hotlist and Folder History buttons | - | A04 | Covered | A04 requires visible history and favorites commands |
| Drives | cm_SrcOpenDrives | Active panel: Open drive list or go to drive | - | B02 | Implement (Stage 2) | Drive menu |
| Drives | cm_VolumeId | Set volume label | - | missing | Not | DOS-era; Windows drive Properties |
| Drives | cm_GotoNextDrive | Go one drive up (C->D) | - | B02 | Not | Drive menu and path box |
| Drives | cm_GotoPreviousDrive | Go one drive down (D->C) | - | B02 | Not | Drive menu and path box |
| Drives | cm_GotoDrive[A,C-F,Z] | Go to the specified drive | - | B02 | Covered | Drive menu and path box; one command per drive letter is not needed |
| Drives | cm_VisDriveButtons | Show/hide drive button bars | - | B02 near | Later | Drive buttons row; B02 specifies drive menus |
| Drives | cm_VisTwoDriveButtons | Show/hide two drive bars | - | missing | Not | Second drive row duplicates the first |
| Drives | cm_VisDriveCombo | Show/hide drive combobox | - | B02 | Covered | Drive menu |
| Drives | cm_OpenDriveByIndex | Go to drive by index number in drive button bar or drive combo-box | - | missing | Not | Index into a drive button bar; scripting only |
| Virtual folders | cm_OpenDesktop | Go to the virtual folder Desktop | - | missing | Later | Desktop as a location |
| Virtual folders | cm_OpenDrives | Go to the virtual folder This PC (My Computer) | - | B02 | Implement (Stage 2) | Drive list (This PC) |
| Virtual folders | cm_OpenControls | Go to the virtual folder Control Panel | - | missing | Not | Windows Settings replaces Control Panel |
| Virtual folders | cm_OpenFonts | Go to the virtual folder Fonts | - | missing | Not | Windows Settings replaces the Fonts folder |
| Virtual folders | cm_OpenNetwork | Go to the virtual folder Network (Network neighborhood) | - | missing | Later | Network neighbourhood; needs shell enumeration off the UI thread |
| Virtual folders | cm_OpenPrinters | Go to the virtual folder Printers | - | missing | Not | Windows Settings replaces Printers |
| Virtual folders | cm_OpenRecycled | Go to the virtual folder Recycle Bin | - | missing | Later | No Recycle Bin support for now (owner ruling) |
| Tabs | cm_VisDirTabs | Show/hide folder tabs | - | A07 | Covered | Tabs appear with A07 |
| Tabs | cm_OpenNewTab | Create new folder tab | Ctrl+T | A07 | Implement (Stage 6) | New tab |
| Tabs | cm_OpenNewTabBg | Create new folder tab (stay in current folder tab) | Ctrl+Shift+T | A07 | Later | New tab without switching |
| Tabs | cm_OpenDirInNewTab | Create new folder tab for folder under cursor | Ctrl+Up | A07 | Implement (Stage 6) | Open the focused folder in a new tab |
| Tabs | cm_OpenDirInNewTabOther | Create new folder tab in inactive panel for folder under cursor | Ctrl+Shift+Up | A07 | Later | Same in the other panel |
| Tabs | cm_SwitchToNextTab | Switch to next Tab | - | A07 | Implement (Stage 6) | Next tab |
| Tabs | cm_SwitchToPreviousTab | Switch to previous Tab | Ctrl+Shift+Tab | A07 | Implement (Stage 6) | Previous tab |
| Tabs | cm_CloseCurrentTab | Close tab | Ctrl+W (mem) | A07 | Implement (Stage 6) | Close tab |
| Tabs | cm_CloseAllTabs | Close all tabs | - | A07 | Later | Close all tabs |
| Tabs | cm_DirTabsShowMenu | Show tab menu | - | A07 | Covered | Tab context menu |
| Tabs | cm_ToggleLockCurrentTab | Lock/unlock current folder tab | - | missing | Later | Owner ruling, 2026-11-01 |
| Tabs | cm_GoToLockedDir | Go to the base folder of the locked folder tab | - | missing | Later | Follows tab lock |
| Tabs | cm_ToggleLockDcaCurrentTab | Lock/unlock current folder tab (when locked, folder change allowed) | - | missing | Not | Variant of tab lock |
| Tabs | cm_MoveTabLeft | Move current tab to the left | - | A07 | Covered | WinUI TabView drag reorder |
| Tabs | cm_MoveTabRight | Move current tab to the right | - | A07 | Covered | WinUI TabView drag reorder |
| Tabs | cm_CloseDuplicateTabs | Close duplicate tabs | - | missing | Later | Tidy duplicate tabs |
| Tabs | cm_RenameTab | Rename current tab | - | missing | Later | Custom tab titles |
| Tabs | cm_OpenNewTabOther | Create new folder tab in inactive panel (duplicate of current tab in active panel) | - | A07 | Not | Duplicate: open the tab, then swap or drag |
| Tabs | cm_OpenNewTabBgOther | Create new folder tab in inactive panel (duplicate of current tab in active panel) (stay in current folder tab) | - | A07 | Not | Duplicate of cm_OpenNewTabOther |
| Tabs | cm_SetTabOptionNormal | Unlock current folder tab | - | missing | Not | Variant of tab lock |
| Tabs | cm_SetTabOptionPathLocked | Lock current folder tab | - | missing | Not | Variant of tab lock |
| Tabs | cm_SetTabOptionPathResets | Lock current folder tab (folder change allowed) | - | missing | Not | Variant of tab lock |
| Tabs | cm_SetAllTabsOptionNormal | Unlock all folder tabs | - | missing | Not | Bulk variant of tab lock |
| Tabs | cm_SetAllTabsOptionPathLocked | Lock all folder tabs | - | missing | Not | Bulk variant of tab lock |
| Tabs | cm_SetAllTabsOptionPathResets | Lock all folder tabs (folder change allowed) | - | missing | Not | Bulk variant of tab lock |
| Tabs | cm_SrcActivateTab[1-10] | Active panel: Activate folder tab 1-99 (or tab number specified by parameter, negative means counting from the end) | - | A07 | Later | Jump to tab n; keyboard access for A07 |
| Tabs | cm_SrcTabsList | Show a list of all folder tabs | Ctrl+Shift+A | missing | Later | Tab list popup; WinUI TabView overflow serves meanwhile |
| Selection | cm_SpreadSelection | Select group (files only or also folders, depending on configuration) | Num + | B04 | Implement (Stage 2) | Select by mask |
| Selection | cm_ShrinkSelection | Unselect group (files only or also folders, depending on configuration) | Num - | B04 | Implement (Stage 2) | Unselect by mask |
| Selection | cm_SelectAll | Select all (files only or also folders, depending on configuration) | Ctrl Num + | B04 | Implement (Stage 2) | Select all |
| Selection | cm_ClearAll | Unselect all (files and folders) | - | B04 | Implement (Stage 2) | Clear all marks |
| Selection | cm_ExchangeSelection | Invert selection (files only or also folders, depending on configuration) | Num * | A11 | Implement (Stage 2) | Invert selection |
| Selection | cm_SelectCurrentExtension | Select all files with the same filename extension | Alt Num + | A11 | Implement (Stage 2) | Select by focused extension |
| Selection | cm_UnselectCurrentExtension | Unselect all files with the same filename extension | - | A11 | Implement (Stage 2) | Unselect by focused extension |
| Selection | cm_RestoreSelection | Restore file selection | Num / | missing | Later | Restore the last saved marks |
| Selection | cm_SaveSelection | Temporarily save selection | - | missing | Later | Save marks temporarily |
| Selection | cm_SelectCurrentPath | Select all in the same path (for branch view and search results) | - | missing | Later | Select in the same path for branch view and Search results (A15) |
| Selection | cm_UnselectCurrentPath | Unselect all in the same path (for branch view and search results) | - | missing | Later | Counterpart of cm_SelectCurrentPath |
| Selection | cm_SelectCurrentName | Select all files with the same name | - | missing | Later | Select same names; useful in Search results |
| Selection | cm_UnselectCurrentName | Unselect all files with the same name | - | missing | Later | Counterpart of cm_SelectCurrentName |
| Selection | cm_SelectCurrentNameExt | Select all files with the same name and extension | - | missing | Later | Select same name and extension |
| Selection | cm_UnselectCurrentNameExt | Unselect all files with the same name and extension | - | missing | Later | Counterpart of cm_SelectCurrentNameExt |
| Selection | cm_SpreadSelectionCurrentExt | Select group, suggest extension of file under cursor (files only or also folders, depending on configuration) | - | A11 | Covered | Select by focused extension (A11) |
| Selection | cm_ShrinkSelectionCurrentExt | Unselect group, suggest extension of file under cursor (files only or also folders, depending on configuration) | - | A11 | Covered | Unselect by focused extension (A11) |
| Selection | cm_SaveSelectionToFile | Save file selection to file | - | missing | Later | Marks to a file |
| Selection | cm_LoadSelectionFromFile | Read file selection from file | - | missing | Later | Marks from a file |
| Selection | cm_LoadSelectionFromClip | Read file selection from clipboard | - | missing | Later | Marks from the clipboard |
| Selection | cm_SaveSelectionToFileA | Save file selection to file (ANSI) | - | missing | Not | ANSI variant; encoding is a parameter of cm_SaveSelectionToFile |
| Selection | cm_SaveSelectionToFileW | Save file selection to file (Unicode) | - | missing | Not | Unicode variant; same reason |
| Selection | cm_Select | Select a number of files/folders and go to the next one | - | B04 | Not | Covered by the cm_Reverse toggle; select-N variant |
| Selection | cm_UnSelect | Remove selection of a number of files/folders and go to the next one | - | B04 | Not | Covered by the cm_Reverse toggle; unselect-N variant |
| Selection | cm_Reverse | Reverse selection of a number of files/folders and go to the next one | Insert (mem) | B04 | Implement (Stage 2) | Toggle mark and move to next (Insert or Space) |
| Selection | cm_SelectAllBoth | Select both files+folders | - | B04 | Not | Variant of cm_SelectAll with fixed scope |
| Selection | cm_SelectAllFiles | Select all files | - | B04 | Not | Variant of cm_SelectAll with fixed scope |
| Selection | cm_SelectAllFolders | Select all folders | - | B04 | Not | Variant of cm_SelectAll with fixed scope |
| Selection | cm_ClearAllFiles | Unselect all files | - | B04 | Not | Variant of cm_ClearAll with fixed scope |
| Selection | cm_ClearAllFolders | Unselect all folders | - | B04 | Not | Variant of cm_ClearAll with fixed scope |
| Selection | cm_ClearAllCfg | Unselect all (files only or also folders, depending on configuration) | - | B04 | Not | Variant of cm_ClearAll with configurable scope |
| Selection | cm_SelectBoth | Select group (files and folders) | - | B04 | Not | Variant of mask select with fixed scope |
| Selection | cm_SelectFiles | Select group (files only) | - | B04 | Implement (Stage 2) | Select files by mask |
| Selection | cm_SelectFolders | Select group (folders only) | - | missing | Later | Folders-only mask select |
| Selection | cm_ClearFiles | Unselect group (files only) | - | B04 | Implement (Stage 2) | Unselect files by mask |
| Selection | cm_ClearFolders | Unselect group (folders only) | - | missing | Later | Folders-only unselect |
| Selection | cm_ClearSelCfg | Unselect group (files only or also folders, depending on configuration) | - | B04 | Not | Variant of mask unselect with configurable scope |
| Selection | cm_ExchangeSelBoth | Invert selection (files and folders) | - | A11 | Not | Variant of invert with fixed scope |
| Selection | cm_ExchangeSelFiles | Invert selection (files only) | - | A11 | Not | Variant of invert with fixed scope |
| Selection | cm_ExchangeSelFolders | Invert selection (folders only) | - | A11 | Not | Variant of invert with fixed scope |
| Clipboard | cm_CutToClipboard | Cut selected files/folders to clipboard | Ctrl+X | A02/B11 | Implement (Stage 2) | Cut |
| Clipboard | cm_CopyToClipboard | Copy selected files/folders to clipboard | Ctrl+C, Ctrl+Insert | A02/B11 | Implement (Stage 2) | Copy |
| Clipboard | cm_PasteFromClipboard | Paste file(s)/folder(s) in active file panel | Ctrl+V, Shift+Insert | A02/B11 | Implement (Stage 2) | Paste |
| Clipboard | cm_CopyNamesToClip | Copy filenames to clipboard | - | A02 | Implement (Stage 2) | Copy names |
| Clipboard | cm_CopyFullNamesToClip | Copy filenames with full path to clipboard | - | A02 | Implement (Stage 2) | Copy full paths |
| Clipboard | cm_CopyNetNamesToClip | Copy filenames with full UNC path to clipboard | - | missing | Later | Copy UNC paths |
| Clipboard | cm_CopySrcPathToClip | Active panel: Copy path to clipboard | - | B02 | Covered | Copy from the editable path box |
| Clipboard | cm_CopyTrgPathToClip | Inactive panel: Copy path to clipboard | - | missing | Not | Rarely used; same via the other panel path box |
| Clipboard | cm_CopyFileDetailsToClip | Copy file details to clipboard | - | missing | Later | Copy shown columns as text |
| Clipboard | cm_CopyFpFileDetailsToClip | Copy file details to clipboard with full paths | - | missing | Not | Variant (full paths) of the details copy |
| Clipboard | cm_CopyNetFileDetailsToClip | Copy file details to clipboard with UNC paths | - | missing | Not | Variant (UNC paths) of the details copy |
| Clipboard | cm_CopyHdrFileDetailsToClip | Copy file details to clipboard with headers | - | missing | Not | Variant (headers) of the details copy |
| Clipboard | cm_CopyHdrFpFileDetailsToClip | Copy file details to clipboard with headers and full paths | - | missing | Not | Variant (headers, full paths) of the details copy |
| Clipboard | cm_CopyHdrNetFileDetailsToClip | Copy file details to clipboard with headers and UNC paths | - | missing | Not | Variant (headers, UNC paths) of the details copy |
| Clipboard | cm_CopyPathOfFilesToClip | Copy paths of selected files/folders to clipboard | - | A02 | Implement (Stage 2) | Copy containing folder (CopyFolder in the checkpoint) |
| File operations | cm_SetAttrib | Change attributes | - | missing | Later | Change attributes and timestamps; needs engine support for attribute writes |
| File operations | cm_Split | Split a file into multiple parts | - | missing | Later | Split a file into parts; useful but not daily |
| File operations | cm_Combine | Combine file parts into a single file | - | missing | Later | Joins parts from cm_Split |
| File operations | cm_Encode | Encode file using MIME/UUE/XXE | - | missing | Not | MIME/UUE/XXE encoding is e-mail-era |
| File operations | cm_Decode | Decode MIME/UUE/XXE/BinHex/yEnc encoded file | - | missing | Not | MIME/UUE/XXE/yEnc decoding is e-mail-era |
| File operations | cm_Copy | Copy files/folders | F5 (mem) | B12 | Implement (Stage 3) | Copy through the engine |
| File operations | cm_RenMov | Move or rename files/folders | F6 (mem) | B12 | Implement (Stage 3) | Move, with rename via Move |
| File operations | cm_MkDir | Create new folder(s) | F7 (mem) | B12 | Implement (Stage 3) | New folder |
| File operations | cm_Delete | Delete files/folders | F8, Del (mem) | B13 | Implement (Stage 3) | Permanent fast delete only; no Recycle Bin for now (owner ruling) |
| File operations | cm_MkDirOther | Inactive panel: Create new folder(s) | - | B12 | Not | Duplicate of cm_MkDir with another destination; the destination is editable |
| File operations | cm_CreateShortcut | Create shortcut files (.lnk) | - | missing | Later | Make .lnk files; Explorer can do it meanwhile |
| File operations | cm_MoveOnly | Move files/folders | F6 | B12 | Not | Duplicate of cm_RenMov without the rename prompt |
| File operations | cm_CopySamepanel | Copy files/folders to the same folder | Shift+F5 (mem) | B12 | Implement (Stage 3) | Copy to the same folder under a new name (explicit destination, B12) |
| File operations | cm_CopyOtherpanel | Copy files/folders to inactive panel | - | B12 | Not | Duplicate: the other panel is already the default destination of cm_Copy |
| Rename | cm_RenameOnly | Rename files/folders | Shift+F6 | A03 | Implement (Stage 2) | Rename with F2 |
| Rename | cm_RenameSingleFile | Rename file under cursor | - | A03 | Not | Duplicate of cm_RenameOnly on the focused file |
| Rename | cm_MultiRenameFiles | Open the Multi-Rename Tool | Ctrl+M (mem) | A12 | Implement (Stage 7) | Multi-Rename page |
| Archives | cm_PackFiles | Pack files/folders | Alt+F5 (mem) | A09 | Implement (Stage 6) | Create ZIP, 7z, TAR or TGZ |
| Archives | cm_UnpackFiles | Unpack archives | Alt+F9 (mem) | A09 | Implement (Stage 6) | Extract ZIP, TAR, TGZ, 7z or RAR to a chosen folder |
| Archives | cm_TestArchive | Test integrity of selected archives | - | missing | Later | Test integrity; add with extra archive formats |
| Archives | cm_ChangeArchiveEncoding | Show popup menu to change the encoding of filenames in an archive | - | missing | Later | File-name encoding in archives; ZIP extraction may need it |
| Archives | cm_CopyToArchive | Pack files/folders | - | A09 | Not | Duplicate of cm_PackFiles with a preset option |
| Archives | cm_MoveToArchive | Pack files/folders and delete the originals | - | missing | Later | Pack then delete originals; a destructive combination, add after A09 is proven |
| Checksums | cm_CRCcreate | Create checksum (hash/digest) file(s) for selected files/folders | - | A14 | Implement (Stage 7) | Create checksum files |
| Checksums | cm_CRCcheck | Verify selected checksum (hash/digest) file(s) | - | A14 | Implement (Stage 7) | Verify checksum files |
| Compare | cm_CompareDirs | Compare file panels: mark newer files and files unique to one side | Shift+F2 | A21 | Implement (Stage 3) | Mark differences between the panels (A21, owner ruling 2026-11-01) |
| Compare | cm_DirMatch | Compare file panels: mark newer files (and unique files), hide files with the same time stamp and file size | - | missing | Later | Hide equal files after compare; follows the cm_CompareDirs decision |
| Compare | cm_CompareDirsWithSubdirs | Compare file panels: mark newer files and files/folders unique to one side | - | B14 near | Later | Recursive form of cm_CompareDirs; follows that decision |
| Compare | cm_CompareFilesByContent | File comparison | - | A13 | Implement (Stage 7) | External compare tool |
| Compare | cm_IntCompareFilesByContent | Use internal compare tool | - | excluded (A13) | Not | Embedded diff is excluded by A13 |
| Synchronize | cm_FileSync | Synchronize folders | - | B14 | Implement (Stage 3) | Compare, Sync and Mirror; no Synchronize preview page per owner |
| Operations | cm_OpenTransferManager | Open the Background Transfer Manager | - | B15 | Implement (Stage 3) | File operations page |
| Folder size | cm_GetFileSpace | Calculate occupied space | Ctrl+L | B05/B09 partial | Covered | Inspector totals (B09) and Properties (B10); see cm_CountDirContent |
| Folder size | cm_CountDirContent | Active panel: Show sizes of folders instead <DIR> (Abort with Esc) | Alt+Shift+Enter | A20 | Implement (Stage 2) | Compute folder sizes on demand (A20) |
| Branch view | cm_DirBranch | Show all files in current folder and all subfolders | Ctrl+B (mem) | A15 | Implement (Stage 7) | Indexed branch view only; the TC command works without an index |
| Branch view | cm_DirBranchSel | Show selected files and all files in selected folders | - | A15 | Later | Branch of marked folders |
| Search | cm_SearchFor | Open the Search tool | Alt+F7 | B06-B08 | Implement (Stage 4) | Search page by name over the index. Search never looks inside files or archives; search is the gozler index only for now (owner ruling) |
| Search | cm_SearchForInCurdir | Search in folder under the cursor or in specified drive | - | B06 | Implement (Stage 4) | Search in this folder |
| Search | cm_SearchStandalone | Search in separate process | Alt+Shift+F7 | missing | Not | Separate process and window; Search is a page of the one main window |
| Search | cm_ShowQuickSearch | Start quick search for files in the active panel | - | Vocabulary, A01 | Implement (Stage 2) | Quick search by typing |
| Search | cm_QuickSearch | Start quick search for files in the active panel (filter off) | - | Vocabulary, A01 | Implement (Stage 2) | Quick search |
| Search | cm_QuickFilter | Start quick search for files in the active panel (filter on) | Ctrl+S (mem) | A05 | Implement (Stage 2) | Quick filter |
| Viewing | cm_List | View with Lister | F3 (mem) | B17 | Implement (Stage 5) | Text/Hex viewer window |
| Viewing | cm_ListInternalOnly | View single file with Lister without plugins/multimedia | - | B17 | Not | Variant that skips plugins and multimedia; no plugin platform |
| Viewing | cm_ListInternalMulti | View selected files with Lister without plugins/multimedia | - | missing | Not | Same variant for several files |
| Viewing | cm_ListOnly | View single file with Lister with plugins/multimedia | - | missing | Not | Plugin/multimedia variant of cm_List |
| Viewing | cm_ListMulti | View selected files with Lister with plugins/multimedia | - | missing | Not | Plugin/multimedia variant for several files |
| Viewing | cm_ListExternal | Opens file under cursor or specified file with external viewer defined for this file type, or default external viewer... | - | B10 | Covered | Configured external open command (B10) opens the file in another viewer |
| Editing | cm_Edit | Edit files and create new files if they don't exist | F4 (mem) | A08 | Implement (Stage 6) | F4 starts the external editor |
| Editing | cm_EditNewFile | Create and edit new text file | Shift+F4 (mem) | A08 | Implement (Stage 6) | New text file without overwrite |
| Editing | cm_EditExistingFile | Edit file under cursor | - | A08 | Not | Same as cm_Edit on the focused file |
| Editing | cm_EditFileMenu | Choose from various file types to create a new file | - | missing | Later | New-file template menu; A08 only needs a plain text file |
| Preview | cm_SrcQuickview | Active panel: Enable/disable Quick view | Ctrl+Q (mem) | B09, A18 | Implement (Stage 4) | Quick View replaces the other panel and shows the inspector and A18 preview |
| Preview | cm_SrcQuickInternalOnly | Active panel: Enable/disable Quick view without plugins/multimedia | - | missing | Not | Variant that skips plugins; no plugin platform |
| Preview | cm_SrcHideQuickview | Active panel: Disable Quick view | - | B09 | Implement (Stage 4) | Closes Quick View; the other panel returns |
| Preview | cm_SeparateQuickView | Enable/disable Separate Quick view window | - | excluded (one-window ruling) | Not | Only viewers and editors are other windows |
| Preview | cm_SeparateQuickInternalOnly | Enable/disable Separate Quick view window without plugins/multimedia | - | missing | Not | Same, without plugins |
| Preview | cm_UpdateQuickView | Reload file in Quick view panel | - | missing | Covered | Inspector and preview update themselves |
| Shell actions | cm_Associate | Define associations | - | B10 | Covered | Windows Default Apps plus configured external open commands (B10) |
| Shell actions | cm_VersionInfo | Show Properties of selected file(s)/folder(s) | - | B10 | Not | Duplicate of cm_Properties |
| Shell actions | cm_InternalAssociate | Define internal associations | - | missing | Not | TC internal associations |
| Shell actions | cm_Properties | Show Properties (single file/folder) or calculate occupied space (multiple files/folders) | Alt+Enter (mem) | B10 | Implement (Stage 2) | Open Windows Properties |
| Shell actions | cm_ModernShare | Share files with contacts, nearby devices, or apps (Windows 10+) | - | missing | Covered | Windows share sheet through the shell context menu |
| Shell actions | cm_ContextMenu | Show context menu | Shift+F10/Menu key | B10/B19 | Implement (Stage 2) | Context menu from keyboard |
| Shell actions | cm_OpenAsUser | Open program under cursor as different user | - | missing | Later | Run as another user; reachable through the Windows context menu until then |
| Shell actions | cm_ContextMenuInternal | Show context menu for internal associations | - | missing | Not | TC internal associations menu |
| Shell actions | cm_ContextMenuInternalCursor | Show context menu for internal associations for file under cursor | - | missing | Not | TC internal associations menu |
| Shell actions | cm_ShowRemoteMenu | Context menu for Media Center remote control (Play/Pause) | - | missing | Not | Media Center remote; obsolete |
| Shell actions | cm_DriveContextMenu | Show drive context menu | - | missing | Later | Shell menu for a drive |
| Command line | cm_ExecuteDOS | Open command prompt window | - | A01 | Implement (Stage 2) | Open terminal |
| Command line | cm_ClearCmdLine | Clear the command line | - | A01 | Implement (Stage 2) | Esc in the command line clears it and returns to the panel, as in Total Commander (PRODUCT Command line behavior) |
| Command line | cm_NextCommand | Next command line | - | A01 | Implement (Stage 2) | History recall |
| Command line | cm_PrevCommand | Previous command line | - | A01 | Implement (Stage 2) | History recall |
| Command line | cm_AddPathToCmdline | Copy path to command line | - | A01 | Implement (Stage 2) | Insert the panel path |
| Command line | cm_AddFileNameToCmdline | Copy filename to command line | Ctrl+Enter | A01 | Implement (Stage 2) | Insert the focused name |
| Command line | cm_AddPathAndFileNameToCmdline | Copy full filename (including path) to command line | Ctrl+Shift+Enter | A01 | Implement (Stage 2) | Insert the full path |
| Command line | cm_ShowCmdLineHistory | Open/close command line history | ALT+F8 | A01 | Implement (Stage 2) | History drop-down |
| Focus | cm_FocusLeft | Focus the left file list | - | B22 | Covered | Tab and focus restoration (B22) |
| Focus | cm_FocusRight | Focus the right file list | - | B22 | Covered | Tab and focus restoration (B22) |
| Focus | cm_FocusCmdLine | Focus the command line | - | A01 | Implement (Stage 2) | Shortcut into the command bar |
| Focus | cm_FocusButtonBar | Focus the button bar | Alt+Shift+F11 | missing | Later | Only if a button bar is accepted |
| Focus | cm_FocusSrc | Focus the file list of the active panel | - | B22 | Covered | Focus restoration (B22) |
| Focus | cm_FocusTrg | Focus the file list of the inactive panel | - | B22 | Covered | Tab between panels |
| Focus | cm_FocusLeftTree | Focus the left separate tree | - | missing | Later | Only if the tree panel is accepted |
| Focus | cm_FocusRightTree | Focus the right separate tree | - | missing | Later | Only if the tree panel is accepted |
| Focus | cm_FocusSrcTree | Focus the separate tree of the active panel | - | missing | Later | Only if the tree panel is accepted |
| Focus | cm_FocusTrgTree | Focus the separate tree of the inactive panel | - | missing | Later | Only if the tree panel is accepted |
| Focus | cm_FocusMainMenu | Focus the main menu | F10 | B22 | Covered | WinUI MenuBar access keys |
| Focus | cm_FocusButtonBarVertical | Focus the vertical button bar | Alt+Shift+F12 | missing | Not | Vertical bar duplicates the first |
| Button bar | cm_ButtonConfig | Change button bar | - | missing | Later | Owner ruling, 2026-11-01 |
| Button bar | cm_ButtonConfig2 | Change vertical button bar | - | missing | Not | Second (vertical) bar duplicates the first |
| Button bar | cm_ChangeStartMenu | Change Start menu | - | missing | Later | User command menu; depends on the cm_ButtonConfig decision |
| Button bar | cm_UserMenu[1-10] | Execute Start menu item 1-199 | - | missing | Later | User Start-menu entries; follows cm_ChangeStartMenu |
| Button bar | cm_VisButtonbar | Show/hide button bar | - | missing | Later | Follows the cm_ButtonConfig decision |
| Button bar | cm_VisButtonbar2 | Show/hide vertical button bar | - | missing | Not | Vertical bar duplicates the first |
| Button bar | cm_ReloadBarIcons | Force reload icons in button bars and main menu | - | missing | Not | Internal icon cache |
| Appearance | cm_VisFlatDriveButtons | Buttons: Flat/normal mode | - | missing | Not | Flat or normal button look; Fluent 2 decides |
| Appearance | cm_VisFlatInterface | Interface: Flat/normal mode | - | missing | Not | Flat or normal interface look; Fluent 2 decides |
| Appearance | cm_SwitchOverlayIcons | Switch icon overlays on/off | - | B05 | Covered | Shell icons carry overlays |
| Appearance | cm_VisXPthemeBackground | Show/hide XP theme background | - | missing | Not | Windows XP theme |
| Appearance | cm_SwitchDarkmode | Turn dark mode on and off | - | B22 | Covered | Light and dark follow Windows (B22) |
| Appearance | cm_EnableDarkmode | Turn dark mode on | - | B22 | Covered | Follows Windows (B22) |
| Appearance | cm_DisableDarkmode | Turn dark mode off | - | B22 | Covered | Follows Windows (B22) |
| Appearance | cm_SwitchColorsByFileType | Enable/disable color filters | - | missing | Not | Fluent 2 theming and high contrast decide colours (owner ruling, 2026-11-01) |
| Settings | cm_7ZipPackerConfig | Open configuration page 7-Zip packer | - | A09 | Implement (Stage 6) | Archive formats (zip, tar, tgz, 7z, rar) are format modules behind one archive interface (owner ruling) |
| Settings | cm_HistoryConfig | Open configuration page History | - | A04 | Covered | Settings page: history size |
| Settings | cm_LayoutConfig | Open configuration page Layout | - | B19 | Covered | Settings page (layout/panel options) |
| Settings | cm_IconConfig | Open configuration page Icons | - | B05 | Not | Icons come from the shell |
| Settings | cm_RefreshConfig | Open configuration page Refresh (auto-refresh file data) | - | B03 | Covered | Settings page: refresh behaviour |
| Settings | cm_QuickSearchConfig | Open configuration page Quick Search | - | B19 | Covered | Settings page |
| Settings | cm_IgnoreConfig | Open configuration page Ignore list | - | missing | Later | Follows the ignore list |
| Settings | cm_LogConfig | Open configuration page Log file | - | B16 | Covered | Retained operation reports (B16) replace the log file |
| Settings | cm_ThumbnailsConfig | Open configuration page Thumbnails | - | missing | Later | Follows thumbnail view |
| Settings | cm_CustomColumnConfig | Open configuration page Custom Columns | - | missing | Later | Follows named column sets |
| Settings | cm_PluginsConfig | Open configuration page Plugins | - | excluded (plugin platform) | Not | No plugin platform |
| Settings | cm_ZipPackerConfig | Open configuration page Zip packer | - | A09 | Later | ZIP options such as compression level |
| Settings | cm_DisplayConfig | Open configuration page Display | - | B19 | Covered | Settings page (display options) |
| Settings | cm_CopyConfig | Open configuration page Copy/Delete | - | B19 | Covered | Settings page: File operations |
| Settings | cm_DirTabsConfig | Open configuration page Folder Tabs | - | B19 | Covered | Settings page (tab options, Stage 6) |
| Settings | cm_FtpConfig | Open configuration page FTP | - | excluded (FTP) | Not | FTP is out of scope |
| Settings | cm_Config | Open configuration options (opens first page: Layout) | - | B19 | Covered | The Settings page |
| Settings | cm_PackerConfig | Open configuration page Packer | - | excluded (plugin platform) | Not | External packer plug-ins |
| Settings | cm_FontConfig | Open configuration page Font | - | B22 | Not | Fluent 2 and the Windows text size own fonts |
| Settings | cm_ConfigSavePos | Save position and size of the Total Commander window and column widths of the file panels | - | B02 | Covered | Window and view state are remembered automatically |
| Settings | cm_ColorConfig | Open configuration page Colors | - | missing | Not | Fluent 2 theming and high contrast decide colours (owner ruling, 2026-11-01) |
| Settings | cm_Confirmation | Open configuration page Misc. (confirmations, hotkeys, aliases, sounds) | - | B19 | Covered | Settings page: confirmations |
| Settings | cm_EditConfig | Open configuration page Edit/View (Editor and Viewer) | - | B19 | Covered | Settings page: editor and viewer commands |
| Settings | cm_ConfTabChange | Open configuration page Tabstops/Data Formats | - | B21 | Covered | Regional formats follow Windows |
| Settings | cm_LanguageConfig | Open configuration page Language | - | B21 | Covered | Language setting |
| Settings | cm_Config2 | Open configuration page Operation | - | B19 | Covered | Settings page: File operations |
| Settings | cm_ConfigSaveSettings | Save settings | - | B19 | Not | Settings commit individually (B19) |
| Settings | cm_ConfigChangeIniFiles | Open the main configuration files in Notepad | - | B19 | Not | No INI files; the Settings page is the editor |
| Settings | cm_ConfigSaveDirHistory | Save folder history and frequently visited folders list | - | A04 | Not | History saves automatically |
| Settings | cm_CommandBrowser | Browse and create internal commands | - | B19 | Later | Command picker for key bindings; the Keyboard page comes first |
| Settings | cm_ConfigViewModes | Open configuration page View Mode | - | missing | Later | Follows view modes |
| Settings | cm_ConfigViewModeSwitch | Open configuration page Auto Switch Mode (Auto-switch view modes) | - | missing | Later | Follows view modes |
| Settings | cm_ConfigLister | Open Lister's configuration options | - | B17 | Covered | Viewer options in Settings |
| Help | cm_HelpIndex | Open the Help Index page | F1 (mem) | B21 partial | Later | Only search syntax help is in scope |
| Help | cm_Keyboard | Open Help page "Keyboard layout" | - | B19 | Covered | Keyboard page in Settings |
| Help | cm_Register | Open Help page "How to order Total Commander" | - | missing | Not | TC licensing |
| Help | cm_VisitHomepage | Visit https://www.ghisler.com/ | - | missing | Not | TC web site |
| Help | cm_CheckForUpdates | Check for Total Commander updates | - | B01 | Implement (Stage 5) | Own-product update check |
| Help | cm_About | Show Total Commander's version information | - | B01 | Implement (Stage 5) | About |
| Window | cm_Minimize | Minimize Total Commander | - | B22 | Covered | Windows title bar |
| Window | cm_Maximize | Maximize Total Commander window | - | B22 | Covered | Windows title bar |
| Window | cm_Restore | Restore Total Commander window (from maximized state) | - | B22 | Covered | Windows title bar |
| Window | cm_Exit | Exit or restart Total Commander | Alt+F4 (mem) | B01 | Implement (Stage 5) | Exit |
| Export | cm_SaveDetailsToFile | Save all shown columns to file | - | B18 near | Later | Panel listing to file; reuse the B18 CSV/TSV/TXT writer |
| Export | cm_SaveDetailsToFileA | Save all shown columns to file (ANSI) | - | missing | Not | ANSI variant of cm_SaveDetailsToFile |
| Export | cm_SaveDetailsToFileW | Save all shown columns to file (Unicode) | - | missing | Not | Unicode variant of cm_SaveDetailsToFile |
| Export | cm_SaveHdrDetailsToFile | Save all shown columns to file, with headers | - | B18 near | Later | Same with column headers |
| Export | cm_SaveHdrDetailsToFileA | Save all shown columns to file, with headers (ANSI) | - | missing | Not | ANSI variant |
| Export | cm_SaveHdrDetailsToFileW | Save all shown columns to file, with headers (Unicode) | - | missing | Not | Unicode variant |
| Network | cm_NetConnect | Network connections | - | missing | Not | Windows Explorer maps network drives; B02 shows the result |
| Network | cm_NetDisconnect | Disconnect network drives | - | missing | Not | Replaced by Windows |
| Network | cm_NetShareDir | Share current folder | - | missing | Not | Replaced by Windows Properties, Sharing tab (B10) |
| Network | cm_NetUnshareDir | Unshare current folder | - | missing | Not | Replaced by Windows Properties, Sharing tab (B10) |
| Network | cm_ShowFileUser | Show which remote user has opened a local file (run as administrator) | - | missing | Not | Niche admin task; Windows Resource Monitor |
| Network | cm_AdministerServer | Show administrative shares on the currently accessed server during the Total Commander session (to go to \\server\c$... | - | missing | Not | Type the server share path in the path box (B02) |
| Security | cm_EditPermissionInfo | Permissions dialog (NTFS) (Windows NT/XP) | - | missing | Not | Windows Properties, Security tab (B10) |
| Security | cm_EditAuditInfo | File auditing (NTFS) (Windows NT/XP) | - | missing | Not | Windows Properties, Security tab (B10) |
| Security | cm_EditOwnerInfo | Take ownership (NTFS) (Windows NT/XP) | - | missing | Not | Windows Properties, Security tab (B10) |
| Print | cm_PrintFile | Print file | Ctrl+F9 | missing | Covered | Windows Print verb through the shell context menu |
| Print | cm_PrintDir | Print selected files/folders | - | missing | Not | Replaced by Windows; a listing can be exported instead |
| Print | cm_PrintDirSub | Print selected files/folders including subfolders | - | missing | Not | Replaced by Windows |
| Tools | cm_SysInfo | System information | - | missing | Not | Windows System Information |
| Plugins | cm_UnloadPlugins | Unload plugins | - | excluded (plugin platform) | Not | No plugin platform |
| Plugins | cm_LoadAllOnDemandFields | Load on demand fields for all files | - | excluded (plugin platform) | Not | Content-plugin columns |
| Plugins | cm_LoadSelOnDemandFields | Load on demand fields for selected files | - | excluded (plugin platform) | Not | Content-plugin columns |
| Plugins | cm_ContentStopLoadFields | Stop loading on demand fields or delayed fields in custom columns view or thumbnails view | - | excluded (plugin platform) | Not | Content-plugin columns |
| FTP | cm_FtpConnect | Open the FTP Connection Manager | Ctrl+F | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpNew | Connect to an FTP server or download a file | Ctrl+N | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpDisconnect | Close an FTP connection | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpHiddenFiles | Show/hide hidden FTP files (filenames that start with a dot) | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpAbort | Abort current FTP command | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpResumeDownload | Resume aborted FTP download | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpSelectTransferMode | Select Binary, ASCII or Auto transfer mode | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpAddToList | Add selected files to download list | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| FTP | cm_FtpDownloadList | Download files listed in a download list (uploads and some file operations are also supported) | - | excluded (FTP) | Not | FTP is not planned (owner ruling, 2026-11-01) |
| Scripting | cm_Wait | Wait x milliseconds | - | missing | Not | No macro system |
| Scripting | cm_Return | Simulate pressing the Enter key | Enter (mem) | missing | Not | Simulates Enter for macros; no macro system |
| Obsolete | cm_DirectCableConnect | Connect to other PC by cable (via USB or parallel port) | - | missing | Not | Parallel-port cable link is obsolete |
| Obsolete | cm_NTinstallDriver | Install parallel port driver on an NT based OS (run as administrator) | - | missing | Not | Parallel-port driver is obsolete |
| Obsolete | cm_NTremoveDriver | Remove parallel port driver on an NT based OS (run as administrator) | - | missing | Not | Parallel-port driver is obsolete |
| Obsolete | cm_SwitchX64Redirection | 32-bit TC on 64-bit Windows: Redirection of system32 folder off/on | - | missing | Not | 32-bit TC only |
| Panel side copies | cm_LeftThumbs | Left panel: Thumbnail view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftCustomViewMenu | Left panel: Show custom columns menu | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftCustomView[1-9] | Left panel: Activate custom columns view 1-29 | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftComments | Left panel: Enable/disable showing comments instead of file details | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftShort | Left panel: Show only filenames | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftLong | Left panel: Show all file details | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftTree | Left panel: Show folder tree | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftQuickview | Left panel: Enable/disable Quick view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftQuickInternalOnly | Left panel: Enable/disable Quick view without plugins/multimedia | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftHideQuickview | Left panel: Disable Quick view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftExecs | Left panel: Show only programs | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftAllFiles | Left panel: Show all files | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftUserSpec | Left panel: Enable last chosen custom filter, or temporarily save current filter (parameter 8), or load temporarily s... | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftUserDef | Left panel: Set custom filter (show only certain files/folders) | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftByName | Left panel: Sort by name | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftByExt | Left panel: Sort by extension | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftBySize | Left panel: Sort by size | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftByDateTime | Left panel: Sort by date-time | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftUnsorted | Left panel: Change sorting to unsorted | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftNegOrder | Left panel: Reverse sort order | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftOpenDrives | Left panel: Open drive list or go to drive | Alt+F1 (mem) | (active-panel command) | Covered | Alt+F1 runs cm_SrcOpenDrives for the left panel (ARCHITECTURE key map) |
| Panel side copies | cm_LeftPathFocus | Focus the path of the left panel | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftViewModeList | Left panel: Show custom views menu or switch to specified custom view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightThumbs | Right panel: Thumbnail view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightCustomViewMenu | Right panel: Show custom columns menu | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightCustomView[1-9] | Right panel: Activate custom columns view 1-29 | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightComments | Right panel: Enable/disable showing comments instead of file details | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightShort | Right panel: Show only filenames | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightLong | Right panel: Show all file details | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightTree | Right panel: Show folder tree | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightQuickview | Right panel: Enable/disable Quick view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightQuickInternalOnly | Right panel: Enable/disable Quick view without plugins/multimedia | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightHideQuickview | Right panel: Disable Quick view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightExecs | Right panel: Show only programs | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightAllFiles | Right panel: Show all files | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightUserSpec | Right panel: Enable last chosen custom filter, or temporarily save current filter (parameter 8), or load temporarily... | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightUserDef | Right panel: Set custom filter (show only certain files/folders) | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightByName | Right panel: Sort by name | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightByExt | Right panel: Sort by extension | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightBySize | Right panel: Sort by size | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightByDateTime | Right panel: Sort by date-time | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightUnsorted | Right panel: Change sorting to unsorted | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightNegOrder | Right panel: Reverse sort order | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightOpenDrives | Right panel: Open drive list or go to drive | Alt+F2 (mem) | (active-panel command) | Covered | Alt+F2 runs cm_SrcOpenDrives for the right panel (ARCHITECTURE key map) |
| Panel side copies | cm_RightPathFocus | Focus the path of the right panel | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightViewModeList | Right panel: Show custom views menu or switch to specified custom view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgCustomView[1-9] | Inactive panel: Activate custom columns view 1-29 | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftDirBranch | Left panel: Branch view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightDirBranch | Right panel: Branch view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftDirBranchSel | Left panel: Branch view, only selected | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightDirBranchSel | Right panel: Branch view, only selected | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgActivateTab[1-10] | Inactive panel: Activate folder tab 1-99 (or tab number specified by parameter, negative means counting from the end) | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftActivateTab[1-10] | Left panel: Activate folder tab 1-99 (or tab number specified by parameter, negative means counting from the end) | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightActivateTab[1-10] | Right panel: Activate folder tab 1-99 (or tab number specified by parameter, negative means counting from the end) | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgNextCustomView | Inactive panel: Next custom columns view | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgPrevCustomView | Inactive panel: Previous custom columns view | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftNextCustomView | Left panel: Next custom columns view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftPrevCustomView | Left panel: Previous custom columns view | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightNextCustomView | Right panel: Next custom columns view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightPrevCustomView | Right panel: Previous custom columns view | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftSwitchToThisCustomView | Left panel: Activate custom columns view 0-99 (for scripting, lparam=view no.) | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightSwitchToThisCustomView | Right panel: Activate custom columns view 0-99 (for scripting, lparam=view no.) | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgTabsList | Inactive panel: Show a list of all folder tabs | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftTabsList | Left panel: Show a list of all folder tabs | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightTabsList | Right panel: Show a list of all folder tabs | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgSortByCol[1-10,99] | Inactive panel: Sort by column 1-99 | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftSortByCol&lt;1-10,99> | Left panel: Sort by column 1-99 | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightSortByCol[1-10,99] | Right panel: Sort by column 1-99 | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftViewMode0 | Left panel: Activate default view mode | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_LeftViewMode[1-9] | Left panel: Activate custom view 1-249 | - | (active-panel command) | Not | Duplicate: fixed left-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightViewMode0 | Right panel: Activate default view mode | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_RightViewMode[1-9] | Right panel: Activate custom view 1-249 | - | (active-panel command) | Not | Duplicate: fixed right-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgViewMode0 | Inactive panel: Activate default view mode | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Panel side copies | cm_TrgViewMode[1-9] | Inactive panel: Activate custom view 1-249 | - | (active-panel command) | Not | Duplicate: fixed inactive-panel copy of the active-panel command; one command that takes the side is enough |
| Tree | cm_SeparateTreeOff | Disable separate tree panel | - | missing | Not | Covered by the single tree toggle |
| Tree | cm_SeparateTree1 | One separate tree panel | - | missing | Not | Covered by the single tree toggle |
| Tree | cm_SeparateTree2 | Two separate tree panels | - | missing | Not | Two separate trees; one tree toggle is enough |
| Tree | cm_SwitchSeparateTree | Switch between 0, 1, or 2 separate tree panels | Ctrl+Shift+F8 | missing | Not | Cycle variant of the tree toggle |
| Tree | cm_ToggleSeparateTree1 | One separate tree panel on/off | - | missing | Later | Tree panel on or off; follows cm_SrcTree |
| Tree | cm_ToggleSeparateTree2 | Two separate tree panels on/off | - | missing | Not | Two separate trees; one tree toggle is enough |
