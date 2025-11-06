using System.Text.Json;
using test1140101.Models;

namespace test1140101.Services
{
    /// <summary>
    /// 交通照相機資料服務類別
    /// </summary>
    public class TrafficCameraService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public TrafficCameraService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        /// <summary>
        /// 從 JSON 檔案讀取並反序列化交通照相機資料
        /// </summary>
        /// <param name="filePath">JSON 檔案路徑</param>
        /// <returns>反序列化後的 API 回應物件</returns>
        public async Task<ApiResponse?> LoadFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"找不到檔案: {filePath}");
                }

                string jsonContent = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);
                return JsonSerializer.Deserialize<ApiResponse>(jsonContent, _jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"JSON 格式錯誤: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"讀取檔案時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 從 JSON 字串反序列化交通照相機資料
        /// </summary>
        /// <param name="jsonString">JSON 字串</param>
        /// <returns>反序列化後的 API 回應物件</returns>
        public ApiResponse? LoadFromString(string jsonString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    throw new ArgumentException("JSON 字串不能為空", nameof(jsonString));
                }

                return JsonSerializer.Deserialize<ApiResponse>(jsonString, _jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"JSON 格式錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 將交通照相機資料序列化為 JSON 字串
        /// </summary>
        /// <param name="data">要序列化的資料</param>
        /// <returns>JSON 字串</returns>
        public string SerializeToString(ApiResponse data)
        {
            try
            {
                return JsonSerializer.Serialize(data, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"序列化時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 將交通照相機資料儲存到 JSON 檔案
        /// </summary>
        /// <param name="data">要儲存的資料</param>
        /// <param name="filePath">儲存檔案路徑</param>
        public async Task SaveToFileAsync(ApiResponse data, string filePath)
        {
            try
            {
                string jsonString = SerializeToString(data);
                await File.WriteAllTextAsync(filePath, jsonString, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"儲存檔案時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 根據縣市名稱篩選交通照相機資料
        /// </summary>
        /// <param name="data">原始資料</param>
        /// <param name="cityName">縣市名稱</param>
        /// <returns>篩選後的照相機清單</returns>
        public List<TrafficCamera> FilterByCity(ApiResponse data, string cityName)
        {
            if (data?.Result?.Records == null)
                return new List<TrafficCamera>();

            return data.Result.Records
                .Where(camera => camera.CityName.Contains(cityName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// 根據速限篩選交通照相機資料
        /// </summary>
        /// <param name="data">原始資料</param>
        /// <param name="speedLimit">速限</param>
        /// <returns>篩選後的照相機清單</returns>
        public List<TrafficCamera> FilterBySpeedLimit(ApiResponse data, int speedLimit)
        {
            if (data?.Result?.Records == null)
                return new List<TrafficCamera>();

            return data.Result.Records
                .Where(camera => camera.LimitValue == speedLimit)
                .ToList();
        }

        /// <summary>
        /// 根據方向篩選交通照相機資料
        /// </summary>
        /// <param name="data">原始資料</param>
        /// <param name="direction">方向 (如: 往北、往南等)</param>
        /// <returns>篩選後的照相機清單</returns>
        public List<TrafficCamera> FilterByDirection(ApiResponse data, string direction)
        {
            if (data?.Result?.Records == null)
                return new List<TrafficCamera>();

            return data.Result.Records
                .Where(camera => camera.Direct.Contains(direction, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// 取得所有不重複的縣市名稱
        /// </summary>
        /// <param name="data">原始資料</param>
        /// <returns>縣市名稱清單</returns>
        public List<string> GetUniqueCities(ApiResponse data)
        {
            if (data?.Result?.Records == null)
                return new List<string>();

            return data.Result.Records
                .Select(camera => camera.CityName)
                .Where(city => !string.IsNullOrWhiteSpace(city))
                .Distinct()
                .OrderBy(city => city)
                .ToList();
        }

        /// <summary>
        /// 取得資料統計資訊
        /// </summary>
        /// <param name="data">原始資料</param>
        /// <returns>統計資訊</returns>
        public TrafficCameraStatistics GetStatistics(ApiResponse data)
        {
            if (data?.Result?.Records == null)
                return new TrafficCameraStatistics();

            var cameras = data.Result.Records;

            return new TrafficCameraStatistics
            {
                TotalCameras = cameras.Count,
                UniqueCities = GetUniqueCities(data).Count,
                AverageSpeedLimit = cameras.Where(c => c.LimitValue > 0).Average(c => c.LimitValue),
                MaxSpeedLimit = cameras.Where(c => c.LimitValue > 0).Max(c => c.LimitValue),
                MinSpeedLimit = cameras.Where(c => c.LimitValue > 0).Min(c => c.LimitValue),
                DirectionCounts = cameras
                    .GroupBy(c => c.Direct)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }

    /// <summary>
    /// 交通照相機統計資訊
    /// </summary>
    public class TrafficCameraStatistics
    {
        public int TotalCameras { get; set; }
        public int UniqueCities { get; set; }
        public double AverageSpeedLimit { get; set; }
        public int MaxSpeedLimit { get; set; }
        public int MinSpeedLimit { get; set; }
        public Dictionary<string, int> DirectionCounts { get; set; } = new();

        public override string ToString()
        {
            return $"總照相機數: {TotalCameras}, 縣市數: {UniqueCities}, 平均速限: {AverageSpeedLimit:F1}km/h";
        }
    }
}