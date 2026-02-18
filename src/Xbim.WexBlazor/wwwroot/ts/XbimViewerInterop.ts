// XbimViewerInterop.ts
// This module provides interop between Blazor and Xbim Viewer (using xBIM Viewer library)

import type { Viewer as XbimViewer } from '@xbim/viewer';

const ViewType = {
    TOP: 0,
    BOTTOM: 1,
    FRONT: 2,
    BACK: 3,
    LEFT: 4,
    RIGHT: 5,
    DEFAULT: 6
} as const;

const State = {
    UNDEFINED: 255,
    HIDDEN: 254,
    HIGHLIGHTED: 253,
    XRAYVISIBLE: 252,
    PICKING_ONLY: 251,
    HOVEROVER: 250,
    UNSTYLED: 225
} as const;

const CameraType = {
    PERSPECTIVE: 0,
    ORTHOGONAL: 1
} as const;

const XBIM_SCRIPT_PATH =
    '_content/Xbim.WexBlazor/lib/xbim-viewer/index.js' as const;

declare global {
    interface Window {
        xbim?: {
            Viewer: { new(canvasId: string, messageHandler: (message: string) => void): XbimViewer };
        };
        Viewer?: { new(canvasId: string, messageHandler: (message: string) => void): XbimViewer };
    }
}

const viewerInstances = new Map<string, XbimViewer>();
const eventHandlers = new Map<string, Map<string, any>>(); // viewerId -> eventName -> handler
const loadedModels = new Map<string, Map<number, any>>(); // viewerId -> modelId -> modelInfo
const resizeObservers = new Map<string, ResizeObserver>(); // viewerId -> ResizeObserver
const viewerCanvasMap = new Map<string, string>(); // viewerId -> canvasId
let viewerIdCounter = 0;
let ViewerCtor: any = null;
let xbimModule: any = null;
let loadXbimPromise: Promise<void> | null = null;

function loadXbimViewer(scriptUrl = XBIM_SCRIPT_PATH): Promise<void> {
    if (loadXbimPromise) return loadXbimPromise;
    
    loadXbimPromise = new Promise<void>((resolve, reject) => {
        const tag = document.createElement('script');
        tag.src = scriptUrl;
        tag.type = 'module';
        tag.onload = () => {
            setTimeout(() => {
                ViewerCtor = (window as any).Viewer;
                xbimModule = window as any;
                
                if (!ViewerCtor) {
                    reject(new Error('Viewer constructor not found'));
                    return;
                }
                
                console.log('xBIM Viewer loaded successfully');
                resolve();
            }, 100);
        };
        tag.onerror = () => reject(new Error(`Failed to load ${scriptUrl}`));
        document.head.append(tag);
    });
    
    return loadXbimPromise;
}


export async function initViewer(canvasId: string): Promise<string | null> {
    try {
        await loadXbimViewer();
        const canvas = document.getElementById(canvasId) as HTMLCanvasElement;
        if (!canvas) {
            console.error(`Canvas element with id ${canvasId} not found`);
            return null;
        }

        if (!ViewerCtor) {
            console.error("Viewer class is not defined. xBIM library may not be properly loaded.");
            return null;
        }

        // Set initial canvas size based on container
        resizeCanvas(canvasId);

        const viewer = new ViewerCtor(canvasId, (message: string) => {
            console.error(message);
        });
        viewer.background = [0, 0, 0, 0];
        viewer.highlightingColour = [72, 73, 208, 255];
        viewer.hoverPickEnabled = true;

        const viewerId = `viewer_${viewerIdCounter++}`;
        viewerInstances.set(viewerId, viewer);
        viewerCanvasMap.set(viewerId, canvasId);

        // Setup resize observer to handle container size changes
        setupResizeObserver(canvasId, viewerId);

        return viewerId;
    } catch (error) {
        console.error('Error initializing xBIM Viewer:', error);
        return null;
    }
}

