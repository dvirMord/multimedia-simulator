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

    public class StartStreamDTO
    {
        [Required(ErrorMessage = "FileName is required.")]
        public required string FileName { get; init; }

        [Required(ErrorMessage = "SourceFileId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SourceFileId must be a valid positive integer.")]
        public required int SourceFileId { get; init; }

        public StreamType Type { get; init; } = StreamType.Video;
    }
}