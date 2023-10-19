// -----------------------------------------------------------------------------------
// MockData.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetCa.Domain.Entities;

namespace NetCa.Application.Common.Models;

/// <summary>
/// MockData
/// </summary>
public static class MockData
{
    /// <summary>
    /// GetUserAttribute
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, List<string>> GetUserAttribute()
    {
        var result = new Dictionary<string, List<string>>();
        var customers = new List<string> { Constants.All };
        var plants = new List<string> { Constants.All };
        var customerSites = new List<string> { Constants.All };
        var workCenters = new List<string> { Constants.All };
        var abcInds = new List<string> { Constants.All };
        result.Add(Constants.CustomerName, customers);
        result.Add(Constants.PlantFieldName, plants);
        result.Add(Constants.CustomerSiteFieldName, customerSites);
        result.Add(Constants.WorkCenterFieldName, workCenters);
        result.Add(Constants.ABCFieldName, abcInds);
        return result;
    }

    /// <summary>
    /// GetUnitModelsOrCodes
    /// </summary>
    /// <returns></returns>
    public static string[] GetUnitModelsOrCodes()
    {
        return new[]
        {
            "HD785-7",
            "PC2000-8",
            "HD1500-7",
            "HD785-5",
            "GD825A-2",
            "PC3000-6",
            "PC4000-6",
            "PC3000-6E",
            "P420CB-8X4",
            "P360CB-6X4",
            "P360LA-6X4",
            "P410CB-8X4",
            "P460LA-6x4",
            "R580LA-6X4"
        };
    }

    /// <summary>
    /// Get Changelogs data Examples
    /// </summary>
    /// <returns></returns>
    public static List<Changelog> GetChangelogs()
    {
        var changelogs = new List<Changelog>();
        var mockGuids = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        };
        int sequence = 1;

        foreach (var guid in mockGuids)
        {
            changelogs.Add(new Changelog
            {
                Id = guid,
                Method = GetDbMethod(),
                KeyValues = "Value",
                NewValues = "Value",
                OldValues = "Value",
                TableName = "Table",
                ChangeDate = DateTime.UtcNow.AddMonths(-1 * sequence)
            });
            sequence += 1;
        }

