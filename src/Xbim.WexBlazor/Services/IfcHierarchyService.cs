using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

public class IfcHierarchyService
{
    public HierarchyNode? GetSpatialStructure(IModel model, int modelId)
    {
        if (model == null) return null;

        var project = model.Instances.OfType<IIfcProject>().FirstOrDefault();
        if (project == null) return null;

        return BuildSpatialNode(project, modelId);
    }

    private HierarchyNode BuildSpatialNode(IIfcObjectDefinition obj, int modelId)
    {
        var node = new HierarchyNode
        {
            Id = obj.EntityLabel,
            Name = GetName(obj),
            ModelId = modelId,
            Icon = GetIcon(obj),
            ProductType = GetProductTypeId(obj)
        };

        var children = new List<HierarchyNode>();

        if (obj is IIfcProject project)
        {
            foreach (var rel in project.IsDecomposedBy)
            {
                foreach (var child in rel.RelatedObjects.OfType<IIfcSpatialStructureElement>())
                {
                    children.Add(BuildSpatialNode(child, modelId));
                }
            }
        }
        else if (obj is IIfcSpatialStructureElement spatial)
        {
            foreach (var rel in spatial.IsDecomposedBy)
            {
                foreach (var child in rel.RelatedObjects.OfType<IIfcSpatialStructureElement>())
                {
                    children.Add(BuildSpatialNode(child, modelId));
                }
            }

            var containedProducts = spatial.ContainsElements
                .SelectMany(r => r.RelatedElements)
                .ToList();

            node.ProductCount = containedProducts.Count;

            foreach (var product in containedProducts.Take(100))
            {
                children.Add(new HierarchyNode
                {
                    Id = product.EntityLabel,
                    Name = GetName(product),
                    ModelId = modelId,
                    Icon = GetIcon(product),
                    ProductType = GetProductTypeId(product)
                });
            }

            if (containedProducts.Count > 100)
            {
                children.Add(new HierarchyNode
                {
                    Id = -1,
                    Name = $"+{containedProducts.Count - 100} more products",
                    ModelId = modelId,
                    Icon = "bi-three-dots"
                });
            }
        }

        node.Children = children;
        return node;
    }

    private string GetName(IIfcObjectDefinition obj)
    {
        if (obj is IIfcRoot root)
        {
            var name = root.Name?.ToString();
            if (!string.IsNullOrWhiteSpace(name))
                return name;
        }
        return $"#{obj.EntityLabel}";
    }

    private string GetIcon(IIfcObjectDefinition obj)
    {
        return obj switch
        {
            IIfcProject => "bi-folder",
            IIfcSite => "bi-geo-alt",
            IIfcBuilding => "bi-building",
            IIfcBuildingStorey => "bi-layers",
            IIfcSpace => "bi-box",
            IIfcWall => "bi-square",
            IIfcDoor => "bi-door-open",
            IIfcWindow => "bi-window",
            IIfcSlab => "bi-square-fill",
            IIfcRoof => "bi-house",
            IIfcStair => "bi-ladder",
            IIfcColumn => "bi-bar-chart",
            IIfcBeam => "bi-dash-lg",
            IIfcFurnishingElement => "bi-lamp",
            IIfcFlowTerminal => "bi-gear",
            IIfcOpeningElement => "bi-dash-square",
            _ => "bi-box"
        };
    }

    private int? GetProductTypeId(IIfcObjectDefinition obj)
    {
        var typeName = obj.GetType().Name.ToUpperInvariant();
        
        return typeName switch
        {
            "IFCPROJECT" => null,
            "IFCSITE" => 349,
            "IFCBUILDING" => 169,
            "IFCBUILDINGSTOREY" => 459,
            "IFCSPACE" => 454,
            "IFCWALL" => 452,
            "IFCWALLSTANDARDCASE" => 453,
            "IFCDOOR" => 213,
            "IFCWINDOW" => 667,
            "IFCSLAB" => 99,
            "IFCROOF" => 347,
            "IFCSTAIR" => 346,
            "IFCCOLUMN" => 383,
            "IFCBEAM" => 171,
            "IFCMEMBER" => 310,
            "IFCPLATE" => 351,
            "IFCRAILING" => 350,
            "IFCCOVERING" => 382,
            "IFCFURNISHINGELEMENT" => 253,
            "IFCFURNITURE" => 1184,
            "IFCOPENINGELEMENT" => 498,
            _ => 20
        };
    }
}