// Resize the canvas buffer to match its display size
export function resizeCanvas(canvasId: string): boolean {
    try {
        const canvas = document.getElementById(canvasId) as HTMLCanvasElement;
        if (!canvas) {
            console.error(`Canvas element with id ${canvasId} not found`);
            return false;
        }

        // Get the display size from CSS
        const rect = canvas.getBoundingClientRect();
        const displayWidth = Math.floor(rect.width);
        const displayHeight = Math.floor(rect.height);

        // Use devicePixelRatio for high-DPI displays
        const dpr = window.devicePixelRatio || 1;
        const bufferWidth = Math.floor(displayWidth * dpr);
        const bufferHeight = Math.floor(displayHeight * dpr);

        // Only resize if dimensions have changed
        if (canvas.width !== bufferWidth || canvas.height !== bufferHeight) {
            canvas.width = bufferWidth;
            canvas.height = bufferHeight;
            console.log(`Canvas ${canvasId} resized to ${bufferWidth}x${bufferHeight} (display: ${displayWidth}x${displayHeight}, dpr: ${dpr})`);
            return true;
        }

        return false;
    } catch (error) {
        console.error('Error resizing canvas:', error);
        return false;
    }
}

// Setup a ResizeObserver to automatically resize the canvas when its container changes
function setupResizeObserver(canvasId: string, viewerId: string): void {
    const canvas = document.getElementById(canvasId) as HTMLCanvasElement;
    if (!canvas) return;

    // Get the wrapper element (parent of canvas)
    const wrapper = canvas.parentElement;
    if (!wrapper) return;

    // Clean up any existing observer for this viewer
    const existingObserver = resizeObservers.get(viewerId);
    if (existingObserver) {
        existingObserver.disconnect();
    }

    const observer = new ResizeObserver((entries) => {
        for (const entry of entries) {
            // Resize the canvas buffer
            const resized = resizeCanvas(canvasId);

            // If resized, trigger a redraw on the viewer
            if (resized) {
                const viewer = viewerInstances.get(viewerId);
                if (viewer) {
                    try {
                        viewer.draw();
                    } catch (e) {
                        // Viewer might not be ready yet
                    }
                }
            }
        }
    });

    observer.observe(wrapper);
    resizeObservers.set(viewerId, observer);
    console.log(`ResizeObserver set up for viewer ${viewerId} (canvas ${canvasId})`);
}

// Load a model from a URL with optional tag for identification
export async function loadModel(viewerId: string, modelUrl: string, tag?: any): Promise<number | null> {
    try {
        console.log(`Loading model from URL: ${modelUrl}`);
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return null;
        }
        
        console.log("Found viewer instance, loading model...");
        
        // Wait for the model to load using a Promise
        return await new Promise<number | null>((resolve, reject) => {
            // Set up one-time event handlers for load success and failure
            const onLoaded = (args: any) => {
                console.log("Model loaded successfully. Event args:", JSON.stringify(args));
                viewer.off('loaded', onLoaded);
                viewer.off('error', onError);
                
                // Extract model ID from the loaded event
                // The xBIM viewer returns the handle ID in the event
                let modelId = args?.model;
                
                // If model ID not in args, try to get it from the viewer's handles
                if (modelId === undefined || modelId === null) {
                    // The viewer stores model handles, try to access them
                    const viewerAny = viewer as any;
                    if (viewerAny._handles && viewerAny._handles.length > 0) {
                        // Get the last added handle (most recently loaded)
                        const lastHandle = viewerAny._handles[viewerAny._handles.length - 1];
                        modelId = lastHandle?.id || viewerAny._handles.length - 1;
                        console.log("Extracted model ID from viewer handles:", modelId);
                    }
                }
                
                // If still no model ID, use 0 as default (first model)
                if (modelId === undefined || modelId === null) {
                    modelId = 0;
                    console.warn("Could not determine model ID, using default:", modelId);
                }
                
                console.log("Final model ID:", modelId);
                
                // Track the loaded model
                if (!loadedModels.has(viewerId)) {
                    loadedModels.set(viewerId, new Map());
                }
                loadedModels.get(viewerId)!.set(modelId, {
                    id: modelId,
                    url: modelUrl,
                    tag: tag,
                    loadedAt: new Date()
                });
                
                resolve(modelId);
            };
            
            const onError = (args: any) => {
                console.error("Error loading model:", args);
                viewer.off('loaded', onLoaded);
                viewer.off('error', onError);
                resolve(null);
            };
            
            viewer.on('loaded', onLoaded);
            viewer.on('error', onError);
            
            // Start loading the model with tag
            viewer.loadAsync(modelUrl, tag);
        });
    } catch (error) {
        console.error('Error loading model:', error);
        return null;
    }
}

