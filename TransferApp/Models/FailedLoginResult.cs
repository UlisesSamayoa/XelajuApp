namespace TransferApp.Models
{
    public class FailedLoginResult
    {
        public bool IsLocked { get; set; }
        public DateTime? LockedUntil { get; set; }
        public int FailedAttempts { get; set; }
    }
}
