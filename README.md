#Bubble Shooter VR
This 3D match-3 bubble shooting game was developed in Unity originally for use with Oculus Rift and Oculus Touch controllers, but it can be used with other VR systems, even ones without motion sensing controllers.

Video: https://youtu.be/NPy7xM5XTS4

##Prerequisites
You will need prior knowledge of how to develop with Unity and with a Virtual Reality SDK. This project does not contain a VR SDK, you'll need to import the one you wish to use.

##Disclaimers
* This codebase is provided as-is and there is no warranty. I will look at pull requests and bug reports submitted through github, but I make no guarantee that I will ever address any bugs or make any further improvements.
* There are very limited game mechanics here (no scoring, no progression, etc), so it's not a game per se. It's more a foundation on which to build a bubble shooting game.

##How To Use
1. Download (or fork+clone) this repo, then open the BubbleShooterUnityProject folder in Unity as a Project.
2. Open one of the scenes you wish to experiment with.
3. Drag a camera rig from your VR SDK of choice into the scene.
4. Find the Shoot Controller instance in the scene, and drag the transform of the controller into the Controller Transform field.
  * For example, if using Oculus Touch, drag in [Scene]/OVRCameraRig/TrackingSpace/RightHandAnchor to aim with your right hand.
  * Or, if using Gear VR, drag in [Scene]/OVRCameraRig/TrackingSpace/CenterEyeAnchor to aim with your face.

NOTE: Currently, only the Touch Right Trigger is supported for shooting a bubble.

## Tips
* Create a Layer named Poolers and hide it from any cameras. This will make sure objects are invisible while they're pooled. (The Pool Manager will automatically detect if there is a "Poolers" layer and assign/unassign the layer as needed.)
