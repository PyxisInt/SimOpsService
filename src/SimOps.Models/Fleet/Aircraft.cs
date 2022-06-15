using System.ComponentModel.DataAnnotations;

namespace SimOps.Models.Fleet;

public class Aircraft
{
    [StringLength(3)]
    [Required]
    public string Airline { get; set; }
    
    [StringLength(10)]
    [Required]
    public string Registration { get; set; }
    
    [StringLength(4)]
    [Required]
    public string IcaoType { get; set; }
    
    [StringLength(3)]
    [Required]
    public string IataType { get; set; }
    
    [StringLength(4)]
    public string Hub { get; set; }
    
    [StringLength(10)]
    public string EquipmentCode { get; set; }
    
    [StringLength(10)]
    public string EngineCode { get; set; }
    
    [Required]
    public long MaxRamp { get; set; }
    
    [Required]
    public long MaxTakeoff { get; set; }
    
    [Required]
    public long MaxLanding { get; set; }
    
    [Required]
    public long MaxZeroFuel { get; set; }
    
    [Required]
    public long EmptyOperating { get; set; }
    
    [Required]
    public long TankCapacity { get; set; }

    public long AuxFuelCapacity { get; set; } = 0;
    
    public long? MaxPassengers { get; set; }
    
    [Required]
    [StringLength(1)]
    public string LandingCategory { get; set; }

    [Required] 
    public int AverageTAS { get; set; } = 0;
    
    [Required]
    public int Ceiling { get; set; }
}