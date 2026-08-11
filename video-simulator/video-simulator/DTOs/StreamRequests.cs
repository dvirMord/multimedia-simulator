using System.ComponentModel.DataAnnotations;

namespace video_simulator.DTOs
{
    public class StartStreamRequest
    {
        [Required(ErrorMessage = "FileName is required.")]
        public string FileName { get; set; }
    }

    public class StopStreamRequest
    {
        [Required(ErrorMessage = "StreamId is required.")]
        public string StreamName { get; set; }
    }
}
