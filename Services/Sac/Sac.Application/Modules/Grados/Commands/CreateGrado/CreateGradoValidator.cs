#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Grados.Commands.CreateGrado;

public class CreateGradoValidator : AbstractValidator<CreateGradoCommand>
{
    public CreateGradoValidator()
    {
        _ = RuleFor(x => x.Grado.Nombre).NotEmpty().MaximumLength(50);
        _ = RuleFor(x => x.Grado.Descripcion).MaximumLength(255);
    }
}
