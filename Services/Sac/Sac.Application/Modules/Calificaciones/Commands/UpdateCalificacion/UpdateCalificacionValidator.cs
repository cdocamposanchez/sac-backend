#region

using FluentValidation;
using DomainCalificacion = Sac.Domain.Models.Calificacion;

#endregion

namespace Sac.Application.Modules.Calificaciones.Commands.UpdateCalificacion;

public class UpdateCalificacionValidator : AbstractValidator<UpdateCalificacionCommand>
{
    public UpdateCalificacionValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);

        _ = RuleFor(x => x.Calificacion.Nota)
            .InclusiveBetween(DomainCalificacion.NotaMinima, DomainCalificacion.NotaMaxima)
            .WithMessage("La nota debe estar entre 1.0 y 5.0.");
    }
}
