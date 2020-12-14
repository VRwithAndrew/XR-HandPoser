using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoseContainer))]
public class PoseContainerEditor : Editor
{
    private PoseContainer poseContainer = null;

    private void OnEnable()
    {
        poseContainer = (PoseContainer)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Open Pose Editor"))
            PoseWindow.Open(poseContainer.pose);
    }
}
