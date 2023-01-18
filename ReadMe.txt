Author: mbalrog6 (Scott Heinrichs)

These set of files were inspired by the following YouTube creators and their Tutorials: 

WarpedImagination
(https://www.youtube.com/watch?v=p5DCv7loLbw)
[This was for the actual ScreenFade System]

Bardent
(https://www.youtube.com/watch?v=Ubuu4ZoTb5Y)
[This was for using Reflection to populate the Editor Inspector with the EaseFunctions]

GDC
"Math for Game Programmers: Fast and Funky 1D Nonlinear Transformations"
(https://www.youtube.com/watch?v=mr5xkf6zSzk)
[I used this for creating the Easing Functions]

VR with Andrew
(https://www.youtube.com/watch?v=OGDOC4ACfSE)
[WarpedImagination was inspired by this video for his tutorial)

NOTE: I used the #if UNITY_EDITOR #endif to wrap the Editor Inspector script so that the custom inspector will not conflict with your build process, if you are using assebly defs, since that causes the Editor Folder functionality to stop working in Unity and you would have to move those files out of the runtime compile assemblies otherwise. 

This currently was only tested and known to work for URP (Universial Render Pipeline.) 

Unity Assembly Dependencies: 
- Unity.RendererPipelines.Universal.Runtime
- Unity.RendererPipelines.Core.Runtime

WHAT IT DOES: 
This package adds a URP Renderer Feature that will allow you to create a FadeIn and FadeOut Effect that does not require additional Geometry in the Scene to obscure the camera view. It just applies a shader that blends a mat material to the view after the Post Processing Pass. You can easily add some supplied easing functions to the Fade, or create your own that will work with it. It has a callback event to tie into if you have code that needs to be made aware of the fade completing. After setting it up you should only ever have to call the the public void ScreenFade(bool fade) function from the ScreenFade Component to activate the screen fade. 

HOW TO USE: 
1. You will need to find the URP Renderer and click "Add Renderer Feature"
2. Make sure the "Render Pass Event" is set to "After Rendering Post Processing"
3. Drag the "ScreenFade" Material located under the ScreenFade/FadeShader folder into the Material slot. (This is only used to make local copies from for the ScreenFader Script)
4. Add the ScreenFade Component Script to a GameObject. (Typically added to a Camera)
5. Expand the URP Renderer Scriptable Object to reveal child object, and Drag the ScreenFadeFeature Scriptable Object to the Screen Fade Feature Dependency slot on the ScreenFade Component. (You may need to save the scene to get the Renderer Object to update in the Project Hierarchy.)
6. While in the Editor you can click the "Add FadeIn Ease Function" and "Add FadeOut Ease Function" buttons to add an Ease Function. (If it is left blank then on Run it will instanciate a LinearEase Function from 0f to 1f for FadeIn, and a LinearEase from 1f to 0f for FadeOut.)
7. You will need to be sure and set the Start and End Values so that the End Value of FadeIn Matches the Start Value of FadeOut. This will make sure that there is no visible jump to a different Alpha for the transition. You may need to expand the "Fade In Ease Function" and "Fade Out Ease Function" if you have added one of the custom Ease Functions. 

You can test the Fade while the editor is playing by pressing the Fade Button. Some Easing Functions will not update their values while in playmode they are setup by an initialization function on first update. 

You can call the function: 

public void Fade(bool fade, float? delayTime = null, Color? color = null, float? duration = null)

setting fade to true causes ScreenFade to call FadeIn. 
setting fade to false cause ScreenFade to call Fadeout. 

delayTime (optional) will run a timer prior to running the FadeEffect, is useful if you want to wait before starting the FadeOut Effect to ensure your GameLogic has had time to process. 

color and duration will override the value set in the inspector and are optional. 

Event: 
public Action<bool> FinishedFading;

You can assign any function with the signature:
public void Function(bool fadingIn)

If bool is true, the function fired after the FadeIn Ease completed. 

If bool is false the function fired after the FadeOut Ease completed. 

Custom EaseFunctions: 
public interface IEaseFunction {public float Evaluate(float time);}

Evaluate(float time) just needs to return a value from 0f to 1f.

The Add Ease Function Buttons will populate with any classes that implement IEaseFunction in the current assembly domain.








