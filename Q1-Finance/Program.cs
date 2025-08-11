
using System;
using System.Collections.Generic;

// Record for Transaction
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[BankTransfer] Processed transaction {transaction.Id}: {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[MobileMoney] Processed transaction {transaction.Id}: {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[CryptoWallet] Processed transaction {transaction.Id}: {transaction.Amount:C} for {transaction.Category}");
    }
}

// Account base
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Applied transaction {transaction.Id}. New balance: {Balance:C}");
    }
}

// Sealed SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) 
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine($"Insufficient funds for transaction {transaction.Id} ({transaction.Amount:C}). Current balance: {Balance:C}");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction {transaction.Id} applied. Updated balance: {Balance:C}");
        }
    }
}

// FinanceApp
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("SA-001", 2000m);

        var t1 = new Transaction(1, DateTime.Now, 150.00m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 300.00m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 500.00m, "Entertainment");

        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        p1.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);

        p2.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);

        p3.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}

