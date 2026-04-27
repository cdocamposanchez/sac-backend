#region

using System.Text.Json.Serialization;
using BuildingBlocks.Source.Application.Utils;

#endregion

namespace BuildingBlocks.Source.Application.Dtos;

public class ApiResponseDto<T>
{
    public string Message { get; set; } = "Success";
    public DateTime ResponseTime { get; set; } = AppDateTime.Now;
    public T? Data { get; set; }

    [JsonExtensionData] public Dictionary<string, object>? Detalles { get; set; }
}
