using System;

namespace MyForms.Utils.EKGMonitor.Signal
{
    /// <summary>
    /// 正弦波訊號
    /// 產生純正弦波訊號
    /// </summary>
    public class SinWaveSignal : SignalBase
    {
        private readonly double _amplitude;
        private readonly double _frequency;

        /// <summary>
        /// 振幅 (mV)
        /// </summary>
        public double Amplitude => _amplitude;

        /// <summary>
        /// 頻率
        /// </summary>
        public double Frequency => _frequency;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="amplitude">振幅 (mV)</param>
        /// <param name="frequency">頻率</param>
        public SinWaveSignal(double amplitude = 1.0, double frequency = Math.PI)
        {
            _amplitude = amplitude;
            _frequency = frequency;
        }

        /// <summary>
        /// 產生下一個正弦波訊號值
        /// </summary>
        /// <returns>正弦波訊號值</returns>
        public override double Next()
        {
            const double fromDegreeToRadian = Math.PI / 180;
            double signal = _amplitude * Math.Sin(2 * Math.PI * _frequency * _time * fromDegreeToRadian);
            _time++;
            return signal;
        }
    }
} 