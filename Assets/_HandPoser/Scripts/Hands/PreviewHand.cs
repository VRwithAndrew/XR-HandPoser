using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
public class PreviewHand : BaseHand
{
    public void MirrorAndApplyPose(PreviewHand sourceHand)
    {
        // Mirror and apply the joint values
        List<Quaternion> mirroredRotations = MirrorJoints(sourceHand.Joints);
        ApplyFingerRotations(mirroredRotations);

        // Mirror and apply the position and rotation
        Vector3 mirroredPosition = MirrorPosition(sourceHand.transform);
        Quaternion mirroredRotation = MirrorRotation(sourceHand.transform);
        ApplyOffset(mirroredPosition, mirroredRotation);
    }

    private List<Quaternion> MirrorJoints(List<Transform> joints)
    {
        List<Quaternion> mirroredJoints = new List<Quaternion>();

        foreach (Transform joint in joints)
        {
            Quaternion inversedRotation = MirrorJoint(joint);
            mirroredJoints.Add(inversedRotation);
        }

        return mirroredJoints;
    }

    private Quaternion MirrorJoint(Transform sourceTransform)
    {
        Quaternion mirrorRotation = sourceTransform.localRotation;
        mirrorRotation.x *= -1.0f;

        return mirrorRotation;
    }

    private Quaternion MirrorRotation(Transform sourceTransform)
    {
        Quaternion mirrorRotation = sourceTransform.localRotation;
        mirrorRotation.y *= -1.0f;
        mirrorRotation.z *= -1.0f;
        return mirrorRotation;
    }

    private Vector3 MirrorPosition(Transform sourceTransform)
    {
        Vector3 mirroredPosition = sourceTransform.localPosition;
        mirroredPosition.x *= -1.0f;
        return mirroredPosition;
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        transform.localPosition = position;
        transform.localRotation = rotation;
    }
}
