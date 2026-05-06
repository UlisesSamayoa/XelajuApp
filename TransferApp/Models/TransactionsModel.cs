using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransferApp.Models
{
    public class TransactionsModel
    {
        public int IdTransaction { get; set; }
        public int TransactionType { get; set; }
        public string TransactionTypeName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceNumber { get; set; }
        //SENDER
        public string SenderCountry { get; set; }
        public int SenderCompany { get; set; }
        public string SenderCompanyName { get; set; }
        public int SenderCurrency { get; set; }
        public string SenderCurrencyName { get; set; }
        public string SenderName { get; set; }
        public int SenderDocumentType { get; set; }
        public string SenderDocumentNumber { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }

        //RECEIVER
        public string ReceiverCountry { get; set; }
        public int ReceiverCompany { get; set; }
        public string ReceiverCompanyName { get; set; }
        public int ReceiverCurrency { get; set; }
        public string ReceiverCurrencyName { get; set; }
        public string ReceiverName { get; set; }
        public int ReceiverDocumentType { get; set; }
        public string ReceiverDocumentNumber { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }

        public string JustifyDetails { get; set; }
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
    }
}
