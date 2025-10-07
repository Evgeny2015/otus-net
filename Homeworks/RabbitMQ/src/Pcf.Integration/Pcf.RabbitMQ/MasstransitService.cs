using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pcf.RabbitMQ
{
    public class MasstransitService : IHostedService
    {
        private IBusControl _busControl;        
        private readonly ILogger<MasstransitService> _logger;

        public MasstransitService(ILogger<MasstransitService> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _busControl.StartAsync(cancellationToken);            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StopAsync(cancellationToken);
        }

        public async Task SendAsync(string queue, object message, CancellationToken cancellationToken)
        {
            var address = _busControl.Address;
            
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"{address.Scheme}://{address.Host}/{queue}"));
            try
            {
                await StartAsync(cancellationToken);
                await sendEndpoint.Send(message, cancellationToken);
            }
            finally
            {
                await StopAsync(cancellationToken);
            }
        }
    }
}
