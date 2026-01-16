public abstract class Animal {
    public abstract void MakeNoise();
}

// In this case we use inheritance to concretize
// a more general Animal class. Bird is a more specific case
// of an animal and makes for example a more specific noise.
public class Bird : Animal
{
    public override void MakeNoise()
    {
        Console.WriteLine("I am a bird!");
    }
}

public class BasicAccount
{
    public decimal Balance { get; private set; }
    
    public virtual void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"Deposited {amount}. New balance is {Balance}.");
    }

    public virtual void CheckBalance()
    {
        Console.WriteLine($"Your balance is {Balance}.");
    }
}

// In this case we extended our base class
// BasicAccount and added new functionality to the derived class(if I got it right)
public class PremiumAccount : BasicAccount
{
    public string AccountType { get; private set; }

    public Account(string accountNumber, decimal initialBalance, string accountType)
        : base(accountNumber, initialBalance)
    {
        AccountType = accountType;
    }

    public override void Deposit(decimal amount)
    {
        base.Deposit(amount);
        Console.WriteLine($"{AccountType} benefits applied.");
    }

    public void EarnInterest(decimal interestRate)
    {
        decimal interest = Balance * interestRate;
        Balance += interest;
        Console.WriteLine($"Interest of {interest} earned. New balance is {Balance}.");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount;
            Console.WriteLine($"Withdrew {amount}. New balance is {Balance}.");
        }
        else
        {
            Console.WriteLine("Insufficient funds.");
        }
    }
}