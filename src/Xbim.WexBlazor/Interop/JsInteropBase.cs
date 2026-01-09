using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace Xbim.WexBlazor.Interop;

/// <summary>
/// Base class for JavaScript interop functionality
/// </summary>
public abstract class JsInteropBase : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    protected readonly IJSRuntime JsRuntime;

    protected JsInteropBase(IJSRuntime jsRuntime, string modulePath)
    {
        JsRuntime = jsRuntime;
        _moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
            "import", modulePath).AsTask());
    }

    protected async ValueTask<IJSObjectReference> GetModuleAsync() => 
        await _moduleTask.Value;

    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<T>(identifier, args);
    }

    protected async ValueTask InvokeVoidAsync(string identifier, params object[] args)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync(identifier, args);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
} 