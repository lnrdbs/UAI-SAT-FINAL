using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthSample.Domain;
using AuthSample.Domain.DTO;
using AuthSample.Repository;
using AuthSample.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSample.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        UserApplication userApplication;
        public AuthController()
        {
             userApplication = new UserApplication();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> TokenAsync([FromBody]LoginDTO login)
        {
            string token;
            object response;
            
            try
            {
                // User user = _users.GetAll().Where(u => u.Username.Equals(login.Username) && u.Password.Equals(login.Password)).FirstOrDefault();
               bool ok = await userApplication.Login(login.Username, login.Password);
               
                
               if (!ok)
                {
                    throw new Exception();
                }

                User user = new User() { Id = Guid.NewGuid(), Mail = "mmaciel03@hotmail.com", Username = login.Username, Password = login.Password };

                token = TokenHandler.GenerateToken(user);
                response = new { authenticated = true, token = token, id = user.Id.ToString() };
            }
            catch (Exception E)
            {

                return Unauthorized();

            }

           

            return Ok(response);
        }

    }
}
