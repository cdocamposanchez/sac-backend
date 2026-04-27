#region

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Config;

public class RoutePrefixConvention(string prefix) : IApplicationModelConvention
{
    private AttributeRouteModel RoutePrefix => new(new RouteAttribute(prefix));

    public void Apply(ApplicationModel application)
    {
        var controllers = application.Controllers;

        var controllersWithRoute =
            controllers.Where(c =>
                c.Selectors.Any(s => s.AttributeRouteModel != null));

        var controllerModels = controllersWithRoute.ToList();

        controllerModels
            .SelectMany(c => c.Selectors.Where(s => s.AttributeRouteModel != null))
            .ToList()
            .ForEach(s =>
                s.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                    RoutePrefix,
                    s.AttributeRouteModel));

        foreach (var controller in controllers.Except(controllerModels))
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = RoutePrefix
            });
    }
}
