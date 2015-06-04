using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;

public class MasterAudioClipManager : EditorWindow {
    private const string NO_CLIPS_SELECTED = "There are no clips selected.";
    private const string CACHE_FILE_PATH = "Assets/DarkTonic/MasterAudio/audioImportSettings.xml";
    private const string ALL_FOLDERS_KEY = "[All]";
    private const int MAX_PAGE_SIZE = 200;

    public AudioInfoData clipList = new AudioInfoData();

    public int bulkBitrate = 156000;
    public bool bulk3D = true;
    public bool bulkForceMono = false;
    public AudioImporterFormat bulkFormat = AudioImporterFormat.Native;
    public AudioClipLoadType bulkLoadType = AudioClipLoadType.CompressedInMemory;
    public int pageNumber = 0;

    private List<AudioInfo> filterClips = null;
    private List<AudioInfo> filteredOut = null;
    private Vector2 scrollPos;
    private Vector2 outsideScrollPos;
    private List<string> folderPaths = new List<string>();
    private string selectedFolderPath = ALL_FOLDERS_KEY;

    [MenuItem("Window/Master Audio Clip Manager")]
    static void Init() {
        EditorWindow.GetWindow(typeof(MasterAudioClipManager));
    }

    void OnGUI() {
        outsideScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), outsideScrollPos, new Rect(0, 0, 900, 666));

        if (MasterAudioInspectorResources.logoTexture != null) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.logoTexture);
        }

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUI.contentColor = Color.green;
        if (GUILayout.Button(new GUIContent("Scan Project"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
            BuildCache();
            return;
        }

        GUILayout.Space(10);
        if (GUILayout.Button(new GUIContent("Revert Selected"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
            RevertSelected();
            return;
        }

        GUILayout.Space(10);
        if (GUILayout.Button(new GUIContent("Apply Selected"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
            ApplySelected();
            return;
        }

        GUILayout.Space(10);
        RevertColor();

        GUILayout.Label("Full Path Filter");
        var oldFilter = clipList.searchFilter;
        var newFilter = GUILayout.TextField(clipList.searchFilter, EditorStyles.toolbarTextField, GUILayout.Width(200));
        if (newFilter != oldFilter) {
            clipList.searchFilter = newFilter;
            RebuildFilteredList();
        }

        var myPosition = GUILayoutUtility.GetRect(10, 10, ToolbarSeachCancelButton);
        myPosition.x -= 5;
        if (GUI.Button(myPosition, "", ToolbarSeachCancelButton)) {
            clipList.searchFilter = string.Empty;
            RebuildFilteredList();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (!File.Exists(CACHE_FILE_PATH)) {
            DTGUIHelper.ShowLargeBarAlert("Click 'Scan Project' to generate list of Audio Clips.");
            GUI.EndScrollView();
            return;
        }

        if (clipList.audioInfo.Count == 0 || clipList.needsRefresh) {
            if (!LoadAndTranslateFile()) {
                GUI.EndScrollView();
                return;
            }
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Folder");
        var selectedIndex = folderPaths.IndexOf(selectedFolderPath);
        var newIndex = EditorGUILayout.Popup(selectedIndex, folderPaths.ToArray(), GUILayout.Width(800));
        if (newIndex != selectedIndex) {
            selectedFolderPath = folderPaths[newIndex];
            RebuildFilteredList();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        var totalClips = clipList.audioInfo.Count;
        var dynamicText = string.Format("{0}/{1} clips selected.", SelectedClips.Count, FilteredClips.Count);
        dynamicText += " Total clips: " + totalClips;

        double clipCount = (double)totalClips;
        if (filteredOut != null) {
            clipCount = (double)filteredOut.Count;
        }

        int pageCount = (int)Math.Ceiling(clipCount / (double)MAX_PAGE_SIZE);

        var pageNames = new string[pageCount];
        var pageNums = new int[pageCount];
        for (var i = 0; i < pageCount; i++) {
            pageNames[i] = "Page " + (i + 1);
            pageNums[i] = i;
        }


        EditorGUILayout.LabelField(dynamicText);

        var oldPage = pageNumber;

        EditorGUILayout.BeginHorizontal();
        pageNumber = EditorGUILayout.IntPopup("", pageNumber, pageNames, pageNums, GUILayout.Width(100));
        if (oldPage != pageNumber) {
            RebuildFilteredList(true);
        }
        GUILayout.Label("of " + pageCount);

        EditorGUILayout.EndHorizontal();

        // display
        DisplayClips();

        ShowBulkOperations();

        GUI.EndScrollView();
    }

    private void RebuildFilteredList(bool keepPageNumber = false) {
        if (!keepPageNumber) {
            pageNumber = 0;
        }

        filterClips = null;
        filteredOut = null;
    }

    private void ShowBulkOperations() {
        GUILayout.BeginArea(new Rect(0, 616, 895, 200));
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        GUI.contentColor = Color.yellow;
        GUILayout.Label("Bulk Settings: Click Copy buttons to copy setting to all selected."); //  the setting above it to all selected.
        GUILayout.Space(26);

        GUI.contentColor = Color.green;
        if (GUILayout.Button(new GUIContent("Copy", "Copy Compression bitrate above to all selected"), EditorStyles.toolbarButton, GUILayout.Width(45))) {
            CopyBitrateToSelected();
        }
        GUILayout.Space(6);
        if (GUILayout.Button(new GUIContent("Copy", "Copy 3D setting above to all selected"), EditorStyles.toolbarButton, GUILayout.Width(45))) {
            Copy3DToSelected();
        }

        GUILayout.Space(8);
        if (GUILayout.Button(new GUIContent("Copy", "Copy Force Mono setting above to all selected"), EditorStyles.toolbarButton, GUILayout.Width(45))) {
            CopyForceMonoToSelected();
        }

        GUILayout.Space(26);
        if (GUILayout.Button(new GUIContent("Copy", "Copy Audio Format setting above to all selected"), EditorStyles.toolbarButton, GUILayout.Width(45))) {
            CopyFormatToSelected();
        }

        GUILayout.Space(101);
        if (GUILayout.Button(new GUIContent("Copy", "Copy Load Type setting above to all selected"), EditorStyles.toolbarButton, GUILayout.Width(45))) {
            CopyLoadTypeToSelected();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal(EditorStyles.miniButtonLeft);
        GUILayout.Space(246);

        bulkBitrate = EditorGUILayout.IntSlider("", bulkBitrate / 1000, 32, 256, GUILayout.Width(202)) * 1000;
        GUILayout.Space(13);
        bulk3D = GUILayout.Toggle(bulk3D, "");
        GUILayout.Space(36);
        bulkForceMono = GUILayout.Toggle(bulkForceMono, "");
        GUILayout.Space(35);
        bulkFormat = (AudioImporterFormat)EditorGUILayout.EnumPopup(bulkFormat, GUILayout.Width(136));
        GUILayout.Space(6);
        bulkLoadType = (AudioClipLoadType)EditorGUILayout.EnumPopup(bulkLoadType, GUILayout.Width(140));

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private bool LoadAndTranslateFile() {
        XmlDocument xFiles = null;
        try {
            xFiles = new XmlDocument();
            xFiles.Load(CACHE_FILE_PATH);
        }
        catch {
            DTGUIHelper.ShowRedError("Cache file is malformed. Click 'Scan Project' to regenerate it.");
            return false;
        }

        if (clipList.audioInfo.Count == 0) {
            clipList.audioInfo.Clear();
        }

        // translate
        var success = TranslateFromXML(xFiles);
        if (!success) {
            return false;
        }

        return true;
    }

    private void ApplySelected() {
        if (SelectedClips.Count == 0) {
            DTGUIHelper.ShowAlert(NO_CLIPS_SELECTED);
            return;
        }

        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];
            ApplyClipChanges(aClip, false);
            aClip._hasChanged = true;
        }

        clipList.needsRefresh = true;

        WriteFile(clipList);
    }

    private void RevertSelected() {
        if (SelectedClips.Count == 0) {
            DTGUIHelper.ShowAlert(NO_CLIPS_SELECTED);
            return;
        }

        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];
            RevertChanges(aClip);
        }
    }

    private List<AudioInfo> SelectedClips {
        get {
            var selected = new List<AudioInfo>();

            for (var i = 0; i < FilteredClips.Count; i++) {
                if (!FilteredClips[i]._isSelected) {
                    continue;
                }

                selected.Add(FilteredClips[i]);
            }

            return selected;
        }
    }

    private List<AudioInfo> FilteredClips {
        get {
            if (filterClips == null) {
                filterClips = new List<AudioInfo>();

                if (!string.IsNullOrEmpty(clipList.searchFilter)) {
                    if (filteredOut == null) {
                        filteredOut = new List<AudioInfo>();
                        filteredOut.AddRange(clipList.audioInfo);
                    }

                    filteredOut.RemoveAll(delegate(AudioInfo obj) {
                        return !obj._fullPath.ToLower().Contains(clipList.searchFilter.ToLower());
                    });
                }

                if (selectedFolderPath != ALL_FOLDERS_KEY) {
                    if (filteredOut == null) {
                        filteredOut = new List<AudioInfo>();
                        filteredOut.AddRange(clipList.audioInfo);
                    }

                    filteredOut.RemoveAll(delegate(AudioInfo obj) {
                        var index = obj._fullPath.ToLower().LastIndexOf(selectedFolderPath.ToLower());
                        if (index > -1) {
                            var endPart = obj._fullPath.Substring(index + selectedFolderPath.Length + 1);
                            if (endPart.Contains("/")) {
                                return true; // don't show sub-folders
                            }
                        }
                        return !obj._fullPath.ToLower().Contains(selectedFolderPath.ToLower());
                    });
                }

                var arrayToAddFrom = clipList.audioInfo;
                if (filteredOut != null) {
                    arrayToAddFrom = filteredOut;
                }

                var firstResultNum = MAX_PAGE_SIZE * pageNumber;
                var lastResultNum = firstResultNum + MAX_PAGE_SIZE - 1;
                if (lastResultNum > arrayToAddFrom.Count) {
                    lastResultNum = arrayToAddFrom.Count;
                }

                if (arrayToAddFrom.Count > 0) {
                    var isAsc = clipList.sortDir == ClipSortDirection.Ascending;

                    arrayToAddFrom.Sort(delegate(AudioInfo x, AudioInfo y) {
                        if (clipList.sortColumn == ClipSortColumn.Name) {
                            if (isAsc) {
                                return x._name.CompareTo(y._name);
                            } else {
                                return y._name.CompareTo(x._name);
                            }
                        } else if (clipList.sortColumn == ClipSortColumn.Bitrate) {
                            if (isAsc) {
                                return x._origCompressionBitrate.CompareTo(y._origCompressionBitrate);
                            } else {
                                return y._origCompressionBitrate.CompareTo(x._origCompressionBitrate);
                            }
                        } else if (clipList.sortColumn == ClipSortColumn.Is3d) {
                            if (isAsc) {
                                return x._origIs3d.CompareTo(y._origIs3d);
                            } else {
                                return y._origIs3d.CompareTo(x._origIs3d);
                            }
                        } else if (clipList.sortColumn == ClipSortColumn.ForceMono) {
                            if (isAsc) {
                                return x._origForceMono.CompareTo(y._origForceMono);
                            } else {
                                return y._origForceMono.CompareTo(x._origForceMono);
                            }
                        } else if (clipList.sortColumn == ClipSortColumn.AudioFormat) {
                            if (isAsc) {
                                return x._origFormat.CompareTo(y._origFormat);
                            } else {
                                return y._origFormat.CompareTo(x._origFormat);
                            }
                        } else if (clipList.sortColumn == ClipSortColumn.LoadType) {
                            if (isAsc) {
                                return x._origLoadType.CompareTo(y._origLoadType);
                            } else {
                                return y._origLoadType.CompareTo(x._origLoadType);
                            }
                        }

                        return x._name.CompareTo(y._name);
                    });
                }

                // de-select filtered out clips 
                for (var i = 0; i < clipList.audioInfo.Count; i++) {
                    var aClip = clipList.audioInfo[i];
                    if (!arrayToAddFrom.Contains(aClip)) {
                        aClip._isSelected = false;
                    }
                }

                for (var i = firstResultNum; i < lastResultNum; i++) {
                    filterClips.Add(arrayToAddFrom[i]);
                }
            }

            return filterClips;
        }
    }

    private void ChangeSortColumn(ClipSortColumn col) {
        var oldCol = clipList.sortColumn;
        clipList.sortColumn = col;
        if (oldCol != clipList.sortColumn) {
            clipList.sortDir = ClipSortDirection.Ascending;
        } else {
            clipList.sortDir = clipList.sortDir == ClipSortDirection.Ascending ? ClipSortDirection.Descending : ClipSortDirection.Ascending;
        }

        RebuildFilteredList();
    }

    private string ColumnPrefix(ClipSortColumn col) {
        if (col != clipList.sortColumn) {
            return " ";
        }

        return clipList.sortDir == ClipSortDirection.Ascending ? DTGUIHelper.UP_ARROW : DTGUIHelper.DOWN_ARROW;
    }

    private void DisplayClips() {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUI.contentColor = Color.cyan;
        if (GUILayout.Button("All", EditorStyles.toolbarButton, GUILayout.Width(36))) {
            for (var i = 0; i < FilteredClips.Count; i++) {
                FilteredClips[i]._isSelected = true;
            }
        }

        if (GUILayout.Button("None", EditorStyles.toolbarButton, GUILayout.Width(36))) {
            for (var i = 0; i < clipList.audioInfo.Count; i++) {
                clipList.audioInfo[i]._isSelected = false;
            }
        }

        GUI.contentColor = Color.yellow;
        GUILayout.Space(6);
        var columnPrefix = ColumnPrefix(ClipSortColumn.Name);
        if (GUILayout.Button(new GUIContent(columnPrefix + "Clip Name", "Click to sort by Clip Name"), EditorStyles.toolbarButton, GUILayout.Width(156))) {
            ChangeSortColumn(ClipSortColumn.Name);
        }

        columnPrefix = ColumnPrefix(ClipSortColumn.Bitrate);
        if (GUILayout.Button(new GUIContent(columnPrefix + "Compression (kbps)", "Click to sort by Compression Bitrate"), EditorStyles.toolbarButton, GUILayout.Width(214))) {
            ChangeSortColumn(ClipSortColumn.Bitrate);
        }

        columnPrefix = ColumnPrefix(ClipSortColumn.Is3d);
        if (GUILayout.Button(new GUIContent(columnPrefix + "3D", "Click to sort by 3D"), EditorStyles.toolbarButton, GUILayout.Width(36))) {
            ChangeSortColumn(ClipSortColumn.Is3d);
        }

        columnPrefix = ColumnPrefix(ClipSortColumn.ForceMono);
        if (GUILayout.Button(new GUIContent(columnPrefix + "Force Mono", "Click to sort by Force Mono"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
            ChangeSortColumn(ClipSortColumn.ForceMono);
        }

        columnPrefix = ColumnPrefix(ClipSortColumn.AudioFormat);
        if (GUILayout.Button(new GUIContent(columnPrefix + "Audio Format", "Click to sort by Audio Format"), EditorStyles.toolbarButton, GUILayout.Width(144))) {
            ChangeSortColumn(ClipSortColumn.AudioFormat);
        }

        columnPrefix = ColumnPrefix(ClipSortColumn.LoadType);
        if (GUILayout.Button(new GUIContent(columnPrefix + "Load Type", "Click to sort by Load Type"), EditorStyles.toolbarButton, GUILayout.Width(182))) {
            ChangeSortColumn(ClipSortColumn.LoadType);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (FilteredClips.Count == 0) {
            DTGUIHelper.ShowLargeBarAlert("You have filtered all clips out.");
            return;
        }

        scrollPos = GUI.BeginScrollView(new Rect(0, 123, 896, 485), scrollPos, new Rect(0, 124, 880, 24 * FilteredClips.Count + 4));

        for (var i = 0; i < FilteredClips.Count; i++) {
            var aClip = FilteredClips[i];

            if (aClip._isSelected) {
                GUI.backgroundColor = Color.cyan;
            } else {
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.BeginHorizontal(EditorStyles.miniButtonMid); // miniButtonMid, numberField, textField
            EditorGUILayout.BeginHorizontal();

            var wasSelected = aClip._isSelected;
            aClip._isSelected = GUILayout.Toggle(aClip._isSelected, "");

            if (aClip._isSelected) {
                if (!wasSelected) {
                    SelectClip(aClip);
                }
            }

            var bitrateChanged = !aClip._origCompressionBitrate.Equals(aClip._compressionBitrate);
            var is3dChanged = !aClip._origIs3d.Equals(aClip._is3d);
            var isMonoChanged = !aClip._origForceMono.Equals(aClip._forceMono);
            var isFormatChanged = !aClip._origFormat.Equals(aClip._format);
            var isLoadTypeChanged = !aClip._origLoadType.Equals(aClip._loadType);

            var hasChanged = bitrateChanged || is3dChanged || isMonoChanged || isFormatChanged || isLoadTypeChanged;

            if (!hasChanged) {
                ShowDisabledColors();
            } else {
                GUI.contentColor = Color.green;
            }
            if (GUILayout.Button(new GUIContent("Revert"), EditorStyles.toolbarButton, GUILayout.Width(45))) {
                if (!hasChanged) {
                    DTGUIHelper.ShowAlert("This clip's properties have not changed.");
                } else {
                    RevertChanges(aClip);
                }
            }

            RevertColor();

            GUILayout.Space(10);
            GUILayout.Label(new GUIContent(aClip._name, aClip._fullPath), GUILayout.Width(150));

            GUILayout.Space(10);
            MaybeShowChangedColors(bitrateChanged);
            var oldBitrate = aClip._compressionBitrate;
            var bitRate = (int)(aClip._compressionBitrate * .001f);
            aClip._compressionBitrate = EditorGUILayout.IntSlider("", bitRate, 32, 256, GUILayout.Width(202)) * 1000;
            if (oldBitrate != aClip._compressionBitrate) {
                aClip._isSelected = true;
                SelectClip(aClip);
            }
            RevertColor();

            GUILayout.Space(12);
            MaybeShowChangedColors(is3dChanged);
            var old3d = aClip._is3d;
            aClip._is3d = GUILayout.Toggle(aClip._is3d, "");
            if (old3d != aClip._is3d) {
                aClip._isSelected = true;
                SelectClip(aClip);
            }
            RevertColor();

            GUILayout.Space(36);
            MaybeShowChangedColors(isMonoChanged);
            var oldMono = aClip._forceMono;
            aClip._forceMono = GUILayout.Toggle(aClip._forceMono, "", GUILayout.Width(40));
            if (oldMono != aClip._forceMono) {
                aClip._isSelected = true;
                SelectClip(aClip);
            }
            RevertColor();

            GUILayout.Space(10);
            MaybeShowChangedColors(isFormatChanged);
            var oldFmt = aClip._format;
            aClip._format = (AudioImporterFormat)EditorGUILayout.EnumPopup(aClip._format, GUILayout.Width(136));
            if (oldFmt != aClip._format) {
                aClip._isSelected = true;
                SelectClip(aClip);
            }
            RevertColor();

            GUILayout.Space(6);
            MaybeShowChangedColors(isLoadTypeChanged);
            AudioClipLoadType oldLoad = aClip._loadType;
            aClip._loadType = (AudioClipLoadType)EditorGUILayout.EnumPopup(aClip._loadType, GUILayout.Width(140));
            if (oldLoad != aClip._loadType) {
                aClip._isSelected = true;
                SelectClip(aClip);
            }
            RevertColor();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            RevertColor();
        }

        GUI.EndScrollView();
    }

    private void ShowDisabledColors() {
        GUI.color = Color.gray;
        GUI.contentColor = Color.white;
    }

    private void MaybeShowChangedColors(bool areChanged) {
        if (!areChanged) {
            return;
        }

        GUI.backgroundColor = Color.green;
        GUI.color = Color.yellow;
    }

    private void RevertColor() {
        GUI.backgroundColor = Color.white;
        GUI.color = Color.white;
        GUI.contentColor = Color.white;
    }

    private void RevertChanges(AudioInfo info) {
        info._compressionBitrate = info._origCompressionBitrate;
        info._forceMono = info._origForceMono;
        info._format = info._origFormat;
        info._loadType = info._origLoadType;
        info._is3d = info._origIs3d;
    }

    private void ApplyClipChanges(AudioInfo info, bool writeChanges) {
        Selection.objects = new UnityEngine.Object[] { }; // unselect to get "Apply" to work automatically.

        AudioImporter importer = (AudioImporter)AudioImporter.GetAtPath(info._fullPath);
  //      importer.compressionBitrate = info._compressionBitrate;
        importer.forceToMono = info._forceMono;
 //       importer.format = info._format;
 //       importer.loadType = info._loadType;
  //      importer.threeD = info._is3d;

        AssetDatabase.ImportAsset(info._fullPath, ImportAssetOptions.ForceUpdate);
        info._hasChanged = true;

        if (writeChanges) {
            WriteFile(clipList);
        }
    }

    private bool TranslateFromXML(XmlDocument xDoc) {
        folderPaths.Clear();
        folderPaths.Add("[All]");

        var files = xDoc.SelectNodes("/Files//File");

        if (files.Count == 0) {
            DTGUIHelper.ShowLargeBarAlert("You have no audio files in this project. Add some, then click 'Scan Project'.");
            return false;
        }

        try {
            clipList.searchFilter = xDoc.DocumentElement.Attributes["searchFilter"].Value;
            clipList.sortColumn = (ClipSortColumn)Enum.Parse(typeof(ClipSortColumn), xDoc.DocumentElement.Attributes["sortColumn"].Value);
            clipList.sortDir = (ClipSortDirection)Enum.Parse(typeof(ClipSortDirection), xDoc.DocumentElement.Attributes["sortDir"].Value);

            var currentPaths = new List<string>();

            for (var i = 0; i < files.Count; i++) {
                var aNode = files[i];
                var path = aNode.Attributes["path"].Value.Trim();
                var name = aNode.Attributes["name"].Value.Trim();
                var is3d = bool.Parse(aNode.Attributes["is3d"].Value);
                var compressionBitrate = int.Parse(aNode.Attributes["bitRate"].Value);
                var forceMono = bool.Parse(aNode.Attributes["forceMono"].Value);
                var format = (AudioImporterFormat)Enum.Parse(typeof(AudioImporterFormat), aNode.Attributes["format"].Value);
                AudioClipLoadType loadType = (AudioClipLoadType)Enum.Parse(typeof(AudioClipLoadType), aNode.Attributes["loadType"].Value);

                currentPaths.Add(path);

                var folderPath = Path.GetDirectoryName(path);
                if (!folderPaths.Contains(folderPath)) {
                    folderPaths.Add(folderPath);
                }

                var matchingClip = clipList.audioInfo.Find(delegate(AudioInfo obj) {
                    return obj._fullPath == path;
                });

                if (matchingClip == null) {
                    var aud = new AudioInfo(path, name, compressionBitrate, forceMono);
                    clipList.audioInfo.Add(aud);
                } else {
                    matchingClip._origIs3d = is3d;
                    matchingClip._origFormat = format;
                    matchingClip._origLoadType = loadType;
                    matchingClip._origForceMono = forceMono;
                    matchingClip._origCompressionBitrate = compressionBitrate;
                }

                clipList.needsRefresh = false;
            }

            // delete clips no longer in the XML
            clipList.audioInfo.RemoveAll(delegate(AudioInfo obj) {
                return !currentPaths.Contains(obj._fullPath);
            });
        }
        catch {
            DTGUIHelper.ShowRedError("Could not translate XML from cache file. Please click 'Scan Project'.");
            return false;
        }

        return true;
    }

    private void BuildCache() {
        string[] filePaths = AssetDatabase.GetAllAssetPaths();

        var audioInfo = new AudioInfoData();
        filterClips = null;
        pageNumber = 0;

        var updatedTime = DateTime.Now.Ticks;

        for (var i = 0; i < filePaths.Length; i++) {
            var aPath = filePaths[i];

            if (!aPath.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase)
                && !aPath.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase)
                && !aPath.EndsWith(".ogg", StringComparison.InvariantCultureIgnoreCase)) {

                continue;
            }

            var importer = (AudioImporter)AudioImporter.GetAtPath(aPath);
/*
            var bitrate = importer.compressionBitrate;            SARAH - obsolete in unity 5
            
            if (bitrate < 0) {
                bitrate = 156000;
            }
*/
            int bitrate = 156000;

            var newClip = new AudioInfo(aPath, Path.GetFileNameWithoutExtension(aPath), bitrate, importer.forceToMono);
            newClip._lastUpdated = updatedTime;

            audioInfo.audioInfo.Add(newClip);
        }

        audioInfo.audioInfo.RemoveAll(delegate(AudioInfo obj) {
            return obj._lastUpdated < updatedTime;
        });

        // write file
        if (!WriteFile(audioInfo)) {
            return;
        }

        LoadAndTranslateFile();
    }

    private bool WriteFile(AudioInfoData audInfo) {
        StreamWriter writer = null;

        try {
            var sb = new StringBuilder(string.Empty);

            var safeFilter = audInfo.searchFilter.Replace("'", "").Replace("\"", "");
            sb.Append(string.Format("<Files searchFilter='{0}' sortColumn='{1}' sortDir='{2}'>", safeFilter, audInfo.sortColumn, audInfo.sortDir));
            for (var i = 0; i < audInfo.audioInfo.Count; i++) {
                var aud = audInfo.audioInfo[i];

                var is3d = aud._hasChanged ? aud._is3d : aud._origIs3d;
                var bitrate = aud._hasChanged ? aud._compressionBitrate : aud._origCompressionBitrate;
                var mono = aud._hasChanged ? aud._forceMono : aud._origForceMono;
                var fmt = aud._hasChanged ? aud._format : aud._origFormat;
                AudioClipLoadType loadType = aud._hasChanged ? aud._loadType : aud._origLoadType;

                sb.Append(string.Format("<File path='{0}' name='{1}' is3d='{2}' bitRate='{3}' forceMono='{4}' format='{5}' loadType='{6}' />",
                    UtilStrings.ReplaceUnsafeChars(aud._fullPath),
                    UtilStrings.ReplaceUnsafeChars(aud._name),
                    is3d,
                    bitrate,
                    mono,
                    fmt.ToString(),
                    loadType));
            }
            sb.Append("</Files>");

            writer = new StreamWriter(CACHE_FILE_PATH);
            writer.WriteLine(sb.ToString());

            clipList.audioInfo.RemoveAll(delegate(AudioInfo obj) {
                return obj._hasChanged == true;
            });
        }
        catch (Exception ex) {
            Debug.LogError("Error occurred constructing or writing audioImportSettings.xml file: " + ex.ToString());
            return false;
        }
        finally {
            writer.Close();
        }

        return true;
    }

    private void SelectClip(AudioInfo info) {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(info._fullPath);
    }

    public enum ClipSortColumn {
        Name,
        Is3d,
        Bitrate,
        ForceMono,
        AudioFormat,
        LoadType
    }

    public enum ClipSortDirection {
        Ascending,
        Descending
    }

    public class AudioInfoData {
        public List<AudioInfo> audioInfo = new List<AudioInfo>();
        public string searchFilter = string.Empty;
        public ClipSortColumn sortColumn = ClipSortColumn.Name;
        public ClipSortDirection sortDir = ClipSortDirection.Ascending;

        public bool needsRefresh = false;
    }

    public class AudioInfo {
        public bool _origIs3d;
        public int _origCompressionBitrate;
        public bool _origForceMono;
        public AudioImporterFormat _origFormat;
        public AudioClipLoadType _origLoadType;

        public string _fullPath;
        public string _name;
        public bool _is3d;
        public int _compressionBitrate;
        public bool _forceMono;
        public AudioImporterFormat _format;
        public AudioClipLoadType _loadType;
        public bool _isSelected;
        public bool _hasChanged = false;
        public long _lastUpdated;

        public AudioInfo(string fullPath, string name, int compressionBitrate, bool forceMono) {
         //   _origIs3d = is3d;
            _origCompressionBitrate = compressionBitrate;
            _origForceMono = forceMono;
       //     _origFormat = format;
       //     _origLoadType = loadType;

            _fullPath = fullPath;
            _name = name;
        //    _is3d = is3d;
            _compressionBitrate = compressionBitrate;
            _forceMono = forceMono;
     //       _format = format;
      //      _loadType = loadType;
            _isSelected = false;
            _hasChanged = false;
            _lastUpdated = DateTime.MinValue.Ticks;
        }
    }

    private GUIStyle ToolbarSeachCancelButton { get { return GetStyle("ToolbarSeachCancelButton"); } }

    private GUIStyle GetStyle(string styleName) {
        GUIStyle guiStyle = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
        if (guiStyle == null) {
            Debug.LogError((object)("Missing built-in guistyle " + styleName));
            guiStyle = GUI.skin.button;
        }
        return guiStyle;
    }

    private void CopyBitrateToSelected() {
        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];

            aClip._compressionBitrate = bulkBitrate;
        }
    }

    private void Copy3DToSelected() {
        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];

            aClip._is3d = bulk3D;
        }
    }

    private void CopyForceMonoToSelected() {
        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];

            aClip._forceMono = bulkForceMono;
        }
    }

    private void CopyFormatToSelected() {
        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];

            aClip._format = bulkFormat;
        }
    }

    private void CopyLoadTypeToSelected() {
        for (var i = 0; i < SelectedClips.Count; i++) {
            var aClip = SelectedClips[i];

            aClip._loadType = bulkLoadType;
        }
    }
}
