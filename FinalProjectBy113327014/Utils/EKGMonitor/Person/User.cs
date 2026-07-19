using MyForms.Utils.EKGMonitor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; 

namespace MyForms.Utils.EKGMonitor.Person
{
    public class User : PersonBase
    {
        [JsonProperty("records")]
        List<RecordBase> _records = new List<RecordBase>();//紀錄所有的病歷資訊
        
        [JsonProperty("serialNum")]
        private static int _serialNum = 0;
        
        [JsonIgnore]
        public List<RecordBase> Records { get { return _records; } }

        public User(string firstName, string lastName, string id, sexual sexual)
        {
            SerialNum = ++_serialNum;//紀錄病人流水號, one based

            _firstName = firstName;
            _lastName = lastName;
            ID = id;
            Sexual = sexual;

        }

        public void NewRecord(RecordBase record)
        {
            _records.Add(record);
        }

        public void AddRecords(List<RecordBase> records)
        {
            foreach (var record in records)
            {
                this.NewRecord(record);
            }
        }

        public void DeleteRecord(RecordBase record)
        {
            _records.Remove(record);
        }

        public static User operator +(RecordBase record, User user)
        {
            return AddRecordToUserRelation(record, user);
        }

        public static User operator +(User user, RecordBase record)
        {
            return AddRecordToUserRelation(record, user);
        }

        private static User AddRecordToUserRelation(RecordBase record, User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            var new_user = new User(user.FirstName, user.LastName, user.ID, user.Sexual);
            new_user._records = new List<RecordBase>(user._records);
            new_user.NewRecord(record);
            return new_user;
        }
    }
}
