using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SoundGroupVariation))]
public class SoundGroupVariationInspector : Editor {
    private SoundGroupVariation _variation;

    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 0;
        var isDirty = false;

        _variation = (SoundGroupVariation)target;

        if (MasterAudioInspectorResources.logoTexture != null) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.logoTexture);
        }

        //		if (Application.isPlaying) {
        //			DTGUIHelper.ShowColorWarning(_variation.IsAvailableToPlay.ToString());
        //		}

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUI.contentColor = Color.green;
        if (GUILayout.Button(new GUIContent("Back to Group", "Select Group in Hierarchy"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
            Selection.activeObject = _variation.transform.parent.gameObject;
        }
        GUILayout.FlexibleSpace();
        GUI.contentColor = Color.white;

        var ma = MasterAudio.Instance;
        if (ma != null) {
            var buttonPressed = DTGUIHelper.AddVariationButtons();

            switch (buttonPressed) {
                case DTGUIHelper.DTFunctionButtons.Play:
                    if (Application.isPlaying) {
                        MasterAudio.PlaySound(_variation.transform.parent.name, 1f, null, 0f, _variation.name);
                    } else {
                        isDirty = true;
                        if (_variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
                            MasterAudio.PreviewerInstance.Stop();
                            MasterAudio.PreviewerInstance.PlayOneShot(Resources.Load(_variation.resourceFileName) as AudioClip);
                        } else {
                            PlaySound(_variation.GetComponent<AudioSource>());
                        }
                    }
                    break;
                case DTGUIHelper.DTFunctionButtons.Stop:
                    if (Application.isPlaying) {
                        MasterAudio.StopAllOfSound(_variation.transform.parent.name);
                    } else {
                        if (_variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
                            MasterAudio.PreviewerInstance.Stop();
                        } else {
                            StopSound(_variation.GetComponent<AudioSource>());
                        }
                    }
                    break;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (ma != null && !Application.isPlaying) {
            DTGUIHelper.ShowColorWarning("*Fading & random settings are ignored by preview in edit mode.");
        }

        var oldLocation = _variation.audLocation;
        var newLocation = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Audio Origin", _variation.audLocation);

        if (newLocation != oldLocation) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Audio Origin");
            _variation.audLocation = newLocation;
        }

        switch (_variation.audLocation) {
            case MasterAudio.AudioLocation.Clip:
                var newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", _variation.GetComponent<AudioSource>().clip, typeof(AudioClip), false);

                if (newClip != _variation.GetComponent<AudioSource>().clip) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation.GetComponent<AudioSource>(), "assign Audio Clip");
                    _variation.GetComponent<AudioSource>().clip = newClip;
                }
                break;
            case MasterAudio.AudioLocation.ResourceFile:
                if (oldLocation != _variation.audLocation) {
                    if (_variation.GetComponent<AudioSource>().clip != null) {
                        Debug.Log("Audio clip removed to prevent unnecessary memory usage on Resource file Variation.");
                    }
                    _variation.GetComponent<AudioSource>().clip = null;
                }

                EditorGUILayout.BeginVertical();
                var anEvent = Event.current;

                GUI.color = Color.yellow;
                var dragArea = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
                GUI.Box(dragArea, "Drag Resource Audio clip here to use its name!");
                GUI.color = Color.white;

                var newFilename = string.Empty;

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

                                newFilename = DTGUIHelper.GetResourcePath(aClip);
                                if (string.IsNullOrEmpty(newFilename)) {
                                    newFilename = aClip.name;
                                }

                                if (newFilename != _variation.resourceFileName) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Resource filename");
                                    _variation.resourceFileName = newFilename;
                                }
                                break;
                            }
                        }
                        Event.current.Use();
                        break;
                }
                EditorGUILayout.EndVertical();

                newFilename = EditorGUILayout.TextField("Resource Filename", _variation.resourceFileName);
                if (newFilename != _variation.resourceFileName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Resource filename");
                    _variation.resourceFileName = newFilename;
                }
                break;
        }

        var newVolume = EditorGUILayout.Slider("Volume", _variation.GetComponent<AudioSource>().volume, 0f, 1f);
        if (newVolume != _variation.GetComponent<AudioSource>().volume) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation.GetComponent<AudioSource>(), "change Volume");
            _variation.GetComponent<AudioSource>().volume = newVolume;
        }

        var newPitch = EditorGUILayout.Slider("Pitch", _variation.GetComponent<AudioSource>().pitch, -3f, 3f);
        if (newPitch != _variation.GetComponent<AudioSource>().pitch) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation.GetComponent<AudioSource>(), "change Pitch");
            _variation.GetComponent<AudioSource>().pitch = newPitch;
        }

        var newLoop = EditorGUILayout.Toggle("Loop Clip", _variation.GetComponent<AudioSource>().loop);
        if (newLoop != _variation.GetComponent<AudioSource>().loop) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation.GetComponent<AudioSource>(), "toggle Loop");
            _variation.GetComponent<AudioSource>().loop = newLoop;
        }

        var newWeight = EditorGUILayout.IntSlider("Weight (Instances)", _variation.weight, 0, 100);
        if (newWeight != _variation.weight) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Weight");
            _variation.weight = newWeight;
        }

        var newFxTailTime = EditorGUILayout.Slider("FX Tail Time", _variation.fxTailTime, 0f, 10f);
        if (newFxTailTime != _variation.fxTailTime) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change FX Tail Time");
            _variation.fxTailTime = newFxTailTime;
        }

        var newUseRndPitch = EditorGUILayout.BeginToggleGroup("Use Random Pitch", _variation.useRandomPitch);
        if (newUseRndPitch != _variation.useRandomPitch) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "toggle Use Random Pitch");
            _variation.useRandomPitch = newUseRndPitch;
        }

        if (_variation.useRandomPitch) {
            var newMode = (SoundGroupVariation.RandomPitchMode)EditorGUILayout.EnumPopup("Pitch Compute Mode", _variation.randomPitchMode);
            if (newMode != _variation.randomPitchMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Pitch Compute Mode");
                _variation.randomPitchMode = newMode;
            }

            var newPitchMin = EditorGUILayout.Slider("Random Pitch Min", _variation.randomPitchMin, -3f, 3f);
            if (newPitchMin != _variation.randomPitchMin) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Random Pitch Min");
                _variation.randomPitchMin = newPitchMin;
                if (_variation.randomPitchMax <= _variation.randomPitchMin) {
                    _variation.randomPitchMax = _variation.randomPitchMin;
                }
            }

            var newPitchMax = EditorGUILayout.Slider("Random Pitch Max", _variation.randomPitchMax, -3f, 3f);
            if (newPitchMax != _variation.randomPitchMax) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Random Pitch Max");
                _variation.randomPitchMax = newPitchMax;
                if (_variation.randomPitchMin > _variation.randomPitchMax) {
                    _variation.randomPitchMin = _variation.randomPitchMax;
                }
            }
        }

        EditorGUILayout.EndToggleGroup();

        var newUseRndVol = EditorGUILayout.BeginToggleGroup("Use Random Volume", _variation.useRandomVolume);
        if (newUseRndVol != _variation.useRandomVolume) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "toggle Use Random Volume");
            _variation.useRandomVolume = newUseRndVol;
        }

        if (_variation.useRandomVolume) {
            var newMode = (SoundGroupVariation.RandomVolumeMode)EditorGUILayout.EnumPopup("Volume Compute Mode", _variation.randomVolumeMode);
            if (newMode != _variation.randomVolumeMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Volume Compute Mode");
                _variation.randomVolumeMode = newMode;
            }

            var volMin = 0f;
            if (_variation.randomVolumeMode == SoundGroupVariation.RandomVolumeMode.AddToClipVolume) {
                volMin = -1f;
            }

            var newVolMin = EditorGUILayout.Slider("Random Volume Min", _variation.randomVolumeMin, volMin, 1f);
            if (newVolMin != _variation.randomVolumeMin) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Random Volume Min");
                _variation.randomVolumeMin = newVolMin;
                if (_variation.randomVolumeMax <= _variation.randomVolumeMin) {
                    _variation.randomVolumeMax = _variation.randomVolumeMin;
                }
            }

            var newVolMax = EditorGUILayout.Slider("Random Volume Max", _variation.randomVolumeMax, volMin, 1f);
            if (newVolMax != _variation.randomVolumeMax) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Random Volume Max");
                _variation.randomVolumeMax = newVolMax;
                if (_variation.randomVolumeMin > _variation.randomVolumeMax) {
                    _variation.randomVolumeMin = _variation.randomVolumeMax;
                }
            }
        }

        EditorGUILayout.EndToggleGroup();

        var newSilence = EditorGUILayout.BeginToggleGroup("Use Random Delay", _variation.useIntroSilence);
        if (newSilence != _variation.useIntroSilence) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "toggle Use Random Delay");
            _variation.useIntroSilence = newSilence;
        }

        if (_variation.useIntroSilence) {
            var newSilenceMin = EditorGUILayout.Slider("Delay Min (sec)", _variation.introSilenceMin, 0f, 100f);
            if (newSilenceMin != _variation.introSilenceMin) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Delay Min (sec)");
                _variation.introSilenceMin = newSilenceMin;
                if (_variation.introSilenceMin > _variation.introSilenceMax) {
                    _variation.introSilenceMax = newSilenceMin;
                }
            }

            var newSilenceMax = EditorGUILayout.Slider("Delay Max (sec)", _variation.introSilenceMax, 0f, 100f);
            if (newSilenceMax != _variation.introSilenceMax) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Delay Max (sec)");
                _variation.introSilenceMax = newSilenceMax;
                if (_variation.introSilenceMax < _variation.introSilenceMin) {
                    _variation.introSilenceMin = newSilenceMax;
                }
            }
        }

        EditorGUILayout.EndToggleGroup();


        var newUseFades = EditorGUILayout.BeginToggleGroup("Use Custom Fading", _variation.useFades);
        if (newUseFades != _variation.useFades) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "toggle Use Custom Fading");
            _variation.useFades = newUseFades;
        }

        if (_variation.useFades) {
            var newFadeIn = EditorGUILayout.Slider("Fade In Time (sec)", _variation.fadeInTime, 0f, 10f);
            if (newFadeIn != _variation.fadeInTime) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Fade In Time");
                _variation.fadeInTime = newFadeIn;
            }

            if (_variation.GetComponent<AudioSource>().loop) {
                DTGUIHelper.ShowColorWarning("*Looped clips cannot have a custom fade out.");
            } else {
                var newFadeOut = EditorGUILayout.Slider("Fade Out time (sec)", _variation.fadeOutTime, 0f, 10f);
                if (newFadeOut != _variation.fadeOutTime) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, _variation, "change Fade Out Time");
                    _variation.fadeOutTime = newFadeOut;
                }
            }
        }

        EditorGUILayout.EndToggleGroup();

        var filterList = new List<string>() {
			MasterAudio.NO_GROUP_NAME,
			"Low Pass",
			"High Pass",
			"Distortion",
			"Chorus",
			"Echo",
			"Reverb"
		};

        var newFilterIndex = EditorGUILayout.Popup("Add Filter Effect", 0, filterList.ToArray());
        switch (newFilterIndex) {
            case 1:
                AddFilterComponent(typeof(AudioLowPassFilter));
                break;
            case 2:
                AddFilterComponent(typeof(AudioHighPassFilter));
                break;
            case 3:
                AddFilterComponent(typeof(AudioDistortionFilter));
                break;
            case 4:
                AddFilterComponent(typeof(AudioChorusFilter));
                break;
            case 5:
                AddFilterComponent(typeof(AudioEchoFilter));
                break;
            case 6:
                AddFilterComponent(typeof(AudioReverbFilter));
                break;
        }

        if (GUI.changed || isDirty) {
            EditorUtility.SetDirty(target);
        }

        //DrawDefaultInspector();
    }

    private void AddFilterComponent(Type filterType) {
        _variation.gameObject.AddComponent(filterType);
    }

    private void PlaySound(AudioSource aud) {
        aud.Stop();
        aud.Play();
    }

    private void StopSound(AudioSource aud) {
        aud.Stop();
    }
}
