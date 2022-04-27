using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace DelegatePOC;

public class Benchmark
{
    private static Dictionary<Type, MethodInfo> _miCache = new();
    private static Dictionary<Type, Delegate> _dCache = new();
    private static Type _type = typeof(IPayload);

    [Benchmark]
    public void MethodInfo()
    {
        if (!_miCache.ContainsKey(_type))
        {
            _miCache[_type] = typeof(IThing<>)
                .MakeGenericType(_type)
                .GetMethod(nameof(IThing<IPayload>.GetResult));
        }
        
        var result = (int) _miCache[_type].Invoke(Activator.CreateInstance(typeof(Thing<>).MakeGenericType(_type)), new object[] { default });
    }

    [Benchmark]
    public void Delegate()
    {
        if (!_dCache.ContainsKey(_type))
        {
            _dCache[_type] = typeof(IThing<>)
                .MakeGenericType(_type)
                .GetMethod(nameof(IThing<IPayload>.GetResult))
                .CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(IThing<>).MakeGenericType(_type), _type, typeof(int)));
        }

        var func1 = ((Func<IThing<IPayload>, IPayload, int>)_dCache[_type])(Activator.CreateInstance(typeof(Thing<>).MakeGenericType(_type)) as IThing<IPayload>, new Payload());
    }
}

public interface IPayload { }

public record Payload : IPayload;

public interface IThing<T> where T : IPayload
{
    public int GetResult(T t);
}

public class Thing<T> : IThing<T> where T : IPayload
{
    public int GetResult(T t)
    {
        return 42;
    }
}

public class Thing2<T> : IThing<T> where T : IPayload
{
    public int GetResult(T t)
    {
        return 42;
    }
}