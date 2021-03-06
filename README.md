# StrangeEngine
StrangeEngine is a Library of scripts to be used with unity to make game development easier for Indie and Junior game developers.
 
Despite being called "StrangeEngine", this is not a game engine. It is instead a bunch of scripts that you can copy/paste into your unity project to make development a lot easier. The reason i call it an engine is because if you use it properly it can be quite powerful. All you need to do is tell the engine what to do, and it will do most of the heavy lifting for you, much like a well-designed engine would.
 
Novice Programmers Beware:
 
If you do not have experience with C# or with Unity. I would recommend NOT using this engine until you have a bit more experience. If you think you can write a few lines of code and, Boom! you have a working, functioning game, think again. This engine is a complex tool, and you need to know how to properly use it and c# to use it effectively.
 
# What does it do?
The engine consists of a Dialogue System similar to how the witcher 3 works, a quest helper on the side of the screen to monitor the current active quest. and a working Inventory system.
 
The engine also has a Player interaction system which allows you to use functions "Use()" "OnNearby()" "WhileNearby()" "NoLongerNearby()" on any gameobejct with a collider (see API for more information)
 
In the assets folder you will find a canvas with panels for the Dialogues and Quest Helper Pre-Made for you. You will however be expected to create your own UI for the inventory. The one shown below is just a demo
 
![DialogueExample](/images/dialogueExample.png)
![InventoryExample](/images/inventoryExample.png)
 
![QuestHelper](/images/QuestHelper.png)
 
# importing
importing the engine may be problematic at first, just search through the scripts/solution for "IMPORT::" and you will find multiple comments telling you how to properly attach that class to unity so it will run as expected.
 
when importing the engine, first copy the Canvas in the assets folder over to your project and add it to your scene. now copy the code over.
 
you are welcome to make your own panels and UI, or edit the ones i made. i just left a copy of mine to help people import the engine with less errors.
 
# documentation
the documentation on how to use this engine properly can be found here: https://docs.google.com/document/d/1bozarGPqoZR0TTmgqBhp1LtTd_DLHNd-ugvPni81QnE/edit#heading=h.gxv3vla6h11j

# Example Projects
here is a list of projects usign the engine:

 - [StrangeDev demo](https://github.com/arcturus125/StrangeEngine-Demo)
 - [VoxelDungeons](https://github.com/StrangeDevTeam/Voxel-Dungeons-V2 "VoxelDungeons V2 is a voxel based adventure game made by StrangeDevTeam")
