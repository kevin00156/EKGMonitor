using MyForms.Utils.EKGMonitor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyForms.Utils.EKGMonitor.JsonConverter;

namespace MyForms.Utils.EKGMonitor.Person
{
    [JsonConverter(typeof(DoctorJsonConverter))]
    public class Doctor : PersonBase
    {
        [JsonProperty("patients")]
        List<User> _patients = new List<User>();
        
        [JsonProperty("serialNum")]
        private static int _serialNum = 0;
        
        // 臨時欄位，用於在反序列化過程中存儲患者ID列表
        [JsonIgnore]
        private List<string> _tempPatientIds = new List<string>();
        
        [JsonIgnore]
        public List<User> Patients { get { return _patients; } }

        [JsonIgnore]
        public List<string> TempPatientIds 
        { 
            get { return _tempPatientIds; } 
            set { _tempPatientIds = value ?? new List<string>(); }
        }

        public Doctor(string firstName, string lastName, string id, sexual sexual)
        {
            SerialNum = ++_serialNum;//紀錄醫生流水號, one based

            _firstName = firstName;
            _lastName = lastName;
            ID = id;
            Sexual = sexual;
        }

        public void AddPatient(User user)
        {
            _patients.Add(user);
        }

        public User GetPatient(int serial_num)
        {
            return _patients.FirstOrDefault(p => p.SerialNum == serial_num);
        }

        public User GetPatient(string id)
        {
            return _patients.FirstOrDefault(p => p.ID == id);
        }

        public List<User> GetPatients(string firstName = null, string lastName = null, sexual? sexual = null)
        {
            var result = _patients.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
            {
                result = result.Where(p => p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                result = result.Where(p => p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
            }

            if (sexual.HasValue)
            {
                result = result.Where(p => p.Sexual == sexual.Value);
            }

            return result.ToList();
        }

        public void DeletePatient(User user)
        {
            _patients.Remove(user);
        }

        /// <summary>
        /// 重建患者參考關係（在反序列化後調用）
        /// </summary>
        /// <param name="allUsers">所有可用的User物件列表</param>
        public void RebuildPatientReferences(List<User> allUsers)
        {
            _patients.Clear();
            
            foreach (var patientId in _tempPatientIds)
            {
                var user = allUsers.FirstOrDefault(u => u.ID == patientId);
                if (user != null)
                {
                    _patients.Add(user);
                }
            }
            
            // 清除臨時ID列表
            _tempPatientIds.Clear();
        }

        // User + Doctor 的運算子多載
        public static Doctor operator +(User patient, Doctor doctor)
        {
            return AddPersonToDoctorRelation(patient, doctor);
        }

        // Doctor + User 的運算子多載
        public static Doctor operator +(Doctor doctor, User patient)
        {
            return AddPersonToDoctorRelation(patient, doctor);
        }

        // 私有的共用方法，處理添加病人到醫生的邏輯
        private static Doctor AddPersonToDoctorRelation(User patient, Doctor doctor)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            // 創建一個新的 Doctor 實例，複製原醫生的基本資訊
            var new_doctor = new Doctor(doctor.FirstName, doctor.LastName, doctor.ID, doctor.Sexual);
            
            // 複製原醫生的病人列表
            new_doctor._patients = new List<User>(doctor._patients);
            
            new_doctor.AddPatient(patient);
            
            return new_doctor;
        }
    }
}
