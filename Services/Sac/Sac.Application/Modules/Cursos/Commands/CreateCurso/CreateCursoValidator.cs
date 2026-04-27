#region

using FluentValidation;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.CreateCurso;

public class CreateCursoValidator : AbstractValidator<CreateCursoCommand>
{
    public CreateCursoValidator()
    {
        _ = RuleFor(x => x.Curso.Nombre).NotEmpty().MaximumLength(150);
        _ = RuleFor(x => x.Curso.GradoId).GreaterThan(0);
        _ = RuleFor(x => x.Curso.MateriaId).GreaterThan(0);
        _ = RuleFor(x => x.Curso.DocenteId).GreaterThan(0);
        _ = RuleFor(x => x.Curso.AnioAcademico).InclusiveBetween(2000, 2100);
    }
}
