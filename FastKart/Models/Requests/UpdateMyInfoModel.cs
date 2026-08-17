using FluentValidation;

namespace FastKart.Models.Requests
{
    public class UpdateMyInfoModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
    }

    public class UpdateMyInfoModelValidator : AbstractValidator<UpdateMyInfoModel>
    {
        public UpdateMyInfoModelValidator()
        {
            RuleFor(t => t.Name)
                .MaximumLength(50)
                .WithMessage("Name must not exceed 50 characters.");

            RuleFor(t => t.Email)
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(t => t.Password)
                .MaximumLength(50)
                .WithMessage("Password must not exceed 50 characters.");

            RuleFor(t => t.Phone)
                .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 format
                .WithMessage("A valid phone number is required.");

            RuleFor(t => t)
                .Must(t => MustBeAtLeastOneProperty(t))
                .WithMessage("At least one property must be provided.");
        }

        private bool MustBeAtLeastOneProperty(UpdateMyInfoModel model)
        {
            return model.Email != null
                || model.Phone != null
                || model.Name != null
                || model.Password != null;
        }
    }
}
