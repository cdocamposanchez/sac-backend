#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.UpdateCurso;

public class UpdateCursoValidator : AbstractValidator<UpdateCursoCommand>
{
    public UpdateCursoValidator()
    {
        _ = RuleFor(x => x.Id).GreaterThan(0);
        _ = RuleFor(x => x.Curso.Nombre).NotEmpty().MaximumLength(150);
        _ = RuleFor(x => x.Curso.GradoId).GreaterThan(0);
        _ = RuleFor(x => x.Curso.MateriaId).GreaterThan(0);
        _ = RuleFor(x => x.Curso.DocenteId).GreaterThan(0);
    }
}
