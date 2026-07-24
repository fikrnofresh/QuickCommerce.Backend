using FluentValidation;
using QuickCommerce.Core.DTOs.Store;

namespace QuickCommerce.Core.Validators.Store
{
    public class StoreOnboardingValidator
        : AbstractValidator<StoreOnboardingDto>
    {
        public StoreOnboardingValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Area)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Pincode)
                .NotEmpty()
                .Length(6);

            RuleFor(x => x.OwnerName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.OwnerMobile)
                .NotEmpty()
                .Matches(@"^[6-9]\d{9}$")
                .WithMessage("Invalid Indian Mobile Number.");

            RuleFor(x => x.OwnerEmail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty();

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.OpeningTime)
                .NotNull();

            RuleFor(x => x.ClosingTime)
                .NotNull();
        }
    }
}