using System;

namespace MyForms.Utils.EKGMonitor.Signal
{
    /// <summary>
    /// 雜訊訊號
    /// 產生隨機雜訊訊號
    /// </summary>
    public class NoiseSignal : SignalBase
    {
        private readonly Random _random;
        private readonly double _noiseRange;

        /// <summary>
        /// 雜訊範圍
        /// </summary>
        public double NoiseRange => _noiseRange;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="randomSeed">隨機種子</param>
        /// <param name="noiseRange">雜訊範圍</param>
        public NoiseSignal(int randomSeed = 0, double noiseRange = 1.0)
        {
            _random = new Random(randomSeed);
            _noiseRange = noiseRange;
        }

        /// <summary>
        /// 產生下一個雜訊訊號值
        /// </summary>
        /// <returns>雜訊訊號值</returns>
        public override double Next()
        {
            double signal = _random.NextDouble() * _noiseRange;
            _time++;
            return signal;
        }
    }
} 