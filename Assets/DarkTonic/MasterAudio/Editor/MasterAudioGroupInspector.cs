using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MasterAudioGroup))]
public class MasterAudioGroupInspector : Editor {
	private static bool isValid = true;

    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 0;
        var isDirty = false;

        MasterAudio ma = MasterAudio.Instance;
        if (ma == null) {
            isValid = false;
        }

        MasterAudioGroup _group = (MasterAudioGroup)target;
        _group = RescanChildren(_group);

        if (!isValid) {
            return;
        }

        var isInProjectView = DTGUIHelper.IsPrefabInProjectView(_group);

        if (MasterAudioInspectorResources.logoTexture != null) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.logoTexture);
        }

        var newVol = EditorGUILayout.Slider("Group Master Volume", _group.groupMasterVolume, 0f, 1f);
        if (newVol != _group.groupMasterVolume) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Group Master Volume");
            _group.groupMasterVolume = newVol;
        }

        var newVarSequence = (MasterAudioGroup.VariationSequence)EditorGUILayout.EnumPopup("Variation Sequence", _group.curVariationSequence);
        if (newVarSequence != _group.curVariationSequence) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Variation Sequence");
            _group.curVariationSequence = newVarSequence;
        }

        if (_group.curVariationSequence == MasterAudioGroup.VariationSequence.TopToBottom) {
            var newUseInactive = EditorGUILayout.BeginToggleGroup("Refill Variation Pool After Inactive Time", _group.useInactivePeriodPoolRefill);
            if (newUseInactive != _group.useInactivePeriodPoolRefill) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "toggle Inactive Refill");
                _group.useInactivePeriodPoolRefill = newUseInactive;
            }

            EditorGUI.indentLevel = 1;
            var newInactivePeriod = EditorGUILayout.Slider("Inactive Time (sec)", _group.inactivePeriodSeconds, .2f, 30f);
            if (newInactivePeriod != _group.inactivePeriodSeconds) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Inactive Time");
                _group.inactivePeriodSeconds = newInactivePeriod;
            }

            EditorGUILayout.EndToggleGroup();
        }

        EditorGUI.indentLevel = 0;
        var newVarMode = (MasterAudioGroup.VariationMode)EditorGUILayout.EnumPopup("Variation Mode", _group.curVariationMode);
        if (newVarMode != _group.curVariationMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Variation Mode");
            _group.curVariationMode = newVarMode;
        }

        EditorGUI.indentLevel = 1;

        switch (_group.curVariationMode) {
            case MasterAudioGroup.VariationMode.Normal:
                var newRetrigger = EditorGUILayout.IntSlider("Retrigger Percentage", _group.retriggerPercentage, 0, 100);
                if (newRetrigger != _group.retriggerPercentage) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Retrigger Percentage");
                    _group.retriggerPercentage = newRetrigger;
                }

                var newLimitPoly = EditorGUILayout.Toggle("Limit Polyphony", _group.limitPolyphony);
                if (newLimitPoly != _group.limitPolyphony) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "toggle Limit Polyphony");
                    _group.limitPolyphony = newLimitPoly;
                }
                if (_group.limitPolyphony) {
                    int maxVoices = 0;
                    for (var i = 0; i < _group.groupVariations.Count; i++) {
                        var variation = _group.groupVariations[i];
                        maxVoices += variation.weight;
                    }

                    var newVoiceLimit = EditorGUILayout.IntSlider("Polyphony Voice Limit", _group.voiceLimitCount, 1, maxVoices);
                    if (newVoiceLimit != _group.voiceLimitCount) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Polyphony Voice Limit");
                        _group.voiceLimitCount = newVoiceLimit;
                    }
                }

                var newLimitMode = (MasterAudioGroup.LimitMode)EditorGUILayout.EnumPopup("Replay Limit Mode", _group.limitMode);
                if (newLimitMode != _group.limitMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Replay Limit Mode");
                    _group.limitMode = newLimitMode;
                }

                switch (_group.limitMode) {
                    case MasterAudioGroup.LimitMode.FrameBased:
                        var newFrameLimit = EditorGUILayout.IntSlider("Min Frames Between", _group.limitPerXFrames, 1, 120);
                        if (newFrameLimit != _group.limitPerXFrames) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Min Frames Between");
                            _group.limitPerXFrames = newFrameLimit;
                        }
                        break;
                    case MasterAudioGroup.LimitMode.TimeBased:
                        var newMinTime = EditorGUILayout.Slider("Min Seconds Between", _group.minimumTimeBetween, 0.1f, 10f);
                        if (newMinTime != _group.minimumTimeBetween) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Min Seconds Between");
                            _group.minimumTimeBetween = newMinTime;
                        }
                        break;
                }
                break;
            case MasterAudioGroup.VariationMode.LoopedChain:
                DTGUIHelper.ShowColorWarning("*In this mode, only one Variation can be played at a time.");

                var newLoopMode = (MasterAudioGroup.ChainedLoopLoopMode)EditorGUILayout.EnumPopup("Loop Mode", _group.chainLoopMode);
                if (newLoopMode != _group.chainLoopMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Loop Mode");
                    _group.chainLoopMode = newLoopMode;
                }

                if (_group.chainLoopMode == MasterAudioGroup.ChainedLoopLoopMode.NumberOfLoops) {
                    var newLoopCount = EditorGUILayout.IntSlider("Number of Loops", _group.chainLoopNumLoops, 1, 500);
                    if (newLoopCount != _group.chainLoopNumLoops) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Number of Loops");
                        _group.chainLoopNumLoops = newLoopCount;
                    }
                }

                var newDelayMin = EditorGUILayout.Slider("Clip Change Delay Min", _group.chainLoopDelayMin, 0f, 20f);
                if (newDelayMin != _group.chainLoopDelayMin) {
                    if (_group.chainLoopDelayMax < newDelayMin) {
                        _group.chainLoopDelayMax = newDelayMin;
                    }
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Chained Clip Delay Min");
                    _group.chainLoopDelayMin = newDelayMin;
                }

                var newDelayMax = EditorGUILayout.Slider("Clip Change Delay Max", _group.chainLoopDelayMax, 0f, 20f);
                if (newDelayMax != _group.chainLoopDelayMax) {
                    if (newDelayMax < _group.chainLoopDelayMin) {
                        newDelayMax = _group.chainLoopDelayMin;
                    }
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Chained Clip Delay Max");
                    _group.chainLoopDelayMax = newDelayMax;
                }
                break;
            case MasterAudioGroup.VariationMode.Dialog:
                DTGUIHelper.ShowColorWarning("*In this mode, only one Variation can be played at a time.");

                var newUseDialog = EditorGUILayout.Toggle("Dialog Custom Fade?", _group.useDialogFadeOut);
                if (newUseDialog != _group.useDialogFadeOut) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "toggle Dialog Custom Fade?");
                    _group.useDialogFadeOut = newUseDialog;
                }

                if (_group.useDialogFadeOut) {
                    var newFadeTime = EditorGUILayout.Slider("Custom Fade Out Time", _group.dialogFadeOutTime, 0.1f, 20f);
                    if (newFadeTime != _group.dialogFadeOutTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Custom Fade Out Time");
                        _group.dialogFadeOutTime = newFadeTime;
                    }
                }
                break;
        }

        EditorGUI.indentLevel = 0;
        if (MasterAudio.Instance.prioritizeOnDistance) {
            var newContinual = EditorGUILayout.Toggle("Use Clip Age Priority", _group.useClipAgePriority);
            if (newContinual != _group.useClipAgePriority) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "toggle Use Clip Age Priority");
                _group.useClipAgePriority = newContinual;
            }
        }

        var newBulkMode = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Variation Create Mode", _group.bulkVariationMode);
        if (newBulkMode != _group.bulkVariationMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "change Bulk Variation Mode");
            _group.bulkVariationMode = newBulkMode;
        }
        if (_group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            DTGUIHelper.ShowColorWarning("*Resource mode: make sure to drag from Resource folders only.");
        }

        var newLog = EditorGUILayout.Toggle("Log Sounds", _group.logSound);
        if (newLog != _group.logSound) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _group, "toggle Log Sounds");
            _group.logSound = newLog;
        }

        int? deadChildIndex = null;

        if (!Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.Label("Actions", EditorStyles.wordWrappedLabel, GUILayout.Width(50f));
            GUILayout.Space(96);
            GUI.contentColor = Color.green;
            if (GUILayout.Button(new GUIContent("Equalize Weights", "Reset Weights to one"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
                isDirty = true;
                EqualizeWeights(_group);
            }

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Equalize Variation Volumes"), EditorStyles.toolbarButton, GUILayout.Width(150))) {
                EqualizeVariationVolumes(_group.groupVariations);
            }

            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

        if (!Application.isPlaying) {
            // new variation settings
            EditorGUILayout.BeginVertical();
            var anEvent = Event.current;

            GUI.color = Color.yellow;

            if (isInProjectView) {
                DTGUIHelper.ShowLargeBarAlert("*You are in Project View and cannot create Variations.");
                DTGUIHelper.ShowLargeBarAlert("*Pull this prefab into the Scene to create Variations.");
            } else {
                var dragArea = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
                GUI.Box(dragArea, "Drag Audio clips here to create Variations!");

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

                                CreateVariation(_group, ma, aClip);
                            }
                        }
                        Event.current.Use();
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            // end new variation settings
        }

        if (_group.groupVariations.Count == 0) {
            DTGUIHelper.ShowRedError("You currently have no Variations.");
        } else {
            _group.groupVariations.Sort(delegate(SoundGroupVariation x, SoundGroupVariation y) {
                return x.name.CompareTo(y.name);
            });

            for (var i = 0; i < _group.groupVariations.Count; i++) {
                var variation = _group.groupVariations[i];
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.LabelField(variation.name, EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();

                var varIsDirty = false;

                if (GUILayout.Button(new GUIContent(MasterAudioInspectorResources.gearTexture, "Click to goto Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
                    Selection.activeObject = variation;
                }

                if (!Application.isPlaying) {
                    if (GUILayout.Button(new GUIContent(MasterAudioInspectorResources.deleteTexture, "Click to delete this Variation"), EditorStyles.toolbarButton, GUILayout.Width(40))) {
                        deadChildIndex = i;
                        isDirty = true;
                    }
                }

                var buttonPressed = DTGUIHelper.AddVariationButtons();
                switch (buttonPressed) {
                    case DTGUIHelper.DTFunctionButtons.Play:
                        if (Application.isPlaying) {
                            MasterAudio.PlaySound(_group.name, 1f, null, 0f, variation.name);
                        } else {
                            if (variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
                                MasterAudio.PreviewerInstance.Stop();
                                MasterAudio.PreviewerInstance.PlayOneShot(Resources.Load(variation.resourceFileName) as AudioClip);
                            } else {
                                variation.GetComponent<AudioSource>().Stop();
                                variation.GetComponent<AudioSource>().Play();
                            }
                        }
                        break;
                    case DTGUIHelper.DTFunctionButtons.Stop:
                        if (Application.isPlaying) {
                            MasterAudio.StopAllOfSound(_group.name);
                        } else {
                            if (variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
                                MasterAudio.PreviewerInstance.Stop();
                            } else {
                                variation.GetComponent<AudioSource>().Stop();
                            }
                        }
                        break;
                }

                EditorGUILayout.EndHorizontal();

                if (!Application.isPlaying) {
                    DTGUIHelper.ShowColorWarning("*Fading & random settings are ignored by preview in edit mode.");
                }
                if (variation.GetComponent<AudioSource>() == null) {
                    DTGUIHelper.ShowRedError(string.Format("The Variation: '{0}' has no Audio Source.", variation.name));
                    break;
                }

                var oldLocation = variation.audLocation;
                var newLocation = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Audio Origin", variation.audLocation);
                if (newLocation != variation.audLocation) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Audio Origin");
                    variation.audLocation = newLocation;
                }

                switch (variation.audLocation) {
                    case MasterAudio.AudioLocation.Clip:
                        var newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", variation.GetComponent<AudioSource>().clip, typeof(AudioClip), false);
                        if (newClip != variation.GetComponent<AudioSource>().clip) {
                            UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.GetComponent<AudioSource>(), "change Audio Clip");
                            variation.GetComponent<AudioSource>().clip = newClip;
                        }
                        break;
                    case MasterAudio.AudioLocation.ResourceFile:
                        if (oldLocation != variation.audLocation) {
                            if (variation.GetComponent<AudioSource>().clip != null) {
                                Debug.Log("Audio clip removed to prevent unnecessary memory usage on Resource file Variation.");
                            }
                            variation.GetComponent<AudioSource>().clip = null;
                        }

                        EditorGUILayout.BeginVertical();
                        var anEvent = Event.current;

                        GUI.color = Color.yellow;
                        var dragArea = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
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

                                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Resource Filename");
                                        var resourceFileName = DTGUIHelper.GetResourcePath(aClip);
                                        if (string.IsNullOrEmpty(resourceFileName)) {
                                            resourceFileName = aClip.name;
                                        }

                                        variation.resourceFileName = resourceFileName;
                                    }
                                }
                                Event.current.Use();
                                break;
                        }
                        EditorGUILayout.EndVertical();

                        var newFilename = EditorGUILayout.TextField("Resource Filename", variation.resourceFileName);
                        if (newFilename != variation.resourceFileName) {
                            UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Resource Filename");
                            variation.resourceFileName = newFilename;
                        }
                        break;
                }

                var newVolume = EditorGUILayout.Slider("Volume", variation.GetComponent<AudioSource>().volume, 0f, 1f);
                if (newVolume != variation.GetComponent<AudioSource>().volume) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.GetComponent<AudioSource>(), "change Volume");
                    variation.GetComponent<AudioSource>().volume = newVolume;
                }

                var newPitch = EditorGUILayout.Slider("Pitch", variation.GetComponent<AudioSource>().pitch, -3f, 3f);
                if (newPitch != variation.GetComponent<AudioSource>().pitch) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.GetComponent<AudioSource>(), "change Pitch");
                    variation.GetComponent<AudioSource>().pitch = newPitch;
                }

                var newLoop = EditorGUILayout.Toggle("Loop Clip", variation.GetComponent<AudioSource>().loop);
                if (newLoop != variation.GetComponent<AudioSource>().loop) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.GetComponent<AudioSource>(), "toggle Loop Clip");
                    variation.GetComponent<AudioSource>().loop = newLoop;
                }

                var newWeight = EditorGUILayout.IntSlider("Weight (Instances)", variation.weight, 0, 100);
                if (newWeight != variation.weight) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Weight");
                    variation.weight = newWeight;
                }

                var newFxTailTime = EditorGUILayout.Slider("FX Tail Time", variation.fxTailTime, 0f, 10f);
                if (newFxTailTime != variation.fxTailTime) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change FX Tail Time");
                    variation.fxTailTime = newFxTailTime;
                }

                var newUseRndPitch = EditorGUILayout.BeginToggleGroup("Use Random Pitch", variation.useRandomPitch);
                if (newUseRndPitch != variation.useRandomPitch) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Pitch");
                    variation.useRandomPitch = newUseRndPitch;
                }

                if (variation.useRandomPitch) {
                    var newMode = (SoundGroupVariation.RandomPitchMode)EditorGUILayout.EnumPopup("Pitch Compute Mode", variation.randomPitchMode);
                    if (newMode != variation.randomPitchMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Pitch Compute Mode");
                        variation.randomPitchMode = newMode;
                    }

                    var newPitchMin = EditorGUILayout.Slider("Random Pitch Min", variation.randomPitchMin, -3f, 3f);
                    if (newPitchMin != variation.randomPitchMin) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Pitch Min");
                        variation.randomPitchMin = newPitchMin;
                        if (variation.randomPitchMax <= variation.randomPitchMin) {
                            variation.randomPitchMax = variation.randomPitchMin;
                        }
                    }

                    var newPitchMax = EditorGUILayout.Slider("Random Pitch Max", variation.randomPitchMax, -3f, 3f);
                    if (newPitchMax != variation.randomPitchMax) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Pitch Max");
                        variation.randomPitchMax = newPitchMax;
                        if (variation.randomPitchMin > variation.randomPitchMax) {
                            variation.randomPitchMin = variation.randomPitchMax;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();

                var newUseRndVol = EditorGUILayout.BeginToggleGroup("Use Random Volume", variation.useRandomVolume);
                if (newUseRndVol != variation.useRandomVolume) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Volume");
                    variation.useRandomVolume = newUseRndVol;
                }

                if (variation.useRandomVolume) {
                    var newMode = (SoundGroupVariation.RandomVolumeMode)EditorGUILayout.EnumPopup("Volume Compute Mode", variation.randomVolumeMode);
                    if (newMode != variation.randomVolumeMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Volume Compute Mode");
                        variation.randomVolumeMode = newMode;
                    }

                    var volMin = 0f;
                    if (variation.randomVolumeMode == SoundGroupVariation.RandomVolumeMode.AddToClipVolume) {
                        volMin = -1f;
                    }

                    var newVolMin = EditorGUILayout.Slider("Random Volume Min", variation.randomVolumeMin, volMin, 1f);
                    if (newVolMin != variation.randomVolumeMin) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Volume Min");
                        variation.randomVolumeMin = newVolMin;
                        if (variation.randomVolumeMax <= variation.randomVolumeMin) {
                            variation.randomVolumeMax = variation.randomVolumeMin;
                        }
                    }

                    var newVolMax = EditorGUILayout.Slider("Random Volume Max", variation.randomVolumeMax, volMin, 1f);
                    if (newVolMax != variation.randomVolumeMax) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Volume Max");
                        variation.randomVolumeMax = newVolMax;
                        if (variation.randomVolumeMin > variation.randomVolumeMax) {
                            variation.randomVolumeMin = variation.randomVolumeMax;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();

                var newSilence = EditorGUILayout.BeginToggleGroup("Use Random Delay", variation.useIntroSilence);
                if (newSilence != variation.useIntroSilence) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Delay");
                    variation.useIntroSilence = newSilence;
                }

                if (variation.useIntroSilence) {
                    var newSilenceMin = EditorGUILayout.Slider("Delay Min (sec)", variation.introSilenceMin, 0f, 100f);
                    if (newSilenceMin != variation.introSilenceMin) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Delay Min (sec)");
                        variation.introSilenceMin = newSilenceMin;
                        if (variation.introSilenceMin > variation.introSilenceMax) {
                            variation.introSilenceMax = newSilenceMin;
                        }
                    }

                    var newSilenceMax = EditorGUILayout.Slider("Delay Max (sec)", variation.introSilenceMax, 0f, 100f);
                    if (newSilenceMax != variation.introSilenceMax) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Delay Max (sec)");
                        variation.introSilenceMax = newSilenceMax;
                        if (variation.introSilenceMax < variation.introSilenceMin) {
                            variation.introSilenceMin = newSilenceMax;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();

                var newFades = EditorGUILayout.BeginToggleGroup("Use Custom Fading", variation.useFades);
                if (newFades != variation.useFades) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Custom Fading");
                    variation.useFades = newFades;
                }

                if (variation.useFades) {
                    var newFadeIn = EditorGUILayout.Slider("Fade In Time (sec)", variation.fadeInTime, 0f, 10f);
                    if (newFadeIn != variation.fadeInTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Fade In Time");
                        variation.fadeInTime = newFadeIn;
                    }

                    var newFadeOut = EditorGUILayout.Slider("Fade Out time (sec)", variation.fadeOutTime, 0f, 10f);
                    if (newFadeOut != variation.fadeOutTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Fade Out Time");
                        variation.fadeOutTime = newFadeOut;
                    }
                }
                EditorGUILayout.EndToggleGroup();

                EditorGUILayout.Separator();

                if (varIsDirty) {
                    EditorUtility.SetDirty(variation.GetComponent<AudioSource>());
                    EditorUtility.SetDirty(variation);
                }
            }
        }

        if (deadChildIndex.HasValue) {
            var deadVar = _group.groupVariations[deadChildIndex.Value];

            if (deadVar != null) {
                // delete variation from Hierarchy
                GameObject.DestroyImmediate(deadVar.gameObject);
            }

            // delete group.
            _group.groupVariations.RemoveAt(deadChildIndex.Value);
        }

        if (GUI.changed || isDirty) {
            EditorUtility.SetDirty(target);
        }

        //DrawDefaultInspector();
    }
	
	public static MasterAudioGroup RescanChildren(MasterAudioGroup group) {
		var newChildren = new List<SoundGroupVariation>();
		
		var childNames = new List<string>();
		
		SoundGroupVariation variation = null;
		
		for (var i = 0; i < group.transform.childCount; i++) {
			var child = group.transform.GetChild(i);
			
			if (!Application.isPlaying) {
				if (childNames.Contains(child.name)) {
					DTGUIHelper.ShowColorWarning("You have more than one Variation named: " + child.name + ".");
					DTGUIHelper.ShowColorWarning("Please ensure each Variation of this Group has a unique name.");
					isValid = false;
					return null;
				}
			}
			
			childNames.Add(child.name);
			
			variation = child.GetComponent<SoundGroupVariation>();
			
			newChildren.Add(variation);
		}
		
		group.groupVariations = newChildren;
		return group;
	}
	
	public void EqualizeWeights(MasterAudioGroup _group) {
		var variations = new SoundGroupVariation[_group.groupVariations.Count];
		
		SoundGroupVariation variation = null;
		for (var i = 0; i < _group.groupVariations.Count; i++) {
			variation = _group.groupVariations[i];
			variations[i] = variation;
		}
		
		UndoHelper.RecordObjectsForUndo(variations, "Equalize Weights");
		
		foreach (var vari in variations) {
			vari.weight = 1;
		}
	}

    public void CreateVariation(MasterAudioGroup group, MasterAudio ma, AudioClip clip) {
        var resourceFileName = string.Empty;
        if (group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            resourceFileName = DTGUIHelper.GetResourcePath(clip);
            if (string.IsNullOrEmpty(resourceFileName)) {
                resourceFileName = clip.name;
            }
        }

        var clipName = clip.name;

        if (group.transform.FindChild(clipName) != null) {
            DTGUIHelper.ShowAlert("You already have a Variation for this Group named '" + clipName + "'. \n\nPlease rename these Variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
        }

        var newVar = (GameObject)GameObject.Instantiate(ma.soundGroupVariationTemplate.gameObject, group.transform.position, Quaternion.identity);
        UndoHelper.CreateObjectForUndo(newVar, "create Variation");

        newVar.transform.name = clipName;
        newVar.transform.parent = group.transform;
        var variation = newVar.GetComponent<SoundGroupVariation>();

        if (group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            variation.audLocation = MasterAudio.AudioLocation.ResourceFile;
            variation.resourceFileName = resourceFileName;
        } else {
            newVar.GetComponent<AudioSource>().clip = clip;
        }
    }

    private void EqualizeVariationVolumes(List<SoundGroupVariation> variations) {
        var clips = new Dictionary<SoundGroupVariation, float>();

        if (variations.Count < 2) {
            DTGUIHelper.ShowAlert("You must have at least 2 Variations to use this function.");
            return;
        }

        float lowestVolume = 1f;

        for (var i = 0; i < variations.Count; i++) {
            var setting = variations[i];

            AudioClip ac = null;

            switch (setting.audLocation) {
                case MasterAudio.AudioLocation.Clip:
                    if (setting.GetComponent<AudioSource>().clip == null) {
                        continue;
                    }
                    ac = setting.GetComponent<AudioSource>().clip;
                    break;
                case MasterAudio.AudioLocation.ResourceFile:
                    if (string.IsNullOrEmpty(setting.resourceFileName)) {
                        continue;
                    }

                    ac = Resources.Load(setting.resourceFileName) as AudioClip;

                    if (ac == null) {
                        continue; // bad resource path
                    }
                    break;
            }

            if (!ac.isReadyToPlay) {
                Debug.Log("Clip is not ready to play (streaming?). Skipping '" + setting.name + "'.");
                continue;
            }

            float average = 0f;
            var buffer = new float[ac.samples];

            Debug.Log("Measuring amplitude of '" + ac.name + "'.");

            ac.GetData(buffer, 0);

            for (int c = 0; c < ac.samples; c++) {
                average += Mathf.Pow(buffer[c], 2);
            }

            average = Mathf.Sqrt(1f / (float)ac.samples * average);

            if (average < lowestVolume) {
                lowestVolume = average;
            }

            if (average == 0f) {
                // don't factor in.
                continue;
            }
            clips.Add(setting, average);
        }

        if (clips.Count < 2) {
            DTGUIHelper.ShowAlert("You must have at least 2 Variations with non-compressed, non-streaming clips to use this function.");
            return;
        }

        foreach (var kv in clips) {
            if (kv.Value == 0) {
                // skip
                continue;
            }
            float adjustedVol = lowestVolume / kv.Value;
            //set your volume for each Variation in your Sound Group.
            kv.Key.GetComponent<AudioSource>().volume = adjustedVol;
        }
    }
}
