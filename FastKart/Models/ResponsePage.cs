using FluentValidation;

namespace FastKart.Models
{
    public class ResponsePage
    {
        public int Number { get; set; } = 0;
        public int Count { get; set; } = 10;
    }

    public class ResponsePageValidator : AbstractValidator<ResponsePage>
    {
        public ResponsePageValidator()
        {
            RuleFor(t => t.Number)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Number must be greater than or equal to 0.");

            RuleFor(t => t.Count)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Count must be greater than or equal to 1.")
                .LessThanOrEqualTo(100)
                .WithMessage("Count must be less than or equal to 100.");
        }
    }
}
