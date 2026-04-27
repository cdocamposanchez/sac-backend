#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Estudiantes.Commands.UpdateEstudiante;

public class UpdateEstudianteValidator : AbstractValidator<UpdateEstudianteCommand>
{
    public UpdateEstudianteValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);
        _ = RuleFor(x => x.Estudiante.NombreCompleto).NotEmpty().MaximumLength(150);
        _ = RuleFor(x => x.Estudiante.Correo).NotEmpty().EmailAddress();
    }
}
