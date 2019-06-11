using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Controllers.DTOs;
using API.Core;
using API.Extensions;
using API.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Primitives;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route ("/api/voting/user")]
    public class UserController : Controller
    {
        public IUserRepository _repository { get; }
        public IMapper _mapper { get; }
        public IUnitOfWork _unitOfWork { get; }
        public AppSettings _appSettings { get; }
        public IConfiguration _configuration { get; }
        public UserController (IUserRepository repositpry, IMapper mapper, IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings, IConfiguration configuration) {
            _configuration = configuration;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = repositpry;
        }
        
        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public IActionResult Authenticate ([FromBody] UserDTO userResource) {
            var user = _repository.Authenticate (userResource.UserEmail, userResource.Password);
            if (user == null)
                return Unauthorized ();
            var tokenString = _repository.CreateToken(user);
            return Ok (new {
                    Id = user.UserId,
                    Name = user.UserName,
                    Email = user.UserEmail,
                    Token = tokenString,
                    Roles = user.UserRole.ToString (),
            });
        }
        // [HttpPost("sendResult")]
        // public async Task<IActionResult> SendElectionResult([FromBody] object result) {

        // }
        [AllowAnonymous]
        [HttpPost("contactUs")]
        public async Task<IActionResult> ContactUs([FromBody] Message message) {
            var apiKey = _configuration.GetSection("SkineroMotorsSendGridApiKey").Value;
            var sendGridclient = new SendGridClient (apiKey);
            var from = new EmailAddress (message.fromEmail, message.fromName);
            var subject = "Voting Block";
            var to = new EmailAddress ("adesokantayyib@gmail.com", "Adesokan Toyeeb");
            var plainTextContent = $"something";
            var htmlContent = $"<div style='background-color: #00FF7F; text-align: center;color:rgb(35, 41, 41); margin: 0 auto; padding: 60px; width: 75%'><div style='background-color: #FFEFD5; height: 60% !important;'><h2 style='text-align: center; padding-top: 15px;'><strong>Welcome to Voting Dapp</strong></h2><h3>Hello VotingBlock,</h3><h4>You have a message from {message.fromName}, {message.fromEmail}</h4><h4>Message body: {message.fromMessage}<h4></div></div>";
            var msg = MailHelper.CreateSingleEmail (from, to, subject, plainTextContent, htmlContent);
            var response = await sendGridclient.SendEmailAsync (msg);

            return Ok ();
        }

        [AllowAnonymous]
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword ([FromBody] UserDTO userDTO) {
            // if (!await _repository.ForgotPassword (email.ToString()))
            //     return BadRequest ("User does not exist");
            StringValues originValues;
            Request.Headers.TryGetValue("Referer", out originValues);
            StringValues origin;
            Request.Headers.TryGetValue("Origin", out origin);
            var user = _mapper.Map<User>(userDTO);
            var token = _repository.CreateToken (user);
            // var x  = _configuration.
            var apiKey = _configuration.GetSection("SkineroMotorsSendGridApiKey").Value;
            var sendGridclient = new SendGridClient (apiKey);
            var from = new EmailAddress ("info@votingblock.com", "VotingBlock");
            var subject = "Voting DAPP Password Reset";
            var to = new EmailAddress (user.UserEmail, user.UserName);
            var plainTextContent1 = $"<strong>{originValues.ToString()}?token={token}</strong>";
            var htmlContent = $"<div style='background-color: #00FF7F; text-align: center;color:rgb(35, 41, 41); margin: 0 auto; padding: 60px; width: 75%'><div style='background-color: #FFEFD5; height: 60% !important;'><h2 style='text-align: center; padding-top: 15px;'><strong>Welcome to Voting Dapp</strong></h2><h4>Hello {user.UserName},</h4><p>A transaction was initiated to change your password, Click the button below to reset your password</p> <br> <a class='resetBtn' href='{origin.ToString()}/resetpassword?token={token}' style='background-color: #00FF7F; color: #0c0b0b; padding: 5px; border-radius: 3px;'>Reset Password</a><br><p>&nbsp;<p></div></div>";
            var msg = MailHelper.CreateSingleEmail (from, to, subject, plainTextContent1, htmlContent);
            var response = await sendGridclient.SendEmailAsync (msg);

            return Ok ();
        }

        [AllowAnonymous]
        [HttpPost ("register")]
        public async Task<IActionResult> Register ([FromBody] UserDTO userDTO) {
            // userDTO.LastPasswordChange = DateTime.Now;
            var user = _mapper.Map<User> (userDTO);
            if (await _repository.UserExists (user))
                return BadRequest ("User already exists");
            try {
                if((user.UserEmail == "adesokantayyib@gmail.com") || (user.UserEmail == "musaaliuoluwatoyin@gmail.com")) {
                    user.UserRole = Role.Admin;
                } else {
                    user.UserRole = Role.Client;
                }
                await _repository.CreateUser (user, userDTO.Password);
                return Ok ($"User with email {user.UserEmail} Created");
            } catch (Exception ex) {
                return BadRequest (ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("sendResults")]
        public async Task<IActionResult> SendResult ([FromBody] Result[] results) {
            var apiKey = _configuration.GetSection("SkineroMotorsSendGridApiKey").Value;
            var sendGridclient = new SendGridClient (apiKey);
            var from = new EmailAddress ("results@votingblock.com", "VotingBlock");
            var subject = "Election Result";
            var to = new EmailAddress (results[0].CandidateName, "");
            var plainTextContent1 = $"";
            var resultList = new List<String>();
            for (int i = 1; i < results.Length; i++)
            {
                resultList.Add($"<tr style='padding: 2rem;'><td style='margin: 2rem;'>{results[i].CandidateName}</td><td style='margin: 2rem;'>{results[i].VoteCount}</td></tr><br>");
            }
            var resArray = "";
            for (int i = 0; i < resultList.Count; i++)
            {
                resArray += resultList[i];
            }
            var res = resArray[0].ToString();
            var htmlContent = $"<div style='background-color: #00FF7F; text-align: center;color:rgb(35, 41, 41); margin: 0 auto; padding: 60px; width: 75%'><div style='background-color: #FFEFD5; height: 60% !important; padding: 2rem'><h2>Election Results</h2>{resArray}</div></div>";
            var msg = MailHelper.CreateSingleEmail (from, to, subject, plainTextContent1, htmlContent);
            var response = await sendGridclient.SendEmailAsync (msg);

            return Ok ();

        }

        [HttpGet]
        public async Task<IActionResult> GetAll () {
            var users = await _repository.GetUsers ();
            var userDTO = _mapper.Map<IEnumerable<UserDTO>> (users);
            return Ok (userDTO);
        }

        [HttpGet ("{id}")]
        public async Task<IActionResult> GetUser (int id) {
            var user = await _repository.GetUser (id);
            var userDTO = _mapper.Map<UserDTO> (user);
            return Ok (userDTO);
        }
        [HttpGet ("{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser (string email) {
            var user = await _repository.GetUser (email);
            if (user == null)
                return NotFound("Specified user doesn't exist");
            var userDTO = _mapper.Map<UserDTO> (user);
            return Ok (userDTO);
        }

        [HttpPut ("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser (int id, [FromBody] UserDTO userDTO) {
            if (userDTO == null)
                return BadRequest("User cannot be null");
            var userToUpdate = await _repository.GetUser(id);
            if (userToUpdate == null)
                return NotFound("User does not exist");
            
            var lastTime = DateTime.Now - userToUpdate.LastPasswordChange;
            double lastUpdate = lastTime.Ticks;
            
            if (lastUpdate > 120) {
                userToUpdate.NoOfUpdates = 0;
            }

            if (lastUpdate > 120 && userToUpdate.NoOfUpdates < 4) {
             _mapper.Map<UserDTO, User> (userDTO, userToUpdate);
             _repository.UpdateUser(userDTO.Password, userToUpdate);
             userToUpdate.NoOfUpdates += 1;
             await _unitOfWork.CompleteAsync();
            } else {
                return BadRequest("Too many attempt to update password, retry in 2 hours");
            }
             return Ok("User update was succesfull");
        }

        [HttpDelete ("{id}")]
        public async Task<IActionResult> Delete (int id) {
            var user = await _repository.GetUser (id);
            _repository.DeleteUser (user);
            return Ok (id);
        }
    }
}