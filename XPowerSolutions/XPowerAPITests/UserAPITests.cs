using H4_TrashPlusPlus.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using XPowerClassLibrary.Users.Entities;
using XPowerClassLibrary.Users.Models;

namespace XPowerAPITests
{
    public class MessageObject
    {
        public string Message { get; set; }
    }
    public class UserAPITests
    {
        private string apiURL = "https://localhost:44391/";

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void CreateUserAPI_ValidInput_ShouldReturn200Async()
        {
            // Arrange
            string createUserEndpoint = apiURL + "user/CreateUser";
            Random rnd = new Random();
            string mail = "MailTest" + rnd.Next(0, 10000) + "@email.com";
            string username = "UsernameTest";
            string password = "PasswordTest";
            var request = new CreateUserRequest(mail, username, password);
            IUser returnUser = null;

            // Act
            returnUser = CreateUserAsync(createUserEndpoint, request).Result;

            // Assert
            Assert.IsNotNull(returnUser);
            Assert.AreEqual(returnUser.Mail, mail);
            Assert.AreEqual(returnUser.Username, username);
            Assert.AreNotEqual(returnUser.Id, 0);
        }

        [Test]
        public void CreateUserAPI_InvalidInput_ShouldReturn400Async()
        {
            // Arrange
            string createUserEndpoint = apiURL + "user/CreateUser";
            Random rnd = new Random();
            string mail = "MailTest" + rnd.Next(0, 10000) + "@email.com";
            string username = "UsernameTest";
            string password = "PasswordTest";
            var request = new CreateUserRequest(mail, username, password);
            string statusCode;

            // Act
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(createUserEndpoint, request).Result;
                response.Content.ReadAsAsync<User>();
            }
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(createUserEndpoint, request).Result;
                response.Content.ReadAsAsync<User>();
                statusCode = response.StatusCode.ToString();
            }
            
            // Assert
            Assert.AreEqual(statusCode, System.Net.HttpStatusCode.BadRequest.ToString());
        }

        [Test]
        public void AuthenticateAPI_ValidInput_ShouldReturnToken()
        {
            // Arrange
            string createUserEndpoint = apiURL + "user/Authenticate";
            var request = new AuthenticateRequest();
            string mail = "MailTest4@email.com";
            request.Username = mail;
            request.Password = "PasswordTest";
            AuthenticateResponse returnResponse = null;

            // Act
            returnResponse = Login(createUserEndpoint, request);

            // Assert
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual(returnResponse.UserObject.Mail, mail);
            Assert.AreNotEqual(returnResponse.UserObject.Id, 0);
            Assert.IsNotNull(returnResponse.JwtToken);
            Assert.IsNotNull(returnResponse.RefreshToken);
        }

        [Test]
        public void AuthenticateAPI_ValidInput_ShouldReturnErrorMessage()
        {
            // Arrange
            string createUserEndpoint = apiURL + "user/Authenticate";
            string mail = "MailTest4@email.com";
            var request = new AuthenticateRequest();
            request.Username = mail;
            request.Password = ":(";
            MessageObject message = new MessageObject();

            // Act
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(createUserEndpoint, request).Result;

                message.Message = response.Content.ReadAsAsync<MessageObject>().Result.Message;
            }

            // Assert
            Assert.AreEqual(message.Message, "An unexpected error occured. User could not be validated.");
        }

        [Test]
        public void LogoutAPI_ValidInput_ShouldRevokeToken()
        {
            // Arrange
            string createUserEndpoint = apiURL + "user/Logout";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:44391/");

            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = "MailTest4@email.com";
            loginRequest.Password = "PasswordTest";
            request.Headers.Add("Authorization", "Bearer " + Login(apiURL + "user/Authenticate", loginRequest).JwtToken);
            MessageObject message = new MessageObject();

            // Act
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(createUserEndpoint, request).Result;

                message = response.Content.ReadAsAsync<MessageObject>().Result;
            }

            // Assert
            Assert.AreNotEqual(message.Message, "Token revoked");
        }

        // Helper methods
        private async System.Threading.Tasks.Task<IUser> CreateUserAsync(string createUserEndpoint, CreateUserRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(createUserEndpoint, request);
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    return response.Content.ReadAsAsync<User>().Result;
                }
                else
                {
                    return null;
                }
            }
        }

        private AuthenticateResponse Login(string createUserEndpoint, AuthenticateRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(createUserEndpoint, request).Result;
                //IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

                return response.Content.ReadAsAsync<AuthenticateResponse>().Result;
            }
        }
    }
}