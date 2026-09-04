using System.Collections.Specialized;
using System.Globalization;
using System.Text.Json;
using MauiApp4_elect4.Models;

namespace MauiApp4_elect4.Controls
{
    /// <summary>
    /// Hybrid Leaflet.js + OpenStreetMap MapView component using .NET MAUI WebView.
    /// Renders OpenStreetMap tiles, custom marker pins, popups, and route polylines.
    /// </summary>
    public class LeafletMapView : ContentView
    {
        private readonly WebView _webView;
        private bool _isLoaded;

        public static readonly BindableProperty CenterLatitudeProperty =
            BindableProperty.Create(nameof(CenterLatitude), typeof(double), typeof(LeafletMapView), 44.0462, propertyChanged: OnMapStateChanged);

        public static readonly BindableProperty CenterLongitudeProperty =
            BindableProperty.Create(nameof(CenterLongitude), typeof(double), typeof(LeafletMapView), -123.0220, propertyChanged: OnMapStateChanged);

        public static readonly BindableProperty ZoomLevelProperty =
            BindableProperty.Create(nameof(ZoomLevel), typeof(int), typeof(LeafletMapView), 14, propertyChanged: OnMapStateChanged);

        public static readonly BindableProperty MarkersProperty =
            BindableProperty.Create(nameof(Markers), typeof(IEnumerable<MapMarker>), typeof(LeafletMapView), null, propertyChanged: OnMarkersChanged);

        public static readonly BindableProperty RouteCoordinatesProperty =
            BindableProperty.Create(nameof(RouteCoordinates), typeof(IEnumerable<GeoPoint>), typeof(LeafletMapView), null, propertyChanged: OnMapStateChanged);

        public static readonly BindableProperty RoutePolylineColorProperty =
            BindableProperty.Create(nameof(RoutePolylineColor), typeof(string), typeof(LeafletMapView), "#1E6B39", propertyChanged: OnMapStateChanged);

        public double CenterLatitude
        {
            get => (double)GetValue(CenterLatitudeProperty);
            set => SetValue(CenterLatitudeProperty, value);
        }

        public double CenterLongitude
        {
            get => (double)GetValue(CenterLongitudeProperty);
            set => SetValue(CenterLongitudeProperty, value);
        }

        public int ZoomLevel
        {
            get => (int)GetValue(ZoomLevelProperty);
            set => SetValue(ZoomLevelProperty, value);
        }

        public IEnumerable<MapMarker>? Markers
        {
            get => (IEnumerable<MapMarker>?)GetValue(MarkersProperty);
            set => SetValue(MarkersProperty, value);
        }

        public IEnumerable<GeoPoint>? RouteCoordinates
        {
            get => (IEnumerable<GeoPoint>?)GetValue(RouteCoordinatesProperty);
            set => SetValue(RouteCoordinatesProperty, value);
        }

        public string RoutePolylineColor
        {
            get => (string)GetValue(RoutePolylineColorProperty);
            set => SetValue(RoutePolylineColorProperty, value);
        }

        public LeafletMapView()
        {
            _webView = new WebView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.Transparent
            };

            _webView.Navigated += (s, e) => _isLoaded = true;

            Content = _webView;
            RefreshMap();
        }