// Start the viewer
export function start(viewerId: string): boolean {
    try {
        console.log(`Starting viewer ${viewerId}`);
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        viewer.start();
        console.log("Viewer started");
        return true;
    } catch (error) {
        console.error('Error starting viewer:', error);
        return false;
    }
}

// Set background color
export function setBackgroundColor(viewerId: string, rgba: number[]): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Set background color using rgba array
        viewer.background = rgba;
        return true;
    } catch (error) {
        console.error('Error setting background color:', error);
        return false;
    }
}

// Set highlighting (selection) color
export function setHighlightingColor(viewerId: string, rgba: number[]): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Set highlighting color using rgba array
        viewer.highlightingColour = rgba;
        return true;
    } catch (error) {
        console.error('Error setting highlighting color:', error);
        return false;
    }
}

// Set hover pick color
export function setHoverPickColor(viewerId: string, rgba: number[]): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Set hover pick color using rgba array
        viewer.hoverPickColour = rgba;
        return true;
    } catch (error) {
        console.error('Error setting hover pick color:', error);
        return false;
    }
}

// Zoom to fit all elements in the view
export async function zoomFit(viewerId: string): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Zoom to full extent with animation
        await viewer.zoomTo(undefined, undefined, true);
        console.log("Zoom fit applied");
        return true;
    } catch (error) {
        console.error('Error zooming to fit:', error);
        return false;
    }
}

// Reset the viewer to its initial state
export async function reset(viewerId: string): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Show default view with animation
        await viewer.show(ViewType.DEFAULT, undefined, undefined, true);
        console.log("Viewer reset to default view");
        return true;
    } catch (error) {
        console.error('Error resetting viewer:', error);
        return false;
    }
}

// Show a specific view type
export async function show(viewerId: string, type: number, id?: number, model?: number, withAnimation: boolean = true): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        await viewer.show(type, id, model, withAnimation);
        console.log(`Viewer showing view type ${type}`);
        return true;
    } catch (error) {
        console.error('Error showing view:', error);
        return false;
    }
}

// Hide specific elements by their IDs
export function hideElements(viewerId: string, elementIds: number[]): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Set state to HIDDEN for the specified elements
        viewer.setState(State.HIDDEN, elementIds);
        console.log(`Hidden ${elementIds.length} elements`);
        return true;
    } catch (error) {
        console.error('Error hiding elements:', error);
        return false;
    }
}

// Show specific elements by their IDs
export function showElements(viewerId: string, elementIds: number[]): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Remove HIDDEN state to make elements visible again
        viewer.removeState(State.HIDDEN, elementIds);
        console.log(`Shown ${elementIds.length} elements`);
        return true;
    } catch (error) {
        console.error('Error showing elements:', error);
        return false;
    }
}

// Unhide all elements
export function unhideAllElements(viewerId: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        const hidden = viewer.getProductsWithState(State.HIDDEN);
        if (hidden && hidden.length > 0) {
            const ids = hidden.map(p => p.id);
            viewer.removeState(State.HIDDEN, ids);
        }
        return true;
    } catch (error) {
        console.error('Error unhiding all elements:', error);
        return false;   
    }
}

// Get all products from the model region
export function getAllProducts(viewerId: string): Array<{id: number, model: number}> {
    try {
        console.log(viewerInstances);
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return [];
        }
        
        const allProducts: Array<{id: number, model: number}> = [];
        const viewerAny = viewer as any;
        
        // Iterate through all model handles
        if (!viewerAny._handles || viewerAny._handles.length === 0) {
            console.warn('No model handles found');
            return [];
        }
        
        console.log(`Found ${viewerAny._handles.length} model handle(s)`);
        
        for (const handle of viewerAny._handles) {
            const modelId = handle.id;
            try {
                const region = viewerAny.getMergedRegion();
                if (region && region.population) {
                    for (const productId of region.population) {
                        if (productId > 0) {
                            allProducts.push({ id: productId, model: modelId });
                        }
                    }
                }
            } catch (e) {
                console.warn(`Could not get region for model ${modelId}:`, e);
            }
        }
        
        return allProducts;
    } catch (error) {
        console.error('Error getting all products:', error);
        return [];
    }
}

