# EKG Monitor

碩士班一年級下學期「物件導向程式設計」期末專案的封存版本。
C# / WinForms / .NET Framework 4.7.2，約 4,100 行。

本 repo 只做封存整理：移除同步衝突檔與建置產物、補上 `.gitignore`、撰寫這份說明。
程式碼本身維持當時的樣子，未重構、未修正缺陷、未升級框架。

## 這是什麼

一條可插拔訊號源的資料擷取與持久化管線，外加一層 WinForms 操作介面。

管線的形狀是：訊號產生器逐點取樣，寫入六導程的記憶體佇列，累積到閾值後由背景工作
將整批資料落盤成 CSV；人員與病歷的關聯另以 JSON 持久化。

題目由課程指定，包含六導程、人員角色分離、記錄存取這些需求。
系統以模擬訊號驅動是課程規格的一部分，不是實作上的取捨或退讓。

需要先講明白的一件事：本專案不含任何訊號分析。
畫面上顯示的「生理數據」是亂數，不是從輸入訊號算出來的（見下方已知問題 a）。
所以這份程式碼能佐證的是資料流與生命週期的處理，不是生理訊號處理。

## 架構

### 訊號層

`SignalBase` 是抽象基底，持有時間游標 `_time`，定義 `Next()` 由子類實作。
四個具體子類：

| 類別 | 產生方式 |
|---|---|
| `NoiseSignal` | 以隨機種子產生指定範圍的雜訊 |
| `SinWaveSignal` | 指定振幅與頻率的正弦波 |
| `SinWaveWithNoiseSignal` | 前兩者相加 |
| `RealSignal` | 由外部注入的委派供給訊號值 |

`SignalFactory` 以 `SignalType` 列舉集中建構邏輯，並為 `RealSignal` 額外提供
`CreateRealSignalFromFunction` / `FromConstant` / `FromArray` 三個便捷入口。

`RealSignal` 接受一個外部注入的委派：

```csharp
public delegate double SignalProvider(double currentTime);
```

這是預留給真實擷取來源（硬體、預錄檔案）的接縫，**目前未被使用**：
整個 repo 沒有任何程式路徑會建構 `RealSignal`，也沒有測試涵蓋它。
系統之所以無需硬體即可執行與展示，是因為六導程全部由合成產生器驅動，
在 `PortableEKGMonitor.cs:19-27` 寫死為
`{ NOISE, SINWAVEWITHNOISE, SINWAVE, SINWAVEWITHNOISE, SINWAVE, NOISE }`，
而不是因為這個注入點。同理，`SignalBase.Reset()`、`SetTime()`、`CurrentTime`
也都沒有呼叫點。

### 記錄層

`EKGRecord : RecordBase, IDisposable` 是管線的核心：

- 六導程（`LEADS_COUNT = 6`，命名為 I、II、III、aVR、aVL、aVF），
  各自一條 `Queue<float>` 緩衝
- `AddData()` 每次寫入一組六個取樣值
- 緩衝達 `BUFFER_THRESHOLD = 50` 時觸發非同步落盤
- 寫出 CSV，`AutoFlush = true`，所以每批資料在寫出當下就進磁碟
- `ReadDataFromFile()` 可將既有 CSV 讀回為唯讀模式的記錄
- 實作 `IDisposable`：關閉 CSV writer、清空緩衝、取消進行中的寫入工作
  （注意它不會落盤剩餘資料，原因見已知問題 d）

### 人員與資料層

- `PersonBase` 提供姓名、身分證字號（以中華民國格式的正則驗證）、性別欄位，
  以及 Newtonsoft.Json 的序列化包裝
- `User` 與 `Doctor` 繼承自 `PersonBase`；`User` 持有病歷清單，
  並多載了 `+` 運算子建立「記錄加入使用者」的新實例
