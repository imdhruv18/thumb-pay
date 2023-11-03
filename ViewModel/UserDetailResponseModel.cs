using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Thubpay.Models;
using ThumbpayContext;

namespace Thubpay.ViewModel
{
	public class UserDetailResponseModel
	{
		//public int UserId { get; set; } 
		public int UserTypeId { get; set; }
		public string Fname { get; set; } = null!;
		public string Lname { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string Email { get; set; } = null!;
		public List<BankDetailResponseModel> bankDetails { get; set; }
		public List<CardDetailResponseModel> cardDetails { get; set; }
	}

	public class BankDetailResponseModel
	{
		//public int BankId { get; set; } 
		//public int UserId { get; set; }
		//public string AccountHolderName { get; set; } = null!;
		public string BankName { get; set; } = null!;
		public string AccountNumber { get; set; } = null!;
		//public string IfscCode { get; set; } = null!;
		public int? TotalBalance { get; set; }
	}

	public class CardDetailResponseModel
	{
		//public int CardId { get; set; }
		//public int UserId { get; set; }
		public string? CardNumber { get; set; }
		public string? Exdate { get; set; }
		//public string? Cvv { get; set; }
		public string? HolderName { get; set; }
	}

	public class ThumbPayDetailsResponseModel
	{
		public UserDetail userDetail { get; set; }
		public List<BankDetail> bankDetails { get; set; }
		public List<CardDetail> cardDetails { get; set; }
	}

	//public class FingerDetailsResponceModel
	//{
	//	public FingerDetail? FingerId { get; set; }
	//	public FingerDetail? UserId { get;}
	//	public FingerDetail FingerData { get; set; }

	//}
}
