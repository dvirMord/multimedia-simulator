using System.ComponentModel.DataAnnotations;

namespace multimedia_simulator.DTOs
{
    public class DeleteFileDTO
    {
        [Required(ErrorMessage = "FileName is required.")]
        public required string FileName { get; init; } //init -only property to make it immutable after initialization
    }
}
