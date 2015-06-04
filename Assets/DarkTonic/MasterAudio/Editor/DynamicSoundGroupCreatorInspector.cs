using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(DynamicSoundGroupCreator))]
public class DynamicSoundGroupCreatorInspector : Editor {
    private const string EXISTING_BUS = "[EXISTING BUS]";
    private const string EXISTING_NAME_NAME = "[EXISTING BUS NAME]";

    private DynamicSoundGroupCreator _creator;
    private List<DynamicSoundGroup> _groups;
    private bool isDirty = false;

    private List<DynamicSoundGroup> ScanForGroups() {
        var groups = new List<DynamicSoundGroup>();

        for (var i = 0; i < _creator.transform.childCount; i++) {
            var aChild = _creator.transform.GetChild(i);

            var grp = aChild.GetComponent<DynamicSoundGroup>();
            if (grp == null) {
                continue;
            }

            grp.groupVariations = VariationsForGroup(aChild.transform);

            groups.Add(grp);
        }

        return groups;
    }

    private List<DynamicGroupVariation> VariationsForGroup(Transform groupTrans) {
        var variations = new List<DynamicGroupVariation>();

        for (var i = 0; i < groupTrans.childCount; i++) {
            var aVar = groupTrans.GetChild(i);

            var variation = aVar.GetComponent<DynamicGroupVariation>();
            variations.Add(variation);
        }

        return variations;
    }

    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;
        isDirty = false;

        _creator = (DynamicSoundGroupCreator)target;

        var isInProjectView = DTGUIHelper.IsPrefabInProjectView(_creator);

