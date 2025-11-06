using System.Text.Json.Serialization;
using test1140101.Models;

namespace test1140101.Services
{
    /// <summary>
    /// 地理位置相關的服務類別
    /// </summary>
    public class LocationService
    {
        /// <summary>
        /// 計算兩點之間的距離 (使用 Haversine 公式)
        /// </summary>
        /// <param name="lat1">點1的緯度</param>
        /// <param name="lon1">點1的經度</param>
        /// <param name="lat2">點2的緯度</param>
        /// <param name="lon2">點2的經度</param>
        /// <returns>距離 (公里)</returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371; // 地球半徑 (公里)
            
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return earthRadius * c;
        }

        /// <summary>
        /// 將角度轉換為弧度
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// 找出指定位置附近的照相機
        /// </summary>
        /// <param name="cameras">照相機清單</param>
        /// <param name="targetLat">目標緯度</param>
        /// <param name="targetLon">目標經度</param>
        /// <param name="radiusKm">搜尋半徑 (公里)</param>
        /// <returns>附近的照相機清單</returns>
        public static List<CameraWithDistance> FindNearbyCameras(
            List<TrafficCamera> cameras, 
            double targetLat, 
            double targetLon, 
            double radiusKm)
        {
            var nearbyCameras = new List<CameraWithDistance>();

            foreach (var camera in cameras)
            {
                if (camera.LongitudeValue != 0 && camera.LatitudeValue != 0)
                {
                    double distance = CalculateDistance(
                        targetLat, targetLon, 
                        camera.LatitudeValue, camera.LongitudeValue);

                    if (distance <= radiusKm)
                    {
                        nearbyCameras.Add(new CameraWithDistance
                        {
                            Camera = camera,
                            DistanceKm = distance
                        });
                    }
                }
            }

            return nearbyCameras.OrderBy(c => c.DistanceKm).ToList();
        }

        /// <summary>
        /// 找出指定區域內的照相機
        /// </summary>
        /// <param name="cameras">照相機清單</param>
        /// <param name="minLat">最小緯度</param>
        /// <param name="maxLat">最大緯度</param>
        /// <param name="minLon">最小經度</param>
        /// <param name="maxLon">最大經度</param>
        /// <returns>區域內的照相機清單</returns>
        public static List<TrafficCamera> FindCamerasInBounds(
            List<TrafficCamera> cameras,
            double minLat, double maxLat,
            double minLon, double maxLon)
        {
            return cameras.Where(camera =>
                camera.LatitudeValue >= minLat && camera.LatitudeValue <= maxLat &&
                camera.LongitudeValue >= minLon && camera.LongitudeValue <= maxLon)
                .ToList();
        }

        /// <summary>
        /// 取得所有照相機的地理邊界
        /// </summary>
        /// <param name="cameras">照相機清單</param>
        /// <returns>地理邊界資訊</returns>
        public static GeographicBounds GetGeographicBounds(List<TrafficCamera> cameras)
        {
            var validCameras = cameras.Where(c => c.LatitudeValue != 0 && c.LongitudeValue != 0).ToList();

            if (!validCameras.Any())
            {
                return new GeographicBounds();
            }

            return new GeographicBounds
            {
                MinLatitude = validCameras.Min(c => c.LatitudeValue),
                MaxLatitude = validCameras.Max(c => c.LatitudeValue),
                MinLongitude = validCameras.Min(c => c.LongitudeValue),
                MaxLongitude = validCameras.Max(c => c.LongitudeValue)
            };
        }
    }

    /// <summary>
    /// 包含距離資訊的照相機資料
    /// </summary>
    public class CameraWithDistance
    {
        public TrafficCamera Camera { get; set; } = new();
        public double DistanceKm { get; set; }

        public override string ToString()
        {
            return $"{Camera} (距離: {DistanceKm:F2}km)";
        }
    }

    /// <summary>
    /// 地理邊界資訊
    /// </summary>
    public class GeographicBounds
    {
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }

        public double CenterLatitude => (MinLatitude + MaxLatitude) / 2;
        public double CenterLongitude => (MinLongitude + MaxLongitude) / 2;

        public override string ToString()
        {
            return $"緯度: {MinLatitude:F6} ~ {MaxLatitude:F6}, " +
                   $"經度: {MinLongitude:F6} ~ {MaxLongitude:F6}";
        }
    }
}