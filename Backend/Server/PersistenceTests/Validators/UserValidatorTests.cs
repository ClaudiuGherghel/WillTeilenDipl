using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Validations.EntityValidators;
using Persistence.Validations.ValidationRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceTests.Validators
{
    public class UserValidatorTests : TestBase
    {

        [Fact]
        public async Task ValidateAsync_Throws_When_UserNameExistsInDb()
        {
            // Arrange
            using var context = CreateDbContext();
            context.Users.Add(new User { Id = 1, UserName = "MaxMustermann" });
            await context.SaveChangesAsync();

            var rule = new UserUniqueUserNameRule(context);
            var newUser = new User { Id = 2, UserName = "MaxMustermann" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(newUser)
            );
        }

        [Fact]
        public async Task ValidateAsync_Throws_When_UserNameExistsInMemory()
        {
            using var context = CreateDbContext();

            var user1 = new User { Id = 1, UserName = "admin" };
            var user2 = new User { Id = 2, UserName = "admin" };

            context.Users.Add(user1);
            context.Users.Add(user2); // Noch nicht gespeichert

            var rule = new UserUniqueUserNameRule(context);

            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(user2, checkMemory: true)
            );
        }

        [Fact]
        public async Task ValidateAsync_Throws_When_EmailExistsInDb()
        {
            using var context = CreateDbContext();
            context.Users.Add(new User { Id = 1, Email = "test@example.com" });
            await context.SaveChangesAsync();

            var rule = new UserUniqueEmailRule(context);
            var newUser = new User { Id = 2, Email = "test@example.com" };

            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(newUser)
            );
        }

        [Fact]
        public async Task ValidateAsync_Throws_When_EmailExistsInMemory()
        {
            using var context = CreateDbContext();

            var user1 = new User { Id = 1, Email = "duplicate@example.com" };
            var user2 = new User { Id = 2, Email = "duplicate@example.com" };

            context.Users.Add(user1);
            context.Users.Add(user2);

            var rule = new UserUniqueEmailRule(context);

            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(user2, checkMemory: true)
            );
        }

        [Fact]
        public async Task ValidateAsync_Allows_Unique_Email_And_Username()
        {
            using var context = CreateDbContext();

            var user = new User
            {
                Id = 3,
                UserName = "uniqueUser",
                Email = "unique@example.com"
            };

            var nameRule = new UserUniqueUserNameRule(context);
            var emailRule = new UserUniqueEmailRule(context);

            var ex1 = await Record.ExceptionAsync(() => nameRule.ValidateAsync(user));
            var ex2 = await Record.ExceptionAsync(() => emailRule.ValidateAsync(user));

            Assert.Null(ex1);
            Assert.Null(ex2);
        }

    }

}
