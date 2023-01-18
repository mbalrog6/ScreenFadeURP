// This script was copied from the WarpedImagination Video: https://www.youtube.com/watch?v=p5DCv7loLbw
// which he got from VR with Andrew: https://www.youtube.com/watch?v=OGDOC4ACfSE

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MB6.URP.Fade
{
    public class ScreenFadePass : ScriptableRenderPass
    {
        private FadeSettings _settings = null;

        public ScreenFadePass(FadeSettings newSettings)
        {
            if (newSettings != null)
            {
                _settings = newSettings;
                renderPassEvent = newSettings.RenderPassEvent;
            }
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer command = CommandBufferPool.Get(_settings.ProfilerTag);

            RenderTargetIdentifier src = BuiltinRenderTextureType.CameraTarget;
            RenderTargetIdentifier dest = BuiltinRenderTextureType.CurrentActive;
            
            command.Blit(src, dest, _settings.RunTimeMaterial);
            context.ExecuteCommandBuffer(command);
            
            CommandBufferPool.Release(command);
        }
    }
}