// Isolate specific elements (hide everything else) using native viewer.isolate()
export function isolateElements(viewerId: string, elementIds: number[], modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        if (elementIds.length === 0) {
            console.warn('No elements to isolate');
            return false;
        }
        
        const viewerAny = viewer as any;
        
        // Use native isolate if modelId is provided
        if (modelId !== undefined) {
            viewer.isolate(elementIds, modelId);
            console.log(`✓ Isolated ${elementIds.length} elements in model ${modelId}`);
            return true;
        }
        
        // If no modelId, isolate in all models
        if (viewerAny._handles && viewerAny._handles.length > 0) {
            for (const handle of viewerAny._handles) {
                viewer.isolate(elementIds, handle.id);
            }
            console.log(`✓ Isolated ${elementIds.length} elements in all models`);
            return true;
        }
        
        return false;
    } catch (error) {
        console.error('Error isolating elements:', error);
        return false;
    }
}

// Unisolate (show all elements) by clearing isolatedProducts on handles
export function unisolateElements(viewerId: string, modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        const viewerAny = viewer as any;
        
        // Clear isolation by setting isolatedProducts to undefined on handles
        // Using viewer.isolate([]) doesn't work - it isolates "nothing" instead of clearing
        if (viewerAny._handles && viewerAny._handles.length > 0) {
            for (const handle of viewerAny._handles) {
                if (modelId !== undefined && handle.id !== modelId) {
                    continue;
                }
                // Clear the isolation by setting to undefined (not empty array)
                if (handle.isolatedProducts !== undefined) {
                    handle.isolatedProducts = undefined;
                }
                // Also try resetting via the stopped property if isolation uses that
                if (handle._model) {
                    handle._model.isolatedProducts = undefined;
                }
            }
        }
        
        // Also remove any HIDDEN states
        const hiddenProducts = viewer.getProductsWithState(State.HIDDEN);
        if (hiddenProducts.length > 0) {
            const hiddenIds = hiddenProducts.map((p: any) => p.id);
            viewer.removeState(State.HIDDEN, hiddenIds);
        }
        
        // Reset the section box to infinity to prevent "disjoint" issues
        if (viewer.sectionBox) {
            viewer.sectionBox.setToInfinity();
        }
        
        // Trigger a redraw to refresh the viewer state
        viewer.draw();
        console.log(`✓ Unisolated ${modelId !== undefined ? `model ${modelId}` : 'all models'}`);
        
        return true;
    } catch (error) {
        console.error('Error unisolating elements:', error);
        return false;
    }
}

export function getIsolatedElements(viewerId: string, modelId?: number): number[] {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return [];
        }
        
        const viewerAny = viewer as any;
        const allIsolated: number[] = [];
        
        if (modelId !== undefined) {
            return viewer.getIsolated(modelId);
        }
        
        // Get isolated from all models
        if (viewerAny._handles && viewerAny._handles.length > 0) {
            for (const handle of viewerAny._handles) {
                const isolated = viewer.getIsolated(handle.id);
                if (isolated && isolated.length > 0) {
                    allIsolated.push(...isolated);
                }
            }
        }
        
        return allIsolated;
    } catch (error) {
        console.error('Error getting isolated elements:', error);
        return [];
    }
}

// Generic method to invoke any viewer method
export async function invokeViewerMethod(viewerId: string, methodName: string, ...args: any[]): Promise<any> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return null;
        }
        
        if (typeof (viewer as any)[methodName] === 'function') {
            const result = (viewer as any)[methodName](...args);
            // If the result is a promise, await it
            if (result && typeof result.then === 'function') {
                return await result;
            }
            return result;
        }
        
        console.error(`Method ${methodName} not found on viewer`);
        return null;
    } catch (error) {
        console.error(`Error invoking viewer method ${methodName}:`, error);
        return null;
    }
}

// Highlight/Select specific elements by their IDs
export function highlightElements(viewerId: string, elementIds: number[], modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Use setState to highlight (select) elements
        viewer.setState(State.HIGHLIGHTED, elementIds, modelId);
        console.log(`Highlighted ${elementIds.length} elements`);
        return true;
    } catch (error) {
        console.error('Error highlighting elements:', error);
        return false;
    }
}

// Unhighlight (restore to normal style) elements
export function unhighlightElements(viewerId: string, elementIds: number[], modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        viewer.removeState(State.HIGHLIGHTED, elementIds, modelId);
        viewer.resetState(elementIds, modelId);
        return true;
    } catch (error) {
        console.error('Error unhighlighting elements:', error);
        return false;
    }
}

