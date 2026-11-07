using NUnit.Framework;

// 测试
public class EventBusTests
{
    private struct PingEvent { public int Value; }
    private struct OtherEvent { public int Value; }

    private EventBus _bus;
    private int _pingCount;
    private int _pingLastValue;
    private int _selfRemoveCount;

    // 命名方法：不能用 lambda，否则退订不掉（见下文坑 1）
    private void OnPing(PingEvent e)
    {
        _pingCount++;
        _pingLastValue = e.Value;
    }

    private void OnSelfRemoving(PingEvent e)
    {
        _selfRemoveCount++;
        _bus.Unsubscribe<PingEvent>(OnSelfRemoving);
    }

    [SetUp]
    public void SetUp()
    {
        _bus = new EventBus();   // 每个测试一条独立的总线
        _pingCount = 0;
        _pingLastValue = 0;
        _selfRemoveCount = 0;
    }

    [Test]
    public void SubscribeThenPublish_HandlerReceivesEvent()
    {
        _bus.Subscribe<PingEvent>(OnPing);

        _bus.Publish(new PingEvent { Value = 42 });

        Assert.AreEqual(1, _pingCount, "订阅后应收到一次");
        Assert.AreEqual(42, _pingLastValue, "事件参数应原样传递");
    }

    [Test]
    public void UnsubscribeThenPublish_HandlerDoesNotReceive()
    {
        _bus.Subscribe<PingEvent>(OnPing);
        _bus.Unsubscribe<PingEvent>(OnPing);

        _bus.Publish(new PingEvent { Value = 42 });

        Assert.AreEqual(0, _pingCount, "退订后不应再收到");
    }

    [Test]
    public void Publish_DifferentType_DoesNotCrossTalk()
    {
        _bus.Subscribe<PingEvent>(OnPing);

        _bus.Publish(new OtherEvent { Value = 7 });

        Assert.AreEqual(0, _pingCount, "不同类型的事件不应互相触发");
    }

    [Test]
    public void UnsubscribeInsideHandler_DoesNotThrow_AndTakesEffectNextRound()
    {
        _bus.Subscribe<PingEvent>(OnPing);
        _bus.Subscribe<PingEvent>(OnSelfRemoving);

        Assert.DoesNotThrow(() => _bus.Publish(new PingEvent()));
        Assert.AreEqual(1, _pingCount, "本轮广播要跑完，退订不打断本次");
        Assert.AreEqual(1, _selfRemoveCount);

        _bus.Publish(new PingEvent());
        Assert.AreEqual(2, _pingCount, "第二次广播，正常的订阅者继续收到");
        Assert.AreEqual(1, _selfRemoveCount, "已退订的那位第二轮收不到");
    }

    [Test]
    public void SubscribeTwice_ReceivesTwice()
    {
        _bus.Subscribe<PingEvent>(OnPing);
        _bus.Subscribe<PingEvent>(OnPing);

        _bus.Publish(new PingEvent());

        Assert.AreEqual(2, _pingCount, "订阅两次就收两次，不去重");
    }
}