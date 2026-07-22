namespace TransferApp.Models
{
    public class UserModel
    {
        public int IdUser { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime PasswordChangedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public bool MustChangePassword { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        // Navegación para mostrar los roles del usuario
        public List<RoleModel> Roles { get; set; } = new();
    }
}
