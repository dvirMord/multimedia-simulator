using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace video_simulator.MediaMTX
{
    public class MediaMtxServer : IDisposable
    {
        private Process _process;
        private readonly string _exePath;
        private readonly string _workingDir;

        public MediaMtxServer(string exePath)
        {
            _exePath = exePath;
            _workingDir = Path.GetDirectoryName(exePath);
        }

        public async Task StartAsync(int rtspPort = 8554, TimeSpan? timeout = null)
        {
            var configPath = Path.Combine(_workingDir, "mediamtx.yml");
            var psi = new ProcessStartInfo
            {
                FileName = _exePath,
                Arguments = $"\"{configPath}\"",
                WorkingDirectory = _workingDir,   // so it finds mediamtx.yml next to the exe
                UseShellExecute = false,          // launch directly, not via cmd.exe
                CreateNoWindow = true,             // no visible console window
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            _process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            _process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine("[mediamtx] " + e.Data); };
            _process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine("[mediamtx] " + e.Data); };
            _process.Exited += (s, e) => Console.WriteLine("[mediamtx] process exited unexpectedly!");

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            // Wait until the RTSP port is actually accepting connections
            await WaitUntilPortReadyAsync("127.0.0.1", rtspPort, timeout ?? TimeSpan.FromSeconds(5));
        }

        private async Task WaitUntilPortReadyAsync(string host, int port, TimeSpan timeout)
        {
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < timeout)
            {
                try
                {
                    using var client = new TcpClient();
                    await client.ConnectAsync(host, port);
                    return; // connected successfully -> server is up
                }
                catch (SocketException)
                {
                    await Task.Delay(100);
                }
            }
            throw new TimeoutException("MediaMTX did not become ready in time.");
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
            {
                Console.WriteLine("[mediamtx] Killing MediaMTX process...");
                _process.Kill(entireProcessTree: true); // clean shutdown
                _process.WaitForExit(3000);
            }
        }

        public void Dispose() => Stop();
    }
}