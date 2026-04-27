#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.UpdateDocente;

public class UpdateDocenteValidator : AbstractValidator<UpdateDocenteCommand>
{
    public UpdateDocenteValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);
        _ = RuleFor(x => x.Docente).NotNull();
        _ = RuleFor(x => x.Docente.NombreCompleto).NotEmpty().MaximumLength(150);
        _ = RuleFor(x => x.Docente.Correo).NotEmpty().EmailAddress();
    }
}
