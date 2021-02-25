using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SomeWebApp.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(ILogger<AuthController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet, Route("getuserbynickname")]
        public async Task<IActionResult> GetUser([FromHeader] string nickname)
        {
            return Ok();
        }

        [HttpGet, Route("getuseryid")]
        public async Task<IActionResult> GetUser([FromHeader] UInt64 user_id)
        {
            return Ok();
        }

        [HttpPost, Route("deleteuser")]
        public async Task<IActionResult> DeleteUser([FromBody] UInt64 user_id)
        {
            return Ok();
        }

        [HttpPost, Route("restoreuser")]
        public async Task<IActionResult> RestoreUser([FromBody] UInt64 user_id)
        {
            return Ok();
        }
    }
}
