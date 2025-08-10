using Core.Helper;


namespace CoreTests
{
    public class SecurityHelperTests
    {

        [Fact]
        public void HashPasswordSimple_Should_AlwaysReturnSameHash_ForSameInput()
        {
            var password = "MySecurePassword!";
            var hash1 = SecurityHelper.HashPasswordSimple(password);
            var hash2 = SecurityHelper.HashPasswordSimple(password);

            Assert.Equal(hash1, hash2); // SHA256 ist deterministisch
        }

        [Fact]
        public void HashPassword_Should_ReturnDifferentHashes_ForSamePassword()
        {
            var password = "MySecurePassword!";
            var hash1 = SecurityHelper.HashPassword(password);
            var hash2 = SecurityHelper.HashPassword(password);

            Assert.NotEqual(hash1, hash2); // wegen Salt
        }

        [Fact]
        public void VerifyPassword_Should_ReturnTrue_ForCorrectPassword()
        {
            var password = "Test123!";
            var hash = SecurityHelper.HashPassword(password);

            var result = SecurityHelper.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_Should_ReturnFalse_ForIncorrectPassword()
        {
            var password = "Test123!";
            var hash = SecurityHelper.HashPassword(password);

            var result = SecurityHelper.VerifyPassword("WrongPassword", hash);

            Assert.False(result);
        }
    }
}
