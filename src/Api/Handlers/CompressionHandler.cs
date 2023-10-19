// -----------------------------------------------------------------------------------
// CompressionHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Handlers;

/// <summary>
/// CompressionHandler
/// </summary>
public static class CompressionHandler
{
    /// <summary>
    /// ApplyCompress
    /// </summary>
    /// <param name="services"></param>
    public static void ApplyCompress(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            var mimeTypes = new[]
            {
                Constants.HeaderJsonVndApi,
                Constants.HeaderPdf,
                Constants.HeaderTextPlain,
                Constants.HeaderImageJpg,
                Constants.HeaderJson,
                Constants.HeaderOctetStream,
                Constants.HeaderProblemJson,
                Constants.HeaderTextCsv,
                Constants.HeaderExcelXls,
                Constants.HeaderExcelXlsx
            };
            options.EnableForHttps = true;
            options.MimeTypes = mimeTypes;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
    }
}

/// <summary>
/// AddCompressionHandlerExtension
/// </summary>
public static class AddCompressionHandlerExtension
{
    /// <summary>
    /// AddCompressionHandler
    /// </summary>
    /// <param name="services"></param>
    public static void AddCompressionHandler(this IServiceCollection services)
    {
        CompressionHandler.ApplyCompress(services);
    }
}
