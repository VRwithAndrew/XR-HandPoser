using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoseWindow : EditorWindow
{
    // The pose we're editing
    private Pose activePose = null;

    // Root object
    private GameObject poseHelper = null;

    // Functionality
    private HandManager handManager = null;
    private SelectionHandler selectionHandler = null;

    private void OnEnable()
    {
        CreatePoseHelper();
        Selection.selectionChanged += UpdateSelection;
        EditorApplication.playModeStateChanged += CloseWindow;
        EditorSceneManager.sceneClosing += CloseWindow;
    }

    private void OnDisable()
    {
        DestroyPoseHelper();
        Selection.selectionChanged -= UpdateSelection;
        EditorApplication.playModeStateChanged -= CloseWindow;
        EditorSceneManager.sceneClosing -= CloseWindow;
    }

    void CreatePoseHelper()
    {
        if (!poseHelper)
        {
            // Get the manager from resources
            Object helperPrefab = Resources.Load("PoseHelper");

            // Instantiate it into the scene, mark as not to save
            poseHelper = (GameObject)PrefabUtility.InstantiatePrefab(helperPrefab);
            poseHelper.hideFlags = HideFlags.DontSave;

            // Get functionality
            selectionHandler = poseHelper.GetComponent<SelectionHandler>();
            handManager = poseHelper.GetComponent<HandManager>();

            // Set initial selection setup
            UpdateSelection();
        }
    }

    private void DestroyPoseHelper()
    {
        DestroyImmediate(poseHelper);
    }

    private void CloseWindow(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.ExitingEditMode)
            Close();
    }

    private void CloseWindow(Scene scene, bool removingScene)
    {
        if (removingScene)
            Close();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUIStyle labelStyle = EditorStyles.label;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        string poseName = activePose ? activePose.name : "No Pose";
        GUILayout.Label(poseName, labelStyle);

        using (new EditorGUI.DisabledScope(activePose))
        {
            if (GUILayout.Button("Create Pose"))
                CreatePose();

            if (GUILayout.Button("Refresh Pose"))
                RefreshPose();
        }

        using (new EditorGUI.DisabledScope(!activePose))
        {
            if (GUILayout.Button("Clear Pose"))
                ClearPose();
        }

        using (new EditorGUI.DisabledScope(!handManager.HandsExist))
        {
            PreviewHand leftHand = handManager.LeftHand;
            PreviewHand rightHand = handManager.RightHand;
            float objectWidth = EditorGUIUtility.currentViewWidth * 0.5f;

            // Hand labels
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Left Hand", labelStyle, GUILayout.Width(objectWidth));
                GUILayout.Label("Right Hand", labelStyle, GUILayout.Width(objectWidth));
            }

            // Toggle buttons
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Toggle", GUILayout.Width(objectWidth)))
                    ToggleHand(leftHand);

                if (GUILayout.Button("Toggle", GUILayout.Width(objectWidth)))
                    ToggleHand(rightHand);
            }

            // Buttons that require a pose
            using (new EditorGUI.DisabledScope(!activePose))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Mirror L > R", GUILayout.Width(objectWidth)))
                        MirrorPose(leftHand, rightHand);

                    if (GUILayout.Button("Mirror R > L", GUILayout.Width(objectWidth)))
                        MirrorPose(rightHand, leftHand);
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Undo Changes", GUILayout.Width(objectWidth)))
                        UndoChanges(leftHand);

                    if (GUILayout.Button("Undo Changes", GUILayout.Width(objectWidth)))
                        UndoChanges(rightHand);
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Reset", GUILayout.Width(objectWidth)))
                        ResetPose(leftHand);

                    if (GUILayout.Button("Reset", GUILayout.Width(objectWidth)))
                        ResetPose(rightHand);
                }
            }
        }

        using (new EditorGUI.DisabledScope(!activePose))
        {
            GUILayout.Label("Remember to Save!", labelStyle);

            if (GUILayout.Button("Save Pose"))
                handManager.SavePose(activePose);
        }
    }

    private void CreatePose()
    {
        // Create the new asset
        activePose = CreatePoseAsset();

        // Give the pose to the object we have selected
        GameObject targetObject = selectionHandler.SetObjectPose(activePose);

        // Update the hands
        handManager.UpdateHands(activePose, targetObject.transform);
    }

    Pose CreatePoseAsset()
    {
        // Create new scriptable object
        Pose pose = CreateInstance<Pose>();

        // Store the asset
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewPoseData.asset");
        AssetDatabase.CreateAsset(pose, path);

        return pose;
    }

    private void UpdateSelection()
    {
        if (selectionHandler.CheckForNewInteractable())
            UpdateActivePose(Selection.activeGameObject);
    }

    private void RefreshPose()
    {
        // Get the object we have selected
        GameObject currentObject = selectionHandler.CurretInteractable.gameObject;
        UpdateActivePose(currentObject);
    }

    private void UpdateActivePose(GameObject targetObject)
    {
        // Get pose from container, update hands
        activePose = selectionHandler.TryGetPose(targetObject);

        // If we have a pose, update the hands
        if(activePose)
            handManager.UpdateHands(activePose, targetObject.transform);
    }

    private void ClearPose()
    {
        selectionHandler.SetObjectPose(null);
        activePose = null;
    }

    private void ToggleHand(PreviewHand hand)
    {
        Undo.RecordObject(hand.gameObject, "Toggle Hand");
        bool isActive = !hand.gameObject.activeSelf;
        hand.gameObject.SetActive(isActive);
    }

    private void ResetPose(PreviewHand hand)
    {
        Undo.RecordObject(hand.transform, "Reset Pose");
        hand.ApplyDefaultPose();
    }

    private void UndoChanges(PreviewHand hand)
    {
        Undo.RecordObject(hand.transform, "Undo Changes");
        hand.ApplyPose(activePose);
    }

    private void MirrorPose(PreviewHand sourceHand, PreviewHand targetHand)
    {
        Undo.RecordObject(targetHand.transform, "Mirror Pose");
        targetHand.MirrorAndApplyPose(sourceHand);
    }

    public static void Open(Pose pose)
    {
        PoseWindow window = GetWindow<PoseWindow>("Hand Poser");
        window.activePose = pose;
    }
}
