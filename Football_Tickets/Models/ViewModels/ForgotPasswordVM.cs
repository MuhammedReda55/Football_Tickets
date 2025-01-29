using System.ComponentModel.DataAnnotations;

namespace Football_Tickets.Models.ViewModels
{
    public class ForgotPasswordVM
    {
        public int Id { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
    }
}
