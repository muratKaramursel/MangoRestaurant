using Mango.Services.PaymentAPI.Messaging;

namespace Mango.Services.PaymentAPI.Extension
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer _azureServiceBusConsumer;

        private static void OnStart()
        {
            _azureServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            _azureServiceBusConsumer.Stop();
        }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder applicationBuilder)
        {
            _azureServiceBusConsumer = applicationBuilder.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            IHostApplicationLifetime hostApplicationLifetime = applicationBuilder.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStarted.Register(OnStart);
            hostApplicationLifetime.ApplicationStopped.Register(OnStop);

            return applicationBuilder;
        }
    }
}