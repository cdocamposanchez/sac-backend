#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiante;

public class AsignarEstudianteValidator : AbstractValidator<AsignarEstudianteCommand>
{
    public AsignarEstudianteValidator()
    {
        _ = RuleFor(x => x.Datos.CursoId).GreaterThan(0);
        _ = RuleFor(x => x.Datos.EstudianteId).GreaterThan(0);
    }
}
