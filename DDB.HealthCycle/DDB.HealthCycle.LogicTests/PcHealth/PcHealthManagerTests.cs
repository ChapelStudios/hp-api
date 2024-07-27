using AutoFixture;
using DDB.HealthCycle.DataAccess.DateTimeProvider;
using DDB.HealthCycle.DataAccess.PlayerCharacters;
using DDB.HealthCycle.Models.DTO;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DDB.HealthCycle.Logic.PcHealth.Tests;

// ToDo: come back and add testing for expected exceptions to be surfaced when thrown
[TestFixture()]
public class PcHealthManagerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPlayerCharacterRepo> _pcRepoMock = new();
    private readonly Mock<ILogger<PcHealthManager>> _loggerMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    private readonly PlayerCharacter playerCharacterFixture;

    public PcHealthManagerTests()
    {
        playerCharacterFixture = _fixture.Create<PlayerCharacter>();
        playerCharacterFixture.HitPoints.Max = 50;
        playerCharacterFixture.HitPoints.Current = 25;

        _pcRepoMock.Setup(p => p.GetCharacterByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(playerCharacterFixture);

        _pcRepoMock.Setup(p => p.UpsertPlayerCharacterAsync(It.IsAny<PlayerCharacter>()))
            .ReturnsAsync(true);
    }

    [Test()]
    public async Task GetPlayerCharacter_ReturnsResultFromRepo()
    {
        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.GetPlayerCharacterAsync(playerCharacterFixture.Id);

        Assert.That(result, Is.EqualTo(playerCharacterFixture));

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
    }

    [Test()]
    public async Task Heal_UpdatesHitPoints()
    {
        var expectedHp = 30;
        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.HealAsync(playerCharacterFixture.Id, 5);

        Assert.That(result, Is.Not.Null);
        Assert.That(result?.Temp, Is.EqualTo(playerCharacterFixture.HitPoints.Temp));
        Assert.That(result?.NonLeathal, Is.EqualTo(playerCharacterFixture.HitPoints.NonLeathal));
        Assert.That(result?.Max, Is.EqualTo(playerCharacterFixture.HitPoints.Max));
        Assert.That(result?.Current, Is.EqualTo(expectedHp));

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(p, expectedHp, playerCharacterFixture.HitPoints.Temp))), Times.Once);
    }

    private bool VerifyPlayerCharacterData(PlayerCharacter character, int expectedCurrentHp, int expectedTempHp)
    {
        Assert.That(character.Id, Is.EqualTo(playerCharacterFixture.Id));
        Assert.That(character.Defenses, Is.EquivalentTo(playerCharacterFixture.Defenses));
        Assert.That(character.Classes, Is.EquivalentTo(playerCharacterFixture.Classes));
        Assert.That(character.Level, Is.EqualTo(playerCharacterFixture.Level));
        Assert.That(character.Name, Is.EqualTo(playerCharacterFixture.Name));
        Assert.That(character.Stats, Is.EqualTo(playerCharacterFixture.Stats));
        // HP Deep Verification
        Assert.That(character.HitPoints.NonLeathal, Is.EqualTo(playerCharacterFixture.HitPoints.NonLeathal));
        Assert.That(character.HitPoints.Max, Is.EqualTo(playerCharacterFixture.HitPoints.Max));
        Assert.That(character.HitPoints.Temp, Is.EqualTo(expectedTempHp));
        Assert.That(character.HitPoints.Current, Is.EqualTo(expectedCurrentHp));

        return true;
    }

    [Test()]
    public async Task Heal_RespectsMaximumHp()
    {
        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.HealAsync(playerCharacterFixture.Id, playerCharacterFixture.HitPoints.Max + 1);

        Assert.That(result, Is.Not.Null);
        // This assertion is outside of the scope of this unit but helps ensure that there isn't any regression
        Assert.That(result?.Temp, Is.EqualTo(playerCharacterFixture.HitPoints.Temp));
        Assert.That(result?.Current, Is.EqualTo(playerCharacterFixture.HitPoints.Max));
    }

    [Test()]
    public async Task Heal_ReturnsNullWhenUpsertFails()
    {
        _pcRepoMock.Setup(p => p.UpsertPlayerCharacterAsync(It.IsAny<PlayerCharacter>()))
            .ReturnsAsync(false);

        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.HealAsync(playerCharacterFixture.Id, playerCharacterFixture.HitPoints.Max + 1);

        Assert.That(result, Is.Null);
    }

    private PcHealthManager GetSUT()
    {
        return new(
            _pcRepoMock.Object,
            _loggerMock.Object,
            _dateTimeProviderMock.Object
        );
    }
}
