using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using AndreiWebAPI.Models;

namespace AndreiWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class UserController : ControllerBase
    {

        [HttpPost]
        public IActionResult SignIn(string name, string password)
        {
            if (name == null || password == null)
                return StatusCode(StatusCodes.Status400BadRequest, "wrong login data");

            name = name.Trim();
            password = password.Trim();

            var user = Services.UserService.GetUsers().Where(_ => _.Name == name).FirstOrDefault();

            if (user == null)
                return StatusCode(StatusCodes.Status404NotFound, "wrong user");

            if (user.Password != password)
                return StatusCode(StatusCodes.Status400BadRequest, "wrong password");

            return StatusCode(StatusCodes.Status200OK, JWT.Helpers.GetToken(user));
        }


        [HttpPost]
        public IActionResult SignUp(string name, string mail, string password)
        {
            if (name == null || mail == null || password == null)
                return StatusCode(StatusCodes.Status400BadRequest, "wrong data user");

            if (Services.UserService.UserNameExists(name))
                return StatusCode(StatusCodes.Status409Conflict, "user name already exits");

            if (Services.UserService.UserMailExists(mail))
                return StatusCode(StatusCodes.Status423Locked, "email already exits");

            name = name.Trim();
            mail = mail.Trim().ToLower();
            password = password.Trim();

            User? user = Services.UserService.InsertUser(name, mail, password);

            if (user != null)
            {
                Email.SendNewUserMail(user);
                return StatusCode(StatusCodes.Status201Created, JWT.Helpers.GetToken(user));
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest, "an error has occurred");
        }


        [EnableCors("CorsPolicy")]
        [HttpGet]
        public IActionResult RecoverAccount(string mail)
        {
            User? user = Services.UserService.GetUser(mail.Trim().ToLower());

            if (user == null)
                return StatusCode(StatusCodes.Status423Locked, "email not exits");
            else {
                Email.SendRecoverAccountMail(user);
                return StatusCode(StatusCodes.Status200OK, "email to recover was sent");
            }
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAll()
        {
            return StatusCode(StatusCodes.Status200OK, Services.UserService.GetUsers());
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Delete(int id)
        {
            if (id == 1)
                return StatusCode(StatusCodes.Status400BadRequest, "not allowed");

            if (Services.UserService.DeleteUser(id))
                return StatusCode(StatusCodes.Status204NoContent, "user deleted");
            else
                return StatusCode(StatusCodes.Status400BadRequest, "user not deleted");
        }

    }
}
