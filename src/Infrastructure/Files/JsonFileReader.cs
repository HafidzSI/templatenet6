// -----------------------------------------------------------------------------------
// JsonFileReader.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Extensions;
using Newtonsoft.Json;

namespace NetCa.Infrastructure.Files;

/// <summary>
/// JsonFileReader
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonFileReader<T>
    where T : class
{
    private static readonly ILogger _logger = AppLoggingExtensions.CreateLogger<JsonFileReader<T>>();

    private readonly string _ext = ".json";
    private readonly string _fileName;
    private readonly string _currentDirectory = Path.Combine(Path.GetDirectoryName(typeof(DepedencyInjection).Assembly.Location));

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileReader{T}"/> class.
    /// </summary>
    /// <param name="fileName"></param>
    public JsonFileReader(string fileName)
    {
        _fileName = fileName;

        if (!_fileName.ToLower().Contains(_ext))
            _fileName = $"{_fileName}{_ext}";
    }

    private bool IsFileExist()
    {
        var path = Path.Combine(_currentDirectory, _fileName);
        var fileInfo = new FileInfo(path);

        return fileInfo.Exists;
    }

    /// <summary>
    /// Read data
    /// </summary>
    /// <returns></returns>
    public HashSet<T> Read()
    {
        var data = new HashSet<T>();
        var isExist = IsFileExist();

        _logger.LogDebug("File {fileName} is exist {isExist}", _fileName, isExist);

        if (isExist)
        {
            try
            {
                var path = Path.Combine(_currentDirectory, _fileName);

                using var streamReader = new StreamReader(path);
                var json = streamReader.ReadToEnd();

                data = JsonConvert.DeserializeObject<HashSet<T>>(json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when parsing json file '{name}': {message}", _fileName, e.Message);
            }
        }

        return data;
    }
}