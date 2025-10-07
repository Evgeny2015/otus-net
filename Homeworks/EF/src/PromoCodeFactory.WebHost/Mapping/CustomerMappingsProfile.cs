using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mapping
{
    public class CustomerMappingsProfile : Profile
    {
        public CustomerMappingsProfile()
        {
            CreateMap<Customer, CustomerResponse>();
            CreateMap<Customer, CustomerShortResponse>();
            CreateMap<Preference, PreferenceResponse>();
            CreateMap<PromoCode, PromoCodeShortResponse>();
        }            
    }
}
