using MyForms.Utils.EKGMonitor.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyForms.Utils.EKGMonitor.Person
{
    /// <summary>
    /// 醫療資料容器類別，用於管理多個醫生和病人的資料
    /// </summary>
    public class MedicalDataContainer
    {
        /// <summary>
        /// 醫生列表
        /// </summary>
        [JsonProperty("doctors")]
        public List<Doctor> Doctors { get; set; } = new List<Doctor>();

        /// <summary>
        /// 病人列表
        /// </summary>
        [JsonProperty("users")]
        public List<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// 資料建立時間
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 資料更新時間
        /// </summary>
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 版本資訊
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 無參數建構函式
        /// </summary>
        public MedicalDataContainer()
        {
        }

        /// <summary>
        /// 帶參數建構函式
        /// </summary>
        /// <param name="doctors">醫生列表</param>
        /// <param name="users">病人列表</param>
        public MedicalDataContainer(List<Doctor> doctors, List<User> users)
        {
            Doctors = doctors ?? new List<Doctor>();
            Users = users ?? new List<User>();
        }

        /// <summary>
        /// 新增醫生
        /// </summary>
        /// <param name="doctor">醫生物件</param>
        public void AddDoctor(Doctor doctor)
        {
            if (doctor != null && !Doctors.Any(d => d.ID == doctor.ID))
            {
                Doctors.Add(doctor);
                UpdatedAt = DateTime.Now;
            }
        }

        /// <summary>
        /// 新增病人
        /// </summary>
        /// <param name="user">病人物件</param>
        public void AddUser(User user)
        {
            if (user != null && !Users.Any(u => u.ID == user.ID))
            {
                Users.Add(user);
                UpdatedAt = DateTime.Now;
            }
        }

        /// <summary>
        /// 移除醫生
        /// </summary>
        /// <param name="doctorId">醫生ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveDoctor(string doctorId)
        {
            var doctor = Doctors.FirstOrDefault(d => d.ID == doctorId);
            if (doctor != null)
            {
                Doctors.Remove(doctor);
                UpdatedAt = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除病人
        /// </summary>
        /// <param name="userId">病人ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveUser(string userId)
        {
            var user = Users.FirstOrDefault(u => u.ID == userId);
            if (user != null)
            {
                Users.Remove(user);
                UpdatedAt = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 將容器內容序列化為JSON字串
        /// </summary>
        /// <param name="formatting">JSON格式化選項</param>
        /// <returns>JSON字串</returns>
        public string ToJson(Formatting formatting = Formatting.Indented)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                return JsonConvert.SerializeObject(this, formatting, settings);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"序列化為JSON時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 從JSON字串反序列化為容器物件
        /// </summary>
        /// <param name="json">JSON字串</param>
        /// <returns>醫療資料容器物件</returns>
        public static MedicalDataContainer FromJson(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    throw new ArgumentException("JSON字串不能為空");

                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                var container = JsonConvert.DeserializeObject<MedicalDataContainer>(json, settings);
                
                // 重建Doctor與User之間的參考關係
                container.RebuildDoctorPatientReferences();
                
                return container;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"從JSON反序列化時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 重建Doctor與User之間的參考關係
        /// </summary>
        private void RebuildDoctorPatientReferences()
        {
            foreach (var doctor in Doctors)
            {
                doctor.RebuildPatientReferences(Users);
            }
        }

        /// <summary>
        /// 將容器內容儲存到檔案
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="formatting">JSON格式化選項</param>
        public void SaveToFile(string filePath, Formatting formatting = Formatting.Indented)
        {
            try
            {
                var json = ToJson(formatting);
                File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"儲存檔案時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 從檔案載入容器內容（返回值版本）
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>醫療資料容器物件</returns>
        public static MedicalDataContainer LoadFromFile(string filePath)
        {
            try
            {
                // 檢查檔案是否存在，若不存在則自動建立路徑及檔案
                if (!File.Exists(filePath))
                {
                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory); // 建立目錄
                    }

                    // 建立一個新的 MedicalDataContainer 實例並儲存為 JSON 檔案
                    var newContainer = new MedicalDataContainer();
                    newContainer.SaveToFile(filePath);
                    return newContainer;
                }

                var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                return FromJson(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"載入檔案時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 從檔案載入容器內容（out參數版本）
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="medicalDataContainer">輸出的醫療資料容器物件</param>
        public static void LoadFromFile(string filePath, out MedicalDataContainer medicalDataContainer)
        {
            medicalDataContainer = LoadFromFile(filePath);
        }

        /// <summary>
        /// 取得統計資訊
        /// </summary>
        /// <returns>統計資訊字串</returns>
        public string GetStatistics()
        {
            return $"醫生數量: {Doctors.Count}, 病人數量: {Users.Count}, 建立時間: {CreatedAt:yyyy-MM-dd HH:mm:ss}, 更新時間: {UpdatedAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}