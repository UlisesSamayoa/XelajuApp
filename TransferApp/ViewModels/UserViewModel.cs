namespace TransferApp.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using TransferApp.Models;

    public class UserViewModel
    {
        public int IdUser { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;
        public List<RoleModel> AvailableRoles { get; set; } = [];
        public List<int> SelectedRoles { get; set; } = [];
    }
}
