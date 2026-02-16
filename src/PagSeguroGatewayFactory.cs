namespace DesignPatternChallenge;

public class PagSeguroGatewayFactory : IPaymentGatewayFactory
{
    public IPaymentValidator CreateValidator() => new PagSeguroValidator();
    public IPaymentProcessor CreateProcessor() => new PagSeguroProcessor();
    public IPaymentLogger CreateLogger() => new PagSeguroLogger();
}
