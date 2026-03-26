namespace WpfApp2.Configuration
{
    public sealed class AppSettings
    {
        public string HeaderText { get; set; } = string.Empty;
        public string ItemFileName { get; set; } = string.Empty;

        public int[] CategoryRanges { get; set; }
        public string[] CategoryNames { get; set; }
    }

    
}