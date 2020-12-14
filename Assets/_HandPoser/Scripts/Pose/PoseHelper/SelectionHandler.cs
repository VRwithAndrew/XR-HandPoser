using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class SelectionHandler : MonoBehaviour
{
    public XRBaseInteractable CurretInteractable { get; private set; } = null;

    public bool CheckForNewInteractable()
    {
        // First see if we have an interactable to use
        XRBaseInteractable newInteractable = GetInteractable();

        // Update if different
        bool isDifferent = IsDifferentInteractable(CurretInteractable, newInteractable);
        CurretInteractable = isDifferent ? newInteractable : CurretInteractable;

        return isDifferent;
    }

    private XRBaseInteractable GetInteractable()
    {
        // Set up the stuff now
        XRBaseInteractable newInteractable = null;
        GameObject selectedObject = null;

        #if UNITY_EDITOR
        selectedObject = Selection.activeGameObject;
        #endif

        // If we have an object selected
        if (selectedObject)
        {
            // Does it have an interactable component
            if (selectedObject.TryGetComponent(out XRBaseInteractable interactable))
                newInteractable = interactable;
        }

        return newInteractable;
    }

    private bool IsDifferentInteractable(XRBaseInteractable currentInteractable, XRBaseInteractable newInteractable)
    {
        // Assume it's the same
        bool isDifferent = false;

        // If we're selecting on object for the first time, it's true
        if (!currentInteractable)
            isDifferent = true;

        // If we have a stored object, and we select a new one
        if (currentInteractable && newInteractable)
            isDifferent = currentInteractable != newInteractable;

        return isDifferent;
    }

    public GameObject SetObjectPose(Pose pose)
    {
        GameObject selectedObject = null;

        #if UNITY_EDITOR
        selectedObject = Selection.activeGameObject;
        #endif

        if (selectedObject)
        {
            // Check if the object has a container to put a pose into
            if (selectedObject.TryGetComponent(out PoseContainer poseContainer))
            {
                // Set the pose, mark as dirty to save
                poseContainer.pose = pose;

                // Mark scene for saving
                MarkActiveSceneAsDirty();
            }
        }

        return selectedObject;
    }

    public Pose TryGetPose(GameObject targetObject)
    {
        Pose pose = null;

        if (targetObject)
        {
            // Check if the object has a container to take a pose from
            if (targetObject.TryGetComponent(out PoseContainer poseContainer))
                pose = poseContainer.pose;
        }

        return pose;
    }

    private void MarkActiveSceneAsDirty()
    {
        #if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        #endif
    }
}
