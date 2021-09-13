using NUnit.Framework;
using System;
using XPowerClassLibrary;
using XPowerClassLibrary.Users;
using XPowerClassLibrary.Users.Repository;
using XPowerClassLibrary.Users.Models;
using System.Data;
using XPowerClassLibrary.Hashing;

namespace XPowerClassLibrary.Tests
{
    public class HashingServiceTests
    {

        [Test]
        public void CreateHashedUser_WithValidSettings_ShouldCreateIHashedUser()
        {
            // Arrange
            HashingSettings settings = new HashingSettings(HashingMethodType.SHA256);
            HashingService hashingService = new HashingService(settings);

            string username = "Test1";
            string password = "password123";
            IHashedUser hashedUser = null;

            // Act
            hashedUser = hashingService.CreateHashedUser(username, password);

            // Assert
            Assert.IsNotNull(hashedUser);
        }
        
        [Test]
        public void CreateHashedUser_WithInvalidSettings_ShouldThrowException()
        {
            // Arrange
            HashingSettings settings = new HashingSettings(HashingMethodType.SHA256);
            HashingService hashingService = new HashingService(settings);

            string username = "Test1";
            string password = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => hashingService.CreateHashedUser(username, password));
        }


        [Test]
        public void VerifyPassword_ComparingCorrectPassword_ShouldBeEqual()
        {
            // Arrange
            HashingSettings settings = new HashingSettings(HashingMethodType.SHA256);
            HashingService hashingService = new HashingService(settings);

            string username = "Test1";
            string password = "password123";
            IHashedUser hashedUser = null;
            bool passwordMatched = false;

            // Act
            hashedUser = hashingService.CreateHashedUser(username, password);

            passwordMatched = hashingService.VerifyPassword(password, hashedUser.Password, hashedUser.Salt);

            Console.WriteLine("Original Password: " + password);
            Console.WriteLine("Hashed Password: " + hashedUser.Password);
            Console.WriteLine("Hashed Salt: " + hashedUser.Salt);

            // Assert
            Assert.IsTrue(passwordMatched);
        }

        [Test]
        public void VerifyPassword_ComparingWrongPassword_ShouldNotBeEqual()
        {
            // Arrange
            HashingSettings settings = new HashingSettings(HashingMethodType.SHA256);
            HashingService hashingService = new HashingService(settings);

            string username = "Test1";
            string correctPassword = "correctPassword";
            string wrongPassword = "wrongPassword";
            IHashedUser hashedUser = null;
            bool passwordMatched = false;

            // Act
            hashedUser = hashingService.CreateHashedUser(username, correctPassword);

            passwordMatched = hashingService.VerifyPassword(wrongPassword, hashedUser.Password, hashedUser.Salt);

            Console.WriteLine("Original Correct Password: " + correctPassword);
            Console.WriteLine("Original Wrong Password: " + wrongPassword);
            Console.WriteLine("Hashed Password: " + hashedUser.Password);
            Console.WriteLine("Hashed Salt: " + hashedUser.Salt);

            // Assert
            Assert.IsFalse(passwordMatched);
        }

    }
}
