using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PrabalGhosh.Utilities;

namespace SimOps.Models.Authentication;

public class User
{
    [Key]
    [Required]
    [StringLength(15)]
    public string Username { get; set; }
    
    [StringLength(50)]
    public string FirstName { get; set; }
    
    [StringLength(50)]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(128)]
    public string Email { get; set; }
    
    [StringLength(64, MinimumLength = 8)]
    public string Password { get; set; }

    [Required] [StringLength(16)] 
    public string Salt { get; set; } = String.Empty.GetId();
    
    [Required]
    [StringLength(20)]
    public string Role { get; set; }

    [Required] 
    public DateTime Created { get; set; } = DateTime.MinValue;

    [Required] 
    public DateTime LastLogin { get; set; } = DateTime.MinValue;

}