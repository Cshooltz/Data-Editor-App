/*
This file just contains some snippets
showing better ways to do async coding
*/
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class SomeService
{
    // ValueTask is a struct, not an object, so it avoids heap allocation
    public async ValueTask<int> GetValueAsync(int numberToAdd) // Can return ValueTask<int> instead of Task<int>
    {
        // Doesn't waste a ThreadPool Thread, returns a Task object
        //return await Task.FromResult(numberToAdd * 2);

        // Wastes a whole ThreadPool Thread on a trivial task
        // return await Task.Run(() => numberToAdd * 2); 

        // Returns a ValueTask, which is a struct so it is created and passed by value
        return await new ValueTask<int>(numberToAdd * 2);
    }

    public async Task<int> GetValueAsync()
    {
        return await Task.FromResult(1);
    }

    // Cancellation tokens...
    // These are needed for if you want to cancel
    // a task and make sure it cancels all subsequent tasks.
    // The key is the CancellationToken needs to be passed
    // all the way down. 

    // async constructors:
    // Use the factory-builder pattern
    // Make the actual constructor private
    // And use a public static async Task<>() method
    // to return the new object for you.
    // For example:
    private readonly MyConnection _myConnection;

    private SomeService(MyConnection myConnection)
    {
        _myConnection = myConnection;
    }

    public static async Task<SomeService> CreateAsync(ConnectionFactory factory)
    {
        return new SomeService(await factory.CreateConnectionAsync());
    }

    // If you want to await a callback function, you can't use Action<>
    // And Action<> is treated like an event and cannot be awaited.
    // Use the Func<Task> delegate type instead.
    public static async Task AcceptsDelegateMethod(Func<Task> content)
    {
        await Task.Delay(1);
        await content();
    }

    // Interloacked.Increment(ref varName);
    // Lazy<> versions of variables.
    // i.e. Lazy<string> MyLazyString;
    // MyLazyString.Value; triggers the lazy part. 

    /*
    To make internal items visible outside of an assembly (i.e. for tests)
    You can use a tag for that:
    [assembly: InternalsVisibleTo("InternalsVisibleForTests.Tests)]
    in .csproj:
    <ItemGroup>
        <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
            <_Parameter1>$(AssemblyName).Tests</_PArameter1> <!-- This provides an automated way of allowing internals for all tests -->
        </AssemblyAttribute>
    </ItemGroup>
    */

    //BenchmarkDotNet
}

public struct MyConnection
{
    public string Connection;
}

public class ConnectionFactory
{
    public async Task<MyConnection> CreateConnectionAsync()
    {
        return await Task.Run(() =>
        {
            return new MyConnection() { Connection = "Hello!" };
        });
    }
}