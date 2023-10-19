// -----------------------------------------------------------------------------------
// ExcelExtensions.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using NetCa.Application.Common.Exceptions;
using NetCa.Application.Common.Models;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// Excel Extension
/// </summary>
public static class ExcelExtensions
{
    /// <summary>
    /// Read Xlsx
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<DataTable> FromXlsx(this IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        using var wb = new XLWorkbook(stream);

        var dataTable = new DataTable();
        var firstRow = true;

        foreach (var row in wb.Worksheet(1).Rows())
        {
            if (row.IsEmpty())
                row.Cells().Clear();

            if (!row.Cells().Any())
                break;

            if (firstRow)
            {
                foreach (var cell in row.Cells())
                {
                    dataTable.Columns.Add(cell.Value.ToString());
                }

                firstRow = false;
            }
            else
            {
                dataTable.Rows.Add();
                var i = 0;

                for (var j = 1; j <= dataTable.Columns.Count; j++)
                {
                    var activeCell = $"{(char)(Constants.CustomRuleAsciiAlphabet + j)}{dataTable.Rows.Count + 1}";

                    var combinePattern = Constants.RegexPattern.Replace(
                        "{pattern}",
                        Constants.RegexChar + Constants.RegexNumeric + " " + Constants.RegexSymbol);
                    var pattern = new Regex(combinePattern);

                    if (!string.IsNullOrWhiteSpace(row.Cells(activeCell).Select(x => x).FirstOrDefault()?.Value.ToString()))
                    {
                        var isMatch = pattern.IsMatch(
                            row.Cells(activeCell).Select(x => x).FirstOrDefault()?.Value.ToString() ?? string.Empty);

                        if (!isMatch)
                            throw new BadRequestException($"Only character [{Constants.RegexChar}], numeric [{Constants.RegexNumeric}], and symbol [{Constants.RegexSymbol}] can be accepted in cell {activeCell}");
                    }

                    dataTable.Rows[^1][i] = row.Cells(activeCell).Select(x => x).FirstOrDefault()?.Value.ToString() ?? string.Empty;

                    i++;
                }
            }
        }

        stream.Close();

        return dataTable;
    }
}