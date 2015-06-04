using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EventSounds))]
[CanEditMultipleObjects]
public class AudioEventInspector : Editor {
    private List<string> groupNames = null;
    private List<string> busNames = null;
    private List<string> playlistNames = null;
    private List<string> playlistControllerNames = null;
    private List<string> customEventNames = null;
    private bool maInScene;
    private MasterAudio ma;
    private EventSounds sounds;
    private bool hasMechanim = false;
    bool isDirty = false;

    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        MasterAudio.Instance = null;

        ma = MasterAudio.Instance;
        if (ma != null) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.logoTexture);
        }

        isDirty = false;

        sounds = (EventSounds)target;

        maInScene = ma != null;
        if (maInScene) {
            groupNames = ma.GroupNames;
            busNames = ma.BusNames;
            playlistNames = ma.PlaylistNames;
            customEventNames = ma.CustomEventNames;
        }

        playlistControllerNames = new List<string>();
        playlistControllerNames.Add(MasterAudio.DYNAMIC_GROUP_NAME);
        playlistControllerNames.Add(MasterAudio.NO_GROUP_NAME);

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		// component doesn't exist
#else
        var anim = sounds.GetComponent<Animator>();
        hasMechanim = anim != null;
#endif

        var pcs = GameObject.FindObjectsOfType(typeof(PlaylistController));
        for (var i = 0; i < pcs.Length; i++) {
            playlistControllerNames.Add(pcs[i].name);
        }

        // populate unused Events for dropdown
        var unusedEventTypes = new List<string>();
        if (!sounds.useStartSound) {
            unusedEventTypes.Add("Start");
        }
        if (!sounds.useEnableSound) {
            unusedEventTypes.Add("Enable");
        }
        if (!sounds.useDisableSound) {
            unusedEventTypes.Add("Disable");
        }
        if (!sounds.useVisibleSound) {
            unusedEventTypes.Add("Visible");
        }
        if (!sounds.useInvisibleSound) {
            unusedEventTypes.Add("Invisible");
        }

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			// these events don't exist
#else
        if (!sounds.useCollision2dSound) {
            unusedEventTypes.Add("2D Collision Enter");
        }
        if (!sounds.useCollisionExit2dSound) {
            unusedEventTypes.Add("2D Collision Exit");
        }
        if (!sounds.useTriggerEnter2dSound) {
            unusedEventTypes.Add("2D Trigger Enter");
        }
        if (!sounds.useTriggerExit2dSound) {
            unusedEventTypes.Add("2D Trigger Exit");
        }
