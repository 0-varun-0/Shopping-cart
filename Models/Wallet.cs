namespace ShoppingCart.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
