#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        _ = RuleFor(x => x.Credentials)
            .NotNull().WithMessage("Las credenciales son obligatorias.");

        _ = RuleFor(x => x.Credentials.Cedula)
            .NotEmpty().WithMessage("La cédula es obligatoria.")
            .Matches("^[0-9]+$").WithMessage("La cédula solo puede contener dígitos.");

        _ = RuleFor(x => x.Credentials.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}