// Check if an element is highlighted
export function isElementHighlighted(viewerId: string, elementId: number, modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        const highlightedProducts = viewer.getProductsWithState(State.HIGHLIGHTED);
        return highlightedProducts.some((p: any) => p.id === elementId && (modelId === undefined || p.model === modelId));
    } catch (error) {
        console.error('Error checking if element is highlighted:', error);
        return false;
    }
}

// Add elements to current selection
export function addToSelection(viewerId: string, elementIds: number[], modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Use addState to add to highlighted elements
        viewer.addState(State.HIGHLIGHTED, elementIds, modelId);
        console.log(`Added ${elementIds.length} elements to selection`);
        return true;
    } catch (error) {
        console.error('Error adding to selection:', error);
        return false;
    }
}

// Remove elements from selection
export function removeFromSelection(viewerId: string, elementIds: number[], modelId?: number): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Remove HIGHLIGHTED state from elements
        viewer.removeState(State.HIGHLIGHTED, elementIds, modelId);
        console.log(`Removed ${elementIds.length} elements from selection`);
        return true;
    } catch (error) {
        console.error('Error removing from selection:', error);
        return false;
    }
}

// Clear all selected elements
export function clearSelection(viewerId: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        viewer.clearHighlighting();
        console.log("Selection cleared");
        return true;
    } catch (error) {
        console.error('Error clearing selection:', error);
        return false;
    }
}

// Get all currently selected elements
export function getSelectedElements(viewerId: string): Array<{id: number, model: number}> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return [];
        }
        
        const selected = viewer.getProductsWithState(State.HIGHLIGHTED);
        return selected;
    } catch (error) {
        console.error('Error getting selected elements:', error);
        return [];
    }
}

// Register an event handler that calls back to C#
export function addEventListener(viewerId: string, eventName: string, dotNetHelper: any): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Create event handler that calls back to C#
        const handler = (args: any) => {
            // Prepare event data for C#
            const eventData: any = {
                eventName: eventName
            };
            
            // For interaction events (pick, click, hover, etc.)
            if (args && typeof args === 'object') {
                if (args.id !== undefined) eventData.id = args.id;
                if (args.model !== undefined) eventData.model = args.model;
                if (args.xyz) {
                    eventData.x = args.xyz[0];
                    eventData.y = args.xyz[1];
                    eventData.z = args.xyz[2];
                }
                // For loaded event
                if (args.model !== undefined && args.tag !== undefined) {
                    eventData.modelId = args.model;
                    eventData.tag = args.tag;
                }
                // For error event
                if (args.message) {
                    eventData.message = args.message;
                }
            }
            
            // Call C# callback
            dotNetHelper.invokeMethodAsync('OnViewerEvent', eventData);
        };
        
        // Register handler with viewer
        viewer.on(eventName as any, handler);
        
        // Store handler reference for cleanup
        if (!eventHandlers.has(viewerId)) {
            eventHandlers.set(viewerId, new Map());
        }
        eventHandlers.get(viewerId)!.set(eventName, handler);
        
        console.log(`Registered event: ${eventName} for viewer ${viewerId}`);
        return true;
    } catch (error) {
        console.error(`Error registering event ${eventName}:`, error);
        return false;
    }
}

// Unregister an event handler
export function removeEventListener(viewerId: string, eventName: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        const handlers = eventHandlers.get(viewerId);
        if (handlers && handlers.has(eventName)) {
            const handler = handlers.get(eventName);
            viewer.off(eventName, handler);
            handlers.delete(eventName);
            console.log(`Unregistered event: ${eventName}`);
        }
        
        return true;
    } catch (error) {
        console.error(`Error unregistering event ${eventName}:`, error);
        return false;
    }
}

// Unload a specific model
export async function unloadModel(viewerId: string, modelId: number): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        // Unload the model from the viewer
        viewer.unload(modelId);
        
        // Remove from our tracking
        const models = loadedModels.get(viewerId);
        if (models) {
            models.delete(modelId);
        }
        
        console.log(`Model ${modelId} unloaded successfully`);
        return true;
    } catch (error) {
        console.error(`Error unloading model ${modelId}:`, error);
        return false;
    }
}

// Get list of loaded models
export function getLoadedModels(viewerId: string): any[] {
    const models = loadedModels.get(viewerId);
    if (!models) {
        return [];
    }
    
    return Array.from(models.values());
}

