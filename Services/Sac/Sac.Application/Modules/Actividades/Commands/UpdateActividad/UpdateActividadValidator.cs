#region

using FluentValidation;
using DomainActividad = Sac.Domain.Models.Actividad;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.UpdateActividad;

public class UpdateActividadValidator : AbstractValidator<UpdateActividadCommand>
{
    public UpdateActividadValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);
        _ = RuleFor(x => x.Actividad.Nombre).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Actividad.Descripcion).MaximumLength(500);

        _ = RuleFor(x => x.Actividad.Porcentaje)
            .InclusiveBetween(DomainActividad.PorcentajeMinimo, DomainActividad.PorcentajeMaximo)
            .WithMessage($"El porcentaje de la actividad debe estar entre " +
                         $"{DomainActividad.PorcentajeMinimo:0.##}% y {DomainActividad.PorcentajeMaximo:0.##}%.");
    }
}
