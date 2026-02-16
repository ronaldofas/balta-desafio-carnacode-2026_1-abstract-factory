// DESAFIO: Sistema de Pagamentos Multi-Gateway
// PROBLEMA: Uma plataforma de e-commerce precisa integrar com múltiplos gateways de pagamento
// (PagSeguro, MercadoPago, Stripe) e cada gateway tem componentes específicos (Processador, Validador, Logger)
// O código atual está muito acoplado e dificulta a adição de novos gateways

using System;

namespace DesignPatternChallenge
{
    // Contexto: Sistema de pagamentos que precisa trabalhar com diferentes gateways
    // Cada gateway tem sua própria forma de processar, validar e logar transações
    
    public class PaymentService
    {
        private readonly string _gateway;

        public PaymentService(string gateway)
        {
            _gateway = gateway;
        }

        public void ProcessPayment(decimal amount, string cardNumber)
        {
            // Problema: Switch case gigante para cada gateway
            // Quando adicionar novo gateway, precisa modificar este método
            switch (_gateway.ToLower())
            {
                case "pagseguro":
                    var pagSeguroValidator = new PagSeguroValidator();
                    if (!pagSeguroValidator.ValidateCard(cardNumber))
                    {
                        Console.WriteLine("PagSeguro: Cartão inválido");
                        return;
                    }
                    
                    var pagSeguroProcessor = new PagSeguroProcessor();
                    var pagSeguroResult = pagSeguroProcessor.ProcessTransaction(amount, cardNumber);
                    
                    var pagSeguroLogger = new PagSeguroLogger();
                    pagSeguroLogger.Log($"Transação processada: {pagSeguroResult}");
                    break;

                case "mercadopago":
                    var mercadoPagoValidator = new MercadoPagoValidator();
                    if (!mercadoPagoValidator.ValidateCard(cardNumber))
                    {
                        Console.WriteLine("MercadoPago: Cartão inválido");
                        return;
                    }
                    
                    var mercadoPagoProcessor = new MercadoPagoProcessor();
                    var mercadoPagoResult = mercadoPagoProcessor.ProcessTransaction(amount, cardNumber);
                    
                    var mercadoPagoLogger = new MercadoPagoLogger();
                    mercadoPagoLogger.Log($"Transação processada: {mercadoPagoResult}");
                    break;

                case "stripe":
                    var stripeValidator = new StripeValidator();
                    if (!stripeValidator.ValidateCard(cardNumber))
                    {
                        Console.WriteLine("Stripe: Cartão inválido");
                        return;
                    }
                    
                    var stripeProcessor = new StripeProcessor();
                    var stripeResult = stripeProcessor.ProcessTransaction(amount, cardNumber);
                    
                    var stripeLogger = new StripeLogger();
                    stripeLogger.Log($"Transação processada: {stripeResult}");
                    break;

                default:
                    throw new ArgumentException("Gateway não suportado");
            }
        }
    }

    // Componentes do PagSeguro
    public class PagSeguroValidator
    {
        public bool ValidateCard(string cardNumber) 
        {
            Console.WriteLine("PagSeguro: Validando cartão...");
            return cardNumber.Length == 16;
        }
    }

    public class PagSeguroProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"PagSeguro: Processando R$ {amount}...");
            return $"PAGSEG-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class PagSeguroLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[PagSeguro Log] {DateTime.Now}: {message}");
        }
    }

    // Componentes do MercadoPago
    public class MercadoPagoValidator
    {
        public bool ValidateCard(string cardNumber)
        {
            Console.WriteLine("MercadoPago: Validando cartão...");
            return cardNumber.Length == 16 && cardNumber.StartsWith("5");
        }
    }

    public class MercadoPagoProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"MercadoPago: Processando R$ {amount}...");
            return $"MP-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class MercadoPagoLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[MercadoPago Log] {DateTime.Now}: {message}");
        }
    }

    // Componentes do Stripe
    public class StripeValidator
    {
        public bool ValidateCard(string cardNumber)
        {
            Console.WriteLine("Stripe: Validando cartão...");
            return cardNumber.Length == 16 && cardNumber.StartsWith("4");
        }
    }

    public class StripeProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"Stripe: Processando ${amount}...");
            return $"STRIPE-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class StripeLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[Stripe Log] {DateTime.Now}: {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Pagamentos ===\n");

            // Problema: Cliente precisa saber qual gateway está usando
            // e o código de processamento está todo acoplado
            var pagSeguroService = new PaymentService("pagseguro");
            pagSeguroService.ProcessPayment(150.00m, "1234567890123456");

            Console.WriteLine();

            var mercadoPagoService = new PaymentService("mercadopago");
            mercadoPagoService.ProcessPayment(200.00m, "5234567890123456");

            Console.WriteLine();

            // Pergunta para reflexão:
            // - Como adicionar um novo gateway sem modificar PaymentService?
            // - Como garantir que todos os componentes de um gateway sejam compatíveis entre si?
            // - Como evitar criar componentes de gateways diferentes acidentalmente?
            //
            // Respostas (utilizando o padrão Abstract Factory):
            //
            // 1. Como adicionar um novo gateway sem modificar PaymentService?
            //    R: Criando uma interface IPaymentGatewayFactory com métodos como
            //    CreateValidator(), CreateProcessor() e CreateLogger(). Cada novo gateway
            //    implementa essa interface em uma classe concreta (ex: PagSeguroFactory,
            //    StripeFactory). O PaymentService recebe a factory via injeção de dependência
            //    (no construtor) e utiliza apenas a interface, sem conhecer as implementações
            //    concretas. Assim, para adicionar um novo gateway, basta criar uma nova factory
            //    concreta — sem modificar nenhuma linha do PaymentService (princípio Open/Closed).
            //
            // 2. Como garantir que todos os componentes de um gateway sejam compatíveis entre si?
            //    R: A Abstract Factory resolve exatamente esse problema. Como cada factory concreta
            //    (ex: MercadoPagoFactory) é responsável por criar TODOS os componentes do seu
            //    gateway (Validator, Processor e Logger), ela garante que os objetos retornados
            //    são da mesma "família". É impossível, por exemplo, que a MercadoPagoFactory
            //    crie um StripeValidator por acidente, pois cada factory encapsula a criação de
            //    seus próprios componentes coesos.
            //
            // 3. Como evitar criar componentes de gateways diferentes acidentalmente?
            //    R: Sem o padrão, o desenvolvedor instancia manualmente cada componente (new
            //    PagSeguroValidator(), new StripeProcessor()...), o que abre margem para misturar
            //    componentes de gateways diferentes. Com a Abstract Factory, o código cliente
            //    nunca usa "new" diretamente para criar esses componentes — ele chama os métodos
            //    da factory (factory.CreateValidator(), factory.CreateProcessor(), etc.), e a
            //    factory garante que todos pertencem ao mesmo gateway. Isso elimina o risco de
            //    combinações incorretas e centraliza a criação dos objetos em um único ponto.
        }
    }
}
