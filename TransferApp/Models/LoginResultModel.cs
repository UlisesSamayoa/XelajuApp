namespace TransferApp.Models
{
    public class LoginResultModel
    {
        public bool Success { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsLocked { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public string Message { get; set; } = "";
        public UserModel? User { get; set; }
        //public List<PermissionModel> Permissions { get; set; } = [];
        public PermissionModel? Permissions { get; set; }
    }
}
