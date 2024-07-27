using AutoFixture;
using DDB.HealthCycle.DataAccess.DateTimeProvider;
using DDB.HealthCycle.DataAccess.PlayerCharacters;
using DDB.HealthCycle.Models.DTO;
using Microsoft.Extensions.Logging;
using Moq;

namespace DDB.HealthCycle.Logic.PcHealth.Tests;

// ToDo: come back and add testing for expected exceptions to be surfaced when thrown
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.All)]
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
    public async Task GetPlayerCharacterAsync_ReturnsResultFromRepo()
    {
        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.GetPlayerCharacterAsync(playerCharacterFixture.Id);

        Assert.That(result, Is.EqualTo(playerCharacterFixture));

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
    }

    [Test()]
    public async Task HealAsync_UpdatesHitPoints()
    {
        var expectedHp = 30;
        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.HealAsync(playerCharacterFixture.Id, 5);

        Assert.That(result, Is.Not.Null);
        AssertHitPointDataIsAccurate(
                result,
                expectedHp,
                playerCharacterFixture.HitPoints.Temp);

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(
            p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(
                p, expectedHp, playerCharacterFixture.HitPoints.Temp))),
            Times.Once);
    }

    [Test()]
    public async Task HealAsync_RespectsMaximumHp()
    {
        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.HealAsync(playerCharacterFixture.Id, playerCharacterFixture.HitPoints.Max + 1);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            // This assertion is outside of the scope of this unit but helps ensure that there isn't any regression
            Assert.That(result?.Temp, Is.EqualTo(playerCharacterFixture.HitPoints.Temp));
            Assert.That(result?.Current, Is.EqualTo(playerCharacterFixture.HitPoints.Max));
        });

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(
            p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(
                p, 
                playerCharacterFixture.HitPoints.Max,
                playerCharacterFixture.HitPoints.Temp)))
            , Times.Once);
    }

    [Test()]
    public async Task HealAsync_ReturnsNullWhenUpsertFails()
    {
        _pcRepoMock.Setup(p => p.UpsertPlayerCharacterAsync(It.IsAny<PlayerCharacter>()))
            .ReturnsAsync(false);

        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.HealAsync(playerCharacterFixture.Id, 1);

        Assert.That(result, Is.Null);

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(
            p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(
                p,
                playerCharacterFixture.HitPoints.Current,
                playerCharacterFixture.HitPoints.Temp))),
            Times.Once);
    }

    [Test()]
    public async Task AddTempHpAsync_OnlyEffectsTempHp()
    {
        playerCharacterFixture.HitPoints.Temp = 0;
        var tempHp = _fixture.Create<int>();

        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.AddTempHpAsync(playerCharacterFixture.Id, tempHp);

        Assert.That(result, Is.Not.Null);
        AssertHitPointDataIsAccurate(
                result,
                playerCharacterFixture.HitPoints.Current,
                tempHp);

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(
            p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(
                p,
                playerCharacterFixture.HitPoints.Current,
                tempHp))),
            Times.Once);
    }

    [Test()]
    [TestCase(5, 10, 10)]
    [TestCase(0, 20, 20)]
    [TestCase(10, 10, 10)]
    [TestCase(10, 5, 10)]
    public async Task AddTempHpAsync_OnlyTakesTheHighestTempValue(int existingTempHp, int incomingTempHp, int expectedTempHp)
    {
        playerCharacterFixture.HitPoints.Temp = existingTempHp;

        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.AddTempHpAsync(playerCharacterFixture.Id, incomingTempHp);

        Assert.That(result, Is.Not.Null);
        AssertHitPointDataIsAccurate(
                result,
                playerCharacterFixture.HitPoints.Current,
                expectedTempHp);

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(
            p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(
                p,
                playerCharacterFixture.HitPoints.Current,
                expectedTempHp))),
            Times.Once);
    }

    [Test()]
    public async Task AddTempHpAsync_ReturnsNullWhenUpsertFails()
    {
        playerCharacterFixture.HitPoints.Temp = 0;
        _pcRepoMock.Setup(p => p.UpsertPlayerCharacterAsync(It.IsAny<PlayerCharacter>()))
            .ReturnsAsync(false);

        var pcHealthManager = GetSUT();

        var result = await pcHealthManager.AddTempHpAsync(playerCharacterFixture.Id, 1);

        Assert.That(result, Is.Null);

        _pcRepoMock.Verify(p => p.GetCharacterByIdAsync(playerCharacterFixture.Id), Times.Once);
        _pcRepoMock.Verify(
            p => p.UpsertPlayerCharacterAsync(It.Is<PlayerCharacter>(p => VerifyPlayerCharacterData(
                p,
                playerCharacterFixture.HitPoints.Current,
                1))),
            Times.Once);
    }

    private PcHealthManager GetSUT()
    {
        return new(
            _pcRepoMock.Object,
            _loggerMock.Object,
            _dateTimeProviderMock.Object
        );
    }

    private bool AssertHitPointDataIsAccurate(PlayerCharacterHealthStats? healthStats, int expectedCurrentHp, int expectedTempHp)
    {
        Assert.That(healthStats, Is.Not.Null);
        Assert.Multiple(() =>
        {
            // HP Deep Verification
            Assert.That(healthStats?.NonLeathal, Is.EqualTo(playerCharacterFixture.HitPoints.NonLeathal));
            Assert.That(healthStats?.Max, Is.EqualTo(playerCharacterFixture.HitPoints.Max));
            Assert.That(healthStats?.Temp, Is.EqualTo(expectedTempHp));
            Assert.That(healthStats?.Current, Is.EqualTo(expectedCurrentHp));
        });
        return true;
    }

    private bool VerifyPlayerCharacterData(PlayerCharacter character, int expectedCurrentHp, int expectedTempHp)
    {
        Assert.Multiple(() =>
        {
            Assert.That(character.Id, Is.EqualTo(playerCharacterFixture.Id));
            Assert.That(character.Defenses, Is.EquivalentTo(playerCharacterFixture.Defenses));
            Assert.That(character.Classes, Is.EquivalentTo(playerCharacterFixture.Classes));
            Assert.That(character.Level, Is.EqualTo(playerCharacterFixture.Level));
            Assert.That(character.Name, Is.EqualTo(playerCharacterFixture.Name));
            Assert.That(character.Stats, Is.EqualTo(playerCharacterFixture.Stats));
            AssertHitPointDataIsAccurate(character.HitPoints, expectedCurrentHp, expectedTempHp);
        });
        return true;
    }
}
