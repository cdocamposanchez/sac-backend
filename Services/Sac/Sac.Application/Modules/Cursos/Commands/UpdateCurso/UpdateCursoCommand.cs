#region

using BuildingBlocks.Source.Application.CQRS;
using Sac.Domain.Dtos;

#endregion

namespace Sac.Application.Modules.Cursos.Commands.UpdateCurso;

public record UpdateCursoCommand(long Id, UpdateCursoDto Curso) : ICommand<CursoDto>;
