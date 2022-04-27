// See https://aka.ms/new-console-template for more information

var cache = new Dictionary<Type, Delegate>();

InvokeThing<string>();
InvokeThing<int>();
InvokeThing<long>();
InvokeThing<float>();
InvokeThing<double>();
InvokeThing<decimal>();
InvokeThing<object>();
InvokeThing<bool>();
InvokeThing<char>();

void InvokeThing<T>()
{
    if (!cache.ContainsKey(typeof(IGenericThing<T>)))
    {
        var del = typeof(IGenericThing<>).MakeGenericType(typeof(T)).GetMethod("DoThing").CreateDelegate(typeof(Action<IGenericThing<T>, T>));
    
        cache.Add(typeof(IGenericThing<T>), del);
    }
    
    ((Action<IGenericThing<T>, T>)cache[typeof(IGenericThing<T>)])(new GenericThing<T>(), default);
}


internal interface IGenericThing<T>
{
    void DoThing(T thing);
}

internal class GenericThing<T> : IGenericThing<T>
{
    public void DoThing(T thing)
    {
        Console.WriteLine(thing);
    }
}

