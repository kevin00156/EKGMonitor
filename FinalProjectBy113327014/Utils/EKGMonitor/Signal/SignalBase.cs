using System;

namespace MyForms.Utils.EKGMonitor.Signal
{
    /// <summary>
    /// 訊號的抽象基底類別
    /// 提供時間追蹤和訊號產生的基本架構
    /// </summary>
    public abstract class SignalBase
    {
        /// <summary>
        /// 目前的時間點，用於訊號產生的時間軸
        /// </summary>
        protected double _time = 0;

        /// <summary>
        /// 取得目前時間
        /// </summary>
        public double CurrentTime => _time;

        /// <summary>
        /// 產生下一個訊號資料點
        /// 此方法會自動遞增時間，並由子類別實作具體的訊號產生邏輯
        /// </summary>
        /// <returns>下一個訊號值</returns>
        public abstract double Next();

        /// <summary>
        /// 重置時間為零
        /// </summary>
        public virtual void Reset()
        {
            _time = 0;
        }

        /// <summary>
        /// 設定目前時間
        /// </summary>
        /// <param name="time">要設定的時間值</param>
        public virtual void SetTime(double time)
        {
            _time = time;
        }
    }
} 