- `MedicalDataContainer` 聚合醫生與病人清單，負責 JSON 的存讀
- `DoctorJsonConverter` 是自訂的 `JsonConverter`，處理醫生與病人之間的參照關係，
  避免直接序列化造成的循環
- `PersonRegisterForm<T>` 與 `SelectPersonForm<T>` 是泛型表單，
  透過 `delegate T CreatePersonDelegate(...)` 把型別特有的建構邏輯交給呼叫端

### 資料流

```
System.Windows.Forms.Timer (10ms, UI 執行緒)
  -> TimerScanning()          PortableEKGMonitor.cs:168
  -> SignalBase.Next() x6
  -> EKGRecord.AddData()      EKGRecord.cs:282
       -> lock(_dataLock) 入佇列
       -> ScottPlot DataLogger 繪圖
       -> 達閾值則 Task.Run -> SaveBufferedDataToFileAsync()
            -> lock(_fileLock) -> lock(_dataLock) 快照 -> 離開資料鎖 -> 寫 CSV
```

## 併發設計

**生產端是單一生產者。** `AddData` 在整個 repo 只有一個呼叫點
（`PortableEKGMonitor.cs:180`），由 `System.Windows.Forms.Timer` 以 10ms 間隔驅動。
這個 Timer 的回呼會被 marshal 回 UI 訊息迴圈，所以生產端始終在同一條執行緒上。

**取樣資料的寫檔在背景執行。** 觸發點 `EKGRecord.cs:323` 派送一個 `Task.Run`，
而 `SaveBufferedDataToFileAsync` 內部 `:659` 再包一層 `Task.Run`，
實際的 `WriteLine` 與 `Flush` 落在執行緒集區上。

**鎖的取得順序與臨界區大小。** `SaveBufferedDataToFileAsync` 先取 `_fileLock`（`:664`），
再進入 `_dataLock`（`:680`）。資料鎖內只做三件事：檢查是否全空、
把各導程佇列複製一份並清空原佇列、讀取當下的樣本計數（`:683-695`）。
離開資料鎖之後（`:702`）才呼叫 `_WriteDataToFile` 做實際 I/O。
也就是說檔案 I/O 期間不持有資料鎖，生產端可以繼續往空出來的佇列寫入。

**消費端不是單執行緒。** 有三個入口會走到落盤路徑：`AddData` 派送的背景 Task、
`ManualSaveAsync()`（`PortableEKGMonitor.cs:195`，目前無呼叫者）、
以及 `Dispose()`（`:590`）。它們之間的互斥完全由 `_fileLock` 提供。

## 已知問題與現在會怎麼改

以下分兩組。前三項是我自己對這份程式碼的評估；後兩項是這次封存重讀時才發現的。

### 作者自評

**a. `_CalculatePhysiologicalData()` 是 stub。**
`EKGRecord.cs:792-800` 直接回傳 `_random.NextDouble() * 150`，
完全沒有讀取輸入訊號，程式碼裡留著 `//TODO: 替換成真實計算生理數據使用的公式`。
六個標籤顯示的數值與畫面上的波形無關。
訊號分析超出當時的課程範圍，專案不含任何相關實作。
現在會怎麼改：這個方法不該存在於 `EKGRecord`。心率、間期這類推導量屬於分析層，
應該是吃一段取樣視窗、回傳結果的獨立元件，由呼叫端決定何時執行；
把它塞進資料擷取類別，等於強迫每一次 `AddData` 都付一次分析成本
（現況確實如此：`_AddDataToPlots` 在 `:351` 每筆都呼叫一次）。

**b. 自動落盤的判斷是 check-then-act。**
`AddData:320` 的條件 `BufferedDataCount >= BUFFER_THRESHOLD && !IsWritingToFile` 有兩個問題，
但性質不同。`BufferedDataCount`（`:97-107`）內部有取 `_dataLock`，
所以計數的讀取本身受保護；問題在於放鎖之後才做比較與派送 Task，值到那時已可能過期。
`IsWritingToFile`（`:113`）才是真正未同步的讀取：`_isWritingToFile` 沒有標 `volatile`，
整個 repo 也找不到 `volatile` 或 `Interlocked`。

