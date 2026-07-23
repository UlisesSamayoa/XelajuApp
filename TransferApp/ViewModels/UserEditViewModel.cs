namespace TransferApp.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserEditViewModel
    {
        public int IdUser { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;
    }
}
