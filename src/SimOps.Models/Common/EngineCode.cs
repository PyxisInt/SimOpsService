using System.ComponentModel.DataAnnotations;

namespace SimOps.Models.Common;

public class EngineCode
{
    [Key]
    [StringLength(10)]
    public string Code { get; set; }
    
    [StringLength(30)]
    public string Description { get; set; }
}