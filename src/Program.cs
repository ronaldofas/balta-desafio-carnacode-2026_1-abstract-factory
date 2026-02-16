using System;

namespace DesignPatternChallenge;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Sistema de Pagamentos (Abstract Factory) ===\n");

        // PagSeguro
        IPaymentGatewayFactory pagSeguroFactory = new PagSeguroGatewayFactory();
        var pagSeguroService = new PaymentService(pagSeguroFactory);
        pagSeguroService.ProcessPayment(150.00m, "1234567890123456");

        Console.WriteLine();

        // MercadoPago
        IPaymentGatewayFactory mercadoPagoFactory = new MercadoPagoGatewayFactory();
        var mercadoPagoService = new PaymentService(mercadoPagoFactory);
        mercadoPagoService.ProcessPayment(200.00m, "5234567890123456");

        Console.WriteLine();

        // Stripe
        IPaymentGatewayFactory stripeFactory = new StripeGatewayFactory();
        var stripeService = new PaymentService(stripeFactory);
        stripeService.ProcessPayment(300.00m, "4234567890123456");
    }
}