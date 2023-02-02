namespace MB6.URP.Fade
{
    public class RendererDetails
    {
        public string RenderPipelineName { get; set; }
        public string Name { get; set; }
        public ScreenFadeFeature FadeFeature { get; set; }
        public bool IsDefaultRendererInPipeline { get; set; }
        public bool IsOnDefaultPipeline { get; set; }
        public int DefaultRendererIndex { get; set; }

        public RendererDetails(string renderPipelineName, string name, ScreenFadeFeature fadeFeature, 
            bool isDefaultRendererInPipeline, bool isOnDefaultPipeline, int defaultRendererIndex)
        {
            RenderPipelineName = renderPipelineName;
            Name = name;
            FadeFeature = fadeFeature;
            IsDefaultRendererInPipeline = isDefaultRendererInPipeline;
            IsOnDefaultPipeline = isOnDefaultPipeline;
            DefaultRendererIndex = defaultRendererIndex;
        }

        public override string ToString()
        {
            var message =
                $"Pipeline: {RenderPipelineName}, Renderer: {Name}, SFF, Default: {IsDefaultRendererInPipeline}, OnDefaultPipeline: {IsOnDefaultPipeline}, dIndex: {DefaultRendererIndex}";
            return message;
        }
    }
}
