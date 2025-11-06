using System.Text.Json.Serialization;

namespace test1140101.Models
{
    /// <summary>
    /// 表示 API 回應的根物件
    /// </summary>
    public class ApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("result")]
        public ApiResult Result { get; set; } = new();
    }

    /// <summary>
    /// 表示 API 回應的結果部分
    /// </summary>
    public class ApiResult
    {
        [JsonPropertyName("resource_id")]
        public string ResourceId { get; set; } = string.Empty;

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("fields")]
        public List<ApiField> Fields { get; set; } = new();

        [JsonPropertyName("records")]
        public List<TrafficCamera> Records { get; set; } = new();
    }

    /// <summary>
    /// 表示 API 欄位定義
    /// </summary>
    public class ApiField
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    /// <summary>
    /// 表示交通執法照相機資訊
    /// </summary>
    public class TrafficCamera
    {
        /// <summary>
        /// 縣市名稱
        /// </summary>
        [JsonPropertyName("CityName")]
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// 區域名稱
        /// </summary>
        [JsonPropertyName("RegionName")]
        public string RegionName { get; set; } = string.Empty;

        /// <summary>
        /// 地址
        /// </summary>
        [JsonPropertyName("Address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 部門名稱
        /// </summary>
        [JsonPropertyName("DeptNm")]
        public string DeptNm { get; set; } = string.Empty;

        /// <summary>
        /// 分隊名稱
        /// </summary>
        [JsonPropertyName("BranchNm")]
        public string BranchNm { get; set; } = string.Empty;

        /// <summary>
        /// 經度
        /// </summary>
        [JsonPropertyName("Longitude")]
        public string Longitude { get; set; } = string.Empty;

        /// <summary>
        /// 緯度
        /// </summary>
        [JsonPropertyName("Latitude")]
        public string Latitude { get; set; } = string.Empty;

        /// <summary>
        /// 方向 (往北、往南、往東、往西等)
        /// </summary>
        [JsonPropertyName("direct")]
        public string Direct { get; set; } = string.Empty;

        /// <summary>
        /// 速限
        /// </summary>
        [JsonPropertyName("limit")]
        public string Limit { get; set; } = string.Empty;

        // 便利屬性，將字串轉換為數值型別
        /// <summary>
        /// 經度 (double 型別)
        /// </summary>
        public double LongitudeValue
        {
            get => double.TryParse(Longitude, out double value) ? value : 0.0;
        }

        /// <summary>
        /// 緯度 (double 型別)
        /// </summary>
        public double LatitudeValue
        {
            get => double.TryParse(Latitude, out double value) ? value : 0.0;
        }

        /// <summary>
        /// 速限 (int 型別)
        /// </summary>
        public int LimitValue
        {
            get => int.TryParse(Limit, out int value) ? value : 0;
        }

        /// <summary>
        /// 回傳詳細資訊的字串表示
        /// </summary>
        /// <returns>格式化的字串資訊</returns>
        public override string ToString()
        {
            return $"{CityName} {Address} - {Direct} (速限: {Limit}km/h)";
        }
    }
}