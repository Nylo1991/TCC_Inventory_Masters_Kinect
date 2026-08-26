using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Tests.Infrastructure
{
    internal static class TestEnvironment
    {
        internal static readonly ConcurrentQueue<Log> Logs = new ConcurrentQueue<Log>();
        // Executado antes de qualquer teste: nenhum log vai ao SQLite real.
        [ModuleInitializer]
        internal static void Initialize() => LoggerService.PersistirLog = log => Logs.Enqueue(log);
    }
}
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ModuleInitializerAttribute : Attribute { }
}
