#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Materias.Commands.UpdateMateria;

public class UpdateMateriaValidator : AbstractValidator<UpdateMateriaCommand>
{
    public UpdateMateriaValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);
        _ = RuleFor(x => x.Materia.Nombre).NotEmpty().MaximumLength(100);
    }
}
