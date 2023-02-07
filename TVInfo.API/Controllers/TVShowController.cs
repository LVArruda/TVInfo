using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TVInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TVShowController : ControllerBase
    {
        //TODO: This API project should be an Azure function. Does not make sense to have a full-blown API

        private readonly IMongoDBClient _mongodbClient;

        public TVShowController(IMongoDBClient client)
        {
            _mongodbClient = client;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int page)
        {
            var result = await _mongodbClient.GetAsync(page);

            if (result.Count() == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
