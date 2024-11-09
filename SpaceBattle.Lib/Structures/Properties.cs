namespace SpaceBattle.Lib;

public class Properties {
    public object currentValue;
    public object nextValue;
    public string transactionId;

    public Properties(object currentValue, object nextValue, string transactionId)
    {
        this.currentValue = currentValue;
        this.nextValue = nextValue;
        this.transactionId = transactionId;
    }
}
