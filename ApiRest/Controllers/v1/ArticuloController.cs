using ApiRest.Dto;
using ApiRest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiRest.Controllers.v1
{
    //[Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class ArticuloController : Controller
    {
        private readonly IArticleService _articleService;
        public ArticuloController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _articleService.GetById(id);
            ApiResponse<UserDto> response = new ApiResponse<UserDto> { Data = result };
            return Ok(response);
        }

    }
}
