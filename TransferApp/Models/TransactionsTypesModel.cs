using Microsoft.AspNetCore.Mvc;

namespace TransferApp.Models
{
    public class TransactionsTypesModel
    {
        public int IdTypeTransaction { get; set; }
        public string Name { get; set; }
        public decimal Commission { get; set; }
        public string Description { get; set; }
        public int NumberT { get; set; }
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
    }
}