// Debug helper: Get all products in the viewer (for debugging)
export function debugGetAllProducts(viewerId: string): any {
    const viewer = viewerInstances.get(viewerId);
    if (!viewer) {
        return { error: "Viewer not found" };
    }
    
    const result: any = {
        handles: [],
        productsByState: {}
    };
    
    // Check handles
    const viewerAny = viewer as any;
    if (viewerAny._handles) {
        result.handles = viewerAny._handles.map((h: any, idx: number) => ({
            index: idx,
            id: h?.id,
            hasModel: !!h?._model,
            productCount: h?._model?.products?.length || 0
        }));
    }
    
    // Check products by state
    const states = [State.UNDEFINED, State.HIDDEN, State.HIGHLIGHTED, State.XRAYVISIBLE, State.UNSTYLED];
    for (const state of states) {
        try {
            const products = viewer.getProductsWithState(state);
            result.productsByState[state] = {
                count: products.length,
                sample: products.slice(0, 5),
                models: [...new Set(products.map((p: any) => p.model))]
            };
        } catch (e) {
            result.productsByState[state] = { error: String(e) };
        }
    }
    
    return result;
}

// Hide/show a model using viewer's built-in start/stop methods
export async function setModelVisibility(viewerId: string, modelId: number, visible: boolean): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        
        console.log(`Setting model ${modelId} visibility to ${visible}`);
        
        if (visible) {
            // Start the model to make it visible
            viewer.start(modelId);
        } else {
            // Stop the model to hide it
            viewer.stop(modelId);
        }
        
        return true;
    } catch (error) {
        console.error(`Error setting model ${modelId} visibility:`, error);
        return false;
    }
}

// Plugin Management
const pluginInstances = new Map<string, Map<string, any>>(); // viewerId -> pluginId -> plugin instance

// Add a plugin to the viewer
export async function addPlugin(viewerId: string, pluginId: string, pluginType: string, config?: any): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }

        await loadXbimViewer();
        
        const win = window as any;
        let PluginClass = win[pluginType];
        
        if (!PluginClass && win.xbim) {
            PluginClass = win.xbim[pluginType];
        }
        
        if (!PluginClass) {
            console.error(`Plugin type ${pluginType} not found.`);
            console.log('Available on window:', Object.keys(win).filter(k => k.includes('Plugin') || k === 'xbim'));
            return false;
        }

        console.log(`Adding plugin ${pluginType} with config:`, config);

        const plugin = new PluginClass();
        
        // Set non-stopped properties before adding to viewer
        let stoppedValue: boolean | undefined = undefined;
        if (config && Object.keys(config).length > 0) {
            for (const [key, value] of Object.entries(config)) {
                if (key === 'stopped') {
                    stoppedValue = value as boolean;
                    continue;
                }
                try {
                    (plugin as any)[key] = value;
                } catch (err) {
                    console.warn(`Could not set plugin.${key}:`, err);
                }
            }
        }
        
        // Add plugin to viewer first
        viewer.addPlugin(plugin);
        
        // Set stopped property after plugin is added to viewer
        if (stoppedValue !== undefined) {
            plugin.stopped = stoppedValue;
        } else if ('stopped' in plugin) {
            plugin.stopped = false;
        }

        // Store plugin instance
        if (!pluginInstances.has(viewerId)) {
            pluginInstances.set(viewerId, new Map());
        }
        pluginInstances.get(viewerId)!.set(pluginId, plugin);

        console.log(`Plugin ${pluginType} (${pluginId}) added successfully`);
        return true;
    } catch (error) {
        console.error(`Error adding plugin ${pluginType}:`, error);
        return false;
    }
}

// Remove a plugin from the viewer
export async function removePlugin(viewerId: string, pluginId: string): Promise<boolean> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }

        const plugins = pluginInstances.get(viewerId);
        if (!plugins) {
            console.error(`No plugins found for viewer ${viewerId}`);
            return false;
        }

        const plugin = plugins.get(pluginId);
        if (!plugin) {
            console.error(`Plugin ${pluginId} not found`);
            return false;
        }

        // Remove plugin from viewer
        viewer.removePlugin(plugin);
        
        // Remove from tracking
        plugins.delete(pluginId);

        console.log(`Plugin ${pluginId} removed successfully`);
        return true;
    } catch (error) {
        console.error(`Error removing plugin ${pluginId}:`, error);
        return false;
    }
}

