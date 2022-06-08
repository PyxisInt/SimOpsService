using System.ComponentModel.DataAnnotations;

namespace SimOps.Models.Authentication;

public class RefreshToken
{
    [Key]
    [Required]
    [StringLength(15)]
    public string Username { get; set; }
    
    [StringLength(64)]
    public string Token { get; set; }
    
    public DateTime ExpireAt { get; set; }
}