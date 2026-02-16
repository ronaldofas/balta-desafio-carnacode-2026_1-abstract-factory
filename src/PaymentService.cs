using System;

namespace DesignPatternChallenge;
public class PaymentService
{
    private readonly IPaymentGatewayFactory _gatewayFactory;

    public PaymentService(IPaymentGatewayFactory gatewayFactory)
    {
        _gatewayFactory = gatewayFactory;
    }

    public void ProcessPayment(decimal amount, string cardNumber)
    {
        var validator = _gatewayFactory.CreateValidator();
        if (!validator.ValidateCard(cardNumber))
        {
            Console.WriteLine("Cartão inválido");
            return;
        }

        var processor = _gatewayFactory.CreateProcessor();
        var result = processor.ProcessTransaction(amount, cardNumber);

        var logger = _gatewayFactory.CreateLogger();
        logger.Log($"Transação processada: {result}");
    }
}
