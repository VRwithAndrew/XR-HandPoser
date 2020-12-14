using UnityEngine;

[SerializeField]
[CreateAssetMenu(fileName = "NewPoseData")]
public class Pose : ScriptableObject
{
    // Info for each hand
    public HandInfo leftHandInfo = HandInfo.Empty;
    public HandInfo rightHandInfo = HandInfo.Empty;

    public HandInfo GetHandInfo(HandType handType)
    {
        // Return Left or Right, you can use a dictionary or different pose appliers
        switch (handType)
        {
            case HandType.Left:
                return leftHandInfo;
            case HandType.Right:
                return rightHandInfo;
            case HandType.None:
                return HandInfo.Empty;
        }

        // Return an empty 
        return HandInfo.Empty;
    }
}
