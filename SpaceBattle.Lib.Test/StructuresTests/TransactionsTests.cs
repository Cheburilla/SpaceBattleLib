using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test;

public class TransactionsTest
{
    static TransactionsTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        Dictionary<string, bool> transactionManager = new();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Transactions.GetManager", (object[] args) => transactionManager).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Transactions.IsCommitedByID", (object[] args) =>
        {
            transactionManager.TryGetValue((string)args[0], out bool output);
            return (object)output;
        }).Execute();

        string currentTransactionId = "1";

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Transactions.GetCurrentID", (object[] args) =>
        {
            return currentTransactionId;
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Transactions.SetCurrentID", (object[] args) =>
        {
            currentTransactionId = (string)args[0];
            return currentTransactionId;
        }).Execute();
    }

    [Fact]
    public void PosTestGetCommitedTransaction()
    {
        Dictionary<string, bool> transactionManager = IoC.Resolve<Dictionary<string, bool>>("Transactions.GetManager");

        transactionManager["1"] = true;

        UObject obj = new();

        obj.setProperty("Velocity", 2);

        Assert.Equal(2, obj.getProperty("Velocity"));
    }

    [Fact]
    public void NegTestGetNonExistingKey()
    {
        Dictionary<string, bool> transactionManager = IoC.Resolve<Dictionary<string, bool>>("Transactions.GetManager");

        transactionManager["1"] = true;

        UObject obj = new();

        Assert.Throws<KeyNotFoundException>(() => { var a = obj.getProperty("Velocity"); });
    }

    [Fact]
    public void NegTestGetNotCommittedTransaction()
    {
        Dictionary<string, bool> transactionManager = IoC.Resolve<Dictionary<string, bool>>("Transactions.GetManager");

        transactionManager["1"] = true;

        UObject obj = new();

        obj.setProperty("Velocity", 2);

        Assert.Equal(2, obj.getProperty("Velocity"));

        IoC.Resolve<string>("Transactions.SetCurrentID", "2");

        transactionManager["2"] = false;

        obj.setProperty("Velocity", 4);

        Assert.Equal(2, obj.getProperty("Velocity"));
    }

    [Fact]
    public void PosTestSetCommitedTransaction()
    {
        Dictionary<string, bool> transactionManager = IoC.Resolve<Dictionary<string, bool>>("Transactions.GetManager");

        transactionManager["1"] = true;

        transactionManager["2"] = true;

        UObject obj = new();

        obj.setProperty("Velocity", 4);

        IoC.Resolve<string>("Transactions.SetCurrentID", "2");

        obj.setProperty("Velocity", 5);

        Assert.Equal(5, obj.getProperty("Velocity"));

    }

    [Fact]
    public void PosTestSetNotCommittedTransaction()
    {
        Dictionary<string, bool> transactionManager = IoC.Resolve<Dictionary<string, bool>>("Transactions.GetManager");

        transactionManager["1"] = false;

        transactionManager["2"] = true;

        UObject obj = new();

        obj.setProperty("Velocity", 4);

        IoC.Resolve<string>("Transactions.SetCurrentID", "2");

        obj.setProperty("Velocity", 5);

        Assert.Equal(5, obj.getProperty("Velocity"));
    }
}