最壞情況是白開一個 Task，不會造成資料損毀。但要說清楚原因：
安全性來自 `_fileLock` 本身的互斥，不是來自 `:666` 的二次檢查。
事實上那個二次檢查的 `_isWritingToFile` 半邊是死碼，因為對該旗標的所有寫入
（`:672`、`:717`、`:750`、`:787`）都在持有 `_fileLock` 時發生，
任何取得該鎖的執行緒必然看到 `false`。多開的那個 Task 會阻塞在鎖上，
拿到鎖之後撞上 `:683` 的全空檢查，然後空手返回。

現在會怎麼改：這個旗標與雙重檢查可以整個拿掉。
用 `Interlocked.CompareExchange` 做單一的門閂，或直接讓落盤由一個消費端迴圈
（`BlockingCollection` 或 `Channel`）承擔，生產端只負責投遞。

**c. `_sampleCount++` 在鎖外，卻在鎖內被讀取。**
遞增發生在 `:290`，讀取發生在 `_dataLock` 內的 `:695` 與 `:773`。
單一生產者模型保證不會遺失遞增，但這個不一致本身是設計瑕疵。
它的實際後果比我原先估計的嚴重，見下方 e。

### 封存時重讀才發現

**d. `Dispose()` 的最終落盤是死碼。**
`:580` 先設 `_disposed = true`，`:590` 才呼叫 `SaveBufferedDataToFile()`，
而該方法開頭 `:738` 的守衛是 `if (_isReadOnlyMode || _disposed) return;`。
條件恆為真，這次呼叫保證是 no-op。
影響範圍有限，因為 `AutoFlush = true` 讓每個達閾值的批次在寫出當下就進磁碟；
損失的是尚未滿 50 筆的那一批尾端取樣。
現在會怎麼改：`_disposed = true` 應該移到落盤之後，或改用一個獨立的
`_acceptingNewData` 旗標來擋新資料，把「不再收」與「已釋放」分成兩個狀態。
這也是為什麼上面架構段落把 Dispose 的行為寫成「取消進行中的寫入」而不是「落盤剩餘資料」。

**e. c 會讓 CSV 的 sample 編號整批偏移。**
關鍵在順序：`_sampleCount++`（`:290`）發生在把對應取樣值放進佇列
（`:296` 的 `lock (_dataLock)` 內，`:312`）之前。
落盤執行緒若剛好在這個窗口取得 `_dataLock`，快照到的計數會比實際緩衝的樣本數多 1。
`_WriteDataToFile` 用 `startSampleCount = endSampleCount - dataCount + 1`（`:819`）
回推起始編號，於是那一批寫出的每一列編號都偏移一格。
所以 c 不只是「不一致但安全」，它會產生錯誤的輸出。
現在會怎麼改：計數與佇列必須在同一個臨界區內一起變更。
把 `_sampleCount++` 移進 `:296` 的鎖內即可，成本是零。

## 建置與還原

需要 Windows 與 Visual Studio 2022（.sln 記錄的版本為 17.11）。
專案使用舊式的 packages.config 管理 NuGet 相依，`packages/` 未納入版本控制。

clone 之後先還原套件，再開啟方案：

```
nuget restore FinalProjectBy113327014.sln
```

這一步不能省略，也不能用 `dotnet restore` 或 `msbuild -t:restore` 代替：
那兩個指令服務的是 PackageReference 格式。`FinalProjectBy113327014.csproj` 結尾
以 `Import` 引入四個位於 `packages/` 底下的 `.targets`
（SkiaSharp 與 HarfBuzzSharp 的原生資產），並有一個 `EnsureNuGetPackageBuildImports`
目標在檔案缺席時直接報錯，所以套件不在位時專案連載入都會失敗。

