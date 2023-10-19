using FluentAssertions;
using NetCa.Application.Common.Exceptions;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Exceptions
{
    public class NotFoundExceptionTests
    {
        [Test]
        public void DefaultConstructorCreatesADefaultErrorMessage()
        {
            var actual = new NotFoundException();

            actual.Message.Should().BeEquivalentTo("Exception of type 'NetCa.Application.Common.Exceptions.NotFoundException' was thrown.");
        }

        [Test]
        [TestCase("Url Not Found")]
        public void DefaultConstructorCreatesAnExceptionWithMessage(string message)
        {
            var actual = new NotFoundException(message);

            actual.Message.Should().BeEquivalentTo(message);
        }

        [Test]
        [TestCase("Changelog", "Id")]
        public void DefaultConstructorCreatesAnExceptionWithMessage(string name, string key)
        {
            var actual = new NotFoundException(name, key);

            actual.Message.Should().BeEquivalentTo($"Entity \"{name}\" ({key}) was not found.");
        }
    }
}
