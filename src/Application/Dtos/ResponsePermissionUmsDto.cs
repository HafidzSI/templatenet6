using System;
using System.Collections.Generic;

namespace NetCa.Application.Dtos;

#pragma warning disable
public record ResponsePermissionUmsVm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ResponsePermissionUmsDto> ResponsePermissionDtos { get; set; }
}

public record ResponsePermissionUmsDto
{
    public string Name { get; set; }
    public string Status { get; set; }
}
