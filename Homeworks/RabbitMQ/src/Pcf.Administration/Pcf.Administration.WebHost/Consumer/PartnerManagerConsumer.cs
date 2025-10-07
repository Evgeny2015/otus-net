using MassTransit;
using Microsoft.AspNetCore.Mvc.Formatters;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.WebHost;
using Pcf.Administration.WebHost.Service;
using Pcf.ReceivingFromPartner.Integration.Dto;
using System.Threading.Tasks;

namespace Pcf.RabbitMQ.Consumer
{
    public class PartnerManagerConsumer(        
        IEmployeeService employeeService
        ) : IConsumer<NotifyAdminAboutPartnerManagerDto>
    {        
        public async Task Consume(ConsumeContext<NotifyAdminAboutPartnerManagerDto> context)
        {
            var message = context.Message;

            if (await employeeService.UpdateAppliedPromocodesAsync(message.ManagerId))
                await context.RespondAsync(message);
        }
    }
}
