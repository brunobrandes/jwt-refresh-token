using System;
using System.Threading;
using FluentAssertions;
using Jwt.Refresh.Token.Application;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Moq;
using Xunit;

namespace Jwt.Refresh.Token.Tests.Unit.Application
{
    public class TokenAppServiceTests
    {
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;

        private readonly ITokenAppService _tokenAppService;

        private readonly string _userId;
        private readonly string _password;
        private readonly string _ipAddress;

        public TokenAppServiceTests()
        {
            
            _tokenRepositoryMock = new Mock<ITokenRepository> { };
            _userRepositoryMock = new Mock<IUserRepository> { };
            _jwtTokenServiceMock = new Mock<IJwtTokenService> { };

            _tokenAppService = new TokenAppService(_tokenRepositoryMock.Object,
                _userRepositoryMock.Object, _jwtTokenServiceMock.Object);

            _userId = "test@bs3.dev";
            _password = "password_test";
            _ipAddress = "127.0.0.1";
        }

        [Fact]
        public async Task Create_Token_ReturnsAuhtorized()
        {
            var cancellationToken = default(CancellationToken);

            _userRepositoryMock.Setup(x => x.GetUserIdByIdAndPasswordAsync(_userId, _password, cancellationToken))
                .ReturnsAsync(_userId);

            _jwtTokenServiceMock.Setup(x => x.GetAccessTokenAsync(_userId, MillisecondsConversion.Minute,
                cancellationToken))
                .ReturnsAsync("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJ1c2VyX3Rlc3RAZ21haWwuY29tIn0.R7TGeX7_PJxXfKuojZmgitxlMCdxNQ2MoEuKww6zr4s");

            _tokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Token>(), cancellationToken));

            var token = await _tokenAppService.CreateAsync(_userId, _password, MillisecondsConversion.Minute, _ipAddress,
                cancellationToken);

            token.Status.Should().Be(Domain.Enums.TokenStatus.Authorized);
        }

        [Fact]
        public async Task Create_Token_UserIdOrPasswordInvalid_ReturnsUnauhtorized()
        {
            var token = await _tokenAppService.CreateAsync(string.Empty, string.Empty, MillisecondsConversion.Minute, _ipAddress,
                default(CancellationToken));

            token.Status.Should().Be(Domain.Enums.TokenStatus.Unauthorized);
        }

        [Fact]
        public async Task Create_Token_UserIdNotFound_ReturnsUnauhtorized()
        {
            _userRepositoryMock.Setup(x => x.GetUserIdByIdAndPasswordAsync(_userId, _password, default(CancellationToken)))
            .ReturnsAsync(string.Empty);

            var token = await _tokenAppService.CreateAsync(_userId, _password, MillisecondsConversion.Minute, _ipAddress,
               default(CancellationToken));

            token.Status.Should().Be(Domain.Enums.TokenStatus.Unauthorized);
        }

        [Fact]
        public async Task Create_Token_UnexpectedError_ReturnsError()
        {
            _userRepositoryMock.Setup(x => x.GetUserIdByIdAndPasswordAsync(_userId, _password, default(CancellationToken)))
            .ThrowsAsync(new Exception("Unexpected error"));

            var token = await _tokenAppService.CreateAsync(_userId, _password, MillisecondsConversion.Minute, _ipAddress,
               default(CancellationToken));

            token.Status.Should().Be(Domain.Enums.TokenStatus.Error);
        }
    }
}

