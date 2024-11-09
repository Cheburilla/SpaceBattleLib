using Hwdtech;

namespace SpaceBattle.Lib;

public class UObject : IUObject
{
    private readonly Dictionary<string, Properties> props;
    public UObject()
    {
        props = new();
    }

    public object getProperty(string key)
    {
        if (IoC.Resolve<bool>("Transactions.IsCommitedByID", props[key].transactionId) == false)
            return props[key].currentValue;
        return props[key].nextValue;
    }

    public void setProperty(string key, object val)
    {
        string transactionId = IoC.Resolve<string>("Transactions.GetCurrentID");
        if (!props.TryGetValue(key, out _))
        {
            Properties prop = new(val, val, transactionId);
            props[key] = prop;
        }
        else
        {
            props[key].transactionId = transactionId;
            props[key].nextValue = val;
            if (IoC.Resolve<bool>("Transactions.IsCommitedByID", props[key].transactionId) == true)
            {
                props[key].currentValue = props[key].nextValue;
            }
        }
    }
}
