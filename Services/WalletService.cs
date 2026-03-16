using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;
using ShoppingCart.DTOs;

namespace ShoppingCart.Services
{
    public class WalletService
    {
        private readonly AppDbContext _context;

        public WalletService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WalletDto?> GetWalletByUserId(string userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                // Create a new wallet if one doesn't exist for the user
                wallet = new Wallet { UserId = userId, Balance = 0 };
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            return new WalletDto
            {
                WalletId = wallet.WalletId,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                Transactions = wallet.Transactions.Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    Type = t.Type,
                    TransactionDate = t.TransactionDate
                }).ToList()
            };
        }

        public async Task<WalletDto?> AddFunds(string userId, AddFundsDto addFundsDto)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return null; // Wallet not found (should not happen if GetWalletByUserId creates it)
            }

            wallet.Balance += addFundsDto.Amount;
            _context.Transactions.Add(new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = addFundsDto.Amount,
                Type = "CREDIT",
                TransactionDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return await GetWalletByUserId(userId); // Return updated wallet with transactions
        }

        public async Task<bool> ProcessPayment(string userId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null || wallet.Balance < amount)
            {
                return false; // Insufficient balance or wallet not found
            }

            wallet.Balance -= amount;
            _context.Transactions.Add(new Transaction
            {
                WalletId = wallet.WalletId,
                Amount = amount,
                Type = "DEBIT",
                TransactionDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
