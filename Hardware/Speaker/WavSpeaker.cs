using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.StatisticalRobot;

namespace Speaker
{
    public class WavSpeaker
    {
        private readonly int _sampleRate;
        private readonly short[] _audioData;
        private readonly double[] _dutyCycles;
        private readonly bool _negativeSampleRate;
        private CancellationTokenSource? _cancellationTokenSource = null;

        public WavSpeaker(string wavFilePath, bool negativeSampleRate)
        {
            _audioData = Array.Empty<short>();
            _dutyCycles = Array.Empty<double>();
            _sampleRate = 0;

            try
            {
                Debug.WriteLine($"Initializing WavSpeaker with file: {wavFilePath}");

                using var stream = new FileStream(wavFilePath, FileMode.Open, FileAccess.Read);
                var reader = new BinaryReader(stream);

                // Parse WAV header
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
                    throw new InvalidDataException("Invalid WAV file: Missing 'fmt ' chunk.");

                reader.ReadInt32(); // Chunk size
                if (reader.ReadInt16() != 1) // Audio format
                    throw new NotSupportedException("Only PCM format is supported.");

                reader.ReadInt16(); // Number of channels
                _sampleRate = reader.ReadInt32();
                reader.ReadInt32(); // Byte rate
                reader.ReadInt16(); // Block align
                if (reader.ReadInt16() != 16) // Bits per sample
                    throw new NotSupportedException("Only 16-bit WAV files are supported.");

                // Skip to data chunk
                while (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "data")
                {
                    int chunkSize = reader.ReadInt32();
                    reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                }

                int dataSize = reader.ReadInt32();
                _audioData = new short[dataSize / 2];
                for (int i = 0; i < _audioData.Length; i++)
                {
                    _audioData[i] = reader.ReadInt16();
                }

                // Precompute duty cycles
                _dutyCycles = new double[_audioData.Length];
                for (int i = 0; i < _audioData.Length; i++)
                {
                    _dutyCycles[i] = (_audioData[i] - short.MinValue) / (double)ushort.MaxValue;
                }

                _negativeSampleRate = negativeSampleRate;
                Debug.WriteLine("WavSpeaker initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during WAV parsing: {ex.Message}");
                throw;
            }
        }

        public async Task PlayAsync()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(() => Play(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during playback: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while stopping playback: {ex.Message}");
            }
        }

        private void Play(CancellationToken cancellationToken)
        {
            try
            {
                Robot.SetPwmPin(_sampleRate, 1);
                Robot.StartPwm();

                int waitTimeUs = (int)(1000000.0 / _sampleRate);

                for (int i = 0; i < _dutyCycles.Length; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    Robot.ChangePwmDutyCycle(_dutyCycles[i]);
                    Robot.WaitUs(waitTimeUs);
                }

                Robot.StopPwm();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during playback loop: {ex.Message}");
            }
        }
    }
}
