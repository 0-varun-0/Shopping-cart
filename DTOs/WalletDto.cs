using System;
using System.Collections.Generic;

namespace ShoppingCart.DTOs
{
    public class WalletDto
    {
        public int WalletId { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    }

    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // e.g., "CREDIT", "DEBIT"
        public DateTime TransactionDate { get; set; }
    }

    public class AddFundsDto
    {
        public decimal Amount { get; set; }
    }

    public class ProcessPaymentDto
    {
        public decimal Amount { get; set; }
    }
}
