using H4_TrashPlusPlus.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        // API
        private string apiURL = "https://localhost:44391/";

        // Endpoints
        private string createUserEndpoint = "user/CreateUser";
        private string authenticateEndpoint = "user/Authenticate";
        private string logoutEndpoint = "user/Logout";
        private string refreshTokenEndpoint = "user/refreshtoken";
        private string loginTestEndpoint = "user/TestLogin";

        // Test credentials
        private string testMail = "MailTest4@email.com";
        private string testUsername = "UsernameTest";
        private string testPassword = "PasswordTest";

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void CreateUserAPI_ValidInput_ShouldReturn200()
        {
            // Arrange
            #region Arrange
            Random rnd = new Random();
            string mail = "MailTest" + rnd.Next(0, 10000) + "@email.com";
            var request = new CreateUserRequest(mail, testUsername, testPassword);
            IUser returnUser = null;
            #endregion

            // Act
            returnUser = CreateUser(request).Result;

            // Assert
            Assert.IsNotNull(returnUser);
            Assert.AreEqual(returnUser.Mail, mail);
            Assert.AreEqual(returnUser.Username, testUsername);
            Assert.AreNotEqual(returnUser.Id, 0);
        }

        [Test]
        public void CreateUserAPI_InvalidInput_ShouldReturn400()
        {
            // Arrange
            #region Arrange
            Random rnd = new Random();
            string mail = "MailTest" + rnd.Next(0, 10000) + "@email.com";
            var request = new CreateUserRequest(mail, testUsername, testPassword);
            string statusCode;
            #endregion

            // Act
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(apiURL + createUserEndpoint, request).Result;
                response.Content.ReadAsAsync<User>();
            }
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(apiURL + createUserEndpoint, request).Result;
                response.Content.ReadAsAsync<User>();
                statusCode = response.StatusCode.ToString();
            }
            
            // Assert
            Assert.AreEqual(statusCode, HttpStatusCode.BadRequest.ToString());
        }

        [Test]
        public void AuthenticateAPI_ValidInput_ShouldReturnToken()
        {
            // Arrange
            #region Arrange
            var request = new AuthenticateRequest();
            request.Username = testMail;
            request.Password = testPassword;
            AuthenticateResponse returnResponse = null;
            #endregion

            // Act
            returnResponse = Login(request);

            // Assert
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual(returnResponse.UserObject.Mail, request.Username);
            Assert.AreNotEqual(returnResponse.UserObject.Id, 0);
            Assert.IsNotNull(returnResponse.JwtToken);
            Assert.IsNotNull(returnResponse.RefreshToken);
        }

        [Test]
        public void AuthenticateAPI_InvalidInput_ShouldReturnErrorMessage()
        {
            // Arrange
            #region Arrange
            var request = new AuthenticateRequest();
            request.Username = testMail;
            request.Password = ":(";
            MessageObject message = new MessageObject();
            #endregion

            // Act
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(apiURL + authenticateEndpoint, request).Result;

                message.Message = response.Content.ReadAsAsync<MessageObject>().Result.Message;
            }

            // Assert
            Assert.AreEqual(message.Message, "An unexpected error occured. User could not be validated.");
        }

        [Test]
        public void LogoutAPI_ValidToken_ShouldRevokeToken()
        {
            // Arrange
            #region Arrange
            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = testMail;
            loginRequest.Password = testPassword;
            AuthenticateResponse authenticateResponse = Login(loginRequest);
            string token = authenticateResponse.RefreshToken;
            MessageObject message = new MessageObject();
            #endregion

            // Act
            message.Message = Logout(authenticateResponse, token);

            // Assert
            Assert.AreEqual(message.Message, "Token revoked");
        }

        [Test]
        public void LogoutAPI_InvalidToken_ShouldReturnErrorMessage()
        {
            // Arrange
            #region Arrange
            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = testMail;
            loginRequest.Password = testPassword;
            AuthenticateResponse authenticateResponse = Login(loginRequest);
            string token = ":(";
            MessageObject message = new MessageObject();
            #endregion

            // Act
            message.Message = Logout(authenticateResponse, token);

            // Assert
            Assert.AreEqual(message.Message, "Token not found");
        }

        [Test]
        public void LogoutAPI_NoToken_ShouldReturnErrorMessage()
        {
            // Arrange
            #region Arrange
            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = testMail;
            loginRequest.Password = testPassword;
            AuthenticateResponse authenticateResponse = Login(loginRequest);
            string token = "";
            MessageObject message = new MessageObject();
            #endregion

            // Act
            message.Message = Logout(authenticateResponse, token);

            // Assert
            Assert.AreEqual(message.Message, "Token is required");
        }

        [Test]
        public void RefreshTokenAPI_ValidToken_ShouldReturnNewToken()
        {
            // Arrange
            #region Arrange
            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = testMail;
            loginRequest.Password = testPassword;
            AuthenticateResponse authenticateResponseLogin = Login(loginRequest);
            string token = authenticateResponseLogin.RefreshToken;
            AuthenticateResponse authenticateResponseRefresh;
            #endregion

            // Act
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(apiURL) })
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authenticateResponseLogin.JwtToken);
                    cookieContainer.Add(new Uri(apiURL), new Cookie("refreshToken", token));
                    var response = client.PostAsJsonAsync(refreshTokenEndpoint, token).Result;

                    authenticateResponseRefresh = response.Content.ReadAsAsync<AuthenticateResponse>().Result;
                }
            }

            // Assert
            Assert.NotNull(authenticateResponseRefresh.JwtToken);
            Assert.NotNull(authenticateResponseRefresh.RefreshToken);
            Assert.AreNotEqual(authenticateResponseRefresh.RefreshToken, authenticateResponseLogin.RefreshToken);
        }

        [Test]
        public void LoginTestAPI_LoggedIn_ShouldBeLoggedIn()
        {
            // Arrange
            #region Arrange
            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = testMail;
            loginRequest.Password = testPassword;
            AuthenticateResponse authenticateResponseLogin = Login(loginRequest);
            string token = authenticateResponseLogin.RefreshToken;
            AuthenticateResponse authenticateResponse;
            MessageObject message = new MessageObject();
            #endregion

            // Act
            message.Message = TestLogin(authenticateResponseLogin, token);

            // Assert
            Assert.AreEqual(message.Message, "User is logged in!");
        }

        [Test]
        public void LoginTestAPI_NotLoggedIn_ShouldNotBeLoggedIn()
        {
            // Arrange
            #region Arrange
            AuthenticateRequest loginRequest = new AuthenticateRequest();
            loginRequest.Username = testMail;
            loginRequest.Password = testPassword;
            AuthenticateResponse authenticateResponseLogin = Login(loginRequest);
            string token = authenticateResponseLogin.RefreshToken;
            Logout(authenticateResponseLogin, token);
            MessageObject message = new MessageObject();
            #endregion

            // Act
            message.Message = TestLogin(authenticateResponseLogin, token);

            // Assert
            Assert.AreEqual(message.Message, "User is not logged in.");
        }

        // Helper methods
        private async System.Threading.Tasks.Task<IUser> CreateUser(CreateUserRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(apiURL + createUserEndpoint, request);
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

        private AuthenticateResponse Login(AuthenticateRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(apiURL + authenticateEndpoint, request).Result;
                IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

                return response.Content.ReadAsAsync<AuthenticateResponse>().Result;
            }
        }

        private string Logout(AuthenticateResponse authenticateResponse, string token)
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(apiURL) })
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authenticateResponse.JwtToken);

                    cookieContainer.Add(new Uri(apiURL), new Cookie("refreshToken", token));
                    var response = client.PostAsJsonAsync(apiURL + logoutEndpoint, token).Result;

                    return response.Content.ReadAsAsync<MessageObject>().Result.Message;
                }
            }
        }

        private string TestLogin(AuthenticateResponse authenticateResponse, string token)
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = new Uri(apiURL) })
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authenticateResponse.JwtToken);
                    cookieContainer.Add(new Uri(apiURL), new Cookie("refreshToken", token));
                    var response = client.GetAsync(apiURL + loginTestEndpoint).Result;

                    return response.Content.ReadAsAsync<MessageObject>().Result.Message;
                }
            }
        }
    }
}