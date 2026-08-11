using System.Collections.Concurrent;
using System.Diagnostics;
using video_simulator.constans;
using video_simulator.MediaMTX;
using video_simulator.Validators;
using VideoSimulator.Services;

namespace video_simulator.Services
{
    public class FfmpegManager : IFfmpegManager
    {
        private readonly ConcurrentDictionary<string, Process> _runningStreams;
        private readonly string _ffmpegPath;
        private readonly MediaMtxServer _mediaMtxServer;
        private readonly string _storagePath;

        //-----------------constructor-----------------
        private FfmpegManager()
        {
            this._runningStreams = new ConcurrentDictionary<string, Process>();

            this._ffmpegPath =
                Environment.GetEnvironmentVariable(EnvConstants.ffmpegPathName)
                ?? throw new InvalidOperationException("FFmpeg path is not configured.");

            this._storagePath = Environment.GetEnvironmentVariable(EnvConstants.tsFilesStorageName)
                ?? throw new InvalidOperationException("TS files storage path is not configured.");

            this._mediaMtxServer = new MediaMtxServer(Environment.GetEnvironmentVariable(EnvConstants.mediaMTXPathName)
                    ?? throw new InvalidOperationException("MediaMTX path is not configured."));
        }

        //-----------------interface functions-----------------

        public bool StartStream(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);
            MyValidators.ValidateFileExtension(streamId);

            if (this.IsStreamRunning(streamId))
            {
                throw new ArgumentException($"{streamId} Stream is already running");
            }

            MyValidators.ValidateFileExists(
                streamId,
                this._storagePath);

            string fullPath = Path.Combine(
                this._storagePath,
                streamId);

            string args =
                $"-re -stream_loop -1 " +
                $"-i \"{fullPath}\" " +
                $"-map 0:v:0 " +
                $"-c:v copy " +
                $"-f rtsp " +
                $"rtsp://localhost:8554/{streamId}";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = this._ffmpegPath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            try
            {
                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();

                Thread.Sleep(500);

                if (process.HasExited)
                {
                    string errorLog =
                        process.StandardError.ReadToEnd();

                    Console.WriteLine(
                        "[ffmpeg manager] Process CRASHED immediately!");

                    Console.WriteLine(
                        $"[ffmpeg manager] FFmpeg Error Output:\n{errorLog}");

                    process.Dispose();

                    return false;
                }

                Console.WriteLine(
                    $"[ffmpeg manager] Stream '{streamId}' is running via RTSP!");

                this._runningStreams.TryAdd(
                    streamId,
                    process);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[ffmpeg manager] Failed to start stream: {ex.Message}");

                return false;
            }
        }

        public bool StopStream(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);

            if (!this._runningStreams.TryGetValue(streamId, out Process? process))
            {
               throw new ArgumentException($"Stream '{streamId}' is not running.");
            }

            Console.WriteLine($"[ffmpeg manager] Stopping stream '{streamId}'...");

            process.Kill();

            this._runningStreams.TryRemove(streamId, out _);

            process.Dispose();

            return true;
        }

        public bool IsStreamRunning(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);

            return this._runningStreams.ContainsKey(streamId);
        }

        public List<string> GetRunningStreams()
        {
            return this._runningStreams.Keys.ToList();
        }

        //-----------------create and clean--------------

        public static async Task<FfmpegManager> CreateAsnc(
            string rtspBaseUrl = "rtsp://127.0.0.1:8554")
        {
            FfmpegManager manager = new FfmpegManager();

            await manager._mediaMtxServer.StartAsync(
                rtspPort: 8554);

            return manager;
        }

        public void cleanUpPorcesss()
        {
            foreach (string streamId in this._runningStreams.Keys.ToList())
            {
                this.StopStream(streamId);
            }

            this._mediaMtxServer.Stop();
        }
    }
}