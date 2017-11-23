namespace ifme
{
    public class PluginAudioMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public decimal[] Quality { get; set; }
        public string QualityPrefix { get; set; } = string.Empty;
        public string QualityPostfix { get; set; } = string.Empty;
        public decimal Default { get; set; }
    }
}
