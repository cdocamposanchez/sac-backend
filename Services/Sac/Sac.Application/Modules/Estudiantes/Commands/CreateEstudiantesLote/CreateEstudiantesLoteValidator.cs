#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.CreateEstudiantesLote;

public class CreateEstudiantesLoteValidator : AbstractValidator<CreateEstudiantesLoteCommand>
{
    public CreateEstudiantesLoteValidator()
    {
        _ = RuleFor(x => x.Estudiantes)
            .NotNull().NotEmpty().WithMessage("Debe proveer al menos un estudiante.");

        _ = RuleForEach(x => x.Estudiantes).ChildRules(e =>
        {
            e.RuleFor(s => s.NombreCompleto).NotEmpty().MaximumLength(150);
            e.RuleFor(s => s.Cedula).NotEmpty().Matches("^[0-9]+$").Length(6, 15);
            e.RuleFor(s => s.Correo).NotEmpty().EmailAddress().MaximumLength(150);
        });
    }
}
