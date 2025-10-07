using Pcf.RabbitMQ;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Integration.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class AdministrationGatewayRabbitMQ : IAdministrationGateway
    {
        private readonly MasstransitService _service;

        public AdministrationGatewayRabbitMQ(MasstransitService service)
        {
            _service = service;
        }

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            var dto = new NotifyAdminAboutPartnerManagerDto
            {
                ManagerId = partnerManagerId
            };

            await _service.SendAsync("queue-Admin", dto, CancellationToken.None);
        }
    }
}