        if (MasterAudioInspectorResources.logoTexture != null) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.logoTexture);
        }

        MasterAudio.Instance = null;
        MasterAudio ma = MasterAudio.Instance;

        var busVoiceLimitList = new List<string>();
        busVoiceLimitList.Add(MasterAudio.NO_VOICE_LIMIT_NAME);

        for (var i = 1; i <= 32; i++) {
            busVoiceLimitList.Add(i.ToString());
        }

        var busList = new List<string>();
        busList.Add(MasterAudioGroup.NO_BUS);
        busList.Add(MasterAudioInspector.NEW_BUS_NAME);
        busList.Add(EXISTING_BUS);

        int maxChars = 12;

        GroupBus bus = null;
        for (var i = 0; i < _creator.groupBuses.Count; i++) {
            bus = _creator.groupBuses[i];
            busList.Add(bus.busName);

            if (bus.busName.Length > maxChars) {
                maxChars = bus.busName.Length;
            }
        }
        var busListWidth = 9 * maxChars;

        EditorGUI.indentLevel = 0;  // Space will handle this for the header

        var newAwake = EditorGUILayout.Toggle("Auto-create Items", _creator.createOnAwake);
        if (newAwake != _creator.createOnAwake) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Auto-create Items");
            _creator.createOnAwake = newAwake;
        }
        if (_creator.createOnAwake) {
            DTGUIHelper.ShowColorWarning("*Items will be created as soon as this object is in the Scene.");
        } else {
            DTGUIHelper.ShowLargeBarAlert("You will need to call this object's CreateItems method manually to create the items.");
        }

        var newRemove = EditorGUILayout.Toggle("Auto-remove Items", _creator.removeGroupsOnSceneChange);
        if (newRemove != _creator.removeGroupsOnSceneChange) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Auto-remove Items");
            _creator.removeGroupsOnSceneChange = newRemove;
        }

        if (_creator.removeGroupsOnSceneChange) {
            DTGUIHelper.ShowColorWarning("*Items will be deleted when the Scene changes.");
        } else {
            DTGUIHelper.ShowLargeBarAlert("Items will persist across Scenes if MasterAudio does.");
        }

        EditorGUILayout.Separator();

        _groups = ScanForGroups();
        var groupNameList = GroupNameList;

        EditorGUI.indentLevel = 0;
        GUI.color = _creator.showMusicDucking ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

        var newShowDuck = EditorGUILayout.Toggle("Dynamic Music Ducking", _creator.showMusicDucking);
        if (newShowDuck != _creator.showMusicDucking) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Dynamic Music Ducking");
            _creator.showMusicDucking = newShowDuck;
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_creator.showMusicDucking) {
            GUI.contentColor = Color.green;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("Add Duck Group"), EditorStyles.toolbarButton, GUILayout.Width(100))) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "Add Duck Group");

                var defaultBeginUnduck = 0.5f;
                if (ma != null) {
                    defaultBeginUnduck = ma.defaultRiseVolStart;
                }

                _creator.musicDuckingSounds.Add(new DuckGroupInfo() {
                    soundType = MasterAudio.NO_GROUP_NAME,
                    riseVolStart = defaultBeginUnduck
                });
            }

            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;
            EditorGUILayout.Separator();

            if (_creator.musicDuckingSounds.Count == 0) {
                DTGUIHelper.ShowColorWarning("*You currently have no ducking sounds set up.");
            } else {
                int? duckSoundToRemove = null;

                for (var i = 0; i < _creator.musicDuckingSounds.Count; i++) {
                    var duckSound = _creator.musicDuckingSounds[i];
                    var index = groupNameList.IndexOf(duckSound.soundType);
                    if (index == -1) {
                        index = 0;
                    }

                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    var newIndex = EditorGUILayout.Popup(index, groupNameList.ToArray(), GUILayout.MaxWidth(200));
                    if (newIndex >= 0) {
                        if (index != newIndex) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Duck Group");
                        }
                        duckSound.soundType = groupNameList[newIndex];
                    }

                    GUI.contentColor = Color.green;
                    GUILayout.TextField("Begin Unduck " + duckSound.riseVolStart.ToString("N2"), 20, EditorStyles.miniLabel);

                    var newUnduck = GUILayout.HorizontalSlider(duckSound.riseVolStart, 0f, 1f, GUILayout.Width(60));
                    if (newUnduck != duckSound.riseVolStart) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Begin Unduck");
                        duckSound.riseVolStart = newUnduck;
                    }
                    GUI.contentColor = Color.white;

                    GUILayout.FlexibleSpace();
                    GUILayout.Space(10);
                    if (DTGUIHelper.AddDeleteIcon("Duck Sound")) {
                        duckSoundToRemove = i;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (duckSoundToRemove.HasValue) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "delete Duck Group");
                    _creator.musicDuckingSounds.RemoveAt(duckSoundToRemove.Value);
                }
            }
        }

        EditorGUILayout.Separator();

        GUI.color = _creator.soundGroupsAreExpanded ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;

        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        var newGroupEx = EditorGUILayout.Toggle("Dynamic Group Mixer", _creator.soundGroupsAreExpanded);
        if (newGroupEx != _creator.soundGroupsAreExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Dynamic Group Mixer");
            _creator.soundGroupsAreExpanded = newGroupEx;
        }

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_creator.soundGroupsAreExpanded) {
            var newDragMode = (MasterAudio.DragGroupMode)EditorGUILayout.EnumPopup("Bulk Creation Mode", _creator.curDragGroupMode);
            if (newDragMode != _creator.curDragGroupMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Bulk Creation Mode");
                _creator.curDragGroupMode = newDragMode;
            }

            var bulkMode = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Variation Create Mode", _creator.bulkVariationMode);
            if (bulkMode != _creator.bulkVariationMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Variation Mode");
                _creator.bulkVariationMode = bulkMode;
            }

            // create groups start
            EditorGUILayout.BeginVertical();
            var aEvent = Event.current;

            if (isInProjectView) {
                DTGUIHelper.ShowLargeBarAlert("*You are in Project View and cannot create or navigate Groups.");
                DTGUIHelper.ShowLargeBarAlert("*Pull this prefab into the Scene to create Groups.");
            } else {
                GUI.color = Color.yellow;

                var dragAreaGroup = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
                GUI.Box(dragAreaGroup, "Drag Audio clips here to create groups!");

                GUI.color = Color.white;

                switch (aEvent.type) {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dragAreaGroup.Contains(aEvent.mousePosition)) {
                            break;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (aEvent.type == EventType.DragPerform) {
                            DragAndDrop.AcceptDrag();

                            Transform groupInfo = null;

                            var clips = new List<AudioClip>();

                            foreach (var dragged in DragAndDrop.objectReferences) {
                                var aClip = dragged as AudioClip;
                                if (aClip == null) {
                                    continue;
                                }

                                clips.Add(aClip);
                            }

                            clips.Sort(delegate(AudioClip x, AudioClip y) {
                                return x.name.CompareTo(y.name);
                            });

                            for (var i = 0; i < clips.Count; i++) {
                                var aClip = clips[i];
                                if (_creator.curDragGroupMode == MasterAudio.DragGroupMode.OneGroupPerClip) {
                                    CreateGroup(aClip);
                                } else {
                                    if (groupInfo == null) { // one group with variations
                                        groupInfo = CreateGroup(aClip);
                                    } else {
                                        CreateVariation(groupInfo, aClip);
                                    }
                                }

                                isDirty = true;
                            }
                        }
                        Event.current.Use();
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            // create groups end

            if (_groups.Count == 0) {
                DTGUIHelper.ShowColorWarning("*You currently have no Dynamic Sound Groups created.");
            }

            int? indexToDelete = null;

            EditorGUILayout.LabelField("Group Control", EditorStyles.miniBoldLabel);
            GUI.color = Color.white;
            int? busToCreate = null;
            bool isExistingBus = false;

            for (var i = 0; i < _groups.Count; i++) {
                var aGroup = _groups[i];

                var groupDirty = false;

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label(aGroup.name, GUILayout.Width(150));

                GUILayout.FlexibleSpace();

                // find bus.
                var selectedBusIndex = aGroup.busIndex == -1 ? 0 : aGroup.busIndex;

                GUI.contentColor = Color.white;
                GUI.color = Color.cyan;

                var busIndex = EditorGUILayout.Popup("", selectedBusIndex, busList.ToArray(), GUILayout.Width(busListWidth));
                if (busIndex == -1) {
                    busIndex = 0;
                }

                if (aGroup.busIndex != busIndex && busIndex != 1) {
                    UndoHelper.RecordObjectPropertyForUndo(ref groupDirty, aGroup, "change Group Bus");
                }

                if (busIndex != 1) { // don't change the index, so undo will work.
                    aGroup.busIndex = busIndex;
                }

                GUI.color = Color.white;

                if (selectedBusIndex != busIndex) {
                    if (busIndex == 1 || busIndex == 2) {
                        busToCreate = i;

                        isExistingBus = busIndex == 2;
                    } else if (busIndex >= DynamicSoundGroupCreator.HardCodedBusOptions) {
                        //GroupBus newBus = _creator.groupBuses[busIndex - MasterAudio.HARD_CODED_BUS_OPTIONS];
                        // do nothing unless we add muting and soloing here.
                    }
                }

                GUI.contentColor = Color.green;
                GUILayout.TextField("V " + aGroup.groupMasterVolume.ToString("N2"), 6, EditorStyles.miniLabel);

                var newVol = GUILayout.HorizontalSlider(aGroup.groupMasterVolume, 0f, 1f, GUILayout.Width(100));
                if (newVol != aGroup.groupMasterVolume) {
                    UndoHelper.RecordObjectPropertyForUndo(ref groupDirty, aGroup, "change Group Volume");
                    aGroup.groupMasterVolume = newVol;
                }

                GUI.contentColor = Color.white;

                var buttonPressed = DTGUIHelper.AddDynamicGroupButtons();
                EditorGUILayout.EndHorizontal();

                switch (buttonPressed) {
                    case DTGUIHelper.DTFunctionButtons.Go:
                        Selection.activeGameObject = aGroup.gameObject;
                        break;
                    case DTGUIHelper.DTFunctionButtons.Remove:
                        indexToDelete = i;
                        break;
                    case DTGUIHelper.DTFunctionButtons.Play:
                        PreviewGroup(aGroup);
                        break;
                    case DTGUIHelper.DTFunctionButtons.Stop:
                        StopPreviewingGroup();
                        break;
                }

                if (groupDirty) {
                    EditorUtility.SetDirty(aGroup);
                }
            }

            if (busToCreate.HasValue) {
                CreateBus(busToCreate.Value, isExistingBus);
            }

            if (indexToDelete.HasValue) {
                UndoHelper.DestroyForUndo(_groups[indexToDelete.Value].gameObject);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6);

            GUI.contentColor = Color.green;
            if (GUILayout.Button(new GUIContent("Max Group Volumes", "Reset all group volumes to full"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
                UndoHelper.RecordObjectsForUndo(_groups.ToArray(), "Max Group Volumes");

                for (var l = 0; l < _groups.Count; l++) {
                    var aGroup = _groups[l];
                    aGroup.groupMasterVolume = 1f;
                }
            }
            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();

            //buses
            if (_creator.groupBuses.Count > 0) {
                EditorGUILayout.Separator();

                var oneVoiceBuses = _creator.groupBuses.FindAll(delegate(GroupBus obj) {
                    return obj.voiceLimit == 1;
                }).Count;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Bus Control", EditorStyles.miniBoldLabel, GUILayout.Width(100));
                if (oneVoiceBuses > 0) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Dialog Mode", EditorStyles.miniBoldLabel, GUILayout.Width(100));
                    GUILayout.Space(230);
                }
                EditorGUILayout.EndHorizontal();

                GroupBus aBus = null;
                int? busToDelete = null;

                for (var i = 0; i < _creator.groupBuses.Count; i++) {
                    aBus = _creator.groupBuses[i];

                    EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

                    var newBusName = EditorGUILayout.TextField("", aBus.busName, GUILayout.MaxWidth(170));
                    if (newBusName != aBus.busName) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Bus Name");
                        aBus.busName = newBusName;
                    }

                    GUILayout.FlexibleSpace();

                    if (!aBus.isExisting) {
                        if (aBus.voiceLimit == 1) {
                            GUI.color = Color.yellow;
                            var newMono = GUILayout.Toggle(aBus.isMonoBus, new GUIContent("", "Dialog Bus Mode"));
                            if (newMono != aBus.isMonoBus) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Dialog Bus Mode");
                                aBus.isMonoBus = newMono;
                            }
                        }

                        GUI.color = Color.white;
                        GUILayout.Label("Voices");
                        GUI.color = Color.cyan;

                        var oldLimitIndex = busVoiceLimitList.IndexOf(aBus.voiceLimit.ToString());
                        if (oldLimitIndex == -1) {
                            oldLimitIndex = 0;
                        }
                        var busVoiceLimitIndex = EditorGUILayout.Popup("", oldLimitIndex, busVoiceLimitList.ToArray(), GUILayout.MaxWidth(70));
                        if (busVoiceLimitIndex != oldLimitIndex) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Bus Voice Limit");
                            aBus.voiceLimit = busVoiceLimitIndex <= 0 ? -1 : busVoiceLimitIndex;
                        }

                        GUI.color = Color.green;

                        GUILayout.TextField("V " + aBus.volume.ToString("N2"), 6, EditorStyles.miniLabel);

                        GUI.color = Color.white;
                        var newBusVol = GUILayout.HorizontalSlider(aBus.volume, 0f, 1f, GUILayout.Width(86));
                        if (newBusVol != aBus.volume) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Bus Volume");
                            aBus.volume = newBusVol;
                        }

                        GUI.contentColor = Color.white;
                    } else {
                        DTGUIHelper.ShowColorWarning("Existing bus. No control.");
                    }

                    if (DTGUIHelper.AddDeleteIcon("Bus")) {
                        busToDelete = i;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (busToDelete.HasValue) {
                    DeleteBus(busToDelete.Value);
                }
            }
        }


        EditorGUILayout.Separator();
        // Music playlist Start		
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel = 0;  // Space will handle this for the header

        GUI.color = _creator.playListExpanded ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        var isExp = EditorGUILayout.Toggle("Dynamic Playlist Settings", _creator.playListExpanded);
        if (isExp != _creator.playListExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Dynamic Playlist Settings");
            _creator.playListExpanded = isExp;
        }

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();

        if (_creator.playListExpanded) {
            EditorGUI.indentLevel = 0;  // Space will handle this for the header

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            var oldPlayExpanded = DTGUIHelper.Foldout(_creator.playlistEditorExpanded, "Playlists");
            if (oldPlayExpanded != _creator.playlistEditorExpanded) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Playlists");
                _creator.playlistEditorExpanded = oldPlayExpanded;
            }

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

            GUIContent content;
            var buttonText = "Click to add new Playlist at the end";
            bool addPressed = false;

            // Add button - Process presses later
            addPressed = GUILayout.Button(new GUIContent("+", buttonText),
                                               EditorStyles.toolbarButton);

            var collapseIcon = '\u2261'.ToString();
            content = new GUIContent(collapseIcon, "Click to collapse all");
            var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton);

            var expandIcon = '\u25A1'.ToString();
            content = new GUIContent(expandIcon, "Click to expand all");
            var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton);
            if (masterExpand) {
                ExpandCollapseAllPlaylists(true);
            }
            if (masterCollapse) {
                ExpandCollapseAllPlaylists(false);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            if (_creator.playlistEditorExpanded) {
                int? playlistToRemove = null;
                int? playlistToInsertAt = null;
                int? playlistToMoveUp = null;
                int? playlistToMoveDown = null;

                if (_creator.musicPlaylists.Count == 0) {
                    DTGUIHelper.ShowLargeBarAlert("You have no Playlists set up.");
                }

                for (var i = 0; i < _creator.musicPlaylists.Count; i++) {
                    EditorGUILayout.Separator();
                    var aList = _creator.musicPlaylists[i];

                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    aList.isExpanded = DTGUIHelper.Foldout(aList.isExpanded, "Playlist: " + aList.playlistName);

                    var playlistButtonPressed = DTGUIHelper.AddFoldOutListItemButtons(i, _creator.musicPlaylists.Count, "playlist", false, true);

                    EditorGUILayout.EndHorizontal();

                    if (aList.isExpanded) {
                        EditorGUI.indentLevel = 0;
                        var newPlaylist = EditorGUILayout.TextField("Name", aList.playlistName);
                        if (newPlaylist != aList.playlistName) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Name");
                            aList.playlistName = newPlaylist;
                        }

                        var crossfadeMode = (MasterAudio.Playlist.CrossfadeTimeMode)EditorGUILayout.EnumPopup("Crossfade Mode", aList.crossfadeMode);
                        if (crossfadeMode != aList.crossfadeMode) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Crossfade Mode");
                            aList.crossfadeMode = crossfadeMode;
                        }
                        if (aList.crossfadeMode == MasterAudio.Playlist.CrossfadeTimeMode.Override) {
                            var newCF = EditorGUILayout.Slider("Crossfade time (sec)", aList.crossFadeTime, 0f, 10f);
                            if (newCF != aList.crossFadeTime) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Crossfade time (sec)");
                                aList.crossFadeTime = newCF;
                            }
                        }

                        var newFadeIn = EditorGUILayout.Toggle("Fade In First Song", aList.fadeInFirstSong);
                        if (newFadeIn != aList.fadeInFirstSong) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Fade In First Song");
                            aList.fadeInFirstSong = newFadeIn;
                        }

                        var newFadeOut = EditorGUILayout.Toggle("Fade Out Last Song", aList.fadeOutLastSong);
                        if (newFadeOut != aList.fadeOutLastSong) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Fade Out Last Song");
                            aList.fadeOutLastSong = newFadeOut;
                        }

                        var newTransType = (MasterAudio.SongFadeInPosition)EditorGUILayout.EnumPopup("Song Transition Type", aList.songTransitionType);
                        if (newTransType != aList.songTransitionType) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Song Transition Type");
                            aList.songTransitionType = newTransType;
                        }
                        if (aList.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips) {
                            DTGUIHelper.ShowColorWarning("*All clips must be of exactly the same length.");
                        }

                        EditorGUI.indentLevel = 0;
                        var newBulkMode = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Clip Create Mode", aList.bulkLocationMode);
                        if (newBulkMode != aList.bulkLocationMode) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Bulk Clip Mode");
                            aList.bulkLocationMode = newBulkMode;
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        GUI.contentColor = Color.green;
                        if (GUILayout.Button(new GUIContent("Equalize Song Volumes"), EditorStyles.toolbarButton)) {
                            EqualizePlaylistVolumes(aList.MusicSettings);
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        GUI.contentColor = Color.white;
                        EditorGUILayout.Separator();

                        EditorGUILayout.BeginVertical();
                        var anEvent = Event.current;

                        GUI.color = Color.yellow;

                        var dragArea = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
                        GUI.Box(dragArea, "Drag Audio clips here to add to playlist!");

                        GUI.color = Color.white;

                        switch (anEvent.type) {
                            case EventType.DragUpdated:
                            case EventType.DragPerform:
                                if (!dragArea.Contains(anEvent.mousePosition)) {
                                    break;
                                }

                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                if (anEvent.type == EventType.DragPerform) {
                                    DragAndDrop.AcceptDrag();

                                    foreach (var dragged in DragAndDrop.objectReferences) {
                                        var aClip = dragged as AudioClip;
                                        if (aClip == null) {
                                            continue;
                                        }

                                        AddSongToPlaylist(aList, aClip);
                                    }
                                }
                                Event.current.Use();
                                break;
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel = 2;

                        int? addIndex = null;
                        int? removeIndex = null;
                        int? moveUpIndex = null;
                        int? moveDownIndex = null;

                        for (var j = 0; j < aList.MusicSettings.Count; j++) {
                            var aSong = aList.MusicSettings[j];
                            var clipName = "Empty";
                            switch (aSong.audLocation) {
                                case MasterAudio.AudioLocation.Clip:
                                    if (aSong.clip != null) {
                                        clipName = aSong.clip.name;
                                    }
                                    break;
                                case MasterAudio.AudioLocation.ResourceFile:
                                    if (!string.IsNullOrEmpty(aSong.resourceFileName)) {
                                        clipName = aSong.resourceFileName;
                                    }
                                    break;
                            }
                            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                            EditorGUI.indentLevel = 2;

                            if (!string.IsNullOrEmpty(clipName) && string.IsNullOrEmpty(aSong.songName)) {
                                switch (aSong.audLocation) {
                                    case MasterAudio.AudioLocation.Clip:
                                        aSong.songName = clipName;
                                        break;
                                    case MasterAudio.AudioLocation.ResourceFile:
                                        aSong.songName = clipName;
                                        break;
                                }
                            }

                            var newSongExpanded = DTGUIHelper.Foldout(aSong.isExpanded, aSong.songName);
                            if (newSongExpanded != aSong.isExpanded) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Song expand");
                                aSong.isExpanded = newSongExpanded;
                            }
                            var songButtonPressed = DTGUIHelper.AddFoldOutListItemButtons(j, aList.MusicSettings.Count, "clip", false, true, true);
                            EditorGUILayout.EndHorizontal();

                            if (aSong.isExpanded) {
                                EditorGUI.indentLevel = 0;

                                var oldLocation = aSong.audLocation;
                                var newClipSource = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Audio Origin", aSong.audLocation);
                                if (newClipSource != aSong.audLocation) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Audio Origin");
                                    aSong.audLocation = newClipSource;
                                }

                                switch (aSong.audLocation) {
                                    case MasterAudio.AudioLocation.Clip:
                                        var newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", aSong.clip, typeof(AudioClip), true);
                                        if (newClip != aSong.clip) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Clip");
                                            aSong.clip = newClip;
                                            aSong.songName = newClip.name;
                                        }
                                        break;
                                    case MasterAudio.AudioLocation.ResourceFile:
                                        if (oldLocation != aSong.audLocation) {
                                            if (aSong.clip != null) {
                                                Debug.Log("Audio clip removed to prevent unnecessary memory usage on Resource file Playlist clip.");
                                            }
                                            aSong.clip = null;
                                            aSong.songName = string.Empty;
                                        }

                                        EditorGUILayout.BeginVertical();
                                        anEvent = Event.current;

                                        GUI.color = Color.yellow;
                                        dragArea = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
                                        GUI.Box(dragArea, "Drag Resource Audio clip here to use its name!");
                                        GUI.color = Color.white;

                                        switch (anEvent.type) {
                                            case EventType.DragUpdated:
                                            case EventType.DragPerform:
                                                if (!dragArea.Contains(anEvent.mousePosition)) {
                                                    break;
                                                }

                                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                                if (anEvent.type == EventType.DragPerform) {
                                                    DragAndDrop.AcceptDrag();

                                                    foreach (var dragged in DragAndDrop.objectReferences) {
                                                        var aClip = dragged as AudioClip;
                                                        if (aClip == null) {
                                                            continue;
                                                        }

                                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Resource Filename");

                                                        var resourceFileName = DTGUIHelper.GetResourcePath(aClip);
                                                        if (string.IsNullOrEmpty(resourceFileName)) {
                                                            resourceFileName = aClip.name;
                                                        }

                                                        aSong.resourceFileName = resourceFileName;
                                                        aSong.songName = aClip.name;
                                                    }
                                                }
                                                Event.current.Use();
                                                break;
                                        }
                                        EditorGUILayout.EndVertical();

                                        var newFilename = EditorGUILayout.TextField("Resource Filename", aSong.resourceFileName);
                                        if (newFilename != aSong.resourceFileName) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Resource Filename");
                                            aSong.resourceFileName = newFilename;
                                        }

                                        break;
                                }

                                var newVol = EditorGUILayout.Slider("Volume", aSong.volume, 0f, 1f);
                                if (newVol != aSong.volume) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Volume");
                                    aSong.volume = newVol;
                                }

                                var newPitch = EditorGUILayout.Slider("Pitch", aSong.pitch, -3f, 3f);
                                if (newPitch != aSong.pitch) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Pitch");
                                    aSong.pitch = newPitch;
                                }

                                var newLoop = EditorGUILayout.Toggle("Loop Clip", aSong.isLoop);
                                if (newLoop != aSong.isLoop) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Loop Clip");
                                    aSong.isLoop = newLoop;
                                }
                            }

                            switch (songButtonPressed) {
                                case DTGUIHelper.DTFunctionButtons.Add:
                                    addIndex = j;
                                    break;
                                case DTGUIHelper.DTFunctionButtons.Remove:
                                    removeIndex = j;
                                    break;
                                case DTGUIHelper.DTFunctionButtons.ShiftUp:
                                    moveUpIndex = j;
                                    break;
                                case DTGUIHelper.DTFunctionButtons.ShiftDown:
                                    moveDownIndex = j;
                                    break;
                                case DTGUIHelper.DTFunctionButtons.Play:
                                    MasterAudio.PreviewerInstance.Stop();
                                    MasterAudio.PreviewerInstance.PlayOneShot(aSong.clip, aSong.volume);
                                    break;
                                case DTGUIHelper.DTFunctionButtons.Stop:
                                    MasterAudio.PreviewerInstance.clip = null;
                                    MasterAudio.PreviewerInstance.Stop();
                                    break;
                            }
                        }

                        if (addIndex.HasValue) {
                            var mus = new MusicSetting();
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "add song");
                            aList.MusicSettings.Insert(addIndex.Value + 1, mus);
                        } else if (removeIndex.HasValue) {
                            if (aList.MusicSettings.Count <= 1) {
                                DTGUIHelper.ShowAlert("You cannot delete the last clip. You do not have to use the clips though.");
                            } else {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "delete song");
                                aList.MusicSettings.RemoveAt(removeIndex.Value);
                            }
                        } else if (moveUpIndex.HasValue) {
                            var item = aList.MusicSettings[moveUpIndex.Value];

                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "shift up song");

                            aList.MusicSettings.Insert(moveUpIndex.Value - 1, item);
                            aList.MusicSettings.RemoveAt(moveUpIndex.Value + 1);
                        } else if (moveDownIndex.HasValue) {
                            var index = moveDownIndex.Value + 1;
                            var item = aList.MusicSettings[index];

                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "shift down song");

                            aList.MusicSettings.Insert(index - 1, item);
                            aList.MusicSettings.RemoveAt(index + 1);
                        }
                    }

                    switch (playlistButtonPressed) {
                        case DTGUIHelper.DTFunctionButtons.Remove:
                            playlistToRemove = i;
                            break;
                        case DTGUIHelper.DTFunctionButtons.Add:
                            playlistToInsertAt = i;
                            break;
                        case DTGUIHelper.DTFunctionButtons.ShiftUp:
                            playlistToMoveUp = i;
                            break;
                        case DTGUIHelper.DTFunctionButtons.ShiftDown:
                            playlistToMoveDown = i;
                            break;
                    }
                }

                if (playlistToRemove.HasValue) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "delete Playlist");

                    _creator.musicPlaylists.RemoveAt(playlistToRemove.Value);
                }
                if (playlistToInsertAt.HasValue) {
                    var pl = new MasterAudio.Playlist();
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "add Playlist");
                    _creator.musicPlaylists.Insert(playlistToInsertAt.Value + 1, pl);
                }
                if (playlistToMoveUp.HasValue) {
                    var item = _creator.musicPlaylists[playlistToMoveUp.Value];
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "shift up Playlist");
                    _creator.musicPlaylists.Insert(playlistToMoveUp.Value - 1, item);
                    _creator.musicPlaylists.RemoveAt(playlistToMoveUp.Value + 1);
                }
                if (playlistToMoveDown.HasValue) {
                    var index = playlistToMoveDown.Value + 1;
                    var item = _creator.musicPlaylists[index];

                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "shift down Playlist");

                    _creator.musicPlaylists.Insert(index - 1, item);
                    _creator.musicPlaylists.RemoveAt(index + 1);
                }

                EditorGUILayout.Separator();
            }

            if (addPressed) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "add Playlist");
                _creator.musicPlaylists.Add(new MasterAudio.Playlist());
            }
        }
        // Music playlist End

        EditorGUILayout.Separator();
        // Show Custom Events
        GUI.color = _creator.showCustomEvents ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;

        EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        var newShowEvents = EditorGUILayout.Toggle("Dynamic Custom Events", _creator.showCustomEvents);
        if (_creator.showCustomEvents != newShowEvents) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Dynamic Custom Events");
            _creator.showCustomEvents = newShowEvents;
        }

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_creator.showCustomEvents) {
            var newEvent = EditorGUILayout.TextField("New Event Name", _creator.newEventName);
            if (newEvent != _creator.newEventName) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change New Event Name");
                _creator.newEventName = newEvent;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(154);
            GUI.contentColor = Color.green;
            if (GUILayout.Button("Create New Event", EditorStyles.toolbarButton, GUILayout.Width(100))) {
                CreateCustomEvent(_creator.newEventName);
            }
            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();

            if (_creator.customEventsToCreate.Count == 0) {
                DTGUIHelper.ShowColorWarning("*You currently have no custom events defined here.");
            }

            EditorGUILayout.Separator();

            int? indexToDelete = null;
            int? indexToRename = null;

            for (var i = 0; i < _creator.customEventsToCreate.Count; i++) {
                var anEvent = _creator.customEventsToCreate[i];
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label(anEvent.EventName, GUILayout.Width(170));

                GUILayout.FlexibleSpace();

                var newName = GUILayout.TextField(anEvent.ProspectiveName, GUILayout.Width(170));
                if (newName != anEvent.ProspectiveName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "change Proposed Event Name");
                    anEvent.ProspectiveName = newName;
                }
                var buttonPressed = DTGUIHelper.AddCustomEventDeleteIcon(true);

                switch (buttonPressed) {
                    case DTGUIHelper.DTFunctionButtons.Remove:
                        indexToDelete = i;
                        break;
                    case DTGUIHelper.DTFunctionButtons.Rename:
                        indexToRename = i;
                        break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (indexToDelete.HasValue) {
                _creator.customEventsToCreate.RemoveAt(indexToDelete.Value);
            }
            if (indexToRename.HasValue) {
                RenameEvent(_creator.customEventsToCreate[indexToRename.Value]);
            }
        }

        // End Show Custom Events

        if (GUI.changed || isDirty) {
            EditorUtility.SetDirty(target);
        }

        //DrawDefaultInspector();
    }

    private Transform CreateGroup(AudioClip aClip) {
        if (_creator.groupTemplate == null) {
            DTGUIHelper.ShowAlert("Your 'Group Template' field is empty, please assign it in debug mode. Drag the 'DynamicSoundGroup' prefab from MasterAudio/Sources/Prefabs into that field, then switch back to normal mode.");
            return null;
        }

        var groupName = UtilStrings.TrimSpace(aClip.name);

        var matchingGroup = _groups.Find(delegate(DynamicSoundGroup obj) {
            return obj.transform.name == groupName;
        });

        if (matchingGroup != null) {
            DTGUIHelper.ShowAlert("You already have a Group named '" + groupName + "'. \n\nPlease rename this Group when finished to be unique.");
        }

        var spawnedGroup = (GameObject)GameObject.Instantiate(_creator.groupTemplate, _creator.transform.position, Quaternion.identity);
        spawnedGroup.name = groupName;

        UndoHelper.CreateObjectForUndo(spawnedGroup, "create Dynamic Group");
        spawnedGroup.transform.parent = _creator.transform;

        CreateVariation(spawnedGroup.transform, aClip);

        return spawnedGroup.transform;
    }

    private void CreateVariation(Transform aGroup, AudioClip aClip) {
        if (_creator.variationTemplate == null) {
            DTGUIHelper.ShowAlert("Your 'Variation Template' field is empty, please assign it in debug mode. Drag the 'DynamicGroupVariation' prefab from MasterAudio/Sources/Prefabs into that field, then switch back to normal mode.");
            return;
        }

        var resourceFileName = string.Empty;
        if (_creator.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            resourceFileName = DTGUIHelper.GetResourcePath(aClip);
            if (string.IsNullOrEmpty(resourceFileName)) {
                resourceFileName = aClip.name;
            }
        }

        var clipName = UtilStrings.TrimSpace(aClip.name);

        var myGroup = aGroup.GetComponent<DynamicSoundGroup>();

        var matches = myGroup.groupVariations.FindAll(delegate(DynamicGroupVariation obj) {
            return obj.name == clipName;
        });

        if (matches.Count > 0) {
            DTGUIHelper.ShowAlert("You already have a variation for this Group named '" + clipName + "'. \n\nPlease rename these variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
        }

        var spawnedVar = (GameObject)GameObject.Instantiate(_creator.variationTemplate, _creator.transform.position, Quaternion.identity);
        spawnedVar.name = clipName;

        spawnedVar.transform.parent = aGroup;

        var dynamicVar = spawnedVar.GetComponent<DynamicGroupVariation>();

        if (_creator.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            dynamicVar.audLocation = MasterAudio.AudioLocation.ResourceFile;
            dynamicVar.resourceFileName = resourceFileName;
        } else {
            dynamicVar.GetComponent<AudioSource>().clip = aClip;
        }
    }

    private void CreateCustomEvent(string newEventName) {
        var match = _creator.customEventsToCreate.FindAll(delegate(CustomEvent custEvent) {
            return custEvent.EventName == newEventName;
        });

        if (match.Count > 0) {
            DTGUIHelper.ShowAlert("You already have a custom event named '" + newEventName + "' configured here. Please choose a different name.");
            return;
        }

        var newEvent = new CustomEvent(newEventName);

        _creator.customEventsToCreate.Add(newEvent);
    }

    private void RenameEvent(CustomEvent cEvent) {
        var match = _creator.customEventsToCreate.FindAll(delegate(CustomEvent obj) {
            return obj.EventName == cEvent.ProspectiveName;
        });

        if (match.Count > 0) {
            DTGUIHelper.ShowAlert("You already have a custom event named '" + cEvent.ProspectiveName + "' configured here. Please choose a different name.");
            return;
        }

        cEvent.EventName = cEvent.ProspectiveName;
    }

    private void PreviewGroup(DynamicSoundGroup aGroup) {
        var rndIndex = UnityEngine.Random.Range(0, aGroup.groupVariations.Count);
        var rndVar = aGroup.groupVariations[rndIndex];

        if (rndVar.audLocation == MasterAudio.AudioLocation.ResourceFile) {
            _creator.PreviewerInstance.Stop();
            _creator.PreviewerInstance.PlayOneShot(Resources.Load(rndVar.resourceFileName) as AudioClip, rndVar.GetComponent<AudioSource>().volume);
        } else {
            _creator.PreviewerInstance.PlayOneShot(rndVar.GetComponent<AudioSource>().clip, rndVar.GetComponent<AudioSource>().volume);
        }
    }

    private void StopPreviewingGroup() {
        _creator.PreviewerInstance.Stop();
    }

    private List<string> GroupNameList {
        get {
            var groupNames = new List<string>();
            groupNames.Add(MasterAudio.NO_GROUP_NAME);

            for (var i = 0; i < _groups.Count; i++) {
                groupNames.Add(_groups[i].name);
            }

            return groupNames;
        }
    }

    private void DeleteBus(int busIndex) {
        DynamicSoundGroup aGroup = null;

        var groupsWithBus = new List<DynamicSoundGroup>();
        var groupsWithHigherBus = new List<DynamicSoundGroup>();

        for (var i = 0; i < this._groups.Count; i++) {
            aGroup = this._groups[i];
            if (aGroup.busIndex == -1) {
                continue;
            }
            if (aGroup.busIndex == busIndex + DynamicSoundGroupCreator.HardCodedBusOptions) {
                groupsWithBus.Add(aGroup);
            } else if (aGroup.busIndex > busIndex + DynamicSoundGroupCreator.HardCodedBusOptions) {
                groupsWithHigherBus.Add(aGroup);
            }
        }

        var allObjects = new List<UnityEngine.Object>();
        allObjects.Add(_creator);
        foreach (var g in groupsWithBus) {
            allObjects.Add(g as UnityEngine.Object);
        }

        foreach (var g in groupsWithHigherBus) {
            allObjects.Add(g as UnityEngine.Object);
        }

        UndoHelper.RecordObjectsForUndo(allObjects.ToArray(), "delete Bus");

        // change all
        _creator.groupBuses.RemoveAt(busIndex);

        foreach (var group in groupsWithBus) {
            group.busIndex = -1;
        }

        foreach (var group in groupsWithHigherBus) {
            group.busIndex--;
        }
    }

    private void CreateBus(int groupIndex, bool isExisting) {
        var sourceGroup = _groups[groupIndex];

        var affectedObjects = new UnityEngine.Object[] {
			_creator,
			sourceGroup
		};

        UndoHelper.RecordObjectsForUndo(affectedObjects, "create Bus");

        var newBusName = isExisting ? EXISTING_NAME_NAME : MasterAudioInspector.RENAME_ME_BUS_NAME;

        var newBus = new GroupBus() {
            busName = newBusName
        };

        newBus.isExisting = isExisting;

        _creator.groupBuses.Add(newBus);

        sourceGroup.busIndex = DynamicSoundGroupCreator.HardCodedBusOptions + _creator.groupBuses.Count - 1;
    }

    private void ExpandCollapseAllPlaylists(bool expand) {
        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "toggle Expand / Collapse Playlists");

        for (var i = 0; i < _creator.musicPlaylists.Count; i++) {
            var aList = _creator.musicPlaylists[i];
            aList.isExpanded = expand;

            for (var j = 0; j < aList.MusicSettings.Count; j++) {
                var aSong = aList.MusicSettings[j];
                aSong.isExpanded = expand;
            }
        }
    }

    private void EqualizePlaylistVolumes(List<MusicSetting> playlistClips) {
        var clips = new Dictionary<MusicSetting, float>();

        if (playlistClips.Count < 2) {
            DTGUIHelper.ShowAlert("You must have at least 2 clips in a Playlist to use this function.");
            return;
        }

        float lowestVolume = 1f;

        for (var i = 0; i < playlistClips.Count; i++) {
            var setting = playlistClips[i];
            var ac = setting.clip;

            if (ac == null) {
                continue;
            }

            float average = 0f;
            var buffer = new float[ac.samples];

            Debug.Log("Measuring amplitude of '" + ac.name + "'.");

            try {
                ac.GetData(buffer, 0);
            }
            catch {
                Debug.Log("Could not read data from compressed sample. Skipping '" + setting.clip.name + "'.");
                continue;
            }

            for (int c = 0; c < ac.samples; c++) {
                average += Mathf.Pow(buffer[c], 2);
            }

            average = Mathf.Sqrt(1f / (float)ac.samples * average);

            if (average < lowestVolume) {
                lowestVolume = average;
            }

            clips.Add(setting, average);
        }

        if (clips.Count < 2) {
            DTGUIHelper.ShowAlert("You must have at least 2 clips in a Playlist to use this function.");
            return;
        }

        foreach (var kv in clips) {
            float adjustedVol = lowestVolume / kv.Value;
            //set your volume for each song in your playlist.
            kv.Key.volume = adjustedVol;
        }
    }

    private void AddSongToPlaylist(MasterAudio.Playlist pList, AudioClip aClip) {
        var lastClip = pList.MusicSettings[pList.MusicSettings.Count - 1];

        MusicSetting mus;

        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _creator, "add Song");

        if (lastClip.clip == null) {
            mus = lastClip;
            mus.clip = aClip;
        } else {
            mus = new MusicSetting() {
                volume = 1f,
                pitch = 1f,
                isExpanded = true,
                audLocation = pList.bulkLocationMode
            };

            switch (pList.bulkLocationMode) {
                case MasterAudio.AudioLocation.Clip:
                    mus.clip = aClip;
                    mus.songName = aClip.name;
                    break;
                case MasterAudio.AudioLocation.ResourceFile:
                    var resourceFileName = DTGUIHelper.GetResourcePath(aClip);
                    if (string.IsNullOrEmpty(resourceFileName)) {
                        resourceFileName = aClip.name;
                    }

                    mus.resourceFileName = resourceFileName;
                    mus.songName = aClip.name;
                    break;
            }

            pList.MusicSettings.Add(mus);
        }
    }
}
