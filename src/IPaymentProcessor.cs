namespace DesignPatternChallenge;

public interface IPaymentProcessor
{
    string ProcessTransaction(decimal amount, string cardNumber);
}
