namespace ifme
{
    public class PluginVideoMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public bool MultiPass { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Postfix { get; set; } = string.Empty;
        public PluginVideoModeValue Value { get; set; } = new PluginVideoModeValue();
    }
}
