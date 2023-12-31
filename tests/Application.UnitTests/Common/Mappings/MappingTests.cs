﻿using System;
using System.Runtime.Serialization;
using AutoMapper;
using FluentAssertions;
using NetCa.Application.Common.Mappings;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(null, null)]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            if (source is null || destination is null)
            {
                return;
            }

            var test = _mapper.Map(GetInstanceOf(source), source, destination);
            test.Should().BeAssignableTo(destination);
        }

        private static object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);

            return FormatterServices.GetUninitializedObject(type);
        }
    }
}