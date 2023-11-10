using System.Security.Authentication;
using System.Text.RegularExpressions;

namespace UnitTesting.services;

public class FancyBankAccount
{
    private readonly string _mCustomerName;
    private readonly string _pin;
    private readonly string _currency;
    private readonly double _limit;
    private double _mBalance;
    private readonly List<string> _history = new();

    private static readonly Dictionary<string, int> _currencyTable = new()
    {
        {"usd", 120},
        {"gbp", 78},
        {"jpy", 105},
        {"aud", 67},
        {"cad", 82},
        {"sek", 94},
        {"nok", 88},
        {"dkk", 93},
        {"nzd", 72},
        {"sgd", 65},
        {"hkd", 81},
        {"inr", 54},
        {"cny", 64},
        {"brl", 38},
        {"zar", 72},
        {"try", 29},
        {"rub", 84},
        {"idr", 14},
        {"myr", 25},
        {"thb", 31},
        {"php", 20},
        {"mxn", 16},
        {"ars", 42},
        {"clp", 53},
        {"cop", 18},
        {"pen", 23},
        {"vef", 8},
        {"aed", 28},
        {"sar", 27},
        {"qar", 26},
        {"ils", 36},
        {"egp", 22},
        {"ngn", 12},
        {"zar", 72},
        {"krw", 61},
        {"vnd", 6},
        {"czk", 47},
        {"huf", 39},
        {"pln", 50},
        {"ron", 34},
        {"try", 29},
        {"hrk", 43},
        {"bam", 55},
        {"kes", 9},
        {"ngn", 12},
        {"zar", 72},
        {"chf", 89},
        {"eur", 101}
    };

    public FancyBankAccount(string customerName, string pin, string currency, double balance, double limit)
    {
        _mCustomerName = customerName;
        _pin = pin;
        _currency = currency;
        _mBalance = balance;
        _limit = limit;
    }

    public string CustomerName => _mCustomerName;

    public double Balance => _mBalance;

    public void Debit(string pin, string asCurrency, double amount, bool ignorePin = false)
    {
        var historyEntry = $"[{_mCustomerName}]: Debit {amount} form {_mBalance}";
        _history.Add(historyEntry);
        
        var debitRegex = new Regex(".*: Debit (?<amount>) from .*");
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

        var actualAmount = amount / _currencyTable[asCurrency] * _currencyTable[_currency];
        _mBalance -= actualAmount;
        Console.WriteLine(historyEntry);

        if (!ignorePin && pin != _pin)
        {
            Credit(pin, asCurrency, amount, true);
            throw new InvalidCredentialException(nameof(pin));
        }
    }

    public void Credit(string pin, string asCurrency, double amount, bool ignorePin = false)
    {
        var historyEntry = $"[{_mCustomerName}]: Credit {amount} to {_mBalance}";
        _history.Add(historyEntry);
        
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        var actualAmount = amount / _currencyTable[asCurrency] * _currencyTable[_currency];
        _mBalance += actualAmount;
        Console.WriteLine(historyEntry);
        
        if (!ignorePin && pin != _pin)
        {
            Debit(pin, asCurrency, amount, true);
            throw new InvalidCredentialException(nameof(pin));
        }
    }

    public void Transfer(string pinFrom, string pinTo, FancyBankAccount to, string asCurrency, double amount)
    {
        var historyEntry = $"[{_mCustomerName}] -> [{to.CustomerName}]: Transfer {amount}";
        _history.Add(historyEntry);
        
        Debit(pinFrom, asCurrency, amount);
        to.Credit(pinTo, asCurrency, amount);
        
        Console.WriteLine(historyEntry);
    }
}