using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.UnitTests.Data
{
    public static class TestDataRepository
    {
        public static Partner Partner
        {
            get
            {
                var partner = new Partner()
                {
                    Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                    Name = "Суперигрушки",
                    IsActive = true,
                    PartnerLimits = new List<PartnerPromoCodeLimit>()
                    {
                        new PartnerPromoCodeLimit()
                        {
                            Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
                            CreateDate = new DateTime(2020, 07, 9),
                            EndDate = new DateTime(2020, 10, 9),
                            Limit = 100
                        }
                    }
                };

                return partner;
            }
        }

        public static Guid PartnerId
        {
            get
            {
                return Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            }
        }

        public static SetPartnerPromoCodeLimitRequest SetPartnerPromoCodeLimitRequest
        {
            get
            {
                return new SetPartnerPromoCodeLimitRequest()
                {
                    EndDate = new DateTime(2020, 12, 1),
                    Limit = 10,
                };
            }
        }
    }
}
