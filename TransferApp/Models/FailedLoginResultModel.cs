namespace TransferApp.Models
{
    public class FailedLoginResultModel
    {
        public int FailedAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}
