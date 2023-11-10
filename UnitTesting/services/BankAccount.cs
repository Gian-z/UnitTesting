namespace UnitTesting.services;

using System;

/// <summary>
/// Bank account demo class.
/// </summary>
public class BankAccount
{
    private readonly string _mCustomerName;
    private double _mBalance;

    public BankAccount(string customerName, double balance)
    {
        _mCustomerName = customerName;
        _mBalance = balance;
    }

    public string CustomerName => _mCustomerName;

    public double Balance => _mBalance;
    

    public void Credit(double amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        _mBalance += amount;
    }
}
