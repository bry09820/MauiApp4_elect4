using System.Text.Json.Serialization;

namespace MauiApp4_elect4.Models
{
    /// <summary>
    /// Geographic coordinate point with latitude and longitude.
    /// </summary>
    public class GeoPoint
    {
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("lon")]
        public double Longitude { get; set; }

        public GeoPoint() { }

        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() => $"{Latitude:F5}, {Longitude:F5}";
    }

    /// <summary>
    /// Geocoded location model representing an address with coordinates.
    /// </summary>
    public class GeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Postcode { get; set; }
        public string? Country { get; set; }
        public string ShortName => !string.IsNullOrEmpty(Street) ? Street : (DisplayName.Split(',').FirstOrDefault() ?? "Location");

        public GeoPoint ToGeoPoint() => new(Latitude, Longitude);
    }

    /// <summary>
    /// Represents an interactive marker on the Leaflet OpenStreetMap.
    /// </summary>
    public class MapMarker
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MarkerType { get; set; } = "Destination"; // "Store", "Courier", "Destination"
        public string IconEmoji { get; set; } = "📍";
        public string BadgeColor { get; set; } = "#1E6B39";

        public static MapMarker CreateStore(double lat, double lon, string title = "GreenMarket Store", string desc = "Pickup & Fulfillment Hub")
        {
            return new MapMarker
            {
                Id = "store_marker",
                Title = title,
                Description = desc,
                Latitude = lat,
                Longitude = lon,
                MarkerType = "Store",
                IconEmoji = "🏪",
                BadgeColor = "#2D7F4C"
            };
        }

        public static MapMarker CreateCourier(double lat, double lon, string title = "Courier Driver", string desc = "On the way")
        {
            return new MapMarker
            {
                Id = "courier_marker",
                Title = title,
                Description = desc,
                Latitude = lat,
                Longitude = lon,
                MarkerType = "Courier",
                IconEmoji = "🚚",
                BadgeColor = "#1E6B39"
            };
        }

        public static MapMarker CreateDestination(double lat, double lon, string title = "Delivery Address", string desc = "Destination")
        {
            return new MapMarker
            {
                Id = "dest_marker",
                Title = title,
                Description = desc,
                Latitude = lat,
                Longitude = lon,
                MarkerType = "Destination",
                IconEmoji = "🏡",
                BadgeColor = "#E65100"
            };
        }
    }

    /// <summary>
    /// Calculated route details between source and destination coordinates.
    /// </summary>
    public class RouteResult
    {
        public bool IsSuccess { get; set; } = true;
        public string Provider { get; set; } = "OpenRouteService";
        public double DistanceMeters { get; set; }
        public double DistanceKm => Math.Round(DistanceMeters / 1000.0, 2);
        public string DistanceDisplay => DistanceKm < 1.0 ? $"{Math.Round(DistanceMeters)} m" : $"{DistanceKm:F1} km";

        public double DurationSeconds { get; set; }
        public double DurationMinutes => Math.Round(DurationSeconds / 60.0, 1);
        public string DurationDisplay => DurationMinutes < 1.0 ? "1 min" : $"~{Math.Ceiling(DurationMinutes)} mins";

        public List<GeoPoint> Coordinates { get; set; } = [];
        public string? ErrorMessage { get; set; }
    }

    // ── Nominatim API JSON DTOs ──────────────────────────────────────────
    public class NominatimSearchResult
    {
        [JsonPropertyName("place_id")]
        public long PlaceId { get; set; }

        [JsonPropertyName("lat")]
        public string Lat { get; set; } = "0";

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = "0";

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public NominatimAddress? Address { get; set; }
    }

    public class NominatimAddress
    {
        [JsonPropertyName("road")]
        public string? Road { get; set; }

        [JsonPropertyName("house_number")]
        public string? HouseNumber { get; set; }

        [JsonPropertyName("suburb")]
        public string? Suburb { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("postcode")]
        public string? Postcode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}
