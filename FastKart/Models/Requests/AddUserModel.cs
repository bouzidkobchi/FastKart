using FluentValidation;

namespace FastKart.Models.Requests
{
    public class AddUserModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public bool Status { get; set; } = true; // false if blocked, true otherwise
        public required string Password { get; set; }
        public required string Phone { get; set; }
    }

    public class AddUSerValidator : AbstractValidator<AddUserModel>
    {
        public AddUSerValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(50)
                .WithMessage("Name must not exceed 50 characters.");

            RuleFor(t => t.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(t => t.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MaximumLength(50)
                .WithMessage("Password must not exceed 50 characters.");

            RuleFor(t => t.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 format
                .WithMessage("A valid phone number is required.");
        }
    }
}
