using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a category filter pill with dynamic selection state for data binding.
    /// </summary>
    public class CategoryItem : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _icon = string.Empty;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackgroundColor));
                    OnPropertyChanged(nameof(TextColor));
                    OnPropertyChanged(nameof(BorderColor));
                }
            }
        }

        // Helper UI binding properties
        public Color BackgroundColor => IsSelected ? Color.FromArgb("#FF6B4A") : Color.FromArgb("#22262B");
        public Color TextColor => IsSelected ? Colors.White : Color.FromArgb("#8A94A6");
        public Color BorderColor => IsSelected ? Color.FromArgb("#FF6B4A") : Color.FromArgb("#2C323B");

        public string DisplayText => string.IsNullOrEmpty(Icon) ? Name : $"{Icon} {Name}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