#endif

        if (!sounds.useCollisionSound) {
            unusedEventTypes.Add("Collision Enter");
        }
        if (!sounds.useCollisionExitSound) {
            unusedEventTypes.Add("Collision Exit");
        }
        if (!sounds.useTriggerEnterSound) {
            unusedEventTypes.Add("Trigger Enter");
        }
        if (!sounds.useTriggerExitSound) {
            unusedEventTypes.Add("Trigger Exit");
        }
        if (!sounds.useParticleCollisionSound) {
            unusedEventTypes.Add("Particle Collision");
        }
        if (!sounds.useMouseEnterSound) {
            unusedEventTypes.Add("Mouse Enter");
        }
        if (!sounds.useMouseExitSound) {
            unusedEventTypes.Add("Mouse Exit");
        }
        if (!sounds.useMouseClickSound) {
            unusedEventTypes.Add("Mouse Down");
        }
        if (!sounds.useMouseDragSound) {
            unusedEventTypes.Add("Mouse Drag");
        }
        if (!sounds.useMouseUpSound) {
            unusedEventTypes.Add("Mouse Up");
        }
        if (!sounds.useNguiOnClickSound && sounds.showNGUI) {
            unusedEventTypes.Add("NGUI Mouse Click");
        }
        if (!sounds.useNguiMouseDownSound && sounds.showNGUI) {
            unusedEventTypes.Add("NGUI Mouse Down");
        }
        if (!sounds.useNguiMouseUpSound && sounds.showNGUI) {
            unusedEventTypes.Add("NGUI Mouse Up");
        }
        if (!sounds.useNguiMouseEnterSound && sounds.showNGUI) {
            unusedEventTypes.Add("NGUI Mouse Enter");
        }
        if (!sounds.useNguiMouseExitSound && sounds.showNGUI) {
            unusedEventTypes.Add("NGUI Mouse Exit");
        }
        if (!sounds.useSpawnedSound && sounds.showPoolManager) {
            unusedEventTypes.Add("Spawned");
        }
        if (!sounds.useDespawnedSound && sounds.showPoolManager) {
            unusedEventTypes.Add("Despawned");
        }

        if (hasMechanim) {
            unusedEventTypes.Add("Mechanim State Entered");
        }

        unusedEventTypes.Add("Custom Event");

        var newDisable = EditorGUILayout.Toggle("Disable Sounds", sounds.disableSounds);
        if (newDisable != sounds.disableSounds) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Disable Sounds");
            sounds.disableSounds = newDisable;
        }

        if (!sounds.disableSounds) {
            var newSpawnMode = (MasterAudio.SoundSpawnLocationMode)EditorGUILayout.EnumPopup("Sound Spawn Mode", sounds.soundSpawnMode);
            if (newSpawnMode != sounds.soundSpawnMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Spawn Mode");
                sounds.soundSpawnMode = newSpawnMode;
            }

            var newGiz = EditorGUILayout.Toggle("Show 3D Gizmo", sounds.showGizmo);
            if (newGiz != sounds.showGizmo) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Show 3D Gizmo");
                sounds.showGizmo = newGiz;
            }

            var newNGUI = EditorGUILayout.Toggle("NGUI Events", sounds.showNGUI);
            if (newNGUI != sounds.showNGUI) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle NGUI Events");
                sounds.showNGUI = newNGUI;
            }

            var newPM = EditorGUILayout.Toggle("Pooling Events", sounds.showPoolManager);
            if (newPM != sounds.showPoolManager) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Pooling Events");
                sounds.showPoolManager = newPM;
            }

            var newUnused = EditorGUILayout.Toggle("Minimal Mode", sounds.hideUnused);
            if (newUnused != sounds.hideUnused) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Hide Unused Events");
                sounds.hideUnused = newUnused;
            }

            var newLogMissing = EditorGUILayout.Toggle("Log Missing Events", sounds.logMissingEvents);
            if (newLogMissing != sounds.logMissingEvents) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Log Missing Events");
                sounds.logMissingEvents = newLogMissing;
            }

            if (sounds.hideUnused) {
                var newEventIndex = EditorGUILayout.Popup("Event To Activate", -1, unusedEventTypes.ToArray());
                if (newEventIndex > -1) {
                    var selectedEvent = unusedEventTypes[newEventIndex];
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Active Event");

                    switch (selectedEvent) {
                        case "Start":
                            sounds.useStartSound = true;
                            AddEventIfZero(sounds.startSound);
                            break;
                        case "Enable":
                            sounds.useEnableSound = true;
                            AddEventIfZero(sounds.enableSound);
                            break;
                        case "Disable":
                            sounds.useDisableSound = true;
                            AddEventIfZero(sounds.disableSound);
                            break;
                        case "Visible":
                            sounds.useVisibleSound = true;
                            AddEventIfZero(sounds.visibleSound);
                            break;
                        case "Invisible":
                            sounds.useInvisibleSound = true;
                            AddEventIfZero(sounds.invisibleSound);
                            break;
                        case "2D Collision Enter":
                            sounds.useCollision2dSound = true;
                            AddEventIfZero(sounds.collision2dSound);
                            break;
                        case "2D Collision Exit":
                            sounds.useCollisionExit2dSound = true;
                            AddEventIfZero(sounds.collisionExit2dSound);
                            break;
                        case "2D Trigger Enter":
                            sounds.useTriggerEnter2dSound = true;
                            AddEventIfZero(sounds.triggerEnter2dSound);
                            break;
                        case "2D Trigger Exit":
                            sounds.useTriggerExit2dSound = true;
                            AddEventIfZero(sounds.triggerExit2dSound);
                            break;
                        case "Collision Enter":
                            sounds.useCollisionSound = true;
                            AddEventIfZero(sounds.collisionSound);
                            break;
                        case "Collision Exit":
                            sounds.useCollisionExitSound = true;
                            AddEventIfZero(sounds.collisionExitSound);
                            break;
                        case "Trigger Enter":
                            sounds.useTriggerEnterSound = true;
                            AddEventIfZero(sounds.triggerSound);
                            break;
                        case "Trigger Exit":
                            sounds.useTriggerExitSound = true;
                            AddEventIfZero(sounds.triggerExitSound);
                            break;
                        case "Particle Collision":
                            sounds.useParticleCollisionSound = true;
                            AddEventIfZero(sounds.particleCollisionSound);
                            break;
                        case "Mouse Enter":
                            sounds.useMouseEnterSound = true;
                            AddEventIfZero(sounds.mouseEnterSound);
                            break;
                        case "Mouse Exit":
                            sounds.useMouseExitSound = true;
                            AddEventIfZero(sounds.mouseExitSound);
                            break;
                        case "Mouse Down":
                            sounds.useMouseClickSound = true;
                            AddEventIfZero(sounds.mouseClickSound);
                            break;
                        case "Mouse Drag":
                            sounds.useMouseDragSound = true;
                            AddEventIfZero(sounds.mouseDragSound);
                            break;
                        case "Mouse Up":
                            sounds.useMouseUpSound = true;
                            AddEventIfZero(sounds.mouseUpSound);
                            break;
                        case "NGUI Mouse Click":
                            sounds.useNguiOnClickSound = true;
                            AddEventIfZero(sounds.nguiOnClickSound);
                            break;
                        case "NGUI Mouse Down":
                            sounds.useNguiMouseDownSound = true;
                            AddEventIfZero(sounds.nguiMouseDownSound);
                            break;
                        case "NGUI Mouse Up":
                            sounds.useNguiMouseUpSound = true;
                            AddEventIfZero(sounds.nguiMouseUpSound);
                            break;
                        case "NGUI Mouse Enter":
                            sounds.useNguiMouseEnterSound = true;
                            AddEventIfZero(sounds.nguiMouseEnterSound);
                            break;
                        case "NGUI Mouse Exit":
                            sounds.useNguiMouseExitSound = true;
                            AddEventIfZero(sounds.nguiMouseExitSound);
                            break;
                        case "Spawned":
                            sounds.useSpawnedSound = true;
                            AddEventIfZero(sounds.spawnedSound);
                            break;
                        case "Despawned":
                            sounds.useDespawnedSound = true;
                            AddEventIfZero(sounds.despawnedSound);
                            break;
                        case "Mechanim State Entered":
                            CreateMechanimStateEntered(false);
                            break;
                        case "Custom Event":
                            CreateCustomEvent(false);
                            break;
                        default:
                            Debug.LogError("Add code for event type: " + selectedEvent);
                            break;
                    }
                }
            } else {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(154);
                GUI.contentColor = Color.green;
                if (GUILayout.Button("Add Custom Event", EditorStyles.toolbarButton, GUILayout.Width(110))) {
                    CreateCustomEvent(true);
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Separator();
        var suffix = string.Empty;
        if (sounds.disableSounds) {
            suffix = " (DISABLED)";
        } else if (unusedEventTypes.Count > 0 && sounds.hideUnused) {
            suffix = " (" + unusedEventTypes.Count + " hidden)";
        }
        GUILayout.Label("Sound Triggers" + suffix, EditorStyles.boldLabel);

        List<bool> changedList = new List<bool>();

        // trigger sounds
        if (!sounds.hideUnused || sounds.useStartSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useStartSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

            var newUseStart = EditorGUILayout.Toggle("Start" + DisabledText, sounds.useStartSound);
            if (newUseStart != sounds.useStartSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Start Sound");
                sounds.useStartSound = newUseStart;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useStartSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.startSound, EventSounds.EventType.OnStart));
            }
        }

        if (!sounds.hideUnused || sounds.useEnableSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useEnableSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newUseEnable = EditorGUILayout.Toggle("Enable" + DisabledText, sounds.useEnableSound);
            if (newUseEnable != sounds.useEnableSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Enable Sound");
                sounds.useEnableSound = newUseEnable;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useEnableSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.enableSound, EventSounds.EventType.OnEnable));
            }
        }

        if (!sounds.hideUnused || sounds.useDisableSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useDisableSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newDisableSound = EditorGUILayout.Toggle("Disable" + DisabledText, sounds.useDisableSound);
            if (newDisableSound != sounds.useDisableSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Disable Sound");
                sounds.useDisableSound = newDisableSound;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useDisableSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.disableSound, EventSounds.EventType.OnDisable));
            }
        }

        if (!sounds.hideUnused || sounds.useVisibleSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useVisibleSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newVisible = EditorGUILayout.Toggle("Visible" + DisabledText, sounds.useVisibleSound);
            if (newVisible != sounds.useVisibleSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Visible Sound");
                sounds.useVisibleSound = newVisible;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useVisibleSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.visibleSound, EventSounds.EventType.OnVisible));
            }
        }

        if (!sounds.hideUnused || sounds.useInvisibleSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useInvisibleSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newInvis = EditorGUILayout.Toggle("Invisible" + DisabledText, sounds.useInvisibleSound);
            if (newInvis != sounds.useInvisibleSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Invisible Sound");
                sounds.useInvisibleSound = newInvis;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useInvisibleSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.invisibleSound, EventSounds.EventType.OnInvisible));
            }
        }

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			// these events don't exist
#else
        if (!sounds.hideUnused || sounds.useCollision2dSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useCollision2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newCollision2d = EditorGUILayout.Toggle("2D Collision Enter" + DisabledText, sounds.useCollision2dSound);
            if (newCollision2d != sounds.useCollision2dSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle 2D Collision Enter Sound");
                sounds.useCollision2dSound = newCollision2d;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useCollision2dSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.collision2dSound, EventSounds.EventType.OnCollision2D));
            }
        }

        if (!sounds.hideUnused || sounds.useCollisionExit2dSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useCollisionExit2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newCollision2d = EditorGUILayout.Toggle("2D Collision Exit" + DisabledText, sounds.useCollisionExit2dSound);
            if (newCollision2d != sounds.useCollisionExit2dSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle 2D Collision Exit Sound");
                sounds.useCollisionExit2dSound = newCollision2d;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useCollisionExit2dSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.collisionExit2dSound, EventSounds.EventType.OnCollisionExit2D));
            }
        }

        if (!sounds.hideUnused || sounds.useTriggerEnter2dSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useTriggerEnter2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newTrigger2d = EditorGUILayout.Toggle("2D Trigger Enter" + DisabledText, sounds.useTriggerEnter2dSound);
            if (newTrigger2d != sounds.useTriggerEnter2dSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle 2D Trigger Enter Sound");
                sounds.useTriggerEnter2dSound = newTrigger2d;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useTriggerEnter2dSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.triggerEnter2dSound, EventSounds.EventType.OnTriggerEnter2D));
            }
        }

        if (!sounds.hideUnused || sounds.useTriggerExit2dSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useTriggerExit2dSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newTriggerExit2d = EditorGUILayout.Toggle("2D Trigger Exit" + DisabledText, sounds.useTriggerExit2dSound);
            if (newTriggerExit2d != sounds.useTriggerExit2dSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle 2D Trigger Exit Sound");
                sounds.useTriggerExit2dSound = newTriggerExit2d;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useTriggerExit2dSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.triggerExit2dSound, EventSounds.EventType.OnTriggerExit2D));
            }
        }
