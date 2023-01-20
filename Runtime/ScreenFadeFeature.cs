// This script was copied from the WarpedImagination Video: https://www.youtube.com/watch?v=p5DCv7loLbw
// which he got from VR with Andrew: https://www.youtube.com/watch?v=OGDOC4ACfSE

// This include is not in the original, I added it to facilitate automatically
// setting the material since it is provided in the package. 
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MB6.URP.Fade
{
    public class ScreenFadeFeature : ScriptableRendererFeature
    {
        private ScreenFadePass _renderPass;
        public FadeSettings Settings = null; 
        
        public override void Create()
        {
            _renderPass = new ScreenFadePass(Settings);
            
            #if UNITY_EDITOR
            var mat = GetMaterialForSettings();
            if (mat != null && Settings != null)
            {
                Settings.Material = mat;
            }
            AssetDatabase.SaveAssetIfDirty(this);
            #endif
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (Settings.AreValid())
            {
                renderer.EnqueuePass(_renderPass);
            }
        }

        #if UNITY_EDITOR
        // This function check to see if there is a folder created in the packages section,
        // if so it will search both the whole Asset folder and the mb6.screenfadeurp.runtime folder
        // for a material called ScreenFadeURP.mat and return it.
        public Material GetMaterialForSettings()
        {
            string[] path;
            if (AssetDatabase.IsValidFolder("Packages/com.mb6.screenfadeurp/Runtime/"))
            {
                path = new string[] { "Assets/", "Packages/com.mb6.screenfadeurp/Runtime/" };
            }
            else
            {
                path = new string[] { "Assets/" };
            }
            var results = AssetDatabase.FindAssets("ScreenFadeURP t:Material", path);
            if (results.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(Material)) as Material;
            }

            return null;
        }
        #endif
        
    }
}