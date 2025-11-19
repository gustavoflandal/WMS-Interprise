using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public record CreateUserRequest(
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres")]
    string Username,

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    string Password,

    [Required(ErrorMessage = "O primeiro nome é obrigatório")]
    [StringLength(50, ErrorMessage = "O primeiro nome deve ter no máximo 50 caracteres")]
    string FirstName,

    [Required(ErrorMessage = "O sobrenome é obrigatório")]
    [StringLength(50, ErrorMessage = "O sobrenome deve ter no máximo 50 caracteres")]
    string LastName,

    [Phone(ErrorMessage = "Número de telefone inválido")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    string? Phone = null,

    List<Guid>? RoleIds = null,
    bool IsActive = true
);
