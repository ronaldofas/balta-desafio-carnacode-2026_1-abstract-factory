namespace DesignPatternChallenge;

public interface IPaymentValidator
{
    bool ValidateCard(string cardNumber);
}
