using AutoMapper;
using Moq;
using PropostaService.Domain.Ports.Messaging;
using PropostaService.Domain.Ports.Repositories;

namespace PropostaService.UnitTests.Application.UseCases
{
    public abstract class UseCaseTestBase
    {
        protected readonly Mock<IPropostaRepository> RepositoryMock;
        protected readonly Mock<IMessageBus> MessageBusMock;
        protected readonly Mock<IMapper> MapperMock;

        protected UseCaseTestBase()
        {
            RepositoryMock = new Mock<IPropostaRepository>();
            MessageBusMock = new Mock<IMessageBus>();
            MapperMock = new Mock<IMapper>();
        }
    }
}