// Set plugin stopped state
export async function setPluginStopped(viewerId: string, pluginId: string, stopped: boolean): Promise<boolean> {
    try {
        const plugins = pluginInstances.get(viewerId);
        if (!plugins) {
            return false;
        }

        const plugin = plugins.get(pluginId);
        if (!plugin) {
            return false;
        }

        if ('stopped' in plugin) {
            plugin.stopped = stopped;
            return true;
        }

        return false;
    } catch (error) {
        console.error(`Error setting plugin stopped state:`, error);
        return false;
    }
}

// Get list of active plugins
export function getActivePlugins(viewerId: string): any[] {
    const plugins = pluginInstances.get(viewerId);
    if (!plugins) {
        return [];
    }

    return Array.from(plugins.entries()).map(([id, plugin]) => ({
        id,
        type: plugin.constructor.name,
        stopped: plugin.stopped || false
    }));
}

// Unclip the viewer (remove clipping plane)
export function unclip(viewerId: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }

        viewer.unclip();
        return true;
    } catch (error) {
        console.error(`Error unclipping viewer:`, error);
        return false;
    }
}

export function createSectionBox(viewerId: string, pluginId: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }

        const plugins = pluginInstances.get(viewerId);
        if (!plugins) {
            console.error(`No plugins found for viewer ${viewerId}`);
            return false;
        }

        const plugin = plugins.get(pluginId);
        if (!plugin) {
            console.error(`Plugin ${pluginId} not found`);
            return false;
        }

        const boundingBox = viewer.getMergedRegionWcs().bbox;
        const centre = viewer.getMergedRegionWcs().centre;
        const minX = boundingBox[0], minY = boundingBox[1], minZ = boundingBox[2];
        const maxX = boundingBox[3], maxY = boundingBox[4], maxZ = boundingBox[5];
        const meter = (viewer as any).activeHandles[0].meter;

        const cx = centre[0];
        const cy = centre[1];
        const cz = centre[2];

        const ex = Math.min(3 * meter, Math.abs(maxX - minX) / 5);
        const ey = Math.min(3 * meter, Math.abs(maxY - minY) / 5);
        const ez = Math.min(3 * meter, Math.abs(maxZ - minZ) / 5);

        const planes: any[] = [
            {
                direction: [ 0,  0,  1],
                location:  [cx,     cy,      cz + ez ]
            },
            {
                direction: [ 0,  0, -1],
                location:  [cx,     cy,      cz - ez ]
            },
            {
                direction: [ 1,  0,  0],
                location:  [cx + ex, cy,      cz     ]
            },
            {
                direction: [-1,  0,  0],
                location:  [cx - ex, cy,      cz     ]
            },
            {
                direction: [ 0, -1,  0],
                location:  [cx,     cy - ey, cz      ]
            },
            {
                direction: [ 0,  1,  0],
                location:  [cx,     cy + ey, cz      ]
            }
        ];

        viewer.sectionBox.setToPlanes(planes);
        (plugin as any).setClippingPlanes(planes);
        viewer.zoomTo();
        (plugin as any).stopped = false;

        return true;
    } catch (error) {
        console.error(`Error creating section box:`, error);
        return false;
    }
}

export function clearSectionBox(viewerId: string, pluginId: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }

        const plugins = pluginInstances.get(viewerId);
        if (plugins) {
            const plugin = plugins.get(pluginId);
            if (plugin) {
                (plugin as any).stopped = true;
            }
        }

        viewer.sectionBox.clear();
        viewer.zoomTo();

        return true;
    } catch (error) {
        console.error(`Error clearing section box:`, error);
        return false;
    }
}

// Clean up resources when a viewer instance is disposed
export function disposeViewer(viewerId: string): boolean {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }

        // Clean up the resize observer for this viewer
        const observer = resizeObservers.get(viewerId);
        if (observer) {
            observer.disconnect();
            resizeObservers.delete(viewerId);
            console.log(`ResizeObserver cleaned up for viewer ${viewerId}`);
        }

        // Clean up the canvas ID mapping
        viewerCanvasMap.delete(viewerId);

        // Remove all event handlers
        const handlers = eventHandlers.get(viewerId);
        if (handlers) {
            handlers.forEach((handler, eventName) => {
                viewer.off(eventName, handler);
            });
            eventHandlers.delete(viewerId);
        }

        // Remove all plugins
        const plugins = pluginInstances.get(viewerId);
        if (plugins) {
            plugins.forEach((plugin) => {
                viewer.removePlugin(plugin);
            });
            pluginInstances.delete(viewerId);
        }

        // Clear loaded models tracking
        loadedModels.delete(viewerId);

        // Stop the rendering loop
        viewer.stop();

        // Remove from our map
        viewerInstances.delete(viewerId);

        return true;
    } catch (error) {
        console.error('Error disposing viewer:', error);
        return false;
    }
}

