// -----------------------------------------------------------------------------------
// SendToMsTeams.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NetCa.Application.Common.Models;
using Newtonsoft.Json;
using Serilog;

namespace NetCa.Infrastructure.Apis;

/// <summary>
/// SendToMsTeams
/// </summary>
public static class SendToMsTeams
{
    private static readonly HttpClient _httpClient = new (new HttpHandler(new HttpClientHandler()));
    private static readonly ILogger _logger = Log.ForContext(typeof(SendToMsTeams));

    /// <summary>
    /// SendToMsTeam
    /// </summary>
    /// <param name="appSetting"></param>
    /// <param name="tmpl"></param>
    /// <returns></returns>
    public static async Task Send(AppSetting appSetting, MsTeamTemplate tmpl)
    {
        if (!_httpClient.DefaultRequestHeaders.Contains(appSetting.Bot.Header))
            _httpClient.DefaultRequestHeaders.Add(appSetting.Bot.Header, appSetting.Bot.Secret);

        var str = JsonConvert.SerializeObject(tmpl);
        var count = Encoding.UTF8.GetByteCount(str);

        if (count > Constants.MsTeamsMaxSizeInBytes)
        {
            _logger.Warning($"Cancelling sending message to teams. Size message too large ({count} byte)");
        }
        else
        {
            var content = new StringContent(str, Encoding.UTF8, Constants.HeaderJson);
            var response = _httpClient.PostAsync(new Uri(appSetting.Bot.Address), content);
            await response;
        }
    }
}