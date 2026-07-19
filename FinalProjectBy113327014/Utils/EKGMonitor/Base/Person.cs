using MyForms.Utils.EKGMonitor.Person;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForms.Utils.EKGMonitor.Base
{
    public enum sexual
    {
        None = 0,
        Male,
        Female,
        Other = 3
    }
    public class PersonBase
    {
        protected string _firstName;
        protected string _lastName;
        protected string _id;


        public string FirstName 
        {
            get { return _firstName; }
            set 
            {
            if (value[0] != value.ToUpper()[0])
                {
                    throw new ArgumentException("姓名首字母應該大寫，或不是字母");
                }
                _firstName = value;
            }
        }
        public string LastName 
        {
            get { return _lastName; }
            set
            {
                if (value[0] != value.ToUpper()[0])
                {
                    throw new ArgumentException("姓名首字母應該大寫，或不是字母");
                }
                _lastName = value;
            }
        }
        public int SerialNum { get; protected set; }
        public string ID
        {
            get { return _id; }
            set
            {
                var pattern = @"^[A-Z][1-2]\d{8}$";//中華民國身分證格式的正則表達式，其他國家忽略
                if (!System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), pattern))
                {
                    throw new ArgumentException("身分證格式不正確，例：A123456789");
                }
                _id = value;
            }
        }
        public sexual Sexual { get; set; }

        public override string ToString()
        {
            return $"{FirstName}_{LastName}";
        }
        
        /// <summary>
        /// 將物件轉換為 JSON 字串
        /// </summary>
        /// <param name="formatting">JSON格式化選項</param>
        /// <returns>JSON字串</returns>
        public virtual string ToJson(Formatting formatting = Formatting.Indented)
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
        /// 從 JSON 字串轉換為 PersonBase 物件（泛型版本）
        /// </summary>
        /// <typeparam name="T">目標類型</typeparam>
        /// <param name="json">JSON字串</param>
        /// <returns>指定類型的物件</returns>
        public static T FromJson<T>(string json) where T : PersonBase
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
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"從JSON反序列化時發生錯誤: {ex.Message}", ex);
            }
        }
    }
}
