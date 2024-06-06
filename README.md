
# Folder Structure
The toplevel folders in this repo (except for `EasyRoads3Dv3`) are the default folders from an empty Unity build. Consult external references (e.g. https://medium.com/@jsj5909/a-brief-anatomy-of-a-unity-project-folder-563bd3f4ad40) for better explanations of their roles.

The `Assets` folder contains most of the files of interest for developing Metrocycle

## MainAssets
This folder contains the main assets which are present in (almost) all scenes.

- The `Bike` prefab contains the Motorcycle/Bicycle models and scripts, as well as the dashboard elements (speedometer, blinker signals, timer, etc.). For the Motorcycle/Bicyle models, make sure that the parts you want to interact with obstacles/detects are in the `bikebody` layer, while the wheels (or anything that you want to interact with roads in the `drivable` layer) should be in the `wheels layer`. On the other hand if you find too detects/colliders being triggered too many times, first check if the detect's layer is set to the `DetectLayer` and then reduce the (sub)models under the `bikebody` layer.
- The `CamerasWithHeadCheck` prefab contains 3 cameras for the 3 perspectives (normal, left head check, right head check). If you want to edit the angle of the head check, modify the camera offsets in this prefab.
- The `popUp` prefab contains the different popup types and their corresponding button sets

### BikeAssets
Contains art assets and models for the blinkers, side mirror, and speedometer.

### popUp
Contains art assets and fonts for popup buttons

### Detects
Contains the main prefabs used for detecting behaviors which need to be applied manually.

- The `IntersectionLaneDetects` prefab contains a fully setup set of intersection detects which are responsible for checking proper behavior in intersections. Check the comments in `Assets/Scripts/Intersection/IntersectionChecker.cs` and see how it is used in `Assets/Scenes/Basic_Intersection_Motorcycle.unity`. To apply this on a new scene, you just need to drag and drop the prefab and adjust the position and sizes of the detects to their proper lanes.

- The `TurnDetect` prefab is a simplified version of the `IntersectionLaneDetects` prefab, focusing only on turns FROM one-way roads. Examples are in `Assets/Scenes/Advanced_EDSA_Motorcycle.unity.unity` where the player drives mostly through one-way roads and turns in some streets (e.g. Kamuning Road) where this prefab is used. Note that this prefab can NOT be used when the player can enter a junction/intersection from multiple sides (use `IntersectionLaneDetects` instead)

#### IntersectionAssets
Contains prefabs used by `IntersectionLaneDetects`.

- The `Stop line` prefab is simply a visual line and does NOT contain checks. Checks for proper stopping are handled by `Assets/Scripts/Intersection/IntersectionChecker.cs`, provided that the intersection detects are placed properly AFTER the stop line

- The `NoLaneChangeLine` is used for detecting improper lane changing on the solid line near intersections. It also checks for the 20kph speed limit

#### Materials
Contains the materials used for the in-game detect indicating the current goal (green), error (red), or a check (yellow) we want to be explicitly seen, for example a yellow detect before turns to remind the player that they need to turn on their blinker



## Scripts
Contains the actual code for checkers, dashboard, etc.

- `CheckpointDetection.cs` is the general script for colliders/detects interacting with the player. It should be attached to a collider in the `DetectLayer`. It allows callbacks (hardcoded in the editor or via script) to allow various actions (e.g. save checkpoint upon reaching target, detect if player went the wrong way or was not able to follow instruction, etc.).
- `CollisionWithObstacles.cs` decides on what the collision popup should say based on the object the player collided with (e.g. other car, side of the road)
- `ScenarioStart.cs` should be present in each scene a it is responsible for showing the very first popup message in the scene and setting the vehicle type. It should be applied to the `Assets/MainAssets/Bike` prefab to work properly.
- `EventSequence.cs` handles the activation of checkers for the current challenge only while deactivating checkers for other challenges so that they don't get accidentally triggered. Note that it only works for a linear sequence of challenges. See `Assets/Scenes/Basic_Intersection_Motorcycle.unity`, particularly the `Event Detects` object for an example of how to use it, especially in conjunction with checkpoints
- `blinker.cs` and `HeadCheck.cs` simply contain the mechanics for blinkers and head checks or blind spot checks, respectively. They do NOT contain checks, so you would only modify them if you want to change the control scheme for the blinker (e.g. change button or add on screen button controls), to change the speed of head checks, etc.
- `Speedometer.cs` and `Timer.cs` control the Speedometer on the dashboard and the on-screen timer. `Speedometer.cs` also handles the calculation of the average speed.
- `Stats.s` handles the storage of user stats across scene loads and game reloads, while `ShowStats.cs` handles the formatting of stats when they are presented to the user.
- `BlinkerChecker.cs` and `HeadCheckDetect.cs` are only used in the Tutorial levels to explicitly check the correct usage of blinkers and headchecks, respectively. In all other scenarios, these checks are integrated with each other (since head checks and blinkers are required for lane changes/turns) and checked in `GameManager.cs` which also provides other convenience functions for interacting with the currently active bike, the dashboard elements, etc.
- `popUp.cs` contains the code for activating the different types of popups. If you want to add other types of popups, you need to modify this in addition to `Assets/MainAssets/popUp.prefab`.

### utility
Contains vendored dependencies (SimpleJSON) and simple helper scripts

### LaneChange
Contains checker logic for lane changes, including checks for proper blinker use and head checks. Typically you shouldn't need to apply these manually (too time consuming) since `Assets/GleyPlugins/TrafficSystem/Scripts/Core/Editor/ExternalTools/EasyRoads/EasyRoadsMethods.cs` automatically sets these checkers up with the waypoints

### Intersection
Contains checker logic for intersections. This is used by the `IntersectionLaneDetects` and `TurnDetect` prefabs in the `Assets/MainAssets/Detects` folder, so you usually use those prefabs directly instead of manually applying these scripts.

## Main Menu
Contains art assets for the game's Main Menu.

## Scenes
Contains the scenefiles which represent each "level" of the game. The files are generally in the format `{Category}_{Level Name}_{Vehicle}.unity` (except for `Main Menu.unity`) where `{Category}` is either `Basic` or `Advanced`, while `{Vehicle}` is either `Motorcycle` or `Bicycle`.

To minimize merge conlicts, try to ensure only one person is working on a single scenefile for each development sprint.

## ThirdPartyAssets
Contains third party assets, including the main Motorcycle and Bicycle assets, as well as extra models (e.g. buildings, human characters, road objects) which can be used in the future.

### Ash Assets
Contains the models and code for the [Ash Arcade Bike Physics](https://assetstore.unity.com/packages/tools/physics/arcade-bike-physics-224528) asset. This contains full source code, so you can modify the behavior of the Bicycle. See `git log --follow e6ed153ae064feee3e4ec827837818ba5c87c9d8..HEAD 'Assets/ThirdPartyAssets/Ash Assets/Arcade Bike Physics/Scripts/ArcadeBikeController.cs'` for examples of how we modified the script to limit the reverse speed.

### BycicleSystem (sic)
Contains the models and code for the [Simple Bicycle System](https://github.com/RayznGames/BicycleSystem/releases). See the `Assets/MainAssets/Bike` prefab for the list of modifications done to the model (mostly halfed scale size and adjusted (angular) accelaration)

### TrafficModels
Contains the models used for road obstacles (mostly cars) and AI vehicles.

The `AI Bike`, `AI Motorcycle 1`, etc. prefabs are basically modified versions of `Assets/GleyPlugins/TrafficSystem/Graphics/Vehicle/Prefabs/CompleteVehicles/OriginalSmallSedan.prefab` with the model replaced and the wheels adjusted. To add more AI vehicles, follow this [Gley MTS car tutorial](https://www.youtube.com/watch?v=moGHcd2Jaa4) and add the AI vehicle to the scene Traffic Script's Vehicle Pool. If your AI vehicle does not work properly, try removing all items in the `All Wheels` section of the `Vehicle Component` script then pressing `Configure Car`.

The `AI Bus` model is taken from [FREE School bus - Low Poly by SophieJu on Sketchfab](https://sketchfab.com/3d-models/free-school-bus-low-poly-392a362ca6e6453f8c29ffbf3fc20608), while the the `Jeepney` model is taken from [Jeepney by Maclin Macalindong on Sketchfab](https://sketchfab.com/3d-models/jeepney-0b8bcde5df19458da9fa5606989b1e7d).

### SyntyStudios / Down Town Pack
Asset packs containing a lot of models and prefabs. For example, the buildings in `Assets/Scenes/Advanced_Fictional_Motorcycle.unity` terrain and the police officer and car models in `Assets/Scenes/Advanced_Commonwealth_Bicycle.unity` are taken from `SyntyStudios`.

## RoadTextures
Contains textures for roads in the game. The EasyRoads3D asset used for creating the roads in the game do not have a builtin way to modify the number of lanes, so the way to represent multiple lanes is to widen the width of the road and modify the texture to have more lanes.

For a new road type, you can simply copy a similar texture (`*.jpg`) and its corresponding material (`Materials/*.mat`) then modify the texture.

Note that materials/textures prefixed with `X-` are simply the (horizontally) flipped versions of their counterparts without the `X-` prefix. This is sometimes needed for inverted textures in intersections (e.g. see the intersections in `Assets/Scenes/Advanced_EDSA_Motorcycle.unity.unity` where some `X-` textures are used)

## GleyPlugins
**NOTE: Do not move this folder since the asset relies on absolute paths rooted on the Assets folder**
Code for the [Gley Mobile Trafic System](https://gleygames.com/traffic-system/) which is responsible for adding AI vehicles to the roads via integration with EasyRoads3D.

If your AI vehicles do not show up or do not move, try following the [Gley MTS Tutorial](https://www.youtube.com/watch?v=203UgxPlfNo&list=PLKeb94eicHQtyL7nYgZ4De1htLs8lmz9C&index=2) fully, especially the Layer setup, and Grid Setup. In Metrocycle, roads should be in the `drivable` layer and AI vehicles should be in the `AI Traffic Layer`.

This asset has full source available so you can actually how the other vehicles behave. Additionally, the waypoints (which guide the path of AI vehiclesS) produced by this asset cover all roads, so they have double purpose in Metrocycle where they also contain colliders and scripts for various checks (e.g. Speed limit, lane change).

The main script to be edited would probably be `Assets/GleyPlugins/TrafficSystem/Scripts/Core/Editor/ExternalTools/EasyRoads/EasyRoadsMethods.cs` (see `git log --follow 45233113fad6c3338f6cf66246a04c8b02784b28..HEAD Assets/GleyPlugins/TrafficSystem/Scripts/Core/Editor/ExternalTools/EasyRoads/EasyRoadsMethods.cs`), especially if you want to add additional checks/scripts to the waypoints.

## EasyRoads3D / EasyRoads3D scenes
**NOTE: Do not move this folder since the asset relies on absolute paths rooted on the Assets folder**
Contains the [EasyRoads3D](https://www.easyroads3d.com/) asset, which is mostly binary (dll) and cannot be modified easily. `Assets/EasyRoads3D/Scripts/runtimeScript.cs` might be of interest as it contains a (commented out) list and example usage of ER3D methods for manipulating road objects, some of which are not present in the [tutorial](https://www.easyroads3d.com/tutorials.php) nor the [manual](https://www.easyroads3d.com/v3/manualv3.html).

## Plugins
Generally contains plugins for Unity, but in our case the important part is that it contains `jsfuncs.jslib` which holds the JavaScript functions for interacting with Web code in WebGL builds.

Currently, Metrocycle uses [Web localstorage](https://developer.mozilla.org/en-US/docs/Web/API/Window/localStorage) to store user stats. If you want to add more JavaScript code for interacting with the Web in the WebGL build, read the [Interaction with browser scripting](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html) page of the Unity manual, particularly the [setup](https://docs.unity3d.com/Manual/web-interacting-browser-js.html) and the [cross-interaction between JS/Unity C# code](https://docs.unity3d.com/Manual/web-interacting-browser-js-to-unity.html).

## Tests
Contains the automated tests. In addition to checking for regressions and making future refactors easier, the test cases can also be used as guides to the checks being implemented.


To run the tests, Go to `Window > General > Test Runner` and click on the `PlayMode` tab. Select all the test you want to run then press `Run Selected`. Due to how parametrized test cases work with `UnityTest`, the test names in the UI are non-descriptive so you can read the test code, or run the test and check the log messages.


For more info on how to write additional test cases, you can visit
[the](https://gameconductor.net/blog/unity-test-framework-tutorial.html)
[following](https://docs.unity3d.com/Packages/com.unity.test-framework@1.3/manual/course/test-cases.html)
[UTF](https://medium.com/xrpractices/all-you-need-to-know-about-testing-in-unity3d-part-1-70658818ce5e)
[tutorials](https://medium.com/codex/writing-unit-tests-for-my-game-in-unity-b0163e2c9b47)


## CS198_Playground
Contains currently unused code for some ideas which couldn't be implemented (e.g. procedural road generation for better CPU/RAM performance, procedural obstacle generation).

