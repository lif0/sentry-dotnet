namespace Sentry.Tests;

public class AppDomainProcessExitIntegrationTests
{
    private class Fixture
    {
        public IHub Hub { get; } = Substitute.For<IHub, IDisposable>();

        public IAppDomain AppDomain { get; } = Substitute.For<IAppDomain>();

        public Fixture() => Hub.IsEnabled.Returns(true);

        public AppDomainProcessExitIntegration GetSut() => new(AppDomain);
    }

    private readonly Fixture _fixture = new();

    public SentryOptions SentryOptions { get; set; } = new();

    [Fact]
    public void Handle_WithException_CaptureEvent()
    {
        var sut = _fixture.GetSut();
        sut.Register(_fixture.Hub, SentryOptions);

        sut.HandleProcessExit(this, EventArgs.Empty);

        ((IDisposable)_fixture.Hub).Received(1).Dispose();
    }

    [Fact]
    public void Register_ProcessExit_Subscribes()
    {
        var sut = _fixture.GetSut();
        sut.Register(_fixture.Hub, SentryOptions);

        _fixture.AppDomain.Received().ProcessExit += sut.HandleProcessExit;
    }
}
