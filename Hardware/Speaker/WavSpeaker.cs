using System.Text;
using Avans.StatisticalRobot;

namespace Speaker
{   
    /// <summary>
    /// This class reads a 16-bit WAV file.
    /// The initialisation and preloading of the duty cycle which is needed for the play method gets done in the constructor
    /// to avoid loading when playing is needed.
    /// </summary>
    public class WavSpeaker
    {
        private readonly int _sampleRate;
        private readonly short[] _audioData;
        private readonly double[] _dutyCycles;
        private readonly bool _negativeSampleRate;
        private CancellationTokenSource? _cancellationTokenSource = null;

        public WavSpeaker(string wavFilePath, bool negativeSampleRate) //The negative sample rate is for sample rates specified in headers that dont match the actual sample rate (this is mainly added after wrongly prepping my files)
        {
            // Parse the WAV file and extract audio data
            using var stream = new FileStream(wavFilePath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(stream);

            // Read WAV file header
            byte[] buffer = new byte[4];
            reader.Read(buffer, 0, 4);
            if (Encoding.ASCII.GetString(buffer) != "RIFF")
                throw new InvalidDataException("Invalid WAV file.");
            reader.ReadInt32(); // File size
            reader.Read(buffer, 0, 4);
            if (Encoding.ASCII.GetString(buffer) != "WAVE")
                throw new InvalidDataException("Invalid WAV file.");

            // Read format chunk
            reader.Read(buffer, 0, 4);
            if (Encoding.ASCII.GetString(buffer) != "fmt ")
                throw new InvalidDataException("Invalid WAV file.");
            reader.ReadInt32(); // Chunk size
            var audioFormat = reader.ReadInt16();
            if (audioFormat != 1) // PCM format
                throw new NotSupportedException("Only PCM format is supported.");
            var numChannels = reader.ReadInt16();
            _sampleRate = reader.ReadInt32(); // Read and assign the sample rate
            reader.ReadInt32(); // Byte rate
            reader.ReadInt16(); // Block align
            var bitsPerSample = reader.ReadInt16();
            if (bitsPerSample != 16)
                throw new NotSupportedException("Only 16-bit WAV files are supported.");

            // Skip to data chunk
            while (true)
            {
                if (reader.BaseStream.Position + 4 > reader.BaseStream.Length)
                    throw new InvalidDataException("Unexpected end of file while searching for data chunk.");

                reader.Read(buffer, 0, 4);
                if (Encoding.ASCII.GetString(buffer) == "data")
                    break;

                if (reader.BaseStream.Position + 4 > reader.BaseStream.Length)
                    throw new InvalidDataException("Unexpected end of file while skipping chunk size.");
                int chunkSize = reader.ReadInt32(); // Read chunk size

                if (reader.BaseStream.Position + chunkSize > reader.BaseStream.Length)
                    throw new InvalidDataException("Unexpected end of file while skipping chunk data.");

                reader.BaseStream.Seek(chunkSize, SeekOrigin.Current); // Skip chunk data
            }

            if (reader.BaseStream.Position + 4 > reader.BaseStream.Length)
                throw new InvalidDataException("Unexpected end of file while reading data size.");

            var dataSize = reader.ReadInt32();

            if (reader.BaseStream.Position + dataSize > reader.BaseStream.Length)
                throw new InvalidDataException("Unexpected end of file while reading audio data.");

            // Assuming the sample rate is stored in a variable called _sampleRate
            Console.WriteLine($"Sample Rate: {_sampleRate}");
            Console.WriteLine($"Data Size: {dataSize}");

            // Read audio data
            _audioData = new short[dataSize / 2];
            for (int i = 0; i < _audioData.Length; i++)
            {
                if (reader.BaseStream.Position + 2 > reader.BaseStream.Length)
                    throw new InvalidDataException("Unexpected end of file while reading audio sample.");

                _audioData[i] = reader.ReadInt16();
            }

            // Precompute duty cycles
            _dutyCycles = new double[_audioData.Length / 2];
            for (int i = 0; i < _audioData.Length - 1; i += 2)
            {
                // Average pairs of samples to lower the tone
                short averagedSample = (short)((_audioData[i] + _audioData[i + 1]) / 2);

                // Normalize sample to 0.0 - 1.0 range
                _dutyCycles[i / 2] = (averagedSample + 32768.0) / 65536.0;
            }
            _negativeSampleRate = negativeSampleRate;
        }

        /// <summary>
        /// This method plays the preloaded file on another thread.
        /// </summary>
        /// <returns></returns>
        public async Task PlayAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => Play(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        /// <summary>
        /// This method stops the playing.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// This method plays the song to the pwm pin prespecified by the Avans.StatisticalRobot class.
        /// </summary>
        /// <param name="cancellationToken">The token that tethers the play method.</param>
        private void Play(CancellationToken cancellationToken)
        {
            // Initialize PWM with the correct sample rate
            Robot.SetPwmPin(_sampleRate, 1);
            Robot.StartPwm();
            int waitTimeUs;
            // Play the preloaded duty cycles
            if (_negativeSampleRate)
            {
                waitTimeUs = (int)(1000000.0 / (_sampleRate / 1.5));
            }
            else{
                waitTimeUs = (int)(1000000.0 / (_sampleRate * 1.5));
            }
            for (int i = 0; i < _dutyCycles.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                Robot.ChangePwmDutyCycle(_dutyCycles[i]);
                Robot.WaitUs(waitTimeUs);
            }

            // Stop PWM
            Robot.StopPwm();
        }
    }
}
