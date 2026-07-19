using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;
using ScottPlot.MultiplotLayouts;
using ScottPlot.Plottables;
using Newtonsoft.Json;

namespace MyForms.Utils.EKGMonitor.Base
{
    public interface IRecord
    {
        DateTime TimeStamp { get; }
        string Name { get; }
        string SavedFilePath { get; }
    }
    public enum RecordType
    {
        None = 0,
        EKGRecord,
        Other
    }
    public class RecordBase : IRecord
    {
        protected DateTime _timeStamp;
        protected string _name;
        protected string _savedFilePath;
        protected string _savedFormat;

        [JsonProperty("Name")]
        public string Name { get { return _name; } }
        
        [JsonProperty("SavedFilePath")]
        public string SavedFilePath { get { return _savedFilePath; } }
        
        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get { return _timeStamp; } }
        
        /// <summary>
        /// 用於序列化的記錄類型名稱
        /// </summary>
        [JsonProperty("RecordType")]
        public string RecordTypeName => this.GetType().Name;
        
        public RecordBase()
        {
            _timeStamp = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} at {_timeStamp.ToString("yyyy/MM/dd HH:mm:ss")}";
        }

    }

}
