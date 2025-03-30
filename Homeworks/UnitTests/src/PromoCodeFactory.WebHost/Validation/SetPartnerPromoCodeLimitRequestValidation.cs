using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.DataAccess.Validation
{    
    public class SetPartnerPromoCodeLimitRequestValidation : IBaseValidation<Partner>
    {
        public static readonly string PartnerInNotActive = "Данный партнер не активен";
        public static readonly string LimitShouldBeGreaterThenZero = "Лимит должен быть больше 0";

        private readonly Guid _id;
        private readonly IRepository<Partner> _partnersRepository;
        private Partner _partner;
        private readonly SetPartnerPromoCodeLimitRequest _request;        

        public SetPartnerPromoCodeLimitRequestValidation(IRepository<Partner> partnersRepository, 
            Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            this._id = id;
            this._partnersRepository = partnersRepository;
            this._request = request;
        }

        public Partner GetValidEntity()
        {
            return this._partner;
        }

        public async Task<ValidationResult> Validate()
        {
            this._partner = await _partnersRepository.GetByIdAsync(this._id);

            if (this._partner == null)
                return new ValidationResult(ValidationStatus.NotFound);                

            //Если партнер заблокирован, то нужно выдать исключение
            if (!this._partner.IsActive)
                return new ValidationResult(ValidationStatus.BadRequest, PartnerInNotActive);

            //Установка лимита партнеру
            var activeLimit = this._partner.PartnerLimits.FirstOrDefault(x =>
                !x.CancelDate.HasValue);

            if (activeLimit != null)
            {
                //Если партнеру выставляется лимит, то мы 
                //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
                //то количество не обнуляется
                this._partner.NumberIssuedPromoCodes = 0;

                //При установке лимита нужно отключить предыдущий лимит
                activeLimit.CancelDate = DateTime.Now;
            }

            if (this._request.Limit <= 0)
                return new ValidationResult(ValidationStatus.BadRequest, LimitShouldBeGreaterThenZero);

            return new ValidationResult(ValidationStatus.Success);
        }
    }
}
