using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
using TourMate.UserService.Repositories.ResponseModels;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Utils;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly ITourGuideService _tourGuideService;

        public AccountController(IAccountService accountService, ICustomerService customerService, ITourGuideService tourGuideService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] UserLogin loginRequest)
        {
            // Cố gắng đăng nhập với email và mật khẩu đã cung cấp
            var login = await _accountService.LoginAsync(loginRequest.Email, loginRequest.Password);

            // Nếu đăng nhập không thành công, trả về BadRequest với thông báo lỗi
            if (login == null)
            {
                return BadRequest(new { msg = "Tài khoản hoặc mật khẩu không đúng." });
            }

            // Nếu đăng nhập thành công, trả về phản hồi đăng nhập
            return Ok(login);
        }

        [HttpPost("register-tourguide")]
        public async Task<ActionResult> RegisterTourGuide(RegisterTourGuide request)
        {
            DateOnly dateOfBirth = DateOnly.FromDateTime(request.DateOfBirth);

            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrEmpty(request.Image) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Gender) || string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.Description) || string.IsNullOrEmpty(dateOfBirth.ToString())
                || string.IsNullOrEmpty(request.Company) || string.IsNullOrEmpty(request.BankName) || string.IsNullOrEmpty(request.BankAccountNumber) || string.IsNullOrEmpty(request.BannerImage) || string.IsNullOrEmpty(request.AreaId.ToString()))
                return BadRequest(new { msg = "Thông tin tài khoản chưa đầy đủ!" });

            if (!ValidInput.IsPhoneFormatted(request.Phone.Trim()))
                return BadRequest(new { msg = "Số điện thoại không đúng định dạng!" });

            if (!ValidInput.IsMailFormatted(request.Email))
                return BadRequest(new { msg = "Email không đúng định dạng!" });

            // Kiểm tra tài khoản đã tồn tại
            var existingAccount = await _accountService.GetAccountByEmail(request.Email);
            if (existingAccount != null)
                return Conflict(new { msg = "Tài khoản đã tồn tại." });

            var existingPhone = await _customerService.GetCustomerByPhone(request.Phone);
            if (existingPhone != null)
                return Conflict(new { msg = "Số điện thoại đã được sử dụng." });

            // Tạo đối tượng tài khoản
            var account = new Account
            {
                Email = request.Email,
                Password = HashString.ToHashString(request.Password),
                RoleId = 3,
                Status = true,
                CreatedDate = DateTime.Now,
            };
            // Lưu tài khoản
            var isAccountCreated = await _accountService.CreateAccount(account);
            if (isAccountCreated == null)
                return StatusCode(500, new { msg = "Xảy ra lỗi khi tạo tài khoản!" });

            // Tạo đối tượng hướng dẫn viên
            var tourGuide = new TourGuide
            {
                AccountId = isAccountCreated.AccountId,
                FullName = request.FullName,
                Gender = request.Gender,
                Phone = request.Phone,
                DateOfBirth = dateOfBirth,
                Address = request.Address,
                Image = request.Image,
                AreaId = request.AreaId,
                BannerImage = request.BannerImage,
                BankAccountNumber = request.BankAccountNumber,
                BankName = request.BankName,
                Company = request.Company,
                YearOfExperience = request.YearOfExperience,
                Description = request.Description,
                IsVerified = false, // Mặc định là chưa được xác minh  
            };



            // Lưu khách hàng
            var isTourGuideCreated = await _tourGuideService.CreateTourGuide(tourGuide);
            if (!isTourGuideCreated)
                return StatusCode(500, new { msg = "Xảy ra lỗi khi tạo TourGuide!"});

            return Ok(new { msg = "Tạo tài khoản thành công." });
        }

        [HttpPost("register-customer")]
        public async Task<ActionResult> RegisterCustomer(RegisterCustomer request)
        {
            // Kiểm tra dữ liệu nhập
            if (request.Email == null || request.Password == null || request.FullName == null ||request.Gender == null || request.Phone == null || string.IsNullOrEmpty((request.DateOfBirth.ToString())))
                return BadRequest(new { msg = "Thông tin tài khoản chưa đầy đủ." });

            if (!ValidInput.IsPhoneFormatted(request.Phone.Trim()))
                return BadRequest(new { msg = "Số điện thoại không đúng định dạng." });

            if (!ValidInput.IsMailFormatted(request.Email))
                return BadRequest(new { msg = "Email không đúng định dạng." });

            if (!ValidInput.IsPasswordSecure(request.Password))
                return BadRequest(new { msg = "Mật khẩu cần có ít nhất 12 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt." });

            // Kiểm tra tài khoản đã tồn tại
            var existingAccount = await _accountService.GetAccountByEmail(request.Email);
            if (existingAccount != null)
                return Conflict(new { msg = "Tài khoản đã tồn tại." });

            var existingPhone = await _customerService.GetCustomerByPhone(request.Phone);
            if (existingPhone != null)
                return Conflict(new { msg = "Số điện thoại đã được sử dụng." });

            // Tạo đối tượng tài khoản
            var account = new Account
            {
                Email = request.Email,
                Password = HashString.ToHashString(request.Password),
                RoleId = 2,
                Status = true,
                CreatedDate = DateTime.Now,
            };

            // Lưu tài khoản
            var isAccountCreated = await _accountService.CreateAccount(account);
            if (isAccountCreated == null)
                return StatusCode(500, new { msg = "Đã xảy ra lỗi khi đăng ký tài khoản." });

            // Tạo đối tượng khách hàng
            var customer = new Customer
            {
                AccountId = isAccountCreated.AccountId,
                FullName = request.FullName,
                Gender = request.Gender,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
            };

            // Lưu khách hàng
            var isCustomerCreated = await _customerService.CreateCustomer(customer);
            if (!isCustomerCreated)
                return StatusCode(500, new { msg = "Đã xảy ra lỗi khi đăng ký khách hàng." });

            return Ok(new { msg = "Đăng ký thành công." });
        }
    }
}
