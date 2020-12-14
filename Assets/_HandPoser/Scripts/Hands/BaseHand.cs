using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHand : MonoBehaviour
{
    // Neutral pose for the hand
    [SerializeField] protected Pose defaultPose = null;

    // Serialized so it can be used in editor by the preview hand
    [SerializeField] protected List<Transform> fingerRoots = new List<Transform>();

    // What kind of hand is this?
    [SerializeField] protected HandType handType = HandType.None;
    public HandType HandType => handType;

    public List<Transform> Joints { get; protected set; } = new List<Transform>();

    protected virtual void Awake()
    {
        Joints = CollectJoints();
    }

    protected List<Transform> CollectJoints()
    {
        List<Transform> joints = new List<Transform>();

        foreach (Transform root in fingerRoots)
            joints.AddRange(root.GetComponentsInChildren<Transform>());

        return joints;
    }

    public List<Quaternion> GetJointRotations()
    {
        List<Quaternion> rotations = new List<Quaternion>();

        foreach (Transform joint in Joints)
            rotations.Add(joint.localRotation);

        return rotations;
    }

    public void ApplyDefaultPose()
    {
        ApplyPose(defaultPose);
    }

    public void ApplyPose(Pose pose)
    {
        // Get the proper info using hand's type
        HandInfo handInfo = pose.GetHandInfo(handType);

        // Apply rotations 
        ApplyFingerRotations(handInfo.fingerRotations);

        // Position, and rotate, this differs on the type of hand
        ApplyOffset(handInfo.attachPosition, handInfo.attachRotation);
    }

    public void ApplyFingerRotations(List<Quaternion> rotations)
    {
        // Make sure we have the rotations for all the joints
        if (HasProperCount(rotations))
        {
            // Set the local rotation of each joint
            for (int i = 0; i < Joints.Count; i++)
                Joints[i].localRotation = rotations[i];
        }
    }

    private bool HasProperCount(List<Quaternion> rotations)
    {
        return Joints.Count == rotations.Count;
    }

    public abstract void ApplyOffset(Vector3 position, Quaternion rotation);
}
