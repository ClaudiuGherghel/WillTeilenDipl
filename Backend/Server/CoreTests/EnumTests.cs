using Core.Enums;

namespace CoreTests
{
    public class EnumTests
    {
        [Fact]
        public void Enum_ItemCondition_Should_Be_Unknown()
        {
            var valid = Enum.IsDefined(typeof(ItemCondition), "Unknown");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_ItemCondition_Should_Be_LikeNew()
        {
            var valid = Enum.IsDefined(typeof(ItemCondition), "LikeNew");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_ItemCondition_Should_Be_Good()
        {
            var valid = Enum.IsDefined(typeof(ItemCondition), "Good");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_ItemCondition_Should_Be_Used()
        {
            var valid = Enum.IsDefined(typeof(ItemCondition), "Used");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_RentalStatus_Should_Be_Valid_Active()
        {
            var valid = Enum.IsDefined(typeof(RentalStatus), "Active");

            Assert.True(valid);
        }
        [Fact]
        public void Enum_RentalStatus_Should_Be_Valid_Cancelled()
        {
            var valid = Enum.IsDefined(typeof(RentalStatus), "Cancelled");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_RentalStatus_Should_Be_Valid_Completed()
        {
            var valid = Enum.IsDefined(typeof(RentalStatus), "Completed");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_RentalType_Should_Be_Valid_Unknown()
        {
            var valid = Enum.IsDefined(typeof(RentalType), "Unknown");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_RentalType_Should_Be_Valid_Privat()
        {
            var valid = Enum.IsDefined(typeof(RentalType), "Privat");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_RentalType_Should_Be_Valid_Dealer()
        {
            var valid = Enum.IsDefined(typeof(RentalType), "Dealer");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_Roles_Should_Be_Valid_Admin()
        {
            var valid = Enum.IsDefined(typeof(Roles), "Admin");

            Assert.True(valid);
        }

        [Fact]
        public void Enum_Roles_Should_Be_Valid_User()
        {
            var valid = Enum.IsDefined(typeof(Roles), "User");

            Assert.True(valid);
        }
    }
}