主要相依：ScottPlot 5.0.55 與 ScottPlot.WinForms（繪圖）、
SkiaSharp 3.119.0（ScottPlot 的算繪後端）、Newtonsoft.Json 13.0.3（序列化）。

### 建置驗證狀態

**編譯已驗證，執行未驗證。**

封存整理在 Linux 上進行，該環境只有 .NET SDK 8.0.423，沒有 .NET Framework 4.7.2 的
參考組件，也沒有 mono。專案仍可在此環境編譯成功（0 警告 0 錯誤，產出 88KB 的 PE32 組件），
但這只證明 4,100 行原始碼通過 Roslyn 的語法與型別檢查。
產物是 WinForms 執行檔，需要 Windows 才能執行，其執行期行為未在此驗證。

過程遇到三個障礙，全部與工具鏈有關，與程式碼無關。三者的解法都透過命令列參數與一個
位於 repo 之外的 props 檔提供，專案檔與原始碼零改動：

| 錯誤 | 原因 | 解法 |
|---|---|---|
| `MSB3644` | 找不到 v4.7.2 參考組件 | `Microsoft.NETFramework.ReferenceAssemblies.net472` 搭配 `FrameworkPathOverride` |
| `MSB4216` | `GenerateResource` 要求 x86 工作主機（舊式專案預設走 resgen.exe） | `GenerateResourceMSBuildArchitecture` 與 `Runtime` 覆寫為 `Current*` |
| `MSB3823` / `MSB3822` | `.resx` 含非字串資源（視窗圖示），需要序列化支援 | `GenerateResourceUsePreserializedResources` 搭配 `System.Resources.Extensions` 參考 |

重現方式（`$P` 為參考組件目錄，`$S` 為存放 props 檔的目錄）：

```
dotnet build FinalProjectBy113327014.sln \
  /p:FrameworkPathOverride="$P" \
  /p:GenerateResourceMSBuildArchitecture=CurrentArchitecture \
  /p:GenerateResourceMSBuildRuntime=CurrentRuntime \
  /p:CustomAfterMicrosoftCommonTargets=$S/linux-build.props
```

其中 `linux-build.props` 設定 `GenerateResourceUsePreserializedResources` 為 true，
並加入指向 `System.Resources.Extensions.dll` 的 `Reference`。
`CustomAfterMicrosoftCommonTargets` 是 MSBuild 的標準擴充點，
用它注入設定可以避免修改 csproj。

在 Windows 與 Visual Studio 2022 上不需要上述任何一項，直接開啟方案建置即可。

另外做過的靜態檢查：csproj 的 16 個 `<HintPath>` 所指組件都存在於 `packages/`；
`EnsureNuGetPackageBuildImports` 硬性要求的 4 個 `.targets` 都存在；
csproj 顯式列舉的 29 個 `<Compile Include>` 與磁碟上的 .cs 檔雙向一致，無缺漏也無孤兒。

## 專案結構

```
FinalProjectBy113327014/
├─ Program.cs                    進入點
├─ Form1.cs                      宿主視窗，掛載 PortableEKGMonitor
├─ PortableEKGMonitor.cs         主要 UserControl：Timer、選單、狀態列
└─ Utils/EKGMonitor/
   ├─ Base/       PersonBase、RecordBase、IRecord
   ├─ Person/     User、Doctor
   ├─ Record/     EKGRecord
   ├─ Signal/     SignalBase 與四個子類、SignalFactory
   ├─ Container/  MedicalDataContainer
   ├─ Forms/      註冊、選取、病歷管理等對話框
   └─ JsonConverter/ DoctorJsonConverter
```

## 範圍聲明

全部程式碼為本人獨立完成。

繪圖由第三方套件 ScottPlot 負責，非本人成果；
其餘 NuGet 相依（SkiaSharp、HarfBuzzSharp、Newtonsoft.Json、OpenTK）同此。

專案名稱與組件名稱保留當時的 `FinalProjectBy113327014`，未改名。
