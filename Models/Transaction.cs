using System;

namespace ShoppingCart.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
