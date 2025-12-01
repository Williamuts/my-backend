using CommunityToolkit.Mvvm.ComponentModel;

namespace Group4RecycleApp.Models
{
    public partial class FaqItem : ObservableObject
    {
        public string Question { get; set; }
        public string Answer { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Rotation))]
        bool isExpanded;

      
        public double Rotation => IsExpanded ? 180 : 0;
    }
}