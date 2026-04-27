#region

using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace BuildingBlocks.Source.Application.Dtos.Pagination;

public class PaginationRequest
{
    public int PageIndex { get; set; }
    public int? PageSize { get; set; }

    [JsonIgnore][SwaggerIgnore] public bool ShouldPaginate => PageSize is > 0;

    [JsonIgnore][SwaggerIgnore] public int Skip => ShouldPaginate ? PageIndex * PageSize!.Value : 0;

    [JsonIgnore][SwaggerIgnore] public int Take => ShouldPaginate ? PageSize!.Value : int.MaxValue;
}
