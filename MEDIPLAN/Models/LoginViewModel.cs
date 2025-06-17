using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Lozinka { get; set; } = string.Empty;
}
