using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Infrastructure.Providers;
using FluentAssertions;
using Moq;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Infrastructure.Providers
{
    public class CurrencyProviderFactoryTests
    {
        [Fact]
        public void Returns_Provider_By_Name()
        {
            var frank = new Mock<ICurrencyProvider>();
            frank.SetupGet(x => x.Name).Returns("Frankfurter");

            var other = new Mock<ICurrencyProvider>();
            other.SetupGet(x => x.Name).Returns("Other");

            var sut = new CurrencyProviderFactory(new[] { frank.Object, other.Object });

            sut.GetProvider("Other").Should().BeSameAs(other.Object);
            sut.GetProvider("Frankfurter").Should().BeSameAs(frank.Object);
        }

        [Fact]
        public void Falls_Back_To_Frankfurter()
        {
            var frank = new Mock<ICurrencyProvider>();
            frank.SetupGet(x => x.Name).Returns("Frankfurter");

            var sut = new CurrencyProviderFactory(new[] { frank.Object });
            sut.GetProvider("Unknown").Should().BeSameAs(frank.Object);
        }

        [Fact]
        public void Throws_If_No_Providers()
        {
            var sut = new CurrencyProviderFactory(Array.Empty<ICurrencyProvider>());
            Action act = () => sut.GetProvider("Anything");
            act.Should().Throw<NotSupportedException>();
        }
    }
}
