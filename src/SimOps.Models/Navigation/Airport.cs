using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SimOps.Models.Navigation;

public class Airport
{
    [Key]
    [StringLength(4)]
    [Required]
    [JsonProperty("icao")]
    public string Icao { get; set; }
    
    [StringLength(2)]
    [Required]
    [JsonProperty("countryCode")]
    public string CountryCode { get; set; }
    
    [StringLength(3)]
    [JsonProperty("iata")]
    public string Iata { get; set; }
    
    [StringLength(50)]
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [StringLength(50)]
    [JsonProperty("timeZone")]
    public string TimeZone { get; set; }
    
    [Required]
    [JsonProperty("latitude")]
    public double Latitude { get; set; }
    
    [Required]
    [JsonProperty("longitude")]
    public double Longitude { get; set; }
    
    [JsonProperty("elevation")]
    public int? Elevation { get; set; }
    
}