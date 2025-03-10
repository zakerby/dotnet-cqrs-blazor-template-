using FluentValidation;

namespace Core.Backend.Application.Commands.Example
{
    public class UpdateExampleNameCommandValidator : AbstractValidator<UpdateExampleNameCommand>
    {
        public UpdateExampleNameCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Id).NotNull();
        }
    }
}