// Get all product types and their products for a model
export function getModelProductTypes(viewerId: string, modelId?: number): Array<{typeId: number, productIds: number[], modelId: number}> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return [];
        }

        const viewerAny = viewer as any;
        const result: Array<{typeId: number, productIds: number[], modelId: number}> = [];
        const typeSet = new Set<string>();

        if (!viewerAny._handles || viewerAny._handles.length === 0) {
            return [];
        }

        for (const handle of viewerAny._handles) {
            if (modelId !== undefined && handle.id !== modelId) {
                continue;
            }

            const handleModelId = handle.id as number;
            
            if (handle._model && handle._model.productMaps) {
                const productMaps = handle._model.productMaps;
                
                // Handle Map type
                if (productMaps instanceof Map) {
                    for (const [productId, productMap] of productMaps) {
                        if (productMap && productMap.type !== undefined) {
                            const typeId = productMap.type as number;
                            const typeKey = `${typeId}-${handleModelId}`;
                            if (!typeSet.has(typeKey)) {
                                typeSet.add(typeKey);
                                const products = viewer.getProductsOfType(typeId, handleModelId);
                                if (products && products.length > 0) {
                                    result.push({
                                        typeId: typeId,
                                        productIds: products,
                                        modelId: handleModelId
                                    });
                                }
                            }
                        }
                    }
                }
                // Handle object/array type
                else if (typeof productMaps === 'object') {
                    const entries = Array.isArray(productMaps) 
                        ? productMaps.map((v, i) => [i, v] as [number, any])
                        : Object.entries(productMaps);
                    
                    for (const [productId, productMap] of entries) {
                        if (productMap && productMap.type !== undefined) {
                            const typeId = productMap.type as number;
                            const typeKey = `${typeId}-${handleModelId}`;
                            if (!typeSet.has(typeKey)) {
                                typeSet.add(typeKey);
                                const products = viewer.getProductsOfType(typeId, handleModelId);
                                if (products && products.length > 0) {
                                    result.push({
                                        typeId: typeId,
                                        productIds: products,
                                        modelId: handleModelId
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        return result;
    } catch (error) {
        console.error('Error getting model product types:', error);
        return [];
    }
}

// Get product type for a specific product
export function getProductType(viewerId: string, productId: number, modelId?: number): number | null {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            return null;
        }
        return viewer.getProductType(productId, modelId);
    } catch (error) {
        console.error('Error getting product type:', error);
        return null;
    }
}

// Get all products of a specific type
export function getProductsOfType(viewerId: string, typeId: number, modelId?: number): number[] {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            return [];
        }
        return viewer.getProductsOfType(typeId, modelId) || [];
    } catch (error) {
        console.error('Error getting products of type:', error);
        return [];
    }
}

// Get all unique product types across all loaded models
export function getAllProductTypes(viewerId: string): Array<{typeId: number, count: number, modelId: number}> {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            return [];
        }

        const viewerAny = viewer as any;
        const typeMap = new Map<string, {typeId: number, count: number, modelId: number}>();

        if (!viewerAny._handles || viewerAny._handles.length === 0) {
            return [];
        }

        for (const handle of viewerAny._handles) {
            const handleModelId = handle.id as number;
            
            if (handle._model && handle._model.productMaps) {
                const typeCounts = new Map<number, number>();
                
                for (const [productId, productMap] of handle._model.productMaps) {
                    if (productMap && productMap.type !== undefined) {
                        const typeId = productMap.type as number;
                        typeCounts.set(typeId, (typeCounts.get(typeId) || 0) + 1);
                    }
                }
                
                for (const [typeId, count] of typeCounts) {
                    const key = `${handleModelId}-${typeId}`;
                    typeMap.set(key, { typeId, count, modelId: handleModelId });
                }
            }
        }

        return Array.from(typeMap.values());
    } catch (error) {
        console.error('Error getting all product types:', error);
        return [];
    }
}