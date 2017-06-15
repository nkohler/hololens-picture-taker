using NUnit.Framework;
using NSubstitute;
using Zenject;
using Futulabs.HoloFramework.Targeting;

[TestFixture]
public class TargetingManagerTest : ZenjectUnitTestFixture
{

    [SetUp]
    public void CommonInstall()
    {
        Container.Bind<TargetingManager>().AsSingle();
    }

    [Test]
    public void TargeterIsUpdated()
    {
        //Arrange
        ITargeter targeter = Substitute.For<ITargeter>();
        Container.Bind<ITargeter>().FromInstance(targeter);
        ICursor cursor = Substitute.For<ICursor>();
        Container.Bind<ICursor>().FromInstance(cursor);
        TargetingManager tMan = Container.Resolve<TargetingManager>();

        //Act
        tMan.Tick();

        //Assert
        targeter.Received(1).AcquireTarget();
    }
}
