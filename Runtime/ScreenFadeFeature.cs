// This script was copied from the WarpedImagination Video: https://www.youtube.com/watch?v=p5DCv7loLbw
// which he got from VR with Andrew: https://www.youtube.com/watch?v=OGDOC4ACfSE

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
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (Settings.AreValid())
            {
                renderer.EnqueuePass(_renderPass);
            }
        }
    }
}