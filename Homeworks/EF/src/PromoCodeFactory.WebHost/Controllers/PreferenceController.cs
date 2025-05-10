using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.EntityFramework;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferenceController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public PreferenceController(IMapper mapper, DataContext dataContext)
        {
            this._mapper = mapper;
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Возвращает список всех предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PreferenceResponse>> GetCustomersAsync()
        {
            var preferences = await _dataContext.Set<Preference>().ToListAsync();
            var result = _mapper.Map<List<PreferenceResponse>>(preferences);

            return Ok(result);
        }
    }
}
