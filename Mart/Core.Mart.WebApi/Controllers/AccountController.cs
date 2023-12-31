﻿using Azure;
using Core.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        private CartDbcoreContext db = new CartDbcoreContext();

        public AccountController( UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),        //new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                // if this ASPNETUSER has a customer in table 
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //var UserObj = db.AspNetUsers.FirstOrDefault(a => a.UserName == userId);

                //if (UserObj != null && UserObj.Email != null)
                //{
                //    var CustomerObj = db.Customers.FirstOrDefault(a => a.AspNetUserId == UserObj.Id);
                //    if (CustomerObj != null && CustomerObj.Email != null)
                //    {
                //        return Ok(new
                //        {
                //            token = new JwtSecurityTokenHandler().WriteToken(token),
                //            expiration = token.ValidTo,
                //            loggedInAs = userRoles == null ? "0" : userRoles[0],
                //            AsCustomerRegister = true,
                //            AsShopRegister = false
                //        });
                //    }

                //    var ShopObj = db.AspNetUsers.FirstOrDefault(a => a.UserName == userId);

                //    if (ShopObj != null && ShopObj.Email != null)
                //    {
                //        return Ok(new
                //        {
                //            token = new JwtSecurityTokenHandler().WriteToken(token),
                //            expiration = token.ValidTo,
                //            loggedInAs = userRoles == null ? "0" : userRoles[0],
                //            AsCustomerRegister = false,
                //            AsShopRegister = true
                //        });
                //    }
                //}
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    loggedInAs = userRoles == null ? "0" : userRoles[0]
                });
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterBindingModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });


                if (model.isShopkeeper == false)
                {
                    if (!await _roleManager.RoleExistsAsync(UserRoles.IsACustomer))  // Agar ye role nhi hai database me to create kr dega
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.IsACustomer));

                    if (await _roleManager.RoleExistsAsync(UserRoles.IsACustomer))
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.IsACustomer);
                    }
                }
                else
                {
                    if (!await _roleManager.RoleExistsAsync(UserRoles.IsAShop))  // Agar ye role nhi hai database me to create kr dega
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.IsAShop));

                    if (await _roleManager.RoleExistsAsync(UserRoles.IsAShop))
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.IsAShop);
                    }
                }

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterBindingModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            //if (!await _roleManager.RoleExistsAsync(UserRoles.IsAShop))
            //    await _roleManager.CreateAsync(new IdentityRole(UserRoles.IsAShop));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
