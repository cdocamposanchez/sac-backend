#region

using FluentValidation;
using DomainCalificacion = Sac.Domain.Models.Calificacion;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.CreateCalificacion;

public class CreateCalificacionValidator : AbstractValidator<CreateCalificacionCommand>
{
    public CreateCalificacionValidator()
    {
        _ = RuleFor(x => x.Calificacion.EstudianteId).GreaterThan(0);
        _ = RuleFor(x => x.Calificacion.ActividadId).GreaterThan(0);
        _ = RuleFor(x => x.Calificacion.CursoId).GreaterThan(0);
        _ = RuleFor(x => x.Calificacion.PeriodoId).GreaterThan(0);

        // RF-08 / RNF-05: validación frontend+backend del rango 1.0 - 5.0
        _ = RuleFor(x => x.Calificacion.Nota)
            .InclusiveBetween(DomainCalificacion.NotaMinima, DomainCalificacion.NotaMaxima)
            .WithMessage("La nota debe estar entre 1.0 y 5.0.");
    }
}
