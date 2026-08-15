using FluentValidation;

namespace FastKart.Controllers
{

    public partial class AuthController
    {
        public class LoginModel()
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class LoginModelValidator : AbstractValidator<LoginModel>
        {
            public LoginModelValidator()
            {
                RuleFor(t => t.Email)
                    .NotEmpty()
                    .WithMessage("Email is required.")
                    .EmailAddress()
                    .WithMessage("Email must be a valid email address.");

                RuleFor(t => t.Password)
                    .NotEmpty()
                    .WithMessage("Password is required.")
                    .MaximumLength(50)
                    .WithMessage("Password must not exceed 50 characters.");
            }
        }
    }
}
