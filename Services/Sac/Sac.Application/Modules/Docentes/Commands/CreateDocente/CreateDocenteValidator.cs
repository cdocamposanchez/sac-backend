#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Docentes.Commands.CreateDocente;

public class CreateDocenteValidator : AbstractValidator<CreateDocenteCommand>
{
    public CreateDocenteValidator()
    {
        _ = RuleFor(x => x.Docente).NotNull().WithMessage("Los datos del docente son obligatorios.");

        _ = RuleFor(x => x.Docente.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar 150 caracteres.");

        _ = RuleFor(x => x.Docente.Cedula)
            .NotEmpty().WithMessage("La cédula es obligatoria.")
            .Matches("^[0-9]+$").WithMessage("La cédula solo puede contener dígitos.")
            .Length(6, 15).WithMessage("La cédula debe tener entre 6 y 15 dígitos.");

        _ = RuleFor(x => x.Docente.Correo)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .MaximumLength(150);

        _ = RuleFor(x => x.Docente.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}