        private static void OnMapStateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LeafletMapView mapView)
            {
                mapView.RefreshMap();
            }
        }

        private static void OnMarkersChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LeafletMapView mapView)
            {
                if (oldValue is INotifyCollectionChanged oldColl)
                {
                    oldColl.CollectionChanged -= mapView.OnMarkersCollectionChanged;
                }
                if (newValue is INotifyCollectionChanged newColl)
                {
                    newColl.CollectionChanged += mapView.OnMarkersCollectionChanged;
                }
                mapView.RefreshMap();
            }
        }

        private void OnMarkersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshMap();
        }

        /// <summary>
        /// Regenerates the Leaflet HTML and updates the WebView source.
        /// </summary>
        public void RefreshMap()
        {
            try
            {
                var html = GenerateLeafletHtml();
                _webView.Source = new HtmlWebViewSource { Html = html };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LeafletMapView] Error refreshing map: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the courier position in real-time via JavaScript interop without full page reload.
        /// </summary>
        public async Task UpdateCourierPositionAsync(double lat, double lon)
        {
            if (!_isLoaded) return;
            try
            {
                var latStr = lat.ToString(CultureInfo.InvariantCulture);
                var lonStr = lon.ToString(CultureInfo.InvariantCulture);
                await _webView.EvaluateJavaScriptAsync($"if(window.updateCourierPosition) {{ window.updateCourierPosition({latStr}, {lonStr}); }}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LeafletMapView] JS update error: {ex.Message}");
            }
        }

        private string GenerateLeafletHtml()
        {
            var latStr = CenterLatitude.ToString(CultureInfo.InvariantCulture);
            var lonStr = CenterLongitude.ToString(CultureInfo.InvariantCulture);
            var zoom = ZoomLevel;
            var polylineColor = RoutePolylineColor ?? "#1E6B39";

            // Serialize markers JSON
            var markersList = Markers?.ToList() ?? [];
            var markerItems = markersList.Select(m => new
            {
                id = m.Id,
                title = m.Title,
                desc = m.Description,
                lat = m.Latitude,
                lon = m.Longitude,
                type = m.MarkerType,
                emoji = m.IconEmoji,
                color = m.BadgeColor
            });
            var markersJson = JsonSerializer.Serialize(markerItems);

            // Serialize route coordinates JSON
            var routeList = RouteCoordinates?.ToList() ?? [];
            var routePoints = routeList.Select(p => new[] { p.Latitude, p.Longitude });
            var routeJson = JsonSerializer.Serialize(routePoints);

            return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
                <title>FreshMart Map</title>
                <!-- Leaflet CSS -->
                <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    html, body, #map { width: 100%; height: 100%; overflow: hidden; background: #E8F5E9; font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; }
                    
                    /* Custom Pin Styling */
                    .custom-marker-pin {
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        border-radius: 50%;
                        border: 2.5px solid #FFFFFF;
                        box-shadow: 0 4px 10px rgba(0,0,0,0.35);
                        font-size: 16px;
                        font-weight: bold;
                        cursor: pointer;
                        transition: transform 0.2s ease;
                    }
                    .custom-marker-pin:hover {
                        transform: scale(1.15);
                    }
                    .pulse-ring {
                        position: absolute;
                        width: 38px;
                        height: 38px;
                        border-radius: 50%;
                        background: rgba(30, 107, 57, 0.4);
                        animation: pulse 1.8s infinite;
                        z-index: -1;
                    }
                    @keyframes pulse {
                        0% { transform: scale(0.9); opacity: 0.8; }
                        70% { transform: scale(1.6); opacity: 0; }
                        100% { transform: scale(1.6); opacity: 0; }
                    }
                    .leaflet-popup-content-wrapper {
                        border-radius: 12px;
                        padding: 4px;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                    }
                    .popup-title {
                        font-weight: bold;
                        font-size: 13px;
                        color: #1E6B39;
                        margin-bottom: 2px;
                    }
                    .popup-desc {
                        font-size: 11px;
                        color: #4A5568;
                    }
                </style>
            </head>
            <body>
                <div id="map"></div>

                <!-- Leaflet JS -->
                <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
                <script>
                    var map = L.map('map', {
                        zoomControl: false,
                        attributionControl: false
                    }).setView([{{latStr}}, {{lonStr}}], {{zoom}});

                    // OpenStreetMap Tile Layer
                    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
                        maxZoom: 19
                    }).addTo(map);

                    // Add custom zoom control in bottom right
                    L.control.zoom({ position: 'bottomright' }).addTo(map);

                    var markersData = {{markersJson}};
                    var routeData = {{routeJson}};
                    var polylineColor = '{{polylineColor}}';
                    var courierMarkerInstance = null;
                    var bounds = L.latLngBounds();

                    // Function to create HTML icons
                    function createCustomIcon(emoji, bgColor, isPulse) {
                        var html = '<div class="custom-marker-pin" style="background-color:' + bgColor + '; width:34px; height:34px;">' +
                                   (isPulse ? '<div class="pulse-ring"></div>' : '') +
                                   '<span>' + emoji + '</span></div>';
                        return L.divIcon({
                            className: 'leaflet-custom-marker',
                            html: html,
                            iconSize: [34, 34],
                            iconAnchor: [17, 17],
                            popupAnchor: [0, -18]
                        });
                    }

                    // Render Markers
                    if (markersData && markersData.length > 0) {
                        markersData.forEach(function(m) {
                            var isPulse = (m.type === 'Courier');
                            var icon = createCustomIcon(m.emoji || '📍', m.color || '#1E6B39', isPulse);
                            var marker = L.marker([m.lat, m.lon], { icon: icon }).addTo(map);
                            
                            var popupHtml = '<div class="popup-title">' + m.title + '</div><div class="popup-desc">' + m.desc + '</div>';
                            marker.bindPopup(popupHtml);

                            if (m.type === 'Courier') {
                                courierMarkerInstance = marker;
                            }
                            bounds.extend([m.lat, m.lon]);
                        });
                    }

                    // Render Route Polyline
                    if (routeData && routeData.length > 1) {
                        var latlngs = routeData.map(function(pt) { return [pt[0], pt[1]]; });
                        var polyline = L.polyline(latlngs, {
                            color: polylineColor,
                            weight: 5,
                            opacity: 0.85,
                            lineCap: 'round',
                            lineJoin: 'round'
                        }).addTo(map);

                        // Decorative inner dashed line
                        L.polyline(latlngs, {
                            color: '#FFFFFF',
                            weight: 2,
                            dashArray: '5, 8',
                            opacity: 0.95
                        }).addTo(map);

                        polyline.getLatLngs().forEach(function(latlng) {
                            bounds.extend(latlng);
                        });
                    }

                    // Fit map bounds if we have points
                    if (bounds.isValid()) {
                        map.fitBounds(bounds, { padding: [30, 30], maxZoom: 16 });
                    }

                    // JS Interop Function for live courier animation
                    window.updateCourierPosition = function(newLat, newLon) {
                        if (courierMarkerInstance) {
                            courierMarkerInstance.setLatLng([newLat, newLon]);
                        }
                    };
                </script>
            </body>
            </html>
            """;
        }
    }
}
