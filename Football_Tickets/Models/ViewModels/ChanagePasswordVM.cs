using System.ComponentModel.DataAnnotations;

namespace Football_Tickets.Models.ViewModels
{
    public class ChanagePasswordVM
    {
        public int Id { get; set; }

        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
    }
}
