using HotelService.Storage;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace HotelService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Orderable>> Get()
        {
            return await Task.Run(() =>
            {
                return Db.Orders.Select(o => o.Value).OrderByDescending(o => o.UpdatedAt).ToList();
            });
        }
    }
}