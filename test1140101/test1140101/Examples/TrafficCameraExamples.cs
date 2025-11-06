using test1140101.Models;
using test1140101.Services;

namespace test1140101.Examples
{
    /// <summary>
    /// 展示如何使用交通照相機服務的範例類別
    /// </summary>
    public class TrafficCameraExamples
    {
        private readonly TrafficCameraService _service;

        public TrafficCameraExamples()
        {
            _service = new TrafficCameraService();
        }

        /// <summary>
        /// 基本使用範例
        /// </summary>
        public async Task BasicUsageExample()
        {
            Console.WriteLine("=== 基本使用範例 ===");

            try
            {
                // 讀取資料
                var data = await _service.LoadFromFileAsync(Path.Combine("data", "A01010000C-000674-011.json"));
                
                if (data == null || !data.Success)
                {
                    Console.WriteLine("無法讀取資料");
                    return;
                }

                Console.WriteLine($"成功讀取 {data.Result.Records.Count} 筆照相機資料");
                
                // 顯示前3筆資料
                foreach (var camera in data.Result.Records.Take(3))
                {
                    Console.WriteLine($"- {camera}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 地理位置查詢範例
        /// </summary>
        public async Task LocationQueryExample()
        {
            Console.WriteLine("\n=== 地理位置查詢範例 ===");

            try
            {
                var data = await _service.LoadFromFileAsync(Path.Combine("data", "A01010000C-000674-011.json"));
                
                if (data == null || !data.Success)
                    return;

                // 以台北車站為中心 (25.0478, 121.5173) 搜尋5公里內的照相機
                double taipeiLat = 25.0478;
                double taipeiLon = 121.5173;
                double radiusKm = 5.0;

                var nearbyCameras = LocationService.FindNearbyCameras(
                    data.Result.Records, taipeiLat, taipeiLon, radiusKm);

                Console.WriteLine($"台北車站 {radiusKm}km 內找到 {nearbyCameras.Count} 個照相機:");
                
                foreach (var camera in nearbyCameras.Take(5))
                {
                    Console.WriteLine($"- {camera}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 資料篩選範例
        /// </summary>
        public async Task FilteringExample()
        {
            Console.WriteLine("\n=== 資料篩選範例 ===");

            try
            {
                var data = await _service.LoadFromFileAsync(Path.Combine("data", "A01010000C-000674-011.json"));
                
                if (data == null || !data.Success)
                    return;

                // 篩選台北市的照相機
                var taipeiCameras = _service.FilterByCity(data, "台北市");
                Console.WriteLine($"台北市照相機數量: {taipeiCameras.Count}");

                // 篩選速限60km/h的照相機
                var speed60Cameras = _service.FilterBySpeedLimit(data, 60);
                Console.WriteLine($"速限60km/h照相機數量: {speed60Cameras.Count}");

                // 篩選往北方向的照相機
                var northBoundCameras = _service.FilterByDirection(data, "往北");
                Console.WriteLine($"往北方向照相機數量: {northBoundCameras.Count}");

                // 複合條件篩選：台北市 + 往北方向
                var taipeiNorthCameras = taipeiCameras
                    .Where(c => c.Direct.Contains("往北", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                Console.WriteLine($"台北市往北方向照相機數量: {taipeiNorthCameras.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 統計分析範例
        /// </summary>
        public async Task StatisticsExample()
        {
            Console.WriteLine("\n=== 統計分析範例 ===");

            try
            {
                var data = await _service.LoadFromFileAsync(Path.Combine("data", "A01010000C-000674-011.json"));
                
                if (data == null || !data.Success)
                    return;

                var stats = _service.GetStatistics(data);
                
                Console.WriteLine($"總照相機數: {stats.TotalCameras}");
                Console.WriteLine($"涵蓋縣市數: {stats.UniqueCities}");
                Console.WriteLine($"平均速限: {stats.AverageSpeedLimit:F1} km/h");
                Console.WriteLine($"最高速限: {stats.MaxSpeedLimit} km/h");
                Console.WriteLine($"最低速限: {stats.MinSpeedLimit} km/h");

                Console.WriteLine("\n方向分佈 (前10名):");
                foreach (var direction in stats.DirectionCounts.OrderByDescending(x => x.Value).Take(10))
                {
                    Console.WriteLine($"  {direction.Key}: {direction.Value} 個");
                }

                // 速限分佈統計
                var speedLimitDistribution = data.Result.Records
                    .Where(c => c.LimitValue > 0)
                    .GroupBy(c => c.LimitValue)
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count());

                Console.WriteLine("\n速限分佈:");
                foreach (var speedLimit in speedLimitDistribution.Take(10))
                {
                    Console.WriteLine($"  {speedLimit.Key}km/h: {speedLimit.Value} 個");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// JSON 序列化/反序列化範例
        /// </summary>
        public async Task SerializationExample()
        {
            Console.WriteLine("\n=== 序列化/反序列化範例 ===");

            try
            {
                // 讀取原始資料
                var originalData = await _service.LoadFromFileAsync(Path.Combine("data", "A01010000C-000674-011.json"));
                
                if (originalData == null || !originalData.Success)
                    return;

                // 序列化為 JSON 字串
                string jsonString = _service.SerializeToString(originalData);
                Console.WriteLine($"序列化後 JSON 長度: {jsonString.Length} 字元");

                // 從 JSON 字串反序列化
                var deserializedData = _service.LoadFromString(jsonString);
                
                if (deserializedData != null && deserializedData.Success)
                {
                    Console.WriteLine($"反序列化成功，照相機數量: {deserializedData.Result.Records.Count}");
                    Console.WriteLine("資料完整性檢查通過！");
                }

                // 儲存到新檔案 (示範用)
                string outputPath = Path.Combine("data", "output_example.json");
                await _service.SaveToFileAsync(originalData, outputPath);
                Console.WriteLine($"資料已儲存到: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 執行所有範例
        /// </summary>
        public async Task RunAllExamples()
        {
            await BasicUsageExample();
            await LocationQueryExample();
            await FilteringExample();
            await StatisticsExample();
            await SerializationExample();
        }
    }
}