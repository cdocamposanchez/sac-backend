#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Grados.Commands.UpdateGrado;

public class UpdateGradoValidator : AbstractValidator<UpdateGradoCommand>
{
    public UpdateGradoValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);
        _ = RuleFor(x => x.Grado.Nombre).NotEmpty().MaximumLength(50);
    }
}
