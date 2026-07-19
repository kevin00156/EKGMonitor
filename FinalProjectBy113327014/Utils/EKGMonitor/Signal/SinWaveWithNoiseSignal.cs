using System;

namespace MyForms.Utils.EKGMonitor.Signal
{
    /// <summary>
    /// 正弦波加雜訊訊號
    /// 產生包含雜訊的正弦波訊號
    /// </summary>
    public class SinWaveWithNoiseSignal : SignalBase
    {
        private readonly Random _random;
        private readonly double _amplitude;
        private readonly double _frequency;
        private readonly double _noiseRange;

        /// <summary>
        /// 振幅 (mV)
        /// </summary>
        public double Amplitude => _amplitude;

        /// <summary>
        /// 頻率
        /// </summary>
        public double Frequency => _frequency;

        /// <summary>
        /// 雜訊範圍
        /// </summary>
        public double NoiseRange => _noiseRange;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="randomSeed">隨機種子</param>
        /// <param name="amplitude">振幅 (mV)</param>
        /// <param name="frequency">頻率</param>
        /// <param name="noiseRange">雜訊範圍</param>
        public SinWaveWithNoiseSignal(
            int randomSeed = 0,
            double amplitude = 1.0,
            double frequency = Math.PI,
            double noiseRange = 0.1)
        {
            _random = new Random(randomSeed);
            _amplitude = amplitude;
            _frequency = frequency;
            _noiseRange = noiseRange;
        }

        /// <summary>
        /// 產生下一個正弦波加雜訊訊號值
        /// </summary>
        /// <returns>正弦波加雜訊訊號值</returns>
        public override double Next()
        {
            const double fromDegreeToRadian = Math.PI / 180;
            double sinWave = _amplitude * Math.Sin(2 * Math.PI * _frequency * _time * fromDegreeToRadian);
            double noise = _random.NextDouble() * _noiseRange;
            double signal = sinWave + noise;
            _time++;
            return signal;
        }
    }
} 