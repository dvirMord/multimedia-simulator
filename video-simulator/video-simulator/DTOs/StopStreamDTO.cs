using System.ComponentModel.DataAnnotations;

namespace video_simulator.DTOs
{
    public class StopStreamRequestDTO
    {
        [Required(ErrorMessage = "StreamId is required.")]
        public string? StreamName { get; set; }
    }
}
