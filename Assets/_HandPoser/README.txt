Welcome! This project is VR with Andrew's Hand Poser for XR Toolkit.

Hand Models are from SteamVR.

For setup and additional info, refer to the video below.

Also, I wanted to document some known limitations incase you run into any issues.

 - The poser does not account for the object's scale. The poser will assume all interactable objects are of uniform (1, 1, 1) scale.

 - The poser works best with instantaneous tracking. If you would like to use velocity or kinematic tracking, there are potential workarounds. 
	- You can attempt to child the hand mesh to the interactable object.
	- Or, use a custom physics-based hand and attach the object to that instead.