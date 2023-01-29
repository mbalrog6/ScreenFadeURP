namespace MB6.URP.Fade
{
    public class RendererListElement
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public RendererListElement() : this(null, null)
        {
        }
        public RendererListElement(string rendererName, string path)
        {
            Name = rendererName;
            Path = path;
        }
    }
}


