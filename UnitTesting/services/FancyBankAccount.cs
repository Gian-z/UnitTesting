using System.Security.Authentication;
using System.Text.RegularExpressions;

namespace UnitTesting.services;

public class FancyBankAccount
{
    private readonly string _mCustomerName;
    private readonly string _pin;
    private readonly double _limit;
    private double _mBalance;
    private readonly List<string> _history = new();

    public FancyBankAccount(string customerName, string pin, double balance, double limit)
    {
        _mCustomerName = customerName;
        _pin = pin;
        _mBalance = balance;
        _limit = limit;
    }

    public string CustomerName => _mCustomerName;

    public double Balance => _mBalance;

    public void Debit(string pin, double amount, bool ignorePin = false)
    {
        var historyEntry = $"[{_mCustomerName}]: Debit {amount} form {_mBalance}";
        _history.Add(historyEntry);
        
        var debitRegex = new Regex(".*: Debit (amount?) from .*");
        if (_history.Where(e => debitRegex.IsMatch(e))
                .Select(e => double.Parse(debitRegex.Match(e).Groups["amount"].Value)).Sum() > _limit)
        {
            throw new ArgumentOutOfRangeException(nameof(_limit));
        }
        
        if (amount > _mBalance)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        _mBalance -= amount;
        Console.WriteLine(historyEntry);

        if (!ignorePin && pin != _pin)
        {
            Credit(pin, amount, true);
            throw new InvalidCredentialException(nameof(pin));
        }
    }

    public void Credit(string pin, double amount, bool ignorePin = false)
    {
        var historyEntry = $"[{_mCustomerName}]: Credit {amount} to {_mBalance}";
        _history.Add(historyEntry);
        
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        _mBalance += amount;
        Console.WriteLine(historyEntry);
        
        if (!ignorePin && pin != _pin)
        {
            Debit(pin, amount, true);
            throw new InvalidCredentialException(nameof(pin));
        }
    }

    public void Transfer(string pinFrom, string pinTo, FancyBankAccount to, double amount)
    {
        var historyEntry = $"[{_mCustomerName}] -> [{to.CustomerName}]: Transfer {amount}";
        _history.Add(historyEntry);
        
        Debit(pinFrom, amount);
        to.Credit(pinTo, amount);
        
        Console.WriteLine(historyEntry);
    }
}