#endif

        if (!sounds.hideUnused || sounds.useCollisionSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useCollisionSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newCollision = EditorGUILayout.Toggle("Collision Enter" + DisabledText, sounds.useCollisionSound);
            if (newCollision != sounds.useCollisionSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Collision Enter Sound");
                sounds.useCollisionSound = newCollision;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useCollisionSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.collisionSound, EventSounds.EventType.OnCollision));
            }
        }

        if (!sounds.hideUnused || sounds.useCollisionExitSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useCollisionExitSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newCollision = EditorGUILayout.Toggle("Collision Exit" + DisabledText, sounds.useCollisionExitSound);
            if (newCollision != sounds.useCollisionExitSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Collision Exit Sound");
                sounds.useCollisionExitSound = newCollision;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useCollisionExitSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.collisionExitSound, EventSounds.EventType.OnCollisionExit));
            }
        }

        if (!sounds.hideUnused || sounds.useTriggerEnterSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useTriggerEnterSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newTrigger = EditorGUILayout.Toggle("Trigger Enter" + DisabledText, sounds.useTriggerEnterSound);
            if (newTrigger != sounds.useTriggerEnterSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Trigger Enter Sound");
                sounds.useTriggerEnterSound = newTrigger;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useTriggerEnterSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.triggerSound, EventSounds.EventType.OnTriggerEnter));
            }
        }

        if (!sounds.hideUnused || sounds.useTriggerExitSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useTriggerExitSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newTriggerExit = EditorGUILayout.Toggle("Trigger Exit" + DisabledText, sounds.useTriggerExitSound);
            if (newTriggerExit != sounds.useTriggerExitSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Trigger Exit Sound");
                sounds.useTriggerExitSound = newTriggerExit;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useTriggerExitSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.triggerExitSound, EventSounds.EventType.OnTriggerExit));
            }
        }

        if (!sounds.hideUnused || sounds.useParticleCollisionSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useParticleCollisionSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newCollision = EditorGUILayout.Toggle("Particle Collision" + DisabledText, sounds.useParticleCollisionSound);
            if (newCollision != sounds.useParticleCollisionSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Particle Collision Sound");
                sounds.useParticleCollisionSound = newCollision;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useParticleCollisionSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.particleCollisionSound, EventSounds.EventType.OnParticleCollision));
            }
        }

        if (!sounds.hideUnused || sounds.useMouseEnterSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useMouseEnterSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newMouseEnter = EditorGUILayout.Toggle("Mouse Enter" + DisabledText, sounds.useMouseEnterSound);
            if (newMouseEnter != sounds.useMouseEnterSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mouse Enter Sound");
                sounds.useMouseEnterSound = newMouseEnter;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useMouseEnterSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.mouseEnterSound, EventSounds.EventType.OnMouseEnter));
            }
        }

        if (!sounds.hideUnused || sounds.useMouseExitSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useMouseExitSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newMouseEnter = EditorGUILayout.Toggle("Mouse Exit" + DisabledText, sounds.useMouseExitSound);
            if (newMouseEnter != sounds.useMouseExitSound) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mouse Exit Sound");
                sounds.useMouseExitSound = newMouseEnter;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useMouseExitSound && !sounds.disableSounds) {
                changedList.Add(RenderAudioEvent(sounds.mouseExitSound, EventSounds.EventType.OnMouseExit));
            }
        }

        if (!sounds.hideUnused || sounds.useMouseClickSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useMouseClickSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newMouseClick = EditorGUILayout.Toggle("Mouse Down" + DisabledText, sounds.useMouseClickSound);
            if (newMouseClick != sounds.useMouseClickSound) {
                sounds.useMouseClickSound = newMouseClick;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useMouseClickSound && !sounds.disableSounds) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mouse Down Sound");
                changedList.Add(RenderAudioEvent(sounds.mouseClickSound, EventSounds.EventType.OnMouseClick));
            }
        }

        if (!sounds.hideUnused || sounds.useMouseDragSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useMouseDragSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newMouseClick = EditorGUILayout.Toggle("Mouse Drag" + DisabledText, sounds.useMouseDragSound);
            if (newMouseClick != sounds.useMouseDragSound) {
                sounds.useMouseDragSound = newMouseClick;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useMouseDragSound && !sounds.disableSounds) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mouse Drag Sound");
                changedList.Add(RenderAudioEvent(sounds.mouseDragSound, EventSounds.EventType.OnMouseDrag));
            }
        }

        if (!sounds.hideUnused || sounds.useMouseUpSound) {
            EditorGUI.indentLevel = 0;
            GUI.color = sounds.useMouseUpSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newMouseClick = EditorGUILayout.Toggle("Mouse Up" + DisabledText, sounds.useMouseUpSound);
            if (newMouseClick != sounds.useMouseUpSound) {
                sounds.useMouseUpSound = newMouseClick;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (sounds.useMouseUpSound && !sounds.disableSounds) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mouse Up Sound");
                changedList.Add(RenderAudioEvent(sounds.mouseUpSound, EventSounds.EventType.OnMouseUp));
            }
        }

        if (sounds.showNGUI) {
            if (!sounds.hideUnused || sounds.useNguiOnClickSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useNguiOnClickSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newMouseClick = EditorGUILayout.Toggle("NGUI Mouse Click" + DisabledText, sounds.useNguiOnClickSound);
                if (newMouseClick != sounds.useNguiOnClickSound) {
                    sounds.useNguiOnClickSound = newMouseClick;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useNguiOnClickSound && !sounds.disableSounds) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle NGUI Mouse Click Sound");
                    changedList.Add(RenderAudioEvent(sounds.nguiOnClickSound, EventSounds.EventType.NGUIOnClick));
                }
            }

            if (!sounds.hideUnused || sounds.useNguiMouseDownSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useNguiMouseDownSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newMouseClick = EditorGUILayout.Toggle("NGUI Mouse Down" + DisabledText, sounds.useNguiMouseDownSound);
                if (newMouseClick != sounds.useNguiMouseDownSound) {
                    sounds.useNguiMouseDownSound = newMouseClick;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useNguiMouseDownSound && !sounds.disableSounds) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle NGUI Mouse Down Sound");
                    changedList.Add(RenderAudioEvent(sounds.nguiMouseDownSound, EventSounds.EventType.NGUIMouseDown));
                }
            }

            if (!sounds.hideUnused || sounds.useNguiMouseUpSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useNguiMouseUpSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newMouseClick = EditorGUILayout.Toggle("NGUI Mouse UP" + DisabledText, sounds.useNguiMouseUpSound);
                if (newMouseClick != sounds.useNguiMouseUpSound) {
                    sounds.useNguiMouseUpSound = newMouseClick;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useNguiMouseUpSound && !sounds.disableSounds) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle NGUI Mouse UP Sound");
                    changedList.Add(RenderAudioEvent(sounds.nguiMouseUpSound, EventSounds.EventType.NGUIMouseUp));
                }
            }

            if (!sounds.hideUnused || sounds.useNguiMouseEnterSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useNguiMouseEnterSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newMouseClick = EditorGUILayout.Toggle("NGUI Mouse Enter" + DisabledText, sounds.useNguiMouseEnterSound);
                if (newMouseClick != sounds.useNguiMouseEnterSound) {
                    sounds.useNguiMouseEnterSound = newMouseClick;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useNguiMouseEnterSound && !sounds.disableSounds) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle NGUI Mouse Enter Sound");
                    changedList.Add(RenderAudioEvent(sounds.nguiMouseEnterSound, EventSounds.EventType.NGUIMouseEnter));
                }
            }

            if (!sounds.hideUnused || sounds.useNguiMouseExitSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useNguiMouseExitSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newMouseClick = EditorGUILayout.Toggle("NGUI Mouse Exit" + DisabledText, sounds.useNguiMouseExitSound);
                if (newMouseClick != sounds.useNguiMouseExitSound) {
                    sounds.useNguiMouseExitSound = newMouseClick;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useNguiMouseExitSound && !sounds.disableSounds) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle NGUI Mouse Exit Sound");
                    changedList.Add(RenderAudioEvent(sounds.nguiMouseExitSound, EventSounds.EventType.NGUIMouseExit));
                }
            }
        }

        if (sounds.showPoolManager) {
            if (!sounds.hideUnused || sounds.useSpawnedSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useSpawnedSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newSpawned = EditorGUILayout.Toggle("Spawned (Pooling)" + DisabledText, sounds.useSpawnedSound);
                if (newSpawned != sounds.useSpawnedSound) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Spawned Sound");
                    sounds.useSpawnedSound = newSpawned;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useSpawnedSound && !sounds.disableSounds) {
                    changedList.Add(RenderAudioEvent(sounds.spawnedSound, EventSounds.EventType.OnSpawned));
                }
            }

            if (!sounds.hideUnused || sounds.useDespawnedSound) {
                EditorGUI.indentLevel = 0;
                GUI.color = sounds.useDespawnedSound ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newDespawned = EditorGUILayout.Toggle("Despawned (Pooling)" + DisabledText, sounds.useDespawnedSound);
                if (newDespawned != sounds.useDespawnedSound) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Despawned Sound");
                    sounds.useDespawnedSound = newDespawned;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (sounds.useDespawnedSound && !sounds.disableSounds) {
                    changedList.Add(RenderAudioEvent(sounds.despawnedSound, EventSounds.EventType.OnDespawned));
                }
            }
        }

        if (sounds.mechanimStateChangedSounds.Count > 0) {
            EditorGUI.indentLevel = 0;

            for (var i = 0; i < sounds.mechanimStateChangedSounds.Count; i++) {
                var mechEvt = sounds.mechanimStateChangedSounds[i];

                changedList.Add(RenderAudioEvent(mechEvt, EventSounds.EventType.MechanimStateChanged, i));
            }
        }

        if (sounds.userDefinedSounds.Count > 0) {
            EditorGUI.indentLevel = 0;

            for (var i = 0; i < sounds.userDefinedSounds.Count; i++) {
                var customEventGrp = sounds.userDefinedSounds[i];

                changedList.Add(RenderAudioEvent(customEventGrp, EventSounds.EventType.UserDefinedEvent, i));
            }
        }

        if (GUI.changed || isDirty || changedList.Contains(true)) {
            EditorUtility.SetDirty(target);
        }

        //DrawDefaultInspector();
    }

    private bool RenderAudioEvent(AudioEventGroup eventGrp, EventSounds.EventType eType, int? itemIndex = null) {
        int? indexToRemove = null;
        int? indexToInsert = null;
        int? indexToShiftUp = null;
        int? indexToShiftDown = null;
        var hideActions = false;

        if (sounds.disableSounds) {
            hideActions = true;
        }

        if (sounds.useMouseDragSound && eType == EventSounds.EventType.OnMouseUp) {
            var newStopDragSound = (EventSounds.PreviousSoundStopMode)EditorGUILayout.EnumPopup("Mouse Drag Sound End", eventGrp.mouseDragStopMode);
            if (newStopDragSound != eventGrp.mouseDragStopMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mouse Drag Sound End");
                eventGrp.mouseDragStopMode = newStopDragSound;
            }

            if (eventGrp.mouseDragStopMode == EventSounds.PreviousSoundStopMode.FadeOut) {
                EditorGUI.indentLevel = 1;
                var newFade = EditorGUILayout.Slider("Mouse Drag Fade Time", eventGrp.mouseDragFadeOutTime, 0f, 1f);
                if (newFade != eventGrp.mouseDragFadeOutTime) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Mouse Drag Fade Time");
                    eventGrp.mouseDragFadeOutTime = newFade;
                }
            }
        }

        EditorGUI.indentLevel = 0;
        bool showLayerTagFilter = EventSounds.layerTagFilterEvents.Contains(eType.ToString());

        if (showLayerTagFilter) {
            var newUseLayers = EditorGUILayout.BeginToggleGroup("Layer filters", eventGrp.useLayerFilter);
            if (newUseLayers != eventGrp.useLayerFilter) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Layer filters");
                eventGrp.useLayerFilter = newUseLayers;
            }
            if (eventGrp.useLayerFilter) {
                for (var i = 0; i < eventGrp.matchingLayers.Count; i++) {
                    var newLayer = EditorGUILayout.LayerField("Layer Match " + (i + 1), eventGrp.matchingLayers[i]);
                    if (newLayer != eventGrp.matchingLayers[i]) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Layer filter");
                        eventGrp.matchingLayers[i] = newLayer;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(24);

                GUI.contentColor = Color.green;
                if (GUILayout.Button(new GUIContent("Add", "Click to add a layer match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "add Layer filter");
                    eventGrp.matchingLayers.Add(0);
                }
                if (eventGrp.matchingLayers.Count > 1) {
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last layer match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "remove Layer filter");
                        eventGrp.matchingLayers.RemoveAt(eventGrp.matchingLayers.Count - 1);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();

            var newTagFilter = EditorGUILayout.BeginToggleGroup("Tag filter", eventGrp.useTagFilter);
            if (newTagFilter != eventGrp.useTagFilter) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Tag filter");
                eventGrp.useTagFilter = newTagFilter;
            }

            if (eventGrp.useTagFilter) {
                for (var i = 0; i < eventGrp.matchingTags.Count; i++) {
                    var newTag = EditorGUILayout.TagField("Tag Match " + (i + 1), eventGrp.matchingTags[i]);
                    if (newTag != eventGrp.matchingTags[i]) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Tag filter");
                        eventGrp.matchingTags[i] = newTag;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(24);
                GUI.contentColor = Color.green;
                if (GUILayout.Button(new GUIContent("Add", "Click to add a tag match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Add Tag filter");
                    eventGrp.matchingTags.Add("Untagged");
                }
                if (eventGrp.matchingTags.Count > 1) {
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last tag match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "remove Tag filter");
                        eventGrp.matchingTags.RemoveAt(eventGrp.matchingLayers.Count - 1);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
        }

        if (eType == EventSounds.EventType.MechanimStateChanged) {
            GUI.color = eventGrp.mechanimEventActive ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newUse = EditorGUILayout.Toggle("Mechanim State Entered " + DisabledText, eventGrp.mechanimEventActive);
            if (newUse != eventGrp.mechanimEventActive) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Mechanim State Entered active");
                eventGrp.mechanimEventActive = newUse;
            }

            if (!eventGrp.mechanimEventActive) {
                hideActions = true;
            }

            var buttonPressed = DTGUIHelper.AddCustomEventDeleteIcon(false);

            switch (buttonPressed) {
                case DTGUIHelper.DTFunctionButtons.Remove:
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "delete Custom Event Sound");
                    sounds.mechanimStateChangedSounds.RemoveAt(itemIndex.Value);
                    eventGrp.mechanimEventActive = false;
                    break;
            }

            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            if (eventGrp.mechanimEventActive && !hideActions) {
                var newName = EditorGUILayout.TextField("State Name", eventGrp.mechanimStateName);
                if (newName != eventGrp.mechanimStateName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change State Name");
                    eventGrp.mechanimStateName = newName;
                }

                if (!hasMechanim) {
                    DTGUIHelper.ShowRedError("This Game Object does not have an Animator component. Add one or delete this.");
                } else {
                    if (string.IsNullOrEmpty(eventGrp.mechanimStateName)) {
                        DTGUIHelper.ShowRedError("No State Name specified. This event will do nothing.");
                    }
                }
            }
        }

        if (eType == EventSounds.EventType.UserDefinedEvent) {
            GUI.color = eventGrp.customSoundActive ? MasterAudioInspector.activeClr : MasterAudioInspector.inactiveClr;
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            var newUse = EditorGUILayout.Toggle("Custom Event " + DisabledText, eventGrp.customSoundActive);
            if (newUse != eventGrp.customSoundActive) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Custom Event active");
                eventGrp.customSoundActive = newUse;
            }

            var buttonPressed = DTGUIHelper.AddCustomEventDeleteIcon(false);

            switch (buttonPressed) {
                case DTGUIHelper.DTFunctionButtons.Remove:
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "delete Custom Event Sound");
                    sounds.userDefinedSounds.RemoveAt(itemIndex.Value);
                    eventGrp.customSoundActive = false;
                    break;
            }

            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            if (!eventGrp.customSoundActive) {
                return true;
            }

            if (!hideActions) {
                if (maInScene) {
                    var existingIndex = customEventNames.IndexOf(eventGrp.customEventName);

                    int? customEventIndex = null;

                    EditorGUI.indentLevel = 0;

                    var noEvent = false;
                    var noMatch = false;

                    if (existingIndex >= 1) {
                        customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
                        if (existingIndex == 1) {
                            noEvent = true;
                        }
                    } else if (existingIndex == -1 && eventGrp.customEventName == MasterAudio.NO_GROUP_NAME) {
                        customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
                    } else { // non-match
                        noMatch = true;
                        var newEventName = EditorGUILayout.TextField("Custom Event Name", eventGrp.customEventName);
                        if (newEventName != eventGrp.customEventName) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Custom Event Name");
                            eventGrp.customEventName = newEventName;
                        }

                        var newIndex = EditorGUILayout.Popup("All Custom Events", -1, customEventNames.ToArray());
                        if (newIndex >= 0) {
                            customEventIndex = newIndex;
                        }
                    }

                    if (noEvent) {
                        DTGUIHelper.ShowRedError("No Custom Event specified. This section will do nothing.");
                    } else if (noMatch) {
                        DTGUIHelper.ShowRedError("Custom Event found no match. Type in or choose one.");
                    }

                    if (customEventIndex.HasValue) {
                        if (existingIndex != customEventIndex.Value) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Custom Event");
                        }
                        if (customEventIndex.Value == -1) {
                            eventGrp.customEventName = MasterAudio.NO_GROUP_NAME;
                        } else {
                            eventGrp.customEventName = customEventNames[customEventIndex.Value];
                        }
                    }
                } else {
                    var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", eventGrp.customEventName);
                    if (newCustomEvent != eventGrp.customEventName) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Custom Event Name");
                        eventGrp.customEventName = newCustomEvent;
                    }
                }
            }
        }

        if (eventGrp.SoundEvents.Count == 0) {
            eventGrp.SoundEvents.Add(new AudioEvent());
        }

        if (!hideActions) {
            for (var j = 0; j < eventGrp.SoundEvents.Count; j++) {
                AudioEvent aEvent = eventGrp.SoundEvents[j];

                var newRetrigger = (EventSounds.RetriggerLimMode)EditorGUILayout.EnumPopup("Retrigger Limit Mode", eventGrp.retriggerLimitMode);
                if (newRetrigger != eventGrp.retriggerLimitMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Retrigger Limit Mode");
                    eventGrp.retriggerLimitMode = newRetrigger;
                }

                EditorGUI.indentLevel = 1;
                switch (eventGrp.retriggerLimitMode) {
                    case EventSounds.RetriggerLimMode.FrameBased:
                        var newFrm = EditorGUILayout.IntSlider("Min Frames Between", eventGrp.limitPerXFrm, 0, 10000);
                        if (newFrm != eventGrp.limitPerXFrm) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Min Frames Between");
                            eventGrp.limitPerXFrm = newFrm;
                        }
                        break;
                    case EventSounds.RetriggerLimMode.TimeBased:
                        var newSec = EditorGUILayout.Slider("Min Seconds Between", eventGrp.limitPerXSec, 0f, 10000f);
                        if (newSec != eventGrp.limitPerXSec) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Min Seconds Between");
                            eventGrp.limitPerXSec = newSec;
                        }
                        break;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                var newExpanded = DTGUIHelper.Foldout(aEvent.isExpanded, "Action #" + (j + 1));
                if (newExpanded != aEvent.isExpanded) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle expand Action");
                    aEvent.isExpanded = newExpanded;
                }

                var newActionName = GUILayout.TextField(aEvent.actionName, GUILayout.Width(200));
                if (newActionName != aEvent.actionName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Rename action");
                    aEvent.actionName = newActionName;
                }

                var buttonPressed = DTGUIHelper.AddFoldOutListItemButtons(j, eventGrp.SoundEvents.Count, "Action", true, true, false);
                EditorGUILayout.EndHorizontal();

                if (aEvent.isExpanded) {
                    EditorGUI.indentLevel = 0;

                    if (eType == EventSounds.EventType.OnEnable) {
                        DTGUIHelper.ShowColorWarning("*If this prefab is in the scene at startup, use Start event instead.");
                    }

                    var newSoundType = (MasterAudio.EventSoundFunctionType)EditorGUILayout.EnumPopup("Action Type", aEvent.currentSoundFunctionType);
                    if (newSoundType != aEvent.currentSoundFunctionType) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Action Type");
                        aEvent.currentSoundFunctionType = newSoundType;
                    }

                    switch (aEvent.currentSoundFunctionType) {
                        case MasterAudio.EventSoundFunctionType.PlaySound:
                            if (maInScene) {
                                var existingIndex = groupNames.IndexOf(aEvent.soundType);

                                int? groupIndex = null;

                                EditorGUI.indentLevel = 1;

                                var noGroup = false;
                                var noMatch = false;

                                if (existingIndex >= 1) {
                                    groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
                                    if (existingIndex == 1) {
                                        noGroup = true;
                                    }
                                } else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
                                    groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
                                } else { // non-match
                                    noMatch = true;
                                    var newSound = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
                                    if (newSound != aEvent.soundType) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Group");
                                        aEvent.soundType = newSound;
                                    }

                                    var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
                                    if (newIndex >= 0) {
                                        groupIndex = newIndex;
                                    }
                                }

                                if (noGroup) {
                                    DTGUIHelper.ShowRedError("No Sound Group specified. Action will do nothing.");
                                } else if (noMatch) {
                                    DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");
                                }

                                if (groupIndex.HasValue) {
                                    if (existingIndex != groupIndex.Value) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Group");
                                    }
                                    if (groupIndex.Value == -1) {
                                        aEvent.soundType = MasterAudio.NO_GROUP_NAME;
                                    } else {
                                        aEvent.soundType = groupNames[groupIndex.Value];
                                    }
                                }
                            } else {
                                var newSType = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
                                if (newSType != aEvent.soundType) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Group");
                                    aEvent.soundType = newSType;
                                }
                            }

                            var newVarType = (EventSounds.VariationType)EditorGUILayout.EnumPopup("Variation Mode", aEvent.variationType);
                            if (newVarType != aEvent.variationType) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Variation Mode");
                                aEvent.variationType = newVarType;
                            }

                            if (aEvent.variationType == EventSounds.VariationType.PlaySpecific) {
                                var newVarName = EditorGUILayout.TextField("Variation Name", aEvent.variationName);
                                if (newVarName != aEvent.variationName) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Variation Name");
                                    aEvent.variationName = newVarName;
                                }

                                if (string.IsNullOrEmpty(aEvent.variationName)) {
                                    DTGUIHelper.ShowRedError("Variation Name is empty. No sound will play.");
                                }
                            }

                            var newVol = EditorGUILayout.Slider("Volume", aEvent.volume, 0f, 1f);
                            if (newVol != aEvent.volume) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Volume");
                                aEvent.volume = newVol;
                            }

                            var newFixedPitch = EditorGUILayout.Toggle("Override pitch?", aEvent.useFixedPitch);
                            if (newFixedPitch != aEvent.useFixedPitch) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Override pitch");
                                aEvent.useFixedPitch = newFixedPitch;
                            }
                            if (aEvent.useFixedPitch) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Pitch");
                                aEvent.pitch = EditorGUILayout.Slider("Pitch", aEvent.pitch, -3f, 3f);
                            }

                            var newDelay = EditorGUILayout.Slider("Delay Sound (sec)", aEvent.delaySound, 0f, 10f);
                            if (newDelay != aEvent.delaySound) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Delay Sound");
                                aEvent.delaySound = newDelay;
                            }
                            break;
                        case MasterAudio.EventSoundFunctionType.PlaylistControl:
                            EditorGUI.indentLevel = 1;
                            var newPlaylistCmd = (MasterAudio.PlaylistCommand)EditorGUILayout.EnumPopup("Playlist Command", aEvent.currentPlaylistCommand);
                            if (newPlaylistCmd != aEvent.currentPlaylistCommand) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Command");
                                aEvent.currentPlaylistCommand = newPlaylistCmd;
                            }

                            if (aEvent.currentPlaylistCommand != MasterAudio.PlaylistCommand.None) {
                                // show Playlist Controller dropdown
                                if (EventSounds.playlistCommandsWithAll.Contains(aEvent.currentPlaylistCommand)) {
                                    var newAllControllers = EditorGUILayout.Toggle("All Playlist Controllers?", aEvent.allPlaylistControllersForGroupCmd);
                                    if (newAllControllers != aEvent.allPlaylistControllersForGroupCmd) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle All Playlist Controllers");
                                        aEvent.allPlaylistControllersForGroupCmd = newAllControllers;
                                    }
                                }

                                if (!aEvent.allPlaylistControllersForGroupCmd) {
                                    if (playlistControllerNames.Count > 0) {
                                        var existingIndex = playlistControllerNames.IndexOf(aEvent.playlistControllerName);

                                        int? playlistControllerIndex = null;

                                        var noPC = false;
                                        var noMatch = false;

                                        if (existingIndex >= 1) {
                                            playlistControllerIndex = EditorGUILayout.Popup("Playlist Controller", existingIndex, playlistControllerNames.ToArray());
                                            if (existingIndex == 1) {
                                                noPC = true;
                                            }
                                        } else if (existingIndex == -1 && aEvent.playlistControllerName == MasterAudio.NO_GROUP_NAME) {
                                            playlistControllerIndex = EditorGUILayout.Popup("Playlist Controller", existingIndex, playlistControllerNames.ToArray());
                                        } else { // non-match
                                            noMatch = true;

                                            var newPlaylistController = EditorGUILayout.TextField("Playlist Controller", aEvent.playlistControllerName);
                                            if (newPlaylistController != aEvent.playlistControllerName) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Controller");
                                                aEvent.playlistControllerName = newPlaylistController;
                                            }
                                            var newIndex = EditorGUILayout.Popup("All Playlist Controllers", -1, playlistControllerNames.ToArray());
                                            if (newIndex >= 0) {
                                                playlistControllerIndex = newIndex;
                                            }
                                        }

                                        if (noPC) {
                                            DTGUIHelper.ShowRedError("No Playlist Controller specified. Action will do nothing.");
                                        } else if (noMatch) {
                                            DTGUIHelper.ShowRedError("Playlist Controller found no match. Type in or choose one.");
                                        }

                                        if (playlistControllerIndex.HasValue) {
                                            if (existingIndex != playlistControllerIndex.Value) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Controller");
                                            }
                                            if (playlistControllerIndex.Value == -1) {
                                                aEvent.playlistControllerName = MasterAudio.NO_GROUP_NAME;
                                            } else {
                                                aEvent.playlistControllerName = playlistControllerNames[playlistControllerIndex.Value];
                                            }
                                        }
                                    } else {
                                        var newPlaylistControllerName = EditorGUILayout.TextField("Playlist Controller", aEvent.playlistControllerName);
                                        if (newPlaylistControllerName != aEvent.playlistControllerName) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Controller");
                                            aEvent.playlistControllerName = newPlaylistControllerName;
                                        }
                                    }
                                }
                            }

                            switch (aEvent.currentPlaylistCommand) {
                                case MasterAudio.PlaylistCommand.None:
                                    DTGUIHelper.ShowRedError("You have no command selected. Action will do nothing.");
                                    break;
                                case MasterAudio.PlaylistCommand.ChangePlaylist:
                                    // show playlist name dropdown
                                    if (maInScene) {
                                        var existingIndex = playlistNames.IndexOf(aEvent.playlistName);

                                        int? playlistIndex = null;

                                        var noPl = false;
                                        var noMatch = false;

                                        if (existingIndex >= 1) {
                                            playlistIndex = EditorGUILayout.Popup("Playlist Name", existingIndex, playlistNames.ToArray());
                                            if (existingIndex == 1) {
                                                noPl = true;
                                            }
                                        } else if (existingIndex == -1 && aEvent.playlistName == MasterAudio.NO_GROUP_NAME) {
                                            playlistIndex = EditorGUILayout.Popup("Playlist Name", existingIndex, playlistNames.ToArray());
                                        } else { // non-match
                                            noMatch = true;

                                            var newPlaylist = EditorGUILayout.TextField("Playlist Name", aEvent.playlistName);
                                            if (newPlaylist != aEvent.playlistName) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Name");
                                                aEvent.playlistName = newPlaylist;
                                            }
                                            var newIndex = EditorGUILayout.Popup("All Playlists", -1, playlistNames.ToArray());
                                            if (newIndex >= 0) {
                                                playlistIndex = newIndex;
                                            }
                                        }

                                        if (noPl) {
                                            DTGUIHelper.ShowRedError("No Playlist Name specified. Action will do nothing.");
                                        } else if (noMatch) {
                                            DTGUIHelper.ShowRedError("Playlist Name found no match. Type in or choose one.");
                                        }

                                        if (playlistIndex.HasValue) {
                                            if (existingIndex != playlistIndex.Value) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Name");
                                            }
                                            if (playlistIndex.Value == -1) {
                                                aEvent.playlistName = MasterAudio.NO_GROUP_NAME;
                                            } else {
                                                aEvent.playlistName = playlistNames[playlistIndex.Value];
                                            }
                                        }
                                    } else {
                                        var newPlaylistName = EditorGUILayout.TextField("Playlist Name", aEvent.playlistName);
                                        if (newPlaylistName != aEvent.playlistName) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Playlist Name");
                                            aEvent.playlistName = newPlaylistName;
                                        }
                                    }

                                    var newStartPlaylist = EditorGUILayout.Toggle("Start Playlist?", aEvent.startPlaylist);
                                    if (newStartPlaylist != aEvent.startPlaylist) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Start Playlist");
                                        aEvent.startPlaylist = newStartPlaylist;
                                    }
                                    break;
                                case MasterAudio.PlaylistCommand.FadeToVolume:
                                    var newFadeVol = EditorGUILayout.Slider("Target Volume", aEvent.fadeVolume, 0f, 1f);
                                    if (newFadeVol != aEvent.fadeVolume) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Target Volume");
                                        aEvent.fadeVolume = newFadeVol;
                                    }

                                    var newFadeTime = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
                                    if (newFadeTime != aEvent.fadeTime) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Fade Time");
                                        aEvent.fadeTime = newFadeTime;
                                    }
                                    break;
                                case MasterAudio.PlaylistCommand.PlayClip:
                                    var newClip = EditorGUILayout.TextField("Clip Name", aEvent.clipName);
                                    if (newClip != aEvent.clipName) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Clip Name");
                                        aEvent.clipName = newClip;
                                    }
                                    if (string.IsNullOrEmpty(aEvent.clipName)) {
                                        DTGUIHelper.ShowRedError("Clip name is empty. Action will do nothing.");
                                    }
                                    break;
                            }
                            break;
                        case MasterAudio.EventSoundFunctionType.GroupControl:
                            EditorGUI.indentLevel = 1;

                            var newGroupCmd = (MasterAudio.SoundGroupCommand)EditorGUILayout.EnumPopup("Group Command", aEvent.currentSoundGroupCommand);
                            if (newGroupCmd != aEvent.currentSoundGroupCommand) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Group Command");
                                aEvent.currentSoundGroupCommand = newGroupCmd;
                            }

                            if (!MasterAudio.GroupCommandsWithNoGroupSelector.Contains(aEvent.currentSoundGroupCommand)) {
                                if (!MasterAudio.GroupCommandsWithNoAllGroupSelector.Contains(aEvent.currentSoundGroupCommand)) {
                                    var newAllTypes = EditorGUILayout.Toggle("Do For Every Group?", aEvent.allSoundTypesForGroupCmd);
                                    if (newAllTypes != aEvent.allSoundTypesForGroupCmd) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Do For Every Group?");
                                        aEvent.allSoundTypesForGroupCmd = newAllTypes;
                                    }
                                }

                                if (!aEvent.allSoundTypesForGroupCmd) {
                                    if (maInScene) {
                                        var existingIndex = groupNames.IndexOf(aEvent.soundType);

                                        int? groupIndex = null;

                                        var noGroup = false;
                                        var noMatch = false;

                                        if (existingIndex >= 1) {
                                            groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
                                            if (existingIndex == 1) {
                                                noGroup = true;
                                            }

                                        } else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
                                            groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, groupNames.ToArray());
                                        } else { // non-match
                                            noMatch = true;

                                            var newSType = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
                                            if (newSType != aEvent.soundType) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Group");
                                                aEvent.soundType = newSType;
                                            }
                                            var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
                                            if (newIndex >= 0) {
                                                groupIndex = newIndex;
                                            }
                                        }

                                        if (noMatch) {
                                            DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");
                                        } else if (noGroup) {
                                            DTGUIHelper.ShowRedError("No Sound Group specified. Action will do nothing.");
                                        }

                                        if (groupIndex.HasValue) {
                                            if (existingIndex != groupIndex.Value) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Group");
                                            }
                                            if (groupIndex.Value == -1) {
                                                aEvent.soundType = MasterAudio.NO_GROUP_NAME;
                                            } else {
                                                aEvent.soundType = groupNames[groupIndex.Value];
                                            }
                                        }
                                    } else {
                                        var newSoundT = EditorGUILayout.TextField("Sound Group", aEvent.soundType);
                                        if (newSoundT != aEvent.soundType) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Sound Group");
                                            aEvent.soundType = newSoundT;
                                        }
                                    }
                                }
                            }

                            switch (aEvent.currentSoundGroupCommand) {
                                case MasterAudio.SoundGroupCommand.None:
                                    DTGUIHelper.ShowRedError("You have no command selected. Action will do nothing.");
                                    break;
                                case MasterAudio.SoundGroupCommand.FadeToVolume:
                                    var newFadeVol = EditorGUILayout.Slider("Target Volume", aEvent.fadeVolume, 0f, 1f);
                                    if (newFadeVol != aEvent.fadeVolume) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Target Volume");
                                        aEvent.fadeVolume = newFadeVol;
                                    }

                                    var newFadeTime = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
                                    if (newFadeTime != aEvent.fadeTime) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Fade Time");
                                        aEvent.fadeTime = newFadeTime;
                                    }
                                    break;
                                case MasterAudio.SoundGroupCommand.FadeOutAllOfSound:
                                    var newFadeT = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
                                    if (newFadeT != aEvent.fadeTime) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Fade Time");
                                        aEvent.fadeTime = newFadeT;
                                    }
                                    break;
                                case MasterAudio.SoundGroupCommand.FadeOutSoundGroupOfTransform:
                                    var newFade = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
                                    if (newFade != aEvent.fadeTime) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Fade Time");
                                        aEvent.fadeTime = newFade;
                                    }
                                    break;
                                case MasterAudio.SoundGroupCommand.Mute:
                                    break;
                                case MasterAudio.SoundGroupCommand.Pause:
                                    break;
                                case MasterAudio.SoundGroupCommand.Solo:
                                    break;
                                case MasterAudio.SoundGroupCommand.Unmute:
                                    break;
                                case MasterAudio.SoundGroupCommand.Unpause:
                                    break;
                                case MasterAudio.SoundGroupCommand.Unsolo:
                                    break;
                            }

                            break;
                        case MasterAudio.EventSoundFunctionType.BusControl:
                            EditorGUI.indentLevel = 1;
                            var newBusCmd = (MasterAudio.BusCommand)EditorGUILayout.EnumPopup("Bus Command", aEvent.currentBusCommand);
                            if (newBusCmd != aEvent.currentBusCommand) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Bus Command");
                                aEvent.currentBusCommand = newBusCmd;
                            }

                            if (aEvent.currentBusCommand != MasterAudio.BusCommand.None) {
                                var newAllTypes = EditorGUILayout.Toggle("Do For Every Bus?", aEvent.allSoundTypesForBusCmd);
                                if (newAllTypes != aEvent.allSoundTypesForBusCmd) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Do For Every Bus?");
                                    aEvent.allSoundTypesForBusCmd = newAllTypes;
                                }

                                if (!aEvent.allSoundTypesForBusCmd) {
                                    if (maInScene) {
                                        var existingIndex = busNames.IndexOf(aEvent.busName);

                                        int? busIndex = null;

                                        var noBus = false;
                                        var noMatch = false;

                                        if (existingIndex >= 1) {
                                            busIndex = EditorGUILayout.Popup("Bus Name", existingIndex, busNames.ToArray());
                                            if (existingIndex == 1) {
                                                noBus = true;
                                            }
                                        } else if (existingIndex == -1 && aEvent.busName == MasterAudio.NO_GROUP_NAME) {
                                            busIndex = EditorGUILayout.Popup("Bus Name", existingIndex, busNames.ToArray());
                                        } else { // non-match
                                            var newBusName = EditorGUILayout.TextField("Bus Name", aEvent.busName);
                                            if (newBusName != aEvent.busName) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Bus Name");
                                                aEvent.busName = newBusName;
                                            }

                                            var newIndex = EditorGUILayout.Popup("All Buses", -1, busNames.ToArray());
                                            if (newIndex >= 0) {
                                                busIndex = newIndex;
                                            }
                                            noMatch = true;
                                        }

                                        if (noBus) {
                                            DTGUIHelper.ShowRedError("No Bus Name specified. Action will do nothing.");
                                        } else if (noMatch) {
                                            DTGUIHelper.ShowRedError("Bus Name found no match. Type in or choose one.");
                                        }

                                        if (busIndex.HasValue) {
                                            if (existingIndex != busIndex.Value) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Bus");
                                            }
                                            if (busIndex.Value == -1) {
                                                aEvent.busName = MasterAudio.NO_GROUP_NAME;
                                            } else {
                                                aEvent.busName = busNames[busIndex.Value];
                                            }
                                        }
                                    } else {
                                        var newBusName = EditorGUILayout.TextField("Bus Name", aEvent.busName);
                                        if (newBusName != aEvent.busName) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Bus Name");
                                            aEvent.busName = newBusName;
                                        }
                                    }
                                }
                            }

                            switch (aEvent.currentBusCommand) {
                                case MasterAudio.BusCommand.None:
                                    DTGUIHelper.ShowRedError("You have no command selected. Action will do nothing.");
                                    break;
                                case MasterAudio.BusCommand.FadeToVolume:
                                    var newFadeVol = EditorGUILayout.Slider("Target Volume", aEvent.fadeVolume, 0f, 1f);
                                    if (newFadeVol != aEvent.fadeVolume) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Target Volume");
                                        aEvent.fadeVolume = newFadeVol;
                                    }

                                    var newFadeTime = EditorGUILayout.Slider("Fade Time", aEvent.fadeTime, 0f, 10f);
                                    if (newFadeTime != aEvent.fadeTime) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Fade Time");
                                        aEvent.fadeTime = newFadeTime;
                                    }
                                    break;
                                case MasterAudio.BusCommand.Pause:
                                    break;
                                case MasterAudio.BusCommand.Unpause:
                                    break;
                            }

                            break;
                        case MasterAudio.EventSoundFunctionType.CustomEventControl:
                            if (eType == EventSounds.EventType.UserDefinedEvent) {
                                DTGUIHelper.ShowRedError("Custom Event Receivers cannot fire events. Select another Action Type.");
                                break;
                            }

                            EditorGUI.indentLevel = 1;
                            var newEventCmd = (MasterAudio.CustomEventCommand)EditorGUILayout.EnumPopup("Custom Event Cmd", aEvent.currentCustomEventCommand);
                            if (newEventCmd != aEvent.currentCustomEventCommand) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Custom Event Command");
                                aEvent.currentCustomEventCommand = newEventCmd;
                            }

                            switch (aEvent.currentCustomEventCommand) {
                                case MasterAudio.CustomEventCommand.None:
                                    DTGUIHelper.ShowRedError("You have no command selected. Action will do nothing.");
                                    break;
                                case MasterAudio.CustomEventCommand.FireEvent:
                                    if (maInScene) {
                                        var existingIndex = customEventNames.IndexOf(aEvent.theCustomEventName);

                                        int? customEventIndex = null;

                                        EditorGUI.indentLevel = 1;

                                        var noEvent = false;
                                        var noMatch = false;

                                        if (existingIndex >= 1) {
                                            customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
                                            if (existingIndex == 1) {
                                                noEvent = true;
                                            }
                                        } else if (existingIndex == -1 && aEvent.soundType == MasterAudio.NO_GROUP_NAME) {
                                            customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, customEventNames.ToArray());
                                        } else { // non-match
                                            noMatch = true;
                                            var newEventName = EditorGUILayout.TextField("Custom Event Name", aEvent.theCustomEventName);
                                            if (newEventName != aEvent.theCustomEventName) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Custom Event Name");
                                                aEvent.theCustomEventName = newEventName;
                                            }

                                            var newIndex = EditorGUILayout.Popup("All Custom Events", -1, customEventNames.ToArray());
                                            if (newIndex >= 0) {
                                                customEventIndex = newIndex;
                                            }
                                        }

                                        if (noEvent) {
                                            DTGUIHelper.ShowRedError("No Custom Event specified. This section will do nothing.");
                                        } else if (noMatch) {
                                            DTGUIHelper.ShowRedError("Custom Event found no match. Type in or choose one.");
                                        }

                                        if (customEventIndex.HasValue) {
                                            if (existingIndex != customEventIndex.Value) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Custom Event");
                                            }
                                            if (customEventIndex.Value == -1) {
                                                aEvent.theCustomEventName = MasterAudio.NO_GROUP_NAME;
                                            } else {
                                                aEvent.theCustomEventName = customEventNames[customEventIndex.Value];
                                            }
                                        }
                                    } else {
                                        var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", aEvent.theCustomEventName);
                                        if (newCustomEvent != aEvent.theCustomEventName) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Custom Event Name");
                                            aEvent.theCustomEventName = newCustomEvent;
                                        }
                                    }

                                    break;
                            }

                            break;
                        case MasterAudio.EventSoundFunctionType.GlobalControl:
                            EditorGUI.indentLevel = 1;
                            var newCmd = (MasterAudio.GlobalCommand)EditorGUILayout.EnumPopup("Global Cmd", aEvent.currentGlobalCommand);
                            if (newCmd != aEvent.currentGlobalCommand) {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Global Command");
                                aEvent.currentGlobalCommand = newCmd;
                            }

                            if (aEvent.currentGlobalCommand == MasterAudio.GlobalCommand.None) {
                                DTGUIHelper.ShowRedError("You have no command selected. Action will do nothing.");
                            }
                            break;
                    }

                    EditorGUI.indentLevel = 0;

                    var newEmit = EditorGUILayout.Toggle("Emit Particle", aEvent.emitParticles);
                    if (newEmit != aEvent.emitParticles) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "toggle Emit Particle");
                        aEvent.emitParticles = newEmit;
                    }
                    if (aEvent.emitParticles) {
                        var newParticleCount = EditorGUILayout.IntSlider("Particle Count", aEvent.particleCountToEmit, 1, 100);
                        if (newParticleCount != aEvent.particleCountToEmit) {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "change Particle Count");
                            aEvent.particleCountToEmit = newParticleCount;
                        }
                    }
                }

                switch (buttonPressed) {
                    case DTGUIHelper.DTFunctionButtons.Add:
                        indexToInsert = j + 1;
                        break;
                    case DTGUIHelper.DTFunctionButtons.Remove:
                        indexToRemove = j;
                        break;
                    case DTGUIHelper.DTFunctionButtons.ShiftUp:
                        indexToShiftUp = j;
                        break;
                    case DTGUIHelper.DTFunctionButtons.ShiftDown:
                        indexToShiftDown = j;
                        break;
                }
            }
        }

        AudioEvent item = null;

        if (indexToInsert.HasValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Add action");
            eventGrp.SoundEvents.Insert(indexToInsert.Value, new AudioEvent());
        } else if (indexToRemove.HasValue) {
            if (eventGrp.SoundEvents.Count <= 1) {
                DTGUIHelper.ShowAlert("You cannot delete the last Action. Disable this event if you don't need it.");
            } else {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Delete action");
                eventGrp.SoundEvents.RemoveAt(indexToRemove.Value);
            }
        } else if (indexToShiftUp.HasValue) {
            item = eventGrp.SoundEvents[indexToShiftUp.Value];

            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Shift up event action");

            eventGrp.SoundEvents.Insert(indexToShiftUp.Value - 1, item);
            eventGrp.SoundEvents.RemoveAt(indexToShiftUp.Value + 1);
        } else if (indexToShiftDown.HasValue) {
            var index = indexToShiftDown.Value + 1;
            item = eventGrp.SoundEvents[index];

            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "Shift down event action");

            eventGrp.SoundEvents.Insert(index - 1, item);
            eventGrp.SoundEvents.RemoveAt(index + 1);
        }

        return isDirty;
    }

    private void CreateMechanimStateEntered(bool recordUndo) {
        var newEvent = new AudioEvent();

        if (recordUndo) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "add Mechanim State Entered Sound");
        }

        var newGrp = new AudioEventGroup();
        newGrp.isMechanimStateCheckEvent = true;
        newGrp.mechanimEventActive = true;

        newGrp.SoundEvents.Add(newEvent);
        sounds.mechanimStateChangedSounds.Add(newGrp);
    }

    private void CreateCustomEvent(bool recordUndo) {
        var newEvent = new AudioEvent();

        if (recordUndo) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, sounds, "add Custom Event Sound");
        }

        var newGrp = new AudioEventGroup();
        newGrp.isCustomEvent = true;
        newGrp.customSoundActive = true;

        newGrp.SoundEvents.Add(newEvent);
        sounds.userDefinedSounds.Add(newGrp);
    }

    private void AddEventIfZero(AudioEventGroup grp) {
        if (grp.SoundEvents.Count == 0) {
            grp.SoundEvents.Add(new AudioEvent());
        }
    }

    private string DisabledText {
        get {
            var disabledText = "";
            if (sounds.disableSounds) {
                disabledText = " (DISABLED) ";
            }

            return disabledText;
        }
    }
}
