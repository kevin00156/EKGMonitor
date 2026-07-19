using MyForms.Utils.EKGMonitor.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using MyForms.Utils.EKGMonitor.Person;

namespace MyForms.Utils.EKGMonitor.JsonConverter
{
    /// <summary>
    /// Doctor類別的自定義JSON轉換器
    /// 解決患者列表中User物件重複序列化records的問題
    /// </summary>
    public class DoctorJsonConverter : JsonConverter<Doctor>
    {
        public override void WriteJson(JsonWriter writer, Doctor value, JsonSerializer serializer)
        {
            var jo = new JObject();
            
            // 序列化基本屬性
            jo["FirstName"] = value.FirstName;
            jo["LastName"] = value.LastName;
            jo["SerialNum"] = value.SerialNum;
            jo["ID"] = value.ID;
            jo["Sexual"] = (int)value.Sexual;
            
            // 序列化患者列表，但不包含records
            var patientsArray = new JArray();
            foreach (var patient in value.Patients)
            {
                var patientObj = new JObject();
                patientObj["serialNum"] = 2; // 保持與原始結構一致
                patientObj["FirstName"] = patient.FirstName;
                patientObj["LastName"] = patient.LastName;
                patientObj["SerialNum"] = patient.SerialNum;
                patientObj["ID"] = patient.ID;
                patientObj["Sexual"] = (int)patient.Sexual;
                // 故意不包含records，避免重複序列化
                patientObj["records"] = new JArray(); // 空陣列
                
                patientsArray.Add(patientObj);
            }
            jo["patients"] = patientsArray;
            jo["serialNum"] = 1; // 保持與原始結構一致
            
            jo.WriteTo(writer);
        }

        public override Doctor ReadJson(JsonReader reader, Type objectType, Doctor existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            
            // 反序列化基本屬性
            var firstName = jo["FirstName"]?.ToString();
            var lastName = jo["LastName"]?.ToString();
            var id = jo["ID"]?.ToString();
            var sexual = (sexual)(jo["Sexual"]?.ToObject<int>() ?? 0);
            
            // 創建Doctor物件
            var doctor = new Doctor(firstName, lastName, id, sexual);
            
            // 設置SerialNum（如果需要保持原始值）
            if (jo["SerialNum"] != null)
            {
                var serialNumField = typeof(Doctor).BaseType.GetProperty("SerialNum");
                serialNumField?.SetValue(doctor, jo["SerialNum"].ToObject<int>());
            }
            
            // 收集患者ID列表
            var patientIds = new List<string>();
            var patientsArray = jo["patients"] as JArray;
            if (patientsArray != null)
            {
                foreach (var patientToken in patientsArray)
                {
                    var patientId = patientToken["ID"]?.ToString();
                    if (!string.IsNullOrEmpty(patientId))
                    {
                        patientIds.Add(patientId);
                    }
                }
            }
            
            // 設置臨時患者ID列表
            doctor.TempPatientIds = patientIds;
            
            return doctor;
        }
    }
} 