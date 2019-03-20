using MvvmLib.Adaptive;

namespace AdaptiveSample.Windows
{
    public class VariableItem : VariableSizedGridViewItem
    {
        public DataItemType ItemType { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
    }

    public enum DataItemType
    {
        Square,
        Border
    }
}