// -----------------------------------------------------------------------------------
// Constants.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace NetCa.Application.Common.Models;

/// <summary>
/// Constants
/// </summary>
public readonly record struct Constants
{
    /// <summary>
    /// DefaultPageSize
    /// </summary>
    public const int DefaultPageSize = 10;

    /// <summary>
    /// DefaultPageSize
    /// </summary>
    public const int DefaultPageNumber = 1;

    /// <summary>
    /// SyncDateFormat
    /// </summary>
    public const string SyncDateFormat = "yyyyMMddHHmmss";

    /// <summary>
    /// CustomRuleAsciiAlphabet (Actual ASCII alphabet start from 65)
    /// </summary>
    public const int CustomRuleAsciiAlphabet = 64;

    #region Regex

    /// <summary>
    /// RegexPattern
    /// </summary>
    public const string RegexPattern = @"^[{pattern}]+$";

    /// <summary>
    /// RegexChar
    /// </summary>
    public const string RegexChar = "a-zA-Z";

    /// <summary>
    /// RegexNumeric
    /// </summary>
    public const string RegexNumeric = "0-9";

    /// <summary>
    /// RegexSymbol
    /// </summary>
    public const string RegexSymbol = @"\&()@_:;/.-";

    #endregion

    #region environment

    /// <summary>
    /// EnvironmentNameTest
    /// </summary>
    public const string EnvironmentNameTest = "Test";

    /// <summary>
    /// ApplicationName
    /// </summary>
    public const string ApplicationName = "NetCa.Api";

    #endregion environment

    #region header

    /// <summary>
    /// OcpApimSubscriptionKey
    /// </summary>
    public const string HeaderOcpApimSubscriptionKey = "Ocp-Apim-Subscription-Key";

    /// <summary>
    /// HeaderJson
    /// </summary>
    public const string HeaderJson = "application/json";

    /// <summary>
    /// HeaderJsonVndApi
    /// </summary>
    public const string HeaderJsonVndApi = "application/vnd.api+json";

    /// <summary>
    /// HeaderPdf
    /// </summary>
    public const string HeaderPdf = "application/pdf";

    /// <summary>
    /// HeaderTextPlain
    /// </summary>
    public const string HeaderTextPlain = "text/plain";

    /// <summary>
    /// HeaderOctetStream
    /// </summary>
    public const string HeaderOctetStream = "application/octet-stream";

    /// <summary>
    /// HeaderProblemJson
    /// </summary>
    public const string HeaderProblemJson = "application/problem+json";

    /// <summary>
    /// HeaderTextCsv
    /// </summary>
    public const string HeaderTextCsv = "text/csv";

    /// <summary>
    /// HeaderExcelXls
    /// </summary>
    public const string HeaderExcelXls = "application/vnd.ms-excel";

    /// <summary>
    /// HeaderExcelXlsx
    /// </summary>
    public const string HeaderExcelXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    /// <summary>
    /// HeaderImageJpg
    /// </summary>
    public const string HeaderImageJpg = "image/jpg";

    /// <summary>
    /// HeaderMultipart
    /// </summary>
    public const string HeaderMultipart = "multipart/form-data";

    /// <summary>
    /// HeaderZip
    /// </summary>
    public const string HeaderZip = "application/zip";

    /// <summary>
    /// HeaderIfNoneMatch
    /// </summary>
    public const string HeaderIfNoneMatch = "If-None-Match";

    /// <summary>
    /// HeaderETag
    /// </summary>
    public const string HeaderETag = "ETag";

    #endregion header

    #region redis

    /// <summary>
    /// RedisSubKeyConsumeMessage
    /// </summary>
    public const string RedisSubKeyConsumeMessage = "ConsumeMessage";

    /// <summary>
    /// RedisSubKeyProduceMessage
    /// </summary>
    public const string RedisSubKeyProduceMessage = "ProduceMessage";

    /// <summary>
    /// RedisSubKeyHttpRequest
    /// </summary>
    public const string RedisSubKeyHttpRequest = "HttpRequest";

    #endregion redis

    #region  system

    /// <summary>
    /// SystemName
    /// </summary>
    public const string SystemName = "System";

    /// <summary>
    /// SystemCustomerName
    /// </summary>
    public const string SystemCustomerName = "PT. United Tractors";

    /// <summary>
    /// SystemEmail
    /// </summary>
    public const string SystemEmail = "system@unitedtractors.com";

    /// <summary>
    /// SystemClientId
    /// </summary>
    public const string SystemClientId = "netca";

    #endregion system

    #region  api

    /// <summary>
    /// ApiErrorDescription
    /// </summary>
    public readonly record struct ApiErrorDescription
    {
        /// <summary>
        /// BadRequest
        /// </summary>
        public const string BadRequest = "BadRequest";

        /// <summary>
        /// Unauthorized
        /// </summary>
        public const string Unauthorized = "Unauthorized";

        /// <summary>
        /// Forbidden
        /// </summary>
        public const string Forbidden = "Forbidden";

        /// <summary>
        /// InternalServerError
        /// </summary>
        public const string InternalServerError = "InternalServerError";
    }

    #endregion api

    #region  UserAttribute

    /// <summary>
    /// ClientId
    /// </summary>
    public const string ClientId = "client_id";

    /// <summary>
    /// CustomerCode
    /// </summary>
    public const string CustomerCode = "customer_code";

    /// <summary>
    /// PlantFieldName
    /// </summary>
    public const string PlantFieldName = "Plant";

    /// <summary>
    /// CustomerSiteFieldName
    /// </summary>
    public const string CustomerSiteFieldName = "Customer Site";

    /// <summary>
    /// ABCFieldName
    /// </summary>
    public const string ABCFieldName = "ABCInd";

    /// <summary>
    /// CustomerName
    /// </summary>
    public const string CustomerName = "Customer";

    /// <summary>
    /// WorkCenterFieldName
    /// </summary>
    public const string WorkCenterFieldName = "Work Center";

    /// <summary>
    /// All
    /// </summary>
    public const string All = "*";

    /// <summary>
    /// ServiceName
    /// </summary>
    public const string ServiceName = "NetCa";

    /// <summary>
    /// CustomerSite
    /// </summary>
    public const string CustomerSite = "Customer Site";

    #endregion UserAttribute

    #region  MsTeams

    /// <summary>
    /// MsTeamsServiceName
    /// </summary>
    public const string MsTeamsServiceName = "NetCa";

    /// <summary>
    /// MsTeamsServiceDomain
    /// </summary>
    public const string MsTeamsServiceDomain = "netca.dev-aks.unitedtractors.com";

    /// <summary>
    /// MsTeamsImageWarning
    /// </summary>
    public const string MsTeamsImageWarning = "https://image.flaticon.com/icons/png/512/1537/premium/1537854.png";

    /// <summary>
    /// MsTeamsImageError
    /// </summary>
    public const string MsTeamsImageError = "https://image.flaticon.com/icons/png/512/2100/premium/2100813.png";

    /// <summary>
    /// MsTeamsSummary
    /// </summary>
    public const string MsTeamsSummaryError = "Someting wrong";

    /// <summary>
    /// MsTeamsactivitySubtitleStart
    /// </summary>
    public const string MsTeamsactivitySubtitleStart = "Application has started";

    /// <summary>
    /// MsTeamsactivitySubtitleStop
    /// </summary>
    public const string MsTeamsactivitySubtitleStop = "Application has stopped";

    /// <summary>
    /// MsTeamsThemeColorError
    /// </summary>
    public const string MsTeamsThemeColorError = "#eb090d";

    /// <summary>
    /// MsTeamsThemeColorWarning
    /// </summary>
    public const string MsTeamsThemeColorWarning = "#f7db05";

    /// <summary>
    /// MsTeamsMaxSizeInBytes
    /// </summary>
    public const int MsTeamsMaxSizeInBytes = 5_242_880;

    #endregion MsTeams

    #region  healthcheck

    /// <summary>
    /// DefaultHealthCheckQuery
    /// </summary>
    public const string DefaultHealthCheckQuery = "SELECT \"Id\" From \"Changelogs\" Limit 1";

    /// <summary>
    /// DefaultHealthCheckTimeoutInSeconds
    /// </summary>
    public const int DefaultHealthCheckTimeoutInSeconds = 60;

    /// <summary>
    /// DefaultHealthCheckCpuUsage
    /// </summary>
    public const string DefaultHealthCheckCpuUsage = "CpuUsage";

    /// <summary>
    /// DefaultHealthCheckMemoryUsage
    /// </summary>
    public const string DefaultHealthCheckMemoryUsage = "Memory";

    /// <summary>
    /// DefaultHealthCheckDatabaseName
    /// </summary>
    public const string DefaultHealthCheckDatabaseName = "DB";

    /// <summary>
    /// DefaultHealthCheckUmsName
    /// </summary>
    public const string DefaultHealthCheckUmsName = "UserManagementService";

    /// <summary>
    /// DefaultHealthCheckMLName
    /// </summary>
    public const string DefaultHealthCheckMLName = "MachineLearningService";

    /// <summary>
    /// DefaultHealthCheckGateWayName
    /// </summary>
    public const string DefaultHealthCheckGateWayName = "Gateway";

    /// <summary>
    /// DefaultHealthCheckKafkaName
    /// </summary>
    public const string DefaultHealthCheckKafkaName = "Kafka";

    /// <summary>
    /// DefaultHealthCheckEventHub
    /// </summary>
    public const string DefaultHealthCheckEventHub = "EventHub";

    /// <summary>
    /// DefaultHealthCheckEventHubName
    /// </summary>
    public const string DefaultHealthCheckEventHubName = "EventHub";

    /// <summary>
    /// DefaultHealthCheckRedisName
    /// </summary>
    public const string DefaultHealthCheckRedisName = "Redis";

    /// <summary>
    /// DefaultHealthCheckPercentageUsedDegraded
    /// </summary>
    public const byte DefaultHealthCheckPercentageUsedDegraded = 90;
    #endregion healthcheck

    #region Filter&SortSeparator

    /// <summary>
    /// Comma Separator
    /// </summary>
    public const string EscapedCommaPattern = @"(?<!($|[^\\])(\\\\)*?\\),";

    /// <summary>
    /// Pipe Separator
    /// </summary>
    public const string EscapedPipePattern = @"(?<!($|[^\\])(\\\\)*?\\)\|";

    /// <summary>
    /// Comma Separator
    /// </summary>
    public static readonly string[] Operators =
    {
        "!@=*",
        "!_=*",
        "!=*",
        "!@=",
        "!_=",
        "==*",
        "@=*",
        "_=*",
        "==",
        "!=",
        ">=",
        "<=",
        ">",
        "<",
        "@=",
        "_=",
        "="
    };
    #endregion

    #region EngineSystem

    /// <summary>
    /// Max data
    /// </summary>
    public const int MaxData = 300;

    /// <summary>
    /// WaningMinPercentage
    /// </summary>
    public const int WaningMinPercentage = 40;

    /// <summary>
    /// WaningMaxPercentage
    /// </summary>
    public const int WaningMaxPercentage = 80;

    #endregion

    #region  excelreport

    /// <summary>
    /// ExcelWorksheet
    /// </summary>
    public const string ExcelWorksheet = "Summary Report";

    /// <summary>
    /// ExcelFileName
    /// </summary>
    public const string ExcelFileName = "DAD";

    /// <summary>
    /// ExcelFileNameSecond
    /// </summary>
    public const string ExcelFileNameSecond = "_REPORT_";

    /// <summary>
    /// ExcelExt
    /// </summary>
    public const string ExcelExt = ".xlsx";

    /// <summary>
    /// DateFormat
    /// </summary>
    public const string DateFormat = "yyyy/MM/dd";

    /// <summary>
    /// TimeFormat
    /// </summary>
    public const string TimeFormat = "HH:mm:ss";

    /// <summary>
    /// Blank
    /// </summary>
    public const string Blank = "-";

    #endregion

    #region  validation

    /// <summary>
    /// Maximum Length Base64 in byte
    /// </summary>
    public const int MaximumLengthBase64 = 512 * 1024;

    /// <summary>
    /// Maximum File Size in byte
    /// </summary>
    public const int MaximumFileSize = 3072 * 1024;

    #endregion

    #region  eventhubs

    /// <summary>
    /// EventHub
    /// </summary>
    public const string EventHub = "dad";

    /// <summary>
    /// EventHubNameFileProcessing
    /// </summary>
    public const string EventHubName = "dad_netca";

    /// <summary>
    /// ConsumerList
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (string, bool)> ConsumerJobList = new Dictionary<string, (string, bool)>
    {
    };

    #endregion

    #region  dashboard

    /// <summary>
    /// DateNotUpdateInDays
    /// </summary>
    public const byte DateNotUpdateInDays = 14;

    #endregion

    /// <summary>
    /// DefaultTotalMaxProcess
    /// </summary>
    public const byte DefaultTotalMaxProcess = 25;

    /// <summary>
    /// File Extensions
    /// </summary>
    public static readonly IReadOnlyList<string> FileExtensions = new List<string>
    {
        "csv",
        "xls",
        "xlsx"
    };
}