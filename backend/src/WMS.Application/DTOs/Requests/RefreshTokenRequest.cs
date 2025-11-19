using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests;

public record RefreshTokenRequest(
    [Required(ErrorMessage = "O refresh token é obrigatório")]
    [StringLength(500, ErrorMessage = "O refresh token deve ter no máximo 500 caracteres")]
    string RefreshToken
);
