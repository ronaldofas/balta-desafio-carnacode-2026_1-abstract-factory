namespace DesignPatternChallenge;

public class StripeGatewayFactory : IPaymentGatewayFactory
{
    public IPaymentValidator CreateValidator() => new StripeValidator();
    public IPaymentProcessor CreateProcessor() => new StripeProcessor();
    public IPaymentLogger CreateLogger() => new StripeLogger();
}