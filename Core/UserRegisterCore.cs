using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using Thubpay.Models;
using Thubpay.ViewModel;
using ThumbpayContext;

namespace Thubpay.Core
{
	public class UserRegisterCore
	{
		public string UserRegister(UserRegisterModel parameter)
		{
			using (var Scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (ThumbpayDataContext c = new ThumbpayDataContext())
				{
					try
					{
						var a = c.UserDetails.FirstOrDefault(z => z.Email == parameter.Email);
						if (a == null)
						{
							var BankaccountCheck = c.BankDetails.FirstOrDefault(z => z.AccountNumber == parameter.AccountNumber);
							if (BankaccountCheck == null)
							{
								if (parameter.CardNumber.Length != 0)
								{
									var Carddetailscheck = c.CardDetails.FirstOrDefault(z => z.CardNumber == parameter.CardNumber);
									if (Carddetailscheck == null)
									{
										var user = new ThumbpayContext.UserDetail
										{
											UserTypeId = 3,
											Fname = parameter.Fname,
											Lname = parameter.Lname,
											Email = parameter.Email,
											PhoneNumber = parameter.PhoneNumber,
											Password = parameter.Password,
										};
										c.UserDetails.InsertOnSubmit(user);
										c.SubmitChanges();
										var UserId = user.UserId;
										var Bankdetails = new ThumbpayContext.BankDetail
										{
											UserId = UserId,
											BankName = parameter.BankName,
											AccountHolderName = parameter.AccountHolderName,
											AccountNumber = parameter.AccountNumber,
											IFSCCode = parameter.IFSCcode,
											TotalBalance = 20000,
										};
										c.BankDetails.InsertOnSubmit(Bankdetails);
										c.SubmitChanges();
										var Carddetails = new ThumbpayContext.CardDetail
										{
											UserId = UserId,
											HolderName = parameter.HolderName,
											CardNumber = parameter.CardNumber,
											EXdate = parameter.Expire,
											Cvv = parameter.Cvv,
											TotalBalance = 20000,
										};
										c.CardDetails.InsertOnSubmit(Carddetails);
										c.SubmitChanges();

										Random rnd = new Random();
										int rnum = rnd.Next(0000, 9999);
										try
										{
											var Fingerdetails = new ThumbpayContext.FingerDetail
											{
												UserId = UserId,
												FingerData = rnum.ToString(),
											};
											c.FingerDetails.InsertOnSubmit(Fingerdetails);
											c.SubmitChanges();
										}
										catch (Exception ex)
										{
										}

										Scope.Complete();
										return "User create sucessfully";
									}
									else
									{
										return "card already";
									}
								}
								else
								{
									var user = new ThumbpayContext.UserDetail
									{
										UserTypeId = 3,
										Fname = parameter.Fname,
										Lname = parameter.Lname,
										Email = parameter.Email,
										PhoneNumber = parameter.PhoneNumber,
										Password = parameter.Password,
									};
									c.UserDetails.InsertOnSubmit(user);
									c.SubmitChanges();
									var UserId = user.UserId;
									var Bankdetails = new ThumbpayContext.BankDetail
									{
										UserId = UserId,
										BankName = parameter.BankName,
										AccountHolderName = parameter.AccountHolderName,
										AccountNumber = parameter.AccountNumber,
										IFSCCode = parameter.IFSCcode,
										TotalBalance = 20000,
									};
									c.BankDetails.InsertOnSubmit(Bankdetails);
									c.SubmitChanges();
									Scope.Complete();
									return "User Login sucessfully";
								}
							}
							else
							{
								Scope.Dispose();
								return "bank already";
							}


						}
						else
						{
							Scope.Dispose();
							return "email already";
						}

					}
					catch (Exception ex)
					{
						Scope.Dispose();
						throw new Exception("error in user register", ex);
					}
				}
			}
		}

		public CommonResponseModel UserLogin(string Email, string password)
		{
			try
			{
				using (ThumbpayDataContext c = new ThumbpayDataContext())
				{
					CommonResponseModel commonResponseModel = new CommonResponseModel();
					commonResponseModel.success = false;
					var user = c.UserDetails.FirstOrDefault(z => z.Email == Email && z.Password == password);
					if (user != null)
					{
						var usertypecheck = c.UserTypes.FirstOrDefault(z => z.UserTypeId == user.UserTypeId);
						if (usertypecheck != null)
						{
							commonResponseModel.success = true;
							commonResponseModel.message = "User login successfully!";
							commonResponseModel.data = UserGenerateToken(user.UserId, usertypecheck.UserTypeName); ;
						}
						else
						{
							commonResponseModel.message = "Invalid credential";
						}
					}
					else
					{
						commonResponseModel.message = "Invalid credential";
					}
					return commonResponseModel;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("error in userlogin", ex);
			}
		}

		public CommonResponseModel UserDetails(int passCode)
		{
			try
			{
				var userId = GetUserIdWithPassCode(passCode);
				CommonResponseModel commonResponseModel = new CommonResponseModel();
				if (userId != null && userId != 0)
				{
					using (ThumbpayDataContext context = new ThumbpayDataContext())
					{
						commonResponseModel.success = true;
						commonResponseModel.message = "User details has been get succesfully!";
						UserDetailResponseModel userDetailResponseModel = (from usr in context.UserDetails
																		   where usr.UserId == userId
																		   select new UserDetailResponseModel()
																		   {
																			   //userid = usr.userid,
																			   Fname = usr.Fname,
																			   Lname = usr.Lname,
																			   Email = usr.Email,
																			   PhoneNumber = usr.PhoneNumber,
																			   UserTypeId = usr.UserTypeId,
																			   bankDetails = (from bnk in context.BankDetails
																							  where bnk.UserId == userId
																							  select new BankDetailResponseModel()
																							  {
																								  //BankId = bnk.BankId,
																								  //UserId = bnk.UserId,
																								  //AccountHolderName = bnk.AccountHolderName,
																								  BankName = bnk.BankName,
																								  AccountNumber = bnk.AccountNumber,
																								  //IfscCode = bnk.IFSCCode, 
																								  TotalBalance = bnk.TotalBalance
																							  }).ToList(),
																			   cardDetails = (from crd in context.CardDetails
																							  where crd.UserId == userId
																							  select new CardDetailResponseModel()
																							  {
																								  //CardId = crd.CardId,
																								  //UserId = crd.UserId,
																								  CardNumber = crd.CardNumber,
																								  Exdate = crd.EXdate,
																								  HolderName = crd.HolderName,
																								  //Cvv = crd.Cvv
																							  }).ToList()
																		   }).FirstOrDefault();
						commonResponseModel.data = userDetailResponseModel;
						return commonResponseModel;
					}
				}
				else
				{
					commonResponseModel.success = false;
					commonResponseModel.message = "No user found for pass code!";
					return commonResponseModel;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("error in UserDetails", ex);
			}
		}

		public int GetUserIdWithPassCode(int passCode)
		{
			using (ThumbpayDataContext context = new ThumbpayDataContext())
			{
				var fingerDetail = context.FingerDetails.FirstOrDefault(f => f.FingerData == passCode.ToString());
				if (fingerDetail != null)
				{
					return fingerDetail.UserId;
				}
				else
				{
					return 0;
				}
			}
		}

		public CommonResponseModel UserDetailsWithToken(string access_token)
		{
			try
			{
				var tokenDetails = GetTokenDetails(access_token);
				using (ThumbpayDataContext context = new ThumbpayDataContext())
				{
					CommonResponseModel commonResponseModel = new CommonResponseModel();
					commonResponseModel.success = true;
					commonResponseModel.message = "User details has been get succesfully!";
					UserDetailResponseModel userDetailResponseModel = (from usr in context.UserDetails
																	   where usr.UserId == tokenDetails.UserId
																	   select new UserDetailResponseModel()
																	   {
																		   //userid = usr.userid,
																		   Fname = usr.Fname,
																		   Lname = usr.Lname,
																		   Email = usr.Email,
																		   PhoneNumber = usr.PhoneNumber,
																		   UserTypeId = usr.UserTypeId,
																		   bankDetails = (from bnk in context.BankDetails
																						  where bnk.UserId == tokenDetails.UserId
																						  select new BankDetailResponseModel()
																						  {
																							  //BankId = bnk.BankId,
																							  //UserId = bnk.UserId,
																							  //AccountHolderName = bnk.AccountHolderName,
																							  BankName = bnk.BankName,
																							  AccountNumber = bnk.AccountNumber,
																							  //IfscCode = bnk.IFSCCode, 
																							  TotalBalance = bnk.TotalBalance
																						  }).ToList(),
																		   cardDetails = (from crd in context.CardDetails
																						  where crd.UserId == tokenDetails.UserId
																						  select new CardDetailResponseModel()
																						  {
																							  //CardId = crd.CardId,
																							  //UserId = crd.UserId,
																							  CardNumber = crd.CardNumber,
																							  Exdate = crd.EXdate,
																							  HolderName = crd.HolderName,
																							  //Cvv = crd.Cvv
																						  }).ToList()
																	   }).FirstOrDefault();
					commonResponseModel.data = userDetailResponseModel;
					return commonResponseModel;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("error in UserDetails", ex);
			}
		}

		public CommonResponseModel AddBankDetails(BankDetail bankDetail)
		{
			try
			{
				CommonResponseModel commonResponseModel = new CommonResponseModel();
				if (bankDetail != null)
				{
					using (ThumbpayDataContext context = new ThumbpayDataContext())
					{
						context.BankDetails.InsertOnSubmit(bankDetail);
						context.SubmitChanges();
						commonResponseModel.success = true;
						commonResponseModel.message = "Bank details has been inserted succesfully!";
					}
				}
				else
				{
					commonResponseModel.success = false;
					commonResponseModel.message = "Bank details can not be null!";
				}
				return commonResponseModel;
			}
			catch (Exception ex)
			{
				throw new Exception("error in add bank details", ex);
			}
		}

		public CommonResponseModel AddCardDetails(CardDetail cardDetail)
		{
			try
			{
				CommonResponseModel commonResponseModel = new CommonResponseModel();
				if (cardDetail != null)
				{
					using (ThumbpayDataContext context = new ThumbpayDataContext())
					{
						context.CardDetails.InsertOnSubmit(cardDetail);
						context.SubmitChanges();
						commonResponseModel.success = true;
						commonResponseModel.message = "Card details has been inserted succesfully!";
					}
				}
				else
				{
					commonResponseModel.success = false;
					commonResponseModel.message = "Card details can not be null!";
				}
				return commonResponseModel;
			}
			catch (Exception ex)
			{
				throw new Exception("error in add card details", ex);
			}
		}

		public CommonResponseModel RemoveBankDetails(int BankId)
		{
			try
			{
				CommonResponseModel commonResponseModel = new CommonResponseModel();
				if (BankId != null && BankId != 0)
				{
					using (ThumbpayDataContext context = new ThumbpayDataContext())
					{
						BankDetail bankDetail = context.BankDetails.FirstOrDefault(x => x.BankId == BankId);
						if (bankDetail != null)
						{
							context.BankDetails.DeleteOnSubmit(bankDetail);
							context.SubmitChanges();
							commonResponseModel.success = true;
							commonResponseModel.message = "Bank details has been deleted succesfully!";
						}
						else
						{
							commonResponseModel.success = false;
							commonResponseModel.message = "Error while delete Bank details!";
						}
					}
				}
				else
				{
					commonResponseModel.success = false;
					commonResponseModel.message = "Bank details not available!";
				}
				return commonResponseModel;
			}
			catch (Exception ex)
			{
				throw new Exception("error in remove Bank details", ex);
			}
		}
		public CommonResponseModel RemoveCardDetails(int CardId)
		{
			try
			{
				CommonResponseModel commonResponseModel = new CommonResponseModel();
				if (CardId != null && CardId != 0)
				{
					using (ThumbpayDataContext context = new ThumbpayDataContext())
					{
						CardDetail cardDetail = context.CardDetails.FirstOrDefault(x => x.CardId == CardId);
						if (cardDetail != null)
						{
							context.CardDetails.DeleteOnSubmit(cardDetail);
							context.SubmitChanges();
							commonResponseModel.success = true;
							commonResponseModel.message = "Card details has been deleted succesfully!";
						}
						else
						{
							commonResponseModel.success = false;
							commonResponseModel.message = "Error while delete card details!";
						}
					}
				}
				else
				{
					commonResponseModel.success = false;
					commonResponseModel.message = "Card details not available!";
				}
				return commonResponseModel;
			}
			catch (Exception ex)
			{
				throw new Exception("error in remove card details", ex);
			}
		}

		private string UserGenerateToken(int id, string UserTypeName)
		{
			Claim[] claims = new Claim[]
			{
				new Claim("Userid", id.ToString()),
				new Claim(ClaimTypes.Role, UserTypeName),
			};
			SymmetricSecurityKey tokenkey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes("don't try to crack me"));
			SigningCredentials credentials = new SigningCredentials(tokenkey, SecurityAlgorithms.HmacSha256);
			SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(claims),
				SigningCredentials = credentials,
				Expires = DateTime.UtcNow.AddDays(1),

			};
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			SecurityToken token = tokenHandler.CreateToken(descriptor);

			return tokenHandler.WriteToken(token);
		}

		public TokenResponseModel GetTokenDetails(string access_token)
		{
			TokenResponseModel tokenResponseModel = new TokenResponseModel();
			var Securitytoken = new JwtSecurityTokenHandler().ReadToken(access_token.Replace("Bearer ", "").Trim());
			var tokenstring = new JwtSecurityTokenHandler().WriteToken(Securitytoken);
			var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
			tokenResponseModel.UserId = Convert.ToInt32(token.Claims.First(c => c.Type == "Userid").Value);
			tokenResponseModel.RoleName = token.Claims.First(c => c.Type == "role").Value;
			return tokenResponseModel;
		}

	}
}