        return changelogs;
    }

    /// <summary>
    /// Get Db Method Exmaple
    /// </summary>
    /// <returns></returns>
    public static string GetDbMethod()
    {
        string[] types =
        {
            "DELETE",
            "EDIT",
            "ADD"
        };
        var random = new Random();
        return types[random.Next(0, 2)];
    }

    /// <summary>
    /// GetAuthorizedUser
    /// </summary>
    /// <returns></returns>
    public static AuthorizedUser GetAuthorizedUser()
    {
        return new AuthorizedUser
        {
            UserId = Guid.NewGuid(),
            UserName = Constants.SystemEmail,
            UserFullName = Constants.SystemName,
            CustomerCode = Constants.All,
            ClientId = Constants.SystemClientId,
            RoleLevel = 0
        };
    }

    /// <summary>
    /// GetUserEmailInfo
    /// </summary>
    /// <returns></returns>
    public static UserEmailInfo GetUserEmailInfo()
    {
        return new UserEmailInfo
        {
            Email = Constants.SystemEmail,
            FirstName = Constants.SystemName,
            LastName = Constants.SystemName
        };
    }

    /// <summary>
    /// GetUserByAttributeAsync
    /// </summary>
    /// <returns></returns>
    public static List<UserClientIdInfo> GetUserByAttribute()
    {
        return new List<UserClientIdInfo>
        {
            new ()
            {
                UserId = new Guid("49f140da-707a-459e-4e60-08d708dc37c0"),
                ClientId = "b1a48ae8-3c71-4ae4-4e7b-08d708dc37c0",
                IsCustomer = false
            },
            new ()
            {
                UserId = new Guid("b1a48ae8-3c71-4ae4-4e7b-08d708dc37c0"),
                ClientId = "3ea6f2ac-92aa-4a4c-8725-daa9350be5c8",
                IsCustomer = false
            },
            new ()
            {
                UserId = new Guid("c8374c2f-08f7-4156-4e75-08d708dc37c0"),
                ClientId = "3ea6f2ac-92aa-4a4c-8725-daa9350be5c8",
                IsCustomer = false
            },
            new ()
            {
                UserId = new Guid("6178aba0-dd0a-4615-4e7e-08d708dc37c0"),
                ClientId = "3ea6f2ac-92aa-4a4c-8725-daa9350be5c8",
                IsCustomer = false
            },
            new ()
            {
                UserId = new Guid("647c1e9c-6fe4-4800-25d7-08d771620e86"),
                ClientId = "3ea6f2ac-92aa-4a4c-8725-daa9350be5c8",
                IsCustomer = false
            }
        };
    }

    /// <summary>
    /// GetAuthorizedUser
    /// </summary>
    /// <returns></returns>
    public static List<UserManagementUser> GetListMechanics()
    {
        var result = new List<UserManagementUser>
        {
            new ()
            {
                UserId = new Guid("49f140da-707a-459e-4e60-08d708dc37c0"),
                UserName = "80107002@unitedtractors.com",
                Email = "80107002@unitedtractors.com",
                FirstName = "Agus",
                LastName = "LISANTO"
            },
            new ()
            {
                UserId = new Guid("b1a48ae8-3c71-4ae4-4e7b-08d708dc37c0"),
                UserName = "70316028@unitedtractors.com",
                Email = "inengahsuarma54@gmail.com",
                FirstName = "I NENGAH",
                LastName = "I NENGAH"
            },
            new ()
            {
                UserId = new Guid("c8374c2f-08f7-4156-4e75-08d708dc37c0"),
                UserName = "81300006@unitedtractors.com",
                Email = "81300006@unitedtractors.com",
                FirstName = "HENDRAWAN",
                LastName = "HENDRAWAN"
            },
            new ()
            {
                UserId = new Guid("6178aba0-dd0a-4615-4e7e-08d708dc37c0"),
                UserName = "80101070@unitedtractors.com",
                Email = "saiful.ongisnade2016@gmail.com",
                FirstName = "SAIFUL",
                LastName = "HADI"
            }
        };

        return result;
    }

    /// <summary>
    /// RandomNum
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RandomNum(int min, int max)
    {
        var random = new Random();
        return random.Next(min, max);
    }

    /// <summary>
    /// RandomUnitModel
    /// </summary>
    /// <returns></returns>
    public static string RandomUnitModel()
    {
        var names = GetUnitModelsOrCodes();
        var random = new Random();
        return names[random.Next(0, names.Length)];
    }

    /// <summary>
    /// RandomString
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string RandomString(int length = 50)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// RandomTimeSpan
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static TimeSpan RandomTimeSpan(int length = 86400)
    {
        return new TimeSpan(0, 0, 0, new Random().Next(length));
    }

    /// <summary>
    /// GetWorkCenters
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, string>> GetWorkCenters()
    {
        return new List<KeyValuePair<string, string>>
        {
            new ("M-ADRTP", "FMC Adaro Tutupan"),
            new ("FM-BDIPM", "FMC Bendili PAMA"),
            new ("FM-BIUPM", "FMC Batukajang PAMA"),
            new ("FM-BKJBU", "FMC Batukajang BUMA"),
            new ("FM-BKJSJ", "FMC Batukajang SIMS"),
            new ("FM-BNEPM", "FMC Bontang East Block Site - KITADIN"),
            new ("FM-DMIPM", "FMC Damai Pama"),
            new ("FM-JBYPM", "FMC Jembayan PAMA"),
            new ("FM-JKTMB", "FMC Jakarta On Road Mayasari Bhakti"),
            new ("FM-JKTTJ", "FMC Jakarta On Road Trans Jakarta"),
            new ("FM-LTIBU", "FMC Lati Buma"),
            new ("FM-MLWSM", "FMC Muaralawa SIMS"),
            new ("FM-SRKVL", "FMC Soroako"),
            new ("FM-TJGBU", "FMC Tanjung Buma"),
            new ("FM-TJGSI", "FMC Tanjung Sis"),
            new ("FM-BGLKP", "FMC Bengalon KPP"),
            new ("FM-BGLKW", "FMC Bengalon KWN"),
            new ("FM-MTBPM", "FMC MTBU Pama"),
            new ("FM-TBGKW", "FMC Tabang KWN")
        };
    }

    /// <summary>
    /// Fill
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="value"></param>
    /// <param name="count"></param>
    public static void Fill(this Stream stream, byte value, int count)
    {
        var buffer = new byte[64];

        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = value;

        while (count > buffer.Length)
        {
            stream.Write(buffer, 0, buffer.Length);
            count -= buffer.Length;
        }

        stream.Write(buffer, 0, count);
    }
}