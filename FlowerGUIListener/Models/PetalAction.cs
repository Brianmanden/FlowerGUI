using System.Diagnostics;

namespace FlowerGUIListener.Models
{
    public class PetalAction
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string LaunchableResource { get; set; }
        public string Arguments { get; set; }
        public ProcessWindowStyle WindowStyle { get; set; } = ProcessWindowStyle.Normal;
        public bool UseShellExecute { get; set; } = true;
    }
}
