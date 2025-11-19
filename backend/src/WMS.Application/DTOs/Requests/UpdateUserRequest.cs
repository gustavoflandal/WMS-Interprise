using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public class UpdateUserRequest
{
    [StringLength(50, ErrorMessage = "O primeiro nome deve ter no máximo 50 caracteres")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "O sobrenome deve ter no máximo 50 caracteres")]
    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Número de telefone inválido")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string? Phone { get; set; }

    public bool? IsActive { get; set; }
}
