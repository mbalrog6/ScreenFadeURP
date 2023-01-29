namespace MB6.URP.Fade
{
    public class FeatureListElement
    {
        public string Renderer { get; private set; }    
        public ScreenFadeFeature Feature { get; private set; }
        public bool IsOnDefaultRenderer { get; set; }

        public FeatureListElement(string renderer, ScreenFadeFeature feature, bool isDefaultRenderer)
        {
            Renderer = renderer;
            Feature = feature;
            IsOnDefaultRenderer = isDefaultRenderer;
        }

        public override string ToString()
        {
            return $"Renderer: {Renderer}, FeatureName: {Feature.name}, IsOnDefaultRenderer: {IsOnDefaultRenderer}\n";
        }
    }
}