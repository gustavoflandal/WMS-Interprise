using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Requests
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        [StringLength(100, ErrorMessage = "A senha atual deve ter no máximo 100 caracteres")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter entre 6 e 100 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação da nova senha é obrigatória")]
        [Compare(nameof(NewPassword), ErrorMessage = "A nova senha e a confirmação não coincidem")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
