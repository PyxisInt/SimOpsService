using System.ComponentModel.DataAnnotations;

namespace SimOps.Models.Common;

public class Airline
{
    [Key]
    [StringLength(3)]
    [Required]
    public string Icao { get; set; }
    
    [StringLength(2)]
    [Required]
    public string Iata { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(4)]
    public string Base { get; set; }
    
    public List<Hub> Hubs { get; set; }
}