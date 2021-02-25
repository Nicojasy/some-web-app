using SomeWebApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        /*
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await unitOfWork.Users.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await unitOfWork.Users.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(User product)
        {
            var data = await unitOfWork.Users.AddAsync(product);
            return Ok(data);
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await unitOfWork.Users.DeleteAsync(id);
            return Ok(data);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update(User product)
        {
            var data = await unitOfWork.Users.UpdateAsync(product);
            return Ok(data);
        }
        */
    }
}