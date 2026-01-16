using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MotorPool.Utils;

public static class EnumExtensions
{

    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName()!;
    }

    public static void Add<TFilterType>(this ICollection<IFilterMetadata> filters) where TFilterType : IFilterMetadata {
        var typeFilterAttribute = new TypeFilterAttribute(typeof(TFilterType));
        filters.Add(typeFilterAttribute);
    }

}