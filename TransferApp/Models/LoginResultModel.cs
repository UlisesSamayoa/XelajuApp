namespace TransferApp.Models
{
    public class LoginResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserModel? User { get; set; }
    }
}
