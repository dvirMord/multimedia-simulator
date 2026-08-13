using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using video_simulator.constans;

namespace video_simulator.MediaMTX
{
    public class MediaMtxServer : IDisposable
    {
        private Process? _process;
        private bool _isExplicitStopping;
        private readonly string _exePath;
        private readonly string _workingDir;
        private readonly ILogger<MediaMtxServer> _logger;

        public MediaMtxServer(string exePath, ILogger<MediaMtxServer> logger)
        {
            _exePath = exePath ?? throw new ArgumentNullException(nameof(exePath), MediaMtxExceptions.ExePathNullException);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), MediaMtxExceptions.LoggerNullException);
            _workingDir = Path.GetDirectoryName(exePath)
                ?? throw new DirectoryNotFoundException(string.Format(MediaMtxExceptions.InvalidDirectoryPathTemplate, exePath));
        }

        public async Task StartAsync(int rtspPort = Constants.RTSP_DEFAULT_PORT, TimeSpan? timeout = null)
        {
            var configPath = Path.Combine(_workingDir, "mediamtx.yml");
            if (!File.Exists(_exePath))
            {
                throw new FileNotFoundException(MediaMtxExceptions.MediaMTXBinaryNotFound, _exePath);
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

            _process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    if (e.Data.Contains("[RTSP]") || e.Data.Contains("RTP packets are too big"))
                    {
                        return;
                    }

                    _logger.LogInformation(MediaMtxServerMessages.Output.OutputTemplate, e.Data);
                }
            };

            _process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    _logger.LogWarning(MediaMtxServerMessages.Output.ErrorTemplate, e.Data);
                }
            };

            _process.Exited += (s, e) =>
            {
                if (!_isExplicitStopping)
                {
                    _logger.LogError(MediaMtxServerMessages.Error.ProcessTerminatedUnexpectedlyTemplate, _process.ExitCode);
                }
            };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            await WaitUntilPortReadyAsync(Constants.LOOPBACK_IP, rtspPort, timeout ?? TimeSpan.FromSeconds(Constants.MEDIAMTX_STARTUP_TIMEOUT_SECONDS));
            _logger.LogInformation(MediaMtxServerMessages.Success.ServerListeningTemplate, rtspPort);
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
                    await Task.Delay(Constants.WAIT_FOR_PORT_MILLISECONDS);
                }
            }
            throw new TimeoutException(string.Format(MediaMtxExceptions.RtspListenerFailedTemplate, port, timeout.TotalSeconds));
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
            {
                _isExplicitStopping = true;
                _logger.LogInformation(MediaMtxServerMessages.Success.ShuttingDown);
                try
                {
                    _process.Kill(entireProcessTree: true);
                    _process.WaitForExit(Constants.MEDIAMTX_SHUTDOWN_TIMEOUT_MILLISECONDS);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, MediaMtxServerMessages.Error.ShutdownExceptionTemplate, ex.Message);
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