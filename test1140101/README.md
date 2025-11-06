# 交通執法照相機資料反序列化模組

這個專案提供了完整的 C# 反序列化模組，用於處理交通執法照相機的 JSON 資料。

## 專案結構

```
test1140101/
├── Models/
│   └── TrafficCameraModels.cs    # 資料模型定義
├── Services/
│   ├── TrafficCameraService.cs   # 主要服務類別
│   └── LocationService.cs        # 地理位置服務
├── Examples/
│   └── TrafficCameraExamples.cs  # 使用範例
├── data/
│   └── A01010000C-000674-011.json # 原始 JSON 資料
└── Program.cs                     # 主程式
```

## 主要類別說明

### 1. 資料模型 (Models)

#### `ApiResponse`
- 表示 API 回應的根物件
- 包含 `Success` 狀態和 `Result` 資料

#### `TrafficCamera`
- 表示單一交通照相機的資訊
- 包含位置、速限、方向等屬性
- 提供便利屬性來轉換字串為數值型別

### 2. 服務類別 (Services)

#### `TrafficCameraService`
- 提供 JSON 檔案讀取和反序列化功能
- 支援資料篩選和統計分析
- 包含序列化/反序列化方法

#### `LocationService`
- 提供地理位置相關的計算功能
- 支援距離計算和範圍查詢
- 使用 Haversine 公式計算地球表面距離

## 使用方法

### 1. 基本使用

```csharp
var service = new TrafficCameraService();

// 從檔案讀取資料
var data = await service.LoadFromFileAsync("data/A01010000C-000674-011.json");

if (data != null && data.Success)
{
    Console.WriteLine($"讀取了 {data.Result.Records.Count} 筆照相機資料");
    
    // 顯示前5筆資料
    foreach (var camera in data.Result.Records.Take(5))
    {
        Console.WriteLine(camera.ToString());
    }
}
```

### 2. 資料篩選

```csharp
// 按縣市篩選
var taipeiCameras = service.FilterByCity(data, "台北市");

// 按速限篩選
var speed100Cameras = service.FilterBySpeedLimit(data, 100);

// 按方向篩選
var northBoundCameras = service.FilterByDirection(data, "往北");
```

### 3. 地理位置查詢

```csharp
// 找出指定位置附近的照相機
double targetLat = 25.0478;  // 台北車站緯度
double targetLon = 121.5173; // 台北車站經度
double radiusKm = 5.0;       // 搜尋半徑

var nearbyCameras = LocationService.FindNearbyCameras(
    data.Result.Records, targetLat, targetLon, radiusKm);

foreach (var camera in nearbyCameras.Take(10))
{
    Console.WriteLine($"{camera.Camera} (距離: {camera.DistanceKm:F2}km)");
}
```

### 4. 統計分析

```csharp
var statistics = service.GetStatistics(data);

Console.WriteLine($"總照相機數: {statistics.TotalCameras}");
Console.WriteLine($"涵蓋縣市數: {statistics.UniqueCities}");
Console.WriteLine($"平均速限: {statistics.AverageSpeedLimit:F1} km/h");

// 方向分佈統計
foreach (var direction in statistics.DirectionCounts.OrderByDescending(x => x.Value).Take(5))
{
    Console.WriteLine($"{direction.Key}: {direction.Value} 個");
}
```

### 5. 序列化/反序列化

```csharp
// 序列化為 JSON 字串
string jsonString = service.SerializeToString(data);

// 從 JSON 字串反序列化
var deserializedData = service.LoadFromString(jsonString);

// 儲存到檔案
await service.SaveToFileAsync(data, "output.json");
```

## 資料格式

原始 JSON 包含以下欄位：

- `CityName`: 縣市名稱 (如: "國道一號", "台北市")
- `RegionName`: 區域名稱
- `Address`: 詳細地址
- `DeptNm`: 管轄部門
- `BranchNm`: 分隊名稱
- `Longitude`: 經度 (字串格式)
- `Latitude`: 緯度 (字串格式)
- `direct`: 執法方向 (如: "往北", "往南")
- `limit`: 速限 (字串格式)

## 便利功能

### 型別轉換屬性
- `LongitudeValue`: 將經度字串轉換為 double
- `LatitudeValue`: 將緯度字串轉換為 double
- `LimitValue`: 將速限字串轉換為 int

### 地理計算
- 使用 Haversine 公式計算兩點間距離
- 支援範圍查詢和邊界計算
- 提供地理統計功能

## 錯誤處理

所有服務方法都包含完整的錯誤處理：

- `FileNotFoundException`: 檔案不存在
- `InvalidDataException`: JSON 格式錯誤
- `ApplicationException`: 其他應用程式錯誤

## 執行範例

```bash
cd test1140101
dotnet run
```

這將執行主程式，展示各種功能的使用方法並輸出統計資訊。

## 效能考量

- 使用 `System.Text.Json` 進行高效能序列化
- 支援大型資料集處理
- 提供 LINQ 查詢優化
- 非同步方法支援檔案 I/O

## 擴充性

這個模組設計為可擴充的架構：

- 可以輕鬆添加新的篩選方法
- 支援自訂統計計算
- 可整合其他地理資訊系統
- 支援不同的資料格式輸入

## 授權

這個專案僅供學習和研究使用。