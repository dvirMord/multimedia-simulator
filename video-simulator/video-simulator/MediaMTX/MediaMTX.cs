using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace video_simulator.MediaMTX
{
    public class MediaMtxServer : IDisposable
    {
        private Process? _process;
        private bool _isExplicitStopping;
        private readonly string _exePath;
        private readonly string _workingDir;

        private static readonly Regex StreamOnlineRegex = new(@"\[path ([^\]]+)\] stream is available and online", RegexOptions.Compiled);
        private static readonly Regex StreamOfflineRegex = new(@"\[path ([^\]]+)\] stream is no longer available", RegexOptions.Compiled);
        private static readonly Regex ErrorRegex = new(@"ERR|WAR|error|decode error|failed", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public MediaMtxServer(string exePath)
        {
            _exePath = exePath ?? throw new ArgumentNullException(nameof(exePath));
            _workingDir = Path.GetDirectoryName(exePath)
                ?? throw new DirectoryNotFoundException($"Invalid directory path: {exePath}");
        }

        public async Task StartAsync(int rtspPort = 8554, TimeSpan? timeout = null)
        {
            var configPath = Path.Combine(_workingDir, "mediamtx.yml");
            if (!File.Exists(_exePath))
            {
                throw new FileNotFoundException("MediaMTX binary not found at specified path.", _exePath);
            }

            var psi = new ProcessStartInfo
            {
                FileName = _exePath,
                Arguments = $"\"{configPath}\"",
                WorkingDirectory = _workingDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            _process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            _process.OutputDataReceived += (s, e) => ProcessLogData(e.Data, isErrorStream: false);
            _process.ErrorDataReceived += (s, e) => ProcessLogData(e.Data, isErrorStream: true);

            _process.Exited += (s, e) =>
            {
                if (!_isExplicitStopping)
                {
                    Console.WriteLine($"[mediamtx] PROCESS TERMINATED UNEXPECTEDLY! (ExitCode: {_process.ExitCode})");
                }
            };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            await WaitUntilPortReadyAsync("127.0.0.1", rtspPort, timeout ?? TimeSpan.FromSeconds(5));
            Console.WriteLine($"[mediamtx] Server is bound and listening on RTSP port {rtspPort}");
        }

        private void ProcessLogData(string? logLine, bool isErrorStream)
        {
            if (string.IsNullOrWhiteSpace(logLine)) return;

            
            var onlineMatch = StreamOnlineRegex.Match(logLine);
            if (onlineMatch.Success)
            {
                Console.WriteLine($"[mediamtx] Stream published: '{onlineMatch.Groups[1].Value}'");
                return;
            }

           
            var offlineMatch = StreamOfflineRegex.Match(logLine);
            if (offlineMatch.Success)
            {
                Console.WriteLine($"[mediamtx] Stream stopped: '{offlineMatch.Groups[1].Value}'");
                return;
            }

            
            if (isErrorStream || ErrorRegex.IsMatch(logLine))
            {
                if (logLine.Contains("RTP packets are too big") || logLine.Contains("wsarecv:"))
                {
                    return;
                }

                Console.WriteLine($"[mediamtx error] {logLine.Trim()}");
            }
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
                    return;
                }
                catch (SocketException)
                {
                    await Task.Delay(100);
                }
            }
            throw new TimeoutException($"MediaMTX RTSP listener failed to start on port {port} within {timeout.TotalSeconds}s.");
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
            {
                _isExplicitStopping = true;
                Console.WriteLine("[mediamtx] Shutting down MediaMTX process...");
                try
                {
                    _process.Kill(entireProcessTree: true);
                    _process.WaitForExit(3000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[mediamtx error] Exception during shutdown: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            Stop();
            _process?.Dispose();
            _process = null;
        }
    }
}