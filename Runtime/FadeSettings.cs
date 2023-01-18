// This script was copied from the WarpedImagination Video: https://www.youtube.com/watch?v=p5DCv7loLbw
// which he got from VR with Andrew: https://www.youtube.com/watch?v=OGDOC4ACfSE

using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MB6.ScreenFade
{
    [Serializable]
    public class FadeSettings
    {
        public bool IsEnabled = true;
        public string ProfilerTag = "Screen Fade";

        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material Material = null;

        [NonSerialized] public Material RunTimeMaterial = null;

        public bool AreValid() => (RunTimeMaterial != null) && IsEnabled;
    }
}
