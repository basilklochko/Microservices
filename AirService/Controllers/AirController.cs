using AirService.Storage;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace AirService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirController : ControllerBase
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