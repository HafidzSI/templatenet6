// -----------------------------------------------------------------------------------
// MappingExtensions.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi;
using NetCa.Application.Common.Extensions;

namespace NetCa.Application.Common.Mappings;

/// <summary>
/// MappingExtensions
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// PaginatedListAsync
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="meta"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static Task<DocumentRootJson<List<TDestination>>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, Meta meta, int pageNumber, int pageSize)
        => JsonApiExtensionPaginated.CreateAsync(queryable, meta, pageNumber, pageSize);

    /// <summary>
    /// ProjectToListAsync
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="configuration"></param>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
        this IQueryable queryable, IConfigurationProvider configuration)
        => queryable.ProjectTo<TDestination>(configuration).ToListAsync();
}