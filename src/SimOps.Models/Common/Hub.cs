using System.ComponentModel.DataAnnotations;

namespace SimOps.Models.Common;

public class Hub
{
    [Key]
    [StringLength(4)]
    [Required]
    public string Icao { get; set; }
    
    [StringLength(40)]
    public string Name { get; set; }
    
    [StringLength(30)]
    public string City { get; set; }
    
    [StringLength(30)]
    public string Country { get; set; }
    
    [StringLength(256)]
    public string Url { get; set; }
}