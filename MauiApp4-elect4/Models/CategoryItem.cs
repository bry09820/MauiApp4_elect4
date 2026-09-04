using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Represents a category item for horizontal filter pills or category icon cards.
    /// </summary>
    public class CategoryItem : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _icon = string.Empty;
        private string _imageUrl = string.Empty;
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

        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                if (_imageUrl != value)
                {
                    _imageUrl = value;
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

        // Helper UI binding properties adapted to Forest Green theme
        public Color BackgroundColor => IsSelected ? Color.FromArgb("#1E6B39") : Color.FromArgb("#FFFFFF");
        public Color TextColor => IsSelected ? Colors.White : Color.FromArgb("#1A202C");
        public Color BorderColor => IsSelected ? Color.FromArgb("#1E6B39") : Color.FromArgb("#E2E8F0");

        public string DisplayText => string.IsNullOrEmpty(Icon) ? Name : $"{Icon} {Name}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
