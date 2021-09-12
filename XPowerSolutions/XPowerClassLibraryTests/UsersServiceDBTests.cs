using NUnit.Framework;
using System;
using XPowerClassLibrary.Users;
using XPowerClassLibrary.Users.Models;
using System.Data;
using System.Threading.Tasks;

namespace XPowerClassLibraryTests.Tests
{
    public class UserServiceDBTests
    {
        private IUserService userServiceDb;

        [SetUp]
        public void Setup()
        {
            userServiceDb = UserServiceFactory.GetUserServiceDB();
        }

        [Test]
        public void CreateUserAsync_NullInput_ShouldThrowAgumentNullExeption()
        {
            // Arrange
            string mail = null;
            string username = null;
            string password = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => { await this.userServiceDb.CreateUserAsync(mail, username, password); });
        }

        [TestCase("")]
        [TestCase("mail test")]
        [TestCase("mail")]
        [TestCase("mail.com")]
        [TestCase(",.--12/()(/(&%&%(/,3-213123.com")]
        [TestCase("105 OR 1=1")]
        [TestCase("hello123rf@com'")]
        public void CreateUserAsync_InvalidMailInput_ShouldThrowArgumenExeption(string mailInput)
        {
            // Arrange
            string mail = mailInput;
            string password = "12341231414143qwemkqwnelkqwe";
            string username = "Testusername";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await this.userServiceDb.CreateUserAsync(mail, username, password); });
        }

        [TestCase("")]
        [TestCase("mail test")]
        [TestCase(",.--12/()(/(&%&%(/,3-213123.com")]
        [TestCase("105 OR 1=1")]
        public void CreateUserAsync_InvalidUsernameInput_ShouldThrowAgumenExeption(string invalidInput)
        {

            // Arrange
            string mail = "test1234@mail.com";
            string password = "12341231414143qwemkqwnelkqwe";
            string username = invalidInput;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await this.userServiceDb.CreateUserAsync(mail, username, password); });
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("password")]
        [TestCase("Password")]
        [TestCase("1234")]
        public void CreateUserAsync_InvalidPasswordInput_ShouldThrowAgumenExeption(string passwordInput)
        {
            // Arrange
            string mail = "test@mail.com";
            string password = passwordInput;
            string username = "testUsername";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await this.userServiceDb.CreateUserAsync(mail, username, password); });
        }

        [Test]
        public async Task CreateUserAsyncDB_ValidInput_ShouldReturnUser()
        {
            // Arrange
            IUser createdUser;
            long randomNumber = new Random().Next(10000, 20000);

            // Act
            createdUser = await this.userServiceDb.CreateUserAsync("testmail" + randomNumber + "@mail.com", "testUsername", "12341234!weqwe");

            // Assert
            Assert.IsNotNull(createdUser);
            Assert.IsNotNull(createdUser.Mail);
            Assert.IsNotEmpty(createdUser.Mail);
        }

        [Test]
        public void CreateUserAsyncDB_InvalidInputEmailAlreadyExists_ShouldThrowArgumentError()
        {
            // Act & Assert
            Assert.ThrowsAsync<DuplicateNameException>(async () => { await this.userServiceDb.CreateUserAsync("MailTest@email.com", "testUsername", "12341234!weqwe"); });

        }

        [Test]
        public async Task GetUserByIdDB_ValidId_ShouldReturnIUserObject()
        {
            // Arrange
            IUser createdUser;
            IUser requestedUser;
            long randomNumber = new Random().Next(10000, 20000);
            string email = "testmail" + randomNumber + "@mail.com";

            createdUser = await this.userServiceDb.CreateUserAsync(email, "testUsername", "12341234!weqwe");

            // Act
            requestedUser = await this.userServiceDb.GetUserByIdAsync(createdUser.Id);

            // Assert
            Assert.IsNotNull(requestedUser);
            Assert.IsNotNull(requestedUser.Id);
            Assert.AreNotEqual(0, requestedUser.Id);
            Assert.AreEqual(email, requestedUser.Mail);
        }

        [Test]
        public async Task GetUserByIdDB_NonExistingId_ShouldReturnNull()
        {
            // Arrange
            IUser requestedUser;

            // Act
            requestedUser = await this.userServiceDb.GetUserByIdAsync(int.MaxValue);

            // Assert
            Assert.IsNull(requestedUser);
        }

        [Test]
        public async Task GetUserByLoginNameDB_ValidId_ShouldReturnIUserObject()
        {
            // Arrange
            IUser createdUser;
            IUser requestedUser;
            long randomNumber = new Random().Next(10000, 20000);
            string email = "testmail" + randomNumber + "@mail.com";

            createdUser = await this.userServiceDb.CreateUserAsync(email, "testUsername", "12341234!weqwe");

            // Act
            requestedUser = await this.userServiceDb.GetUserByLoginNameAsync(email);

            // Assert
            Assert.IsNotNull(requestedUser);
            Assert.IsNotNull(requestedUser.Id);
            Assert.AreNotEqual(0, requestedUser.Id);
            Assert.AreEqual(email, requestedUser.Mail);
            Assert.AreEqual(createdUser.Id, requestedUser.Id);
        }

        [TestCase("")]
        [TestCase("mail test")]
        [TestCase("mail")]
        [TestCase("mail.com")]
        [TestCase(",.--12/()(/(&%&%(/,3-213123.com")]
        [TestCase("105 OR 1=1")]
        [TestCase("hello123rf@com'")]
        public void GetUserByLoginNameDB_IllegalValue_ShouldThrowArgumentException(string loginNameVal)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await this.userServiceDb.GetUserByLoginNameAsync(loginNameVal); });
        }

        [Test]
        public async Task GetUserByLoginNameDB_NonExistingLoginName_ShouldReturnNull()
        {
            // Arrange
            IUser requestedUser;
            long randomNumber = int.MaxValue;
            string email = "testmail" + randomNumber + "@mail.com";

            // Act
            requestedUser = await this.userServiceDb.GetUserByLoginNameAsync(email);

            // Assert
            Assert.IsNull(requestedUser);
        }

        [Test]
        public async Task Authenticate_ValidLogin_ShouldReturnAuthenticateResponseObject()
        {
            // Arrange
            AuthenticateResponse authenticateResponse;
            long randomNumber = new Random().Next(10000, 20000);
            string email = "testmail" + randomNumber + "@mail.com";
            string password = "testpassword" + randomNumber;

            await this.userServiceDb.CreateUserAsync(email, "testUsername", password);

            // Act
            authenticateResponse = await this.userServiceDb.AuthenticateAsync(email, password, "0.0.0.0");

            // Assert
            Assert.IsNotNull(authenticateResponse);
            Assert.IsNotNull(authenticateResponse.UserObject);
            Assert.IsNotNull(authenticateResponse.JwtToken);
            Assert.IsNotNull(authenticateResponse.RefreshToken);
        }

        [Test]
        public async Task Authenticate_InvalidLogin_ShouldReturnNull()
        {
            // Arrange
            AuthenticateResponse authenticateResponse;
            long randomNumber = new Random().Next(10000, 20000);
            string email = "testmail" + randomNumber + "@mail.com";
            string password = "testpassword" + randomNumber;

            await this.userServiceDb.CreateUserAsync(email, "testUsername", password);

            // Act
            authenticateResponse = await this.userServiceDb.AuthenticateAsync(email, password+"1", "0.0.0.0");

            // Assert
            Assert.IsNull(authenticateResponse);
        }

        [Test]
        public async Task Logout_Valid_ShouldReturnTrue()
        {
            // Arrange
            AuthenticateResponse authenticateResponse;
            long randomNumber = new Random().Next(10000, 20000);
            string email = "testmail" + randomNumber + "@mail.com";
            string password = "testpassword" + randomNumber;

            await this .userServiceDb.CreateUserAsync(email, "testUsername", password);
            authenticateResponse = await this.userServiceDb.AuthenticateAsync(email, password, "0.0.0.0");

            // Act
            bool logoutSuccess = await this.userServiceDb.LogoutAsync(authenticateResponse.RefreshToken, "0.0.0.0");

            // Assert
            Assert.IsNotNull(authenticateResponse);
            Assert.IsNotNull(authenticateResponse.UserObject);
            Assert.IsNotNull(authenticateResponse.JwtToken);
            Assert.IsNotNull(authenticateResponse.RefreshToken);
            Assert.IsTrue(logoutSuccess);
        }

        [TestCase("")]
        [TestCase("Token with space")]
        public void Logout_InvalidToken_ShouldThrowArgumentException(string token)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await this.userServiceDb.LogoutAsync(token, "0.0.0.0"); });

        }

        [TestCase(null)]
        public void Logout_InvalidToken_ShouldThrowArgumentNullException(string token)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => { await this.userServiceDb.LogoutAsync(token, "0.0.0.0"); });
        }

    }
}