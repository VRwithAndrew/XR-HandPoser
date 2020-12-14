using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GameplayHand : BaseHand
{
    // The interactor we react to

    private void OnEnable()
    {
        // Subscribe to selected events
    }

    private void OnDisable()
    {
        // Unsubscribe to selected events
    }

    private void TryApplyObjectPose(XRBaseInteractable interactable)
    {
        // Try and get pose container, and apply
    }

    private void TryApplyDefaultPose(XRBaseInteractable interactable)
    {
        // Try and get pose container, and apply
    }

    public override void ApplyOffset(Vector3 position, Quaternion rotation)
    {
        // Invert since the we're moving the attach point instead of the hand

        // Since it's a local position, we can just rotate around zero

        // Set the position and rotach of attach
    }

    private void OnValidate()
    {
        // Let's have this done automatically, but not hide the requirement
    }
}