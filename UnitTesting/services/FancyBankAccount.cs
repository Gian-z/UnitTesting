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
        {"usd", 118},
        {"gbp", 76},
        {"jpy", 103},
        {"aud", 65},
        {"cad", 80},
        {"sek", 92},
        {"nok", 86},
        {"dkk", 91},
        {"nzd", 70},
        {"sgd", 63},
        {"hkd", 79},
        {"inr", 52},
        {"cny", 62},
        {"brl", 36},
        {"zar", 70},
        {"try", 27},
        {"rub", 82},
        {"idr", 12},
        {"myr", 23},
        {"thb", 29},
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

    public void Listen()
    {
        while (true)
        {
            Console.WriteLine("Enter Command:");
            var command = Console.ReadLine();
            typeof(FancyBankAccount).GetMethod(command).Invoke(this, new object?[]{false});
        }
    }

    public void Debit(bool ignorePin = false)
    {
        var pin = string.Empty;
        if (!ignorePin)
        {
            Console.WriteLine("Enter pin:");
            pin = Console.ReadLine();
        }
        
        Console.WriteLine("Enter debit currency:");
        var debitCurrency = Console.ReadLine();
        
        Console.WriteLine("Enter amount:");
        var amount = double.Parse(Console.ReadLine());
            
            
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

        var actualAmount = amount / _currencyTable[debitCurrency] * _currencyTable[_currency];
        _mBalance -= actualAmount;
        Console.WriteLine(historyEntry);

        if (!ignorePin && pin != _pin)
        {
            Credit(true);
            throw new InvalidCredentialException(nameof(pin));
        }
    }

    public void Credit(bool ignorePin = false)
    {
        var pin = string.Empty;
        if (!ignorePin)
        {
            Console.WriteLine("Enter pin:");
            pin = Console.ReadLine();
        }
        
        Console.WriteLine("Enter debit currency:");
        var debitCurrency = Console.ReadLine();
        
        Console.WriteLine("Enter amount:");
        var amount = double.Parse(Console.ReadLine());
        
        var historyEntry = $"[{_mCustomerName}]: Credit {amount} to {_mBalance}";
        _history.Add(historyEntry);
        
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        var actualAmount = amount / _currencyTable[debitCurrency] * _currencyTable[_currency];
        _mBalance += actualAmount;
        Console.WriteLine(historyEntry);
        
        if (!ignorePin && pin != _pin)
        {
            Debit(true);
            throw new InvalidCredentialException(nameof(pin));
        }
    }

    public void Transfer(FancyBankAccount to, double amount)
    {
        var historyEntry = $"[{_mCustomerName}] -> [{to.CustomerName}]: Transfer {amount}";
        _history.Add(historyEntry);
        
        Debit();
        to.Credit();
        
        Console.WriteLine(historyEntry);
    }
}