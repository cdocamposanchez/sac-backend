#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Materias.Commands.CreateMateria;

public class CreateMateriaValidator : AbstractValidator<CreateMateriaCommand>
{
    public CreateMateriaValidator()
    {
        _ = RuleFor(x => x.Materia.Nombre).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Materia.Descripcion).MaximumLength(255);
    }
}
