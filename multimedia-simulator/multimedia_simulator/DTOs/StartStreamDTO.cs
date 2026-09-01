using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace multimedia_simulator.DTOs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StreamType
    {
        Video,
        Audio
    }

    public class StreamResponseDTO
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("rtspStream")]
        public string RtspStream { get; set; } = string.Empty;
    }

    public class StartStreamDTO
    {
        [Required(ErrorMessage = "FileName is required.")]
        public required string FileName { get; init; }

        [Required(ErrorMessage = "SourceFileId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SourceFileId must be a valid positive integer.")]
        public required int SourceFileId { get; init; }

        public StreamType Type { get; init; } = StreamType.Video;
    }

    public class StartStreamByIdDTO
    {
        [Required(ErrorMessage = "SimId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SimId must be a valid positive integer.")]
        public required int SimId { get; init; }

    }

    public class StopStreamByIdDTO
    {
        [Required(ErrorMessage = "SimId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SimId must be a valid positive integer.")]
        public required int SimId { get; init; }

    }
    public class ChannelDTO
    {
        public int Id { get; set; }
        public int SourceFilesId { get; set; }
        public string StreamEndpoint { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? FFmpegProcessId { get; set; }
        public bool IsActive { get; set; }
    }
}