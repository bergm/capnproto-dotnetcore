using System;
using System.Threading;
using System.Threading.Tasks;
using Capnp;
using Capnp.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capnp.Net.Runtime.Tests;

[TestClass]
[TestCategory("Coverage")]
public class SkeletonTests
{
    private const ulong TestInterfaceId = 0x1234567890ABCDEFUL;

    private class TestSkeleton : Skeleton<object>
    {
        public override ulong InterfaceId => TestInterfaceId;

        public TestSkeleton()
        {
            SetMethodTable(DoSomething);
        }

        private static Task<AnswerOrCounterquestion> DoSomething(
            DeserializerState args,
            CancellationToken cancellationToken
        )
        {
            AnswerOrCounterquestion result = new SerializerState();
            return Task.FromResult(result);
        }
    }

    [TestMethod]
    public async Task Invoke_WrongInterfaceId_Throws()
    {
        var skeleton = new TestSkeleton();
        skeleton.Bind(new object());
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            skeleton.Invoke(0UL, 0, default(DeserializerState))
        );
        Assert.AreEqual("Wrong interface id", ex.Message);
    }

    [TestMethod]
    public async Task Invoke_CorrectInterfaceId_PassesCheck()
    {
        var skeleton = new TestSkeleton();
        skeleton.Bind(new object());
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            skeleton.Invoke(TestInterfaceId, 100, default(DeserializerState))
        );
        Assert.AreEqual("Wrong method id", ex.Message);
    }

    [TestMethod]
    public async Task Invoke_CorrectInterfaceId_CallsMethod()
    {
        var skeleton = new TestSkeleton();
        skeleton.Bind(new object());
        var result = await skeleton.Invoke(TestInterfaceId, 0, default(DeserializerState));
        Assert.IsNotNull(result.Answer);
    }
}
