using Pcf.RabbitMQ;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration.Dto;
using System.Threading;
using System.Threading.Tasks;


namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGatewayRabbitMQ : IGivingPromoCodeToCustomerGateway
    {
        private readonly MasstransitService _service;        
        public GivingPromoCodeToCustomerGatewayRabbitMQ(MasstransitService service)
        {        
            _service = service;
        }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var dto = new GivePromoCodeToCustomerDto()
            {
                PartnerId = promoCode.Partner.Id,
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId,
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId
            };            

            await _service.SendAsync("queue-PromoCode", dto, CancellationToken.None);
        }
    }
}
