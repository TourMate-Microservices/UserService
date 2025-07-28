using Microsoft.AspNetCore.Mvc;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Utils;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [HttpGet("get-by-accid/{accId}")]
        public async Task<IActionResult> GetByAccId(int accId)
        {
            var customer = await _customerService.GetCustomerByAccId(accId);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet("get-by-account/{id}")]
        public async Task<IActionResult> GetByAccountId(int id)
        {
            var customer = await _customerService.GetCustomerByAccId(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }


        [HttpGet("get-by-phone/{phone}")]
        public async Task<IActionResult> GetByPhone(string phone)
        {
            var customer = await _customerService.GetCustomerByPhone(phone);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Invalid customer data.");
            }
            var result = await _customerService.CreateCustomer(customer);
            if (!result)
            {
                return StatusCode(500, "An error occurred while creating the customer.");
            }
            return CreatedAtAction(nameof(GetByAccId), new { accId = customer.AccountId }, customer);
        }
        [HttpGet("paged-customers")]
        public async Task<IActionResult> GetPagedCustomers(int pageIndex, int pageSize, string fullName = null)
        {
            var pagedResult = await _customerService.GetPagedCustomersAsync(pageIndex, pageSize, fullName);
            return Ok(pagedResult);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerUpdateRequest request)
        {
            if (!ValidInput.IsPhoneFormatted(request.Phone.Trim()))
                return BadRequest(new { msg = "Số điện thoại không đúng định dạng." });
            var result = await _customerService.UpdateCustomer(id, request);
            if (result)
            {
                return Ok(new { Message = "Cập nhật thành công" });
            }
            return BadRequest(new { Message = "Cập nhật thất bại" });
        }
    }
}
