using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Utils;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly ITourGuideService _tourGuideService;
        private readonly IEmailSender _emailSender;

        public AccountController(IAccountService accountService, ICustomerService customerService, ITourGuideService tourGuideService, IEmailSender emailSender)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
            _emailSender = emailSender;
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
                return StatusCode(500, new { msg = "Xảy ra lỗi khi tạo TourGuide!" });

            return Ok(new { msg = "Tạo tài khoản thành công." });
        }

        [HttpPost("register-customer")]
        public async Task<ActionResult> RegisterCustomer(RegisterCustomer request)
        {
            DateOnly dateOfBirth = DateOnly.FromDateTime(request.DateOfBirth);

            // Kiểm tra dữ liệu nhập
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Gender) || string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.DateOfBirth.ToString()))
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
                DateOfBirth = dateOfBirth,
            };

            // Lưu khách hàng
            var isCustomerCreated = await _customerService.CreateCustomer(customer);
            if (!isCustomerCreated)
                return StatusCode(500, new { msg = "Đã xảy ra lỗi khi đăng ký khách hàng." });

            // Gửi email chào mừng
            string emailBody = GenerateCustomerWelcomeEmail(request.FullName, request.Email);

            try
            {
                await _emailSender.SendEmailAsync(request.Email, "Chào mừng đến với TourMate", emailBody);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi gửi email nhưng không làm gián đoạn quá trình đăng ký
                Console.WriteLine($"Email send failed: {ex.Message}");
            }

            return Ok(new { msg = "Đăng ký thành công." });
        }

        private string GenerateCustomerWelcomeEmail(string fullName, string email)
        {
            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
  <meta charset='UTF-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1' />
  <title>Chào mừng đến với TourMate</title>
  <style>
    body, html {{
      margin: 0; padding: 0; height: 100%; width: 100%;
      background-color: #f5f8fa;
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      color: #000000;
    }}
    a {{
      color: #ffffff; text-decoration: none;
    }}
    .email-wrapper {{
      max-width: 600px;
      margin: 40px auto;
      background-color: #ffffff;
      border-radius: 14px;
      box-shadow: 0 6px 20px rgba(0,0,0,0.05);
      overflow: hidden;
    }}
    .email-header {{
      background-color: #0056b3;
      padding: 30px 20px;
      text-align: center;
      color: #ffffff;
    }}
    .email-body {{
      padding: 40px 40px 60px;
      font-size: 17px;
      line-height: 1.6;
      color: #333333;
    }}
    .email-body h1 {{
      font-size: 26px;
      font-weight: 700;
      margin-bottom: 20px;
    }}
    .email-body p {{
      margin-bottom: 20px;
    }}
    .highlight {{
      background-color: #e2f0d9;
      padding: 10px 15px;
      border-left: 5px solid #28a745;
      border-radius: 6px;
    }}
    .account-info {{
      background-color: #e6f4ff;
      padding: 12px 16px;
      border-left: 5px solid #007bff;
      border-radius: 6px;
      margin-top: 20px;
    }}
    .email-footer {{
      background-color: #f0f4f8;
      color: #555555;
      text-align: center;
      font-size: 13px;
      padding: 20px 30px;
      border-top: 1px solid #dfe3e9;
    }}
    @media only screen and (max-width: 480px) {{
      .email-wrapper {{
        width: 95% !important;
        margin: 20px auto !important;
      }}
      .email-body {{
        font-size: 15px !important;
        padding: 25px 20px 35px !important;
      }}
    }}
  </style>
</head>
<body>
  <div class='email-wrapper' role='article' aria-roledescription='email' lang='vi'>
    <header class='email-header'>
      <h2>Chào mừng bạn đến với TourMate!</h2>
    </header>
    <section class='email-body'>
      <h1>Xin chào {fullName},</h1>
      <p>Cảm ơn bạn đã đăng ký tài khoản tại TourMate.</p>
      
      <div class='highlight'>
        Tài khoản của bạn đã được tạo thành công. Bạn có thể bắt đầu khám phá và đặt các tour du lịch phù hợp với nhu cầu của mình ngay bây giờ!
      </div>

      <div class='account-info'>
        <p><strong>Thông tin tài khoản của bạn:</strong></p>
        <p><strong>Email đăng nhập:</strong> {email}</p>
      </div>

      <p>Để bảo mật tài khoản, vui lòng không chia sẻ thông tin đăng nhập với bất kỳ ai.</p>
      <p>Nếu bạn có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi qua email hỗ trợ hoặc hotline.</p>
      <p>Chúc bạn có những trải nghiệm du lịch tuyệt vời cùng TourMate!</p>
      <p>Trân trọng,<br />
      Đội ngũ TourMate</p>
    </section>
    <footer class='email-footer'>
      © {DateTime.Now.Year} TourMate. Bản quyền mọi quyền được bảo lưu.
    </footer>
  </div>
</body>
</html>";
        }
    }
}
