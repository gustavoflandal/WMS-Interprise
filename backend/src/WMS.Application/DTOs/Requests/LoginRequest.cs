using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public record LoginRequest(
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo 50 caracteres")]
    string Username,

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, ErrorMessage = "A senha deve ter no máximo 100 caracteres")]
    string Password,

    bool RememberMe = false
);
