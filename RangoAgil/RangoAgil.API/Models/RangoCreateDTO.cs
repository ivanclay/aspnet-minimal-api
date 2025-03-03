using System.ComponentModel.DataAnnotations;

namespace RangoAgil.API.Models;

public class RangoCreateDTO
{
    [Required]
    [MinLength(2)]
    public required string Nome { get; set; }
}
