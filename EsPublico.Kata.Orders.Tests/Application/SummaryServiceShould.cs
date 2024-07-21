using EsPublico.Kata.Orders.Application;
using EsPublico.Kata.Orders.Domain;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
using FluentAssertions;
using LanguageExt;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EsPublico.Kata.Orders.Tests.Application;

public class SummaryServiceShould
{
    private readonly FilesRepository _filesRepository;
    private readonly SummaryService _service;
    private readonly SummaryRepository _summaryRepository;

    public SummaryServiceShould()
    {
        _summaryRepository = Substitute.For<SummaryRepository>();
        _filesRepository = Substitute.For<FilesRepository>();
        var logger = Substitute.For<ILogger<SummaryService>>();
        _service = new SummaryService(_summaryRepository, _filesRepository, logger);
    }

    [Fact]
    public async Task Summary()
    {
        var summary = new List<SummaryRecord>();
        _summaryRepository.GetSummary().Returns(Either<Error, List<SummaryRecord>>.Right(summary));
        _filesRepository.Save(summary).Returns(Either<Error, Unit>.Right(Unit.Default));

        await _service.Summary();

        await _summaryRepository.Received(1).GetSummary();
        await _filesRepository.Received(1).Save(summary);
    }

    [Fact]
    public async Task StopIfErrorGettingSummary()
    {
        var error = new Error("Some error getting summary");
        _summaryRepository.GetSummary().Returns(Either<Error, List<SummaryRecord>>.Left(error));

        var act = async () => await _service.Summary();

        await act.Should().ThrowAsync<Exception>().WithMessage("Error getting summary");
    }
}