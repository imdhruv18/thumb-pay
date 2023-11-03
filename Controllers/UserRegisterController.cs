using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thubpay.Models;
using ThumbpayContext;

namespace Thubpay.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class UserRegisterController : ControllerBase
	{
		[HttpPost]
		[Route("Register")]
		public IActionResult UserRegister(UserRegisterModel parameter)
		{
			return Ok(new Core.UserRegisterCore().UserRegister(parameter));
		}

		[HttpGet]
		[Route("Login")]
		public IActionResult Login(string Email, string Password)
		{
			return Ok(new Core.UserRegisterCore().UserLogin(Email, Password));
		}		

		[HttpGet]
		[Route("UserDetails")]
		public IActionResult UserDetails(int passCode)
		{
			return Ok(new Core.UserRegisterCore().UserDetails(passCode));
		}		

		[HttpGet]
		[Route("UserDetailsWithToken")]
		public IActionResult UserDetailsWithToken()
		{
			var access_token = HttpContext.Request.Headers["Authorization"].ToString();
			return Ok(new Core.UserRegisterCore().UserDetailsWithToken(access_token));
		}	

		[HttpPost]
		[Route("AddBankDetails")]
		public IActionResult AddBankDetails(BankDetail bankDetail)
		{
			return Ok(new Core.UserRegisterCore().AddBankDetails(bankDetail));
		}
		
		[HttpPost]
		[Route("AddCardDetails")]
		public IActionResult AddCardDetails(CardDetail cardDetail)
		{
			return Ok(new Core.UserRegisterCore().AddCardDetails(cardDetail));
		}

		[HttpPost]
		[Route("RemoveBankDetails")]
		public IActionResult RemoveBankDetails(int BankId)
		{
			return Ok(new Core.UserRegisterCore().RemoveBankDetails(BankId));
		}

		[HttpPost]
		[Route("RemoveCardDetails")]
		public IActionResult RemoveCardDetails(int CardId)
		{
			return Ok(new Core.UserRegisterCore().RemoveCardDetails(CardId));
		}

		[HttpGet]
		[Route("test")]
		[Authorize(Roles = "User")]
		public IActionResult Logintest()
		{
			var access_token = HttpContext.Request.Headers["Authorization"].ToString();
			return Ok(new Core.UserRegisterCore().GetTokenDetails(access_token));
		}
	}
}
