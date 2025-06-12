using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Lozinka { get; set; }
}
