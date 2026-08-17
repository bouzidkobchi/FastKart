using FluentValidation;

namespace FastKart.Models.Requests
{
    public class UpdateUserRoleModel
    {
        public required Guid UserId { get; set; }
        public required string Role { get; set; }
    }

    public class UpdateUserRoleModelValidator : AbstractValidator<UpdateUserRoleModel>
    {
        public UpdateUserRoleModelValidator()
        {
            RuleFor(t => t.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(t => t.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .MaximumLength(50)
                .WithMessage("Role must not exceed 50 characters.");
        }
    }
}
