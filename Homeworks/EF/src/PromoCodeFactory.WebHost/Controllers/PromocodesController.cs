using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.EntityFramework;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public PromocodesController(IMapper mapper, DataContext dataContext)
        {
            this._mapper = mapper;
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            //TODO: Получить все промокоды 
            var promocodes = await _dataContext.Set<PromoCode>().ToListAsync();
            var result = _mapper.Map<List<PreferenceResponse>>(promocodes);

            return Ok(result);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {            
            // получаем предпочтение
            var preference = await _dataContext.Set<Preference>().FirstOrDefaultAsync(x => x.Name == request.Preference);

            // получаем всех клиентов с указанным предпочтением
            var customers = _dataContext.Set<Customer>()
                .Include(x => x.CustomerPreferences)
                    .ThenInclude(x => x.Preference == preference);

            // создаем новый промокод
            var promocode = new PromoCode
            {
                Code = request.PromoCode,
                Customer = customers,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                PartnerName = request.PartnerName,
                Preference = preference,
                ServiceInfo = request.ServiceInfo,
            };
            _dataContext.Set<PromoCode>().Add(promocode);

            // выдаем промокод всем клиентам с указанным предпочтением
            foreach (var customer in customers)
            {
                customer.Promocode = promocode;
            }

            _dataContext.SaveChanges();

            return Ok();
        }
    }
}