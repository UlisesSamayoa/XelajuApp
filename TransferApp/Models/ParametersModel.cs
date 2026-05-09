using Microsoft.AspNetCore.Mvc;

namespace TransferApp.Models
{
    public class ParametersModel
    {
        public int IdParameters { get; set; }
        public bool LastMonth { get; set; }
        public int? CountDays { get; set; }
        public decimal MaxAmount { get; set; }
        public int MaxTransactions { get; set; }
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
    }
}
