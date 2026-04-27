#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.CreateEstudiante;

public class CreateEstudianteValidator : AbstractValidator<CreateEstudianteCommand>
{
    public CreateEstudianteValidator()
    {
        _ = RuleFor(x => x.Estudiante).NotNull();
        _ = RuleFor(x => x.Estudiante.NombreCompleto).NotEmpty().MaximumLength(150);
        _ = RuleFor(x => x.Estudiante.Cedula)
            .NotEmpty()
            .Matches("^[0-9]+$").WithMessage("La cédula solo puede contener dígitos.")
            .Length(6, 15);
        _ = RuleFor(x => x.Estudiante.Correo).NotEmpty().EmailAddress().MaximumLength(150);
    }
}
