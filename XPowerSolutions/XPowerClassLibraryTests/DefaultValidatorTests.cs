using NUnit.Framework;
using System;
using XPowerClassLibrary.Validator;

namespace XPowerClassLibrary.Tests
{
    public class DefaultValidatorTests
    {

        [TestCase("fuckface")]
        [TestCase("dickhead")]
        public void IllegalNameList_IllegalName_ShouldThrowArgumentException(string testInput)
        {
            // Arrange
            string usernameToTest = testInput;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidateUsernameException(usernameToTest); });
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidateDeviceNameException(usernameToTest); });
        }

        [TestCase("Pa$$lengt")]
        public void ValidatePasswordException_NotLongEnough_ShouldThrowArgumentException(string testInput)
        {
            // Arrange
            string valuetoTest = testInput;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidatePasswordException(valuetoTest); });
        }


        [TestCase("ValidUsername")]
        [TestCase("validusername")]
        [TestCase("username with space")]
        public void ValidateUsernameException_ValidUsername_ShouldNotThrow(string testInput)
        {
            // Arrange
            string usernameToTest = testInput;

            // Act & Assert
            Assert.DoesNotThrow(() => { DefaultValidators.ValidateUsernameException(usernameToTest); });
        }

        [Test]
        public void ValidateUsernameException_NullInput_ShouldThrowArgumentNullException()
        {
            // Arrange
            string usernameToTest = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => { DefaultValidators.ValidateUsernameException(usernameToTest); });
        }

        [TestCase("")]              // Test for empty string
        [TestCase("AAA")]           // Test for username below 4 characters
        [TestCase("SqlName union select * from user")]  // Test for SQL injection.
        [TestCase(",.--12/()(/(&%&%(/,3-213123.com")]
        [TestCase("105 OR 1=1")]
        public void ValidateUsernameException_InvalidInput_ShouldThrowArgumentException(string usernameInput)
        {
            // Arrange
            string usernameToTest = usernameInput;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidateUsernameException(usernameToTest); });
        }





        [TestCase("TestMail@mail.com")]
        [TestCase("mailtest@mail.dk")]
        public void ValidateMailException_ValidMail_ShouldNotThrow(string input)
        {
            // Arrange
            string inputTotest = input;

            // Act & Assert
            Assert.DoesNotThrow(() => { DefaultValidators.ValidateMailException(inputTotest); });
        }

        [Test]
        public void ValidateMailException_NullInput_ShouldThrowArgumentNullException()
        {
            // Arrange
            string inputTotest = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => { DefaultValidators.ValidateMailException(inputTotest); });
        }

        [TestCase("")]
        [TestCase("mail test")]
        [TestCase("mail")]
        [TestCase("mail.com")]
        [TestCase(",.--12/()(/(&%&%(/,3-213123.com")]
        [TestCase("105 OR 1=1")]
        [TestCase("hello123rf@com'")]
        public void ValidateMailException_InvalidInput_ShouldThrowArgumentException(string input)
        {
            // Arrange
            string inputTotest = input;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidateMailException(inputTotest); });
        }




        [TestCase("12341234!weqwe")]
        [TestCase("Pa$$w0rd9872")]
        public void ValidatePasswordException_ValidPassword_ShouldNotThrow(string input)
        {
            // Arrange
            string inputTotest = input;

            // Act & Assert
            Assert.DoesNotThrow(() => { DefaultValidators.ValidatePasswordException(inputTotest); });
        }

        [Test]
        public void ValidatePasswordException_NullInput_ShouldThrowArgumentNullException()
        {
            // Arrange
            string inputTotest = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => { DefaultValidators.ValidatePasswordException(inputTotest); });
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("password")]
        [TestCase("Password")]
        [TestCase("1234")]
        [TestCase("aeJwxGfPrDQBSYh2wz9ME3HFUvgfCJFMcekehpjFnedhhJuLfGBBNFzjtk7XwjHHA")]
        public void ValidatePasswordException_InvalidInput_ShouldThrowArgumentException(string input)
        {
            // Arrange
            string inputTotest = input;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidatePasswordException(inputTotest); });
        }




        [TestCase("BlåLampe")]
        [TestCase("DeviceName1")]
        public void ValidateDeviceNameException_ValidDevicename_ShouldNotThrow(string input)
        {
            // Arrange
            string inputTotest = input;

            // Act & Assert
            Assert.DoesNotThrow(() => { DefaultValidators.ValidateDeviceNameException(inputTotest); });
        }

        [Test]
        public void ValidateDeviceNameException_NullInput_ShouldThrowArgumentNullException()
        {
            // Arrange
            string inputTotest = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => { DefaultValidators.ValidateDeviceNameException(inputTotest); });
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("nam")]
        [TestCase("Device name")]
        [TestCase("1234")]
        [TestCase("aeJwxGfPrDQBSYh2wz9ME3HFUvgfCJFMcekehpjFnedhhJuLfGBBNFzjtk7XwjHHA")]
        public void ValidateDeviceNameException_InvalidInput_ShouldThrowArgumentException(string input)
        {
            // Arrange
            string inputTotest = input;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { DefaultValidators.ValidateDeviceNameException(inputTotest); });
        }

    }
}
