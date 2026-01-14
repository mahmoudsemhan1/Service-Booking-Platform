using Application.Validators.Account;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBooking.UnitTests.UnitTests.Validators.Account
{
    public class RegisterDtoValidatorTests 
    {
        private readonly RegisterDtoValidator _validator;

        public RegisterDtoValidatorTests( )
        {
            _validator = new RegisterDtoValidator();
        }
        // this is a placeholder for actual test methods , tell to the unit test framework that this is a test method
        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            // email format is invalid 
            var model = new Application.DTOs.Account.Account.RegisterDto("wrong-email-format", "Pass123!", "Mahmoud", "Bio text", "user");

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Theory] // use theory to test multiple cases 
        [InlineData("Admin")]
        [InlineData("")]
        [InlineData("123")]
        // test invalid roles
        public void Should_Have_Error_When_Role_Is_Invalid(string role)
        {
            // arrange
            var model = new Application.DTOs.Account.Account.RegisterDto("test@test.com", "Pass123!", "Mahmoud", "Bio", role);

            // act
            var result = _validator.TestValidate(model);

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Role);
        }

        [Fact]
        // valid model should not have any validation errors
        public void Should_Not_Have_Error_When_Model_Is_Valid()
        {
            // arrange
            var model = new Application.DTOs.Account.Account.RegisterDto("valid@email.com", "SecureP@ss123", "Mahmoud Ahmed", "Senior Developer", "Provider");


            // act
            var result = _validator.TestValidate(model);
            // assert
            result.ShouldNotHaveAnyValidationErrors();


        }
    }
}
