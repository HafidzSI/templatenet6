using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NetCa.Application.Common.Utils;

/// <summary>
/// IgnoreJsonAttributesResolver
/// </summary>
/// <returns></returns>
public class IgnoreJsonAttributesResolver : DefaultContractResolver
{
    /// <summary>
    /// CreateProperties
    /// </summary>
    /// <param name="type"></param>
    /// <param name="memberSerialization"></param>
    /// <returns></returns>
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);

        foreach (var property in properties)
        {
            property.Ignored = false;
            property.Converter = null;
            property.PropertyName = property.UnderlyingName;
        }

        return properties;
    }
}