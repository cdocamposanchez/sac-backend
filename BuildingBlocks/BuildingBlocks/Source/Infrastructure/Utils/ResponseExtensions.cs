#region

using Microsoft.AspNetCore.Http;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Utils;

public static class ResponseExtensions
{
    public static void AddResponseDetail(this HttpContext context, string key, object value)
    {
        if (!context.Items.TryGetValue("ResponseDetails", out var existing) ||
            existing is not Dictionary<string, object> dict)
        {
            dict = new Dictionary<string, object>();
            context.Items["ResponseDetails"] = dict;
        }

        dict[key] = value;
    }
}
