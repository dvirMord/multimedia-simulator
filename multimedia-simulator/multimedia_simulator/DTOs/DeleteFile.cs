using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace multimedia_simulator.DTOs
{
    public class DeleteFileDTO
    {
        [JsonPropertyName("simId")]
        [Required(ErrorMessage = "SimId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SimId must be a valid positive integer.")]
        public required int SimId { get; init; }
    }
}