using FluentValidation;

namespace FastKart.Models.Requests
{
    public class UpdateUserStatusModel
    {
        public Guid UserId { get; set; }
        public bool IsApproved { get; set; }
    }

    public class UpdateUserStatusModelValidator : AbstractValidator<UpdateUserStatusModel>
    {
        public UpdateUserStatusModelValidator()
        {
            RuleFor(t => t.UserId)
                .NotEmpty()
                .WithMessage("userId can not be empty.");
        }
    }
}
