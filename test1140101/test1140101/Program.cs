using test1140101.Models;
using test1140101.Services;

namespace test1140101;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            // 創建交通照相機服務實例
            var trafficCameraService = new TrafficCameraService();
            
            // JSON 檔案路徑
            string jsonFilePath = Path.Combine("data", "A01010000C-000674-011.json");
            
            Console.WriteLine("正在讀取交通照相機資料...");
            
            // 從檔案讀取並反序列化 JSON 資料
            ApiResponse? apiResponse = await trafficCameraService.LoadFromFileAsync(jsonFilePath);
            
            if (apiResponse == null || !apiResponse.Success)
            {
                Console.WriteLine("讀取資料失敗或資料格式錯誤");
                return;
            }

            Console.WriteLine($"資料讀取成功！");
            Console.WriteLine($"資源 ID: {apiResponse.Result.ResourceId}");
            Console.WriteLine($"總筆數: {apiResponse.Result.Total}");
            Console.WriteLine($"照相機數量: {apiResponse.Result.Records.Count}");
            Console.WriteLine();

            // 取得統計資訊
            var statistics = trafficCameraService.GetStatistics(apiResponse);
            Console.WriteLine("=== 統計資訊 ===");
            Console.WriteLine(statistics.ToString());
            Console.WriteLine($"最高速限: {statistics.MaxSpeedLimit}km/h");
            Console.WriteLine($"最低速限: {statistics.MinSpeedLimit}km/h");
            Console.WriteLine();

            // 顯示方向統計
            Console.WriteLine("=== 方向統計 ===");
            foreach (var direction in statistics.DirectionCounts.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{direction.Key}: {direction.Value} 個");
            }
            Console.WriteLine();

            // 取得所有縣市
            var cities = trafficCameraService.GetUniqueCities(apiResponse);
            Console.WriteLine($"=== 涵蓋縣市 ({cities.Count}個) ===");
            foreach (var city in cities.Take(10)) // 只顯示前10個
            {
                var cityCount = trafficCameraService.FilterByCity(apiResponse, city).Count;
                Console.WriteLine($"{city}: {cityCount} 個照相機");
            }
            if (cities.Count > 10)
            {
                Console.WriteLine($"... 還有 {cities.Count - 10} 個縣市");
            }
            Console.WriteLine();

            // 篩選範例：顯示國道一號的照相機
            Console.WriteLine("=== 國道一號照相機 (前5個) ===");
            var highway1Cameras = trafficCameraService.FilterByCity(apiResponse, "國道一號");
            foreach (var camera in highway1Cameras.Take(5))
            {
                Console.WriteLine($"{camera.Address} - {camera.Direct} (速限: {camera.Limit}km/h)");
                Console.WriteLine($"  座標: ({camera.LongitudeValue}, {camera.LatitudeValue})");
                Console.WriteLine($"  管轄: {camera.DeptNm} {camera.BranchNm}");
                Console.WriteLine();
            }

            // 篩選範例：顯示速限100km/h的照相機
            Console.WriteLine("=== 速限100km/h照相機 (前3個) ===");
            var speedLimit100Cameras = trafficCameraService.FilterBySpeedLimit(apiResponse, 100);
            foreach (var camera in speedLimit100Cameras.Take(3))
            {
                Console.WriteLine(camera.ToString());
            }
            Console.WriteLine();

            // 篩選範例：顯示往北方向的照相機
            Console.WriteLine("=== 往北方向照相機 (前3個) ===");
            var northBoundCameras = trafficCameraService.FilterByDirection(apiResponse, "往北");
            foreach (var camera in northBoundCameras.Take(3))
            {
                Console.WriteLine(camera.ToString());
            }

            Console.WriteLine("\n資料處理完成！");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"檔案找不到: {ex.Message}");
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine($"資料格式錯誤: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生錯誤: {ex.Message}");
        }
        
        Console.WriteLine("\n按任意鍵結束...");
        Console.ReadKey();
    }
}
