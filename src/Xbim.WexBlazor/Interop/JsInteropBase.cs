using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace Xbim.WexBlazor.Interop;

/// <summary>
/// Base class for JavaScript interop functionality
/// </summary>
public abstract class JsInteropBase : IAsyncDisposable
{
    private IJSObjectReference? _module;
    private bool _isInitialized;
    
    /// <summary>
    /// The JavaScript runtime instance
    /// </summary>
    protected readonly IJSRuntime JsRuntime;
    private readonly string _modulePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsInteropBase"/> class
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime</param>
    /// <param name="modulePath">The path to the JavaScript module to import</param>
    protected JsInteropBase(IJSRuntime jsRuntime, string modulePath)
    {
        JsRuntime = jsRuntime;
        _modulePath = modulePath;
    }

    /// <summary>
    /// Gets a value indicating whether the JavaScript module has been initialized
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Initializes the JavaScript module. Must be called before invoking any JavaScript functions.
    /// Safe to call during interactive render only (not during prerender).
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", _modulePath);
        _isInitialized = true;
    }

    /// <summary>
    /// Gets the JavaScript module reference if initialized
    /// </summary>
    /// <returns>The module reference or null if not initialized</returns>
    protected IJSObjectReference? GetModule()
    {
        return _isInitialized ? _module : null;
    }

    /// <summary>
    /// Invokes a JavaScript function and returns a value
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    /// <param name="identifier">The function identifier</param>
    /// <param name="args">The function arguments</param>
    /// <returns>The function result or default if not initialized</returns>
    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object[] args)
    {
        if (!_isInitialized)
            return default!;

        var module = GetModule();
        if (module == null)
            return default!;

        return await module.InvokeAsync<T>(identifier, args);
    }

    /// <summary>
    /// Invokes a JavaScript function without a return value
    /// </summary>
    /// <param name="identifier">The function identifier</param>
    /// <param name="args">The function arguments</param>
    protected async ValueTask InvokeVoidAsync(string identifier, params object[] args)
    {
        if (!_isInitialized)
            return;

        var module = GetModule();
        if (module == null)
            return;

        await module.InvokeVoidAsync(identifier, args);
    }

    /// <summary>
    /// Disposes the JavaScript module
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
        _isInitialized = false;
    }
} 