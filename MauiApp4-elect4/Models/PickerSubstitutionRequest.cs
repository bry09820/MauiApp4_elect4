using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp4_elect4.Models
{
    public enum SubstitutionStatus
    {
        PendingApproval,
        Approved,
        DeclinedRefunded
    }

    /// <summary>
    /// Represents a live out-of-stock substitution request dispatched by the in-store picker.
    /// </summary>
    public class PickerSubstitutionRequest : INotifyPropertyChanged
    {
        private SubstitutionStatus _status = SubstitutionStatus.PendingApproval;

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int OrderId { get; set; } = 104;
        public string OriginalItemName { get; set; } = "Sourdough Bread";
        public decimal OriginalItemPrice { get; set; } = 1.99m;
        public string ProposedItemName { get; set; } = "Artisan Organic Multigrain Loaf";
        public decimal ProposedItemPrice { get; set; } = 2.49m;
        public decimal PriceDifference => ProposedItemPrice - OriginalItemPrice;
        public string FormattedDifference => PriceDifference >= 0 ? $"+${PriceDifference:F2}" : $"-${Math.Abs(PriceDifference):F2}";
        public string PickerMessage { get; set; } = "The bakery shelf is out of regular Sourdough Bread. I found this freshly baked Organic Multigrain Loaf in Aisle 4 as an organic substitute!";
        public string PickerName { get; set; } = "Elena Ramos (Store Shopper)";
        public string PickerAvatarUrl { get; set; } = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=200&auto=format&fit=crop&q=80";
        public string AislePhotoUrl { get; set; } = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=600&auto=format&fit=crop&q=80";
        public DateTime Timestamp { get; set; } = DateTime.Now.AddMinutes(-3);

        public SubstitutionStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPending));
                    OnPropertyChanged(nameof(IsApproved));
                    OnPropertyChanged(nameof(IsDeclined));
                    OnPropertyChanged(nameof(StatusBadgeText));
                    OnPropertyChanged(nameof(StatusBadgeColor));
                }
            }
        }

        public bool IsPending => Status == SubstitutionStatus.PendingApproval;
        public bool IsApproved => Status == SubstitutionStatus.Approved;
        public bool IsDeclined => Status == SubstitutionStatus.DeclinedRefunded;

        public string StatusBadgeText => Status switch
        {
            SubstitutionStatus.PendingApproval => "Action Required (Response Needed)",
            SubstitutionStatus.Approved => "Replacement Approved ✓",
            SubstitutionStatus.DeclinedRefunded => "Refunded to Original Payment 💰",
            _ => ""
        };

        public string StatusBadgeColor => Status switch
        {
            SubstitutionStatus.PendingApproval => "#E65100",
            SubstitutionStatus.Approved => "#1E6B39",
            SubstitutionStatus.DeclinedRefunded => "#64748B",
            _ => "#1E6B39"
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
