using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PreviewHand))]
public class HandPreviewEditor : Editor
{
    private PreviewHand previewHand = null;
    private Transform activeJoint = null;

    private void OnEnable()
    {
        previewHand = target as PreviewHand;
    }

    private void OnSceneGUI()
    {
        DrawJointButtons();
        DrawJointHandle();
    }

    private void DrawJointButtons()
    {
        // Draw a button for each joint
        foreach (Transform joint in previewHand.Joints)
        {
            // Were one of the buttons pressed?
            bool pressed = Handles.Button(joint.position, joint.rotation, 0.01f, 0.005f, Handles.SphereHandleCap);

            // Did we select the same joint?
            if (pressed)
                activeJoint = IsSelected(joint) ? null : joint;                
        }
    }

    private bool IsSelected(Transform joint)
    {
        return joint == activeJoint;
    }

    private void DrawJointHandle()
    {
        // If a joint is selected
        if(HasActiveJoint())
        {
            // Draw handle
            Quaternion currentRotation = activeJoint.rotation;
            Quaternion newRotation = Handles.RotationHandle(currentRotation, activeJoint.position);

            // Detect if handle has rotated
            if (HandleRotated(currentRotation, newRotation))
            {
                activeJoint.rotation = newRotation;
                Undo.RecordObject(target, "Joint Rotated");
            }
        }
    }

    private bool HasActiveJoint()
    {
        return activeJoint;
    }

    private bool HandleRotated(Quaternion currentRotation, Quaternion newRotation)
    {
        return currentRotation != newRotation;
    }
}
