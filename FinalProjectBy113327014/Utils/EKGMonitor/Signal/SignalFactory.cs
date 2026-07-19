using System;

namespace MyForms.Utils.EKGMonitor.Signal
{
    /// <summary>
    /// 訊號類型枚舉
    /// </summary>
    public enum SignalType
    {
        /// <summary>
        /// 雜訊訊號
        /// </summary>
        NOISE,
        
        /// <summary>
        /// 正弦波訊號
        /// </summary>
        SINWAVE,
        
        /// <summary>
        /// 正弦波加雜訊訊號
        /// </summary>
        SINWAVEWITHNOISE,
        
        /// <summary>
        /// 真實訊號（透過委派實作）
        /// </summary>
        REAL
    }

    /// <summary>
    /// 訊號工廠類別
    /// 提供便利的方法來建立不同類型的訊號
    /// </summary>
    public static class SignalFactory
    {
        /// <summary>
        /// 建立指定類型的訊號
        /// </summary>
        /// <param name="signalType">訊號類型</param>
        /// <param name="randomSeed">隨機種子</param>
        /// <param name="noiseRange">雜訊範圍</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="frequency">頻率</param>
        /// <param name="signalProvider">真實訊號提供者（僅用於 REAL 類型）</param>
        /// <param name="signalName">訊號名稱（僅用於 REAL 類型）</param>
        /// <returns>訊號實例</returns>
        public static SignalBase CreateSignal(
            SignalType signalType,
            int randomSeed = 0,
            double noiseRange = 0.1,
            double amplitude = 1.0,
            double frequency = Math.PI,
            SignalProvider signalProvider = null,
            string signalName = "RealSignal")
        {
            switch (signalType)
            {
                case SignalType.NOISE:
                    return new NoiseSignal(randomSeed, noiseRange);
                
                case SignalType.SINWAVE:
                    return new SinWaveSignal(amplitude, frequency);
                
                case SignalType.SINWAVEWITHNOISE:
                    return new SinWaveWithNoiseSignal(randomSeed, amplitude, frequency, noiseRange);
                
                case SignalType.REAL:
                    if (signalProvider == null)
                        throw new ArgumentNullException(nameof(signalProvider), "真實訊號類型需要提供 signalProvider");
                    return new RealSignal(signalProvider, signalName);
                
                default:
                    throw new ArgumentException($"不支援的訊號類型: {signalType}");
            }
        }

        /// <summary>
        /// 建立雜訊訊號
        /// </summary>
        /// <param name="randomSeed">隨機種子</param>
        /// <param name="noiseRange">雜訊範圍</param>
        /// <returns>雜訊訊號</returns>
        public static NoiseSignal CreateNoiseSignal(int randomSeed = 0, double noiseRange = 1.0)
        {
            return new NoiseSignal(randomSeed, noiseRange);
        }

        /// <summary>
        /// 建立正弦波訊號
        /// </summary>
        /// <param name="amplitude">振幅</param>
        /// <param name="frequency">頻率</param>
        /// <returns>正弦波訊號</returns>
        public static SinWaveSignal CreateSinWaveSignal(double amplitude = 1.0, double frequency = Math.PI)
        {
            return new SinWaveSignal(amplitude, frequency);
        }

        /// <summary>
        /// 建立正弦波加雜訊訊號
        /// </summary>
        /// <param name="randomSeed">隨機種子</param>
        /// <param name="amplitude">振幅</param>
        /// <param name="frequency">頻率</param>
        /// <param name="noiseRange">雜訊範圍</param>
        /// <returns>正弦波加雜訊訊號</returns>
        public static SinWaveWithNoiseSignal CreateSinWaveWithNoiseSignal(
            int randomSeed = 0,
            double amplitude = 1.0,
            double frequency = Math.PI,
            double noiseRange = 0.1)
        {
            return new SinWaveWithNoiseSignal(randomSeed, amplitude, frequency, noiseRange);
        }

        /// <summary>
        /// 建立真實訊號
        /// </summary>
        /// <param name="signalProvider">訊號提供者委派</param>
        /// <param name="signalName">訊號名稱</param>
        /// <returns>真實訊號</returns>
        public static RealSignal CreateRealSignal(SignalProvider signalProvider, string signalName = "RealSignal")
        {
            return new RealSignal(signalProvider, signalName);
        }

        /// <summary>
        /// 建立真實訊號 - 從函數
        /// </summary>
        /// <param name="signalFunction">訊號函數</param>
        /// <param name="signalName">訊號名稱</param>
        /// <returns>真實訊號</returns>
        public static RealSignal CreateRealSignalFromFunction(Func<double, double> signalFunction, string signalName = "FunctionSignal")
        {
            return RealSignal.FromFunction(signalFunction, signalName);
        }

        /// <summary>
        /// 建立真實訊號 - 從常數值
        /// </summary>
        /// <param name="constantValue">常數值</param>
        /// <param name="signalName">訊號名稱</param>
        /// <returns>真實訊號</returns>
        public static RealSignal CreateRealSignalFromConstant(double constantValue, string signalName = "ConstantSignal")
        {
            return RealSignal.FromConstant(constantValue, signalName);
        }

        /// <summary>
        /// 建立真實訊號 - 從陣列資料
        /// </summary>
        /// <param name="data">資料陣列</param>
        /// <param name="signalName">訊號名稱</param>
        /// <param name="loopWhenEnd">當資料結束時是否循環</param>
        /// <returns>真實訊號</returns>
        public static RealSignal CreateRealSignalFromArray(double[] data, string signalName = "ArraySignal", bool loopWhenEnd = true)
        {
            return RealSignal.FromArray(data, signalName, loopWhenEnd);
        }
    }
} 