using System.ComponentModel.DataAnnotations;

namespace TransferApp.ViewModels
{
    //public class ChangePasswordViewModel
    //{
    //    public int IdUser { get; set; }
    //    [Required]
    //    [DataType(DataType.Password)]
    //    public string NewPassword { get; set; } = string.Empty;
    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    //    public string ConfirmPassword { get; set; } = string.Empty;
    //}
    public class ChangePasswordViewModel
    {
        public int IdUser { get; set; }
        //[Required]
        //[DataType(DataType.Password)]
        //public string NewPassword { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
