# Capsule cycling prototype

Two scripts for Unity 6 (including a Unity 6.3 project), suitable for URP.
This is a keyboard-controlled arcade prototype for testing your cycling map.
The camera defaults approximate the supplied low, rear chase view. Exact reference
camera settings cannot be recovered from one screenshot.

## Setup

1. Copy both .cs files to Assets/Scripts. Keep their filenames unchanged.
2. Create GameObject > 3D Object > Capsule. Name it CyclePlayer.
3. Keep Scale at (1, 1, 1), Rotation X/Z at 0, and place its centre about 1.1 metres
   above the road. Rotate Y to face along the road; its blue +Z axis is forward.
4. Remove the primitive's Capsule Collider. Add CapsuleCycleController; Unity
   adds a Character Controller. Do not add a Rigidbody.
5. Check Character Controller: Height 2, Radius 0.45, Center (0,0,0),
   Slope Limit 45, Step Offset 0.3, Skin Width 0.05, Min Move Distance 0.
6. Your road needs a non-trigger collider (for a static road mesh, Mesh Collider).
   Terrain needs a Terrain Collider. Decorative meshes alone cannot support the player.
7. Keep Main Camera outside the capsule hierarchy. Add CycleChaseCamera.
   Drag CyclePlayer into its Target slot. Disable other scripts or Cinemachine
   components that also control this camera's transform.
8. Press Play and click inside the Game view to give it keyboard focus.

## Controls

| Key | Action |
| --- | --- |
| W / Up arrow | Pedal / accelerate |
| A / D or Left / Right arrows | Steer |
| S / Down arrow / Space | Brake (no reverse) |
| Release W | Coast and gradually slow down |
| R | Reset to the starting position |

Enable Auto Pedal in the controller Inspector to move automatically; braking
still takes priority. Max Speed 14 means 14 m/s, approximately 50.4 km/h.
SpeedKmh exposes measured horizontal speed for a future speedometer.

Both the New Input System and legacy Input Manager are supported using compile
symbols. With Both enabled, the New Input System branch is used. No Input Actions
asset or PlayerInput component is required. Standard project assemblies work;
if placing this inside a custom assembly definition with the New Input System,
add a reference to Unity.InputSystem.

## Starting camera values

| Setting | Value |
| --- | --- |
| Distance | 5.5 |
| Height (above capsule centre) | 1.8 |
| Pitch (degrees downward) | 8 |
| Field Of View (vertical) | 60 |
| Position Smooth Time | 0.12 |
| Turn Smooth Time | 0.18 |
| Collision Radius | 0.3 |

Use a 16:9 Game view for similar framing. Increase Distance for a smaller rider;
increase Height for more road visibility. Increase Pitch to aim farther downward.
These values assume the default 2-metre capsule with its pivot at its centre.
When replacing it with a bike model, keep the movement root and add the model
as a child, or retune Height if the root pivot moves to ground level.

Obstacle Mask should include road, terrain, walls and rocks. The camera filters
out the target and its children. For a large scene, put solid scenery on specific
layers and select those layers. The basic sphere cast pulls the camera inward
near obstacles; tight spaces can change the framing. SphereCastAll allocates a
small hit array per frame: profile/replace it with a suitably sized non-allocating
query if this prototype becomes production code.

## Scope and verification

Includes forward acceleration, coasting, braking, speed-dependent steering,
gravity, basic slope/step movement, start-position reset and smooth chase framing.
It is manual steering, not automatic road/spline following. The capsule remains
upright. It does not simulate bicycle balance, wheels, cadence/power, realistic
hill drag, networking or rider animation.

API use was checked against Unity documentation, and source structure was reviewed.
No Unity Editor or C# compiler was available here: these scripts have not been
compiled or play-tested in Unity. In your scene, check flat-road acceleration and
braking, uphill/downhill contact, wall collisions, steering, camera obstacle
clearance and reset. Road scale, mesh seams and steep slopes may need tuning.

Official API references:
- https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CharacterController.Move.html
- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/Migration.html
