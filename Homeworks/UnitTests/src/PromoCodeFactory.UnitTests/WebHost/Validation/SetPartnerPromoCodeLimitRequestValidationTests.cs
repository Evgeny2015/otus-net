using AutoFixture.AutoMoq;
using AutoFixture;
using FluentAssertions;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using Xunit;
using PromoCodeFactory.DataAccess.Validation;
using PromoCodeFactory.UnitTests.Data;
using FluentAssertions.Extensions;

namespace PromoCodeFactory.UnitTests.WebHost.Validation
{
    public class SetPartnerPromoCodeLimitRequestValidationTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly IFixture _fixture;

        public SetPartnerPromoCodeLimitRequestValidationTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitRequestValidation_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = TestDataRepository.PartnerId;
            _fixture.Register(() => partnerId);
            _fixture.Register(() => TestDataRepository.SetPartnerPromoCodeLimitRequest);

            Partner partner = null;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestValidation = _fixture
                .Build<SetPartnerPromoCodeLimitRequestValidation>()
                .Create();

            // Act
            var result = await requestValidation.Validate();

            // Assert
            result.Status.Should().Be(ValidationStatus.NotFound);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitRequestValidation_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = TestDataRepository.PartnerId;
            _fixture.Register(() => TestDataRepository.PartnerId);
            _fixture.Register(() => TestDataRepository.SetPartnerPromoCodeLimitRequest);

            var partner = TestDataRepository.Partner;
            partner.IsActive = false;
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestValidation = _fixture
                .Build<SetPartnerPromoCodeLimitRequestValidation>()
                .Create();

            // Act
            var result = await requestValidation.Validate();

            // Assert
            result.Status.Should().Be(ValidationStatus.BadRequest);
            result.ErrorMessage.Should().Be(SetPartnerPromoCodeLimitRequestValidation.PartnerInNotActive);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitRequestValidation_IfSetLimit_PromocodeNumberShouldBeZero()
        {
            // Arrange
            var partnerId = TestDataRepository.PartnerId;
            _fixture.Register(() => TestDataRepository.PartnerId);
            _fixture.Register(() => TestDataRepository.SetPartnerPromoCodeLimitRequest);

            var partner = TestDataRepository.Partner;
            partner.NumberIssuedPromoCodes = 1;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestValidation = _fixture
                .Build<SetPartnerPromoCodeLimitRequestValidation>()
                .Create();

            // Act
            var result = await requestValidation.Validate();

            // Assert
            result.Status.Should().Be(ValidationStatus.Success);
            partner.NumberIssuedPromoCodes.Should().Be(0);       
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitRequestValidation_IfSetLimit_CencelDateShouldBeCloseToNow()
        {
            // Arrange
            var partnerId = TestDataRepository.PartnerId;
            _fixture.Register(() => TestDataRepository.PartnerId);
            _fixture.Register(() => TestDataRepository.SetPartnerPromoCodeLimitRequest);

            var partner = TestDataRepository.Partner;
            partner.NumberIssuedPromoCodes = 1;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestValidation = _fixture
                .Build<SetPartnerPromoCodeLimitRequestValidation>()
                .Create();

            // Act
            var result = await requestValidation.Validate();

            // Assert
            result.Status.Should().Be(ValidationStatus.Success);
            partner.PartnerLimits.First().CancelDate
                .Should().BeCloseTo(DateTime.Now, 2.Seconds());
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitRequestValidation_NegativeLimit_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = TestDataRepository.PartnerId;
            _fixture.Register(() => TestDataRepository.PartnerId);
            _fixture.Register(() => {
                var request = TestDataRepository.SetPartnerPromoCodeLimitRequest;
                request.Limit = -1;

                return request;
                });

            var partner = TestDataRepository.Partner;            
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var requestValidation = _fixture
                .Build<SetPartnerPromoCodeLimitRequestValidation>()
                .Create();

            // Act
            var result = await requestValidation.Validate();

            // Assert
            result.Status.Should().Be(ValidationStatus.BadRequest);
            result.ErrorMessage.Should().Be(SetPartnerPromoCodeLimitRequestValidation.LimitShouldBeGreaterThenZero);
        }
    }
}
