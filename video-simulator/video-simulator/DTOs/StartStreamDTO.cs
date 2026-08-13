using System.ComponentModel.DataAnnotations;

namespace video_simulator.DTOs
{
    public class StartStreamDTO
    {
        [Required(ErrorMessage = "FileName is required.")]
        public string? FileName { get; set; }
    }
}
