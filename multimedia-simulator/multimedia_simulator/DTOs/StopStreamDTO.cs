using System.ComponentModel.DataAnnotations;

namespace multimedia_simulator.DTOs
{
    public class StopStreamDTO
    {
        [Required(ErrorMessage = "StreamId is required.")]
        public required string StreamName { get; init; }
    }
}
