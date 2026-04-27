#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.CursoEstudiantes.Commands.AsignarEstudiantesLote;

public class AsignarEstudiantesLoteValidator : AbstractValidator<AsignarEstudiantesLoteCommand>
{
    public AsignarEstudiantesLoteValidator()
    {
        _ = RuleFor(x => x.Datos.CursoId).GreaterThan(0);
        _ = RuleFor(x => x.Datos.EstudianteIds)
            .NotNull()
            .NotEmpty().WithMessage("La lista de estudiantes no puede estar vacía.");
    }
}
