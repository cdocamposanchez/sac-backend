#region

using FluentValidation;
using DomainActividad = Sac.Domain.Models.Actividad;

#endregion

namespace Sac.Application.Modules.Actividades.Commands.CreateActividad;

public class CreateActividadValidator : AbstractValidator<CreateActividadCommand>
{
    public CreateActividadValidator()
    {
        _ = RuleFor(x => x.Actividad.Nombre).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Actividad.Descripcion).MaximumLength(500);
        _ = RuleFor(x => x.Actividad.CursoId).GreaterThan(0);
        _ = RuleFor(x => x.Actividad.PeriodoId).GreaterThan(0);

        // RF-08: cada actividad debe valer mínimo 1% y máximo 100%.
        _ = RuleFor(x => x.Actividad.Porcentaje)
            .InclusiveBetween(DomainActividad.PorcentajeMinimo, DomainActividad.PorcentajeMaximo)
            .WithMessage($"El porcentaje de la actividad debe estar entre " +
                         $"{DomainActividad.PorcentajeMinimo:0.##}% y {DomainActividad.PorcentajeMaximo:0.##}%.");
    }
}
