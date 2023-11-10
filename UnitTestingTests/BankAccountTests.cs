using UnitTesting.services;

namespace UnitTestingTests;

public class BankAccountTests
{
    [Test]
    public void Debit_WithValidAmount_UpdatesBalance()
    {
        // Arrange
        var beginningBalance = 11.99;
        var debitAmount = 4.55;
        var expected = 7.44;
        var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        // Act
        account.Debit(debitAmount);

        // Assert
        var actual = account.Balance;
        Assert.That(actual, Is.EqualTo(expected).Within(0.001), "Account not debited correctly");
    }
    
    [Test]
    public void Debit_WhenAmountIsLessThanZero_ShouldThrowArgumentOutOfRange()
    {
        // Arrange
        var beginningBalance = 11.99;
        var debitAmount = -100.00;
        var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        // Act and assert
        Assert.Throws<ArgumentOutOfRangeException>(() => account.Debit(debitAmount));
    }
}