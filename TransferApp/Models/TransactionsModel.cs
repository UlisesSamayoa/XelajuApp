using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransferApp.Models
{
    public class TransactionsModel
    {
        public int IdTransaction { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(10)]
        public string Currency { get; set; } = "USD";
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        [StringLength(50)]
        public string ReferenceNumber { get; set; }
        [Required]
        public string DestinationCountry { get; set; }
        [Required]
        public int IntermediaryId { get; set; }
        [ForeignKey("IntermediaryId")]
        public int Intermediary { get; set; }
        [Required]
        [StringLength(150)]
        public string SenderName { get; set; }
        [Required]
        [StringLength(20)]
        public string SenderDocument { get; set; }
        [Phone]
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        [Required]
        [StringLength(150)]
        public string ReceiverName { get; set; }
        [Required]
        public string ReceiverCountry { get; set; }
        [Phone]
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
