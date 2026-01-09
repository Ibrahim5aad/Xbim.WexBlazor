var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const ViewType = {
    TOP: 0,
    BOTTOM: 1,
    FRONT: 2,
    BACK: 3,
    LEFT: 4,
    RIGHT: 5,
    DEFAULT: 6
};
const State = {
    UNDEFINED: 255,
    HIDDEN: 254,
    HIGHLIGHTED: 253,
    XRAYVISIBLE: 252,
    PICKING_ONLY: 251,
    HOVEROVER: 250,
    UNSTYLED: 225
};
const CameraType = {
    PERSPECTIVE: 0,
    ORTHOGONAL: 1
};
const XBIM_SCRIPT_PATH = '_content/Xbim.WexBlazor/lib/xbim-viewer/index.js';
const viewerInstances = new Map();
const eventHandlers = new Map();
const loadedModels = new Map();
let viewerIdCounter = 0;
let ViewerCtor = null;
let xbimModule = null;
let loadXbimPromise = null;
function loadXbimViewer(scriptUrl = XBIM_SCRIPT_PATH) {
    if (loadXbimPromise)
        return loadXbimPromise;
    loadXbimPromise = new Promise((resolve, reject) => {
        const tag = document.createElement('script');
        tag.src = scriptUrl;
        tag.type = 'module';
        tag.onload = () => {
            setTimeout(() => {
                ViewerCtor = window.Viewer;
                xbimModule = window;
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
export function initViewer(canvasId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            yield loadXbimViewer();
            const canvas = document.getElementById(canvasId);
            if (!canvas) {
                console.error(`Canvas element with id ${canvasId} not found`);
                return null;
            }
            if (!ViewerCtor) {
                console.error("Viewer class is not defined. xBIM library may not be properly loaded.");
                return null;
            }
            const viewer = new ViewerCtor(canvasId, (message) => {
                console.error(message);
            });
            viewer.background = [0, 0, 0, 0];
            viewer.highlightingColour = [72, 73, 208, 255];
            viewer.hoverPickEnabled = true;
            const viewerId = `viewer_${viewerIdCounter++}`;
            viewerInstances.set(viewerId, viewer);
            return viewerId;
        }
        catch (error) {
            console.error('Error initializing xBIM Viewer:', error);
            return null;
        }
    });
}
export function loadModel(viewerId, modelUrl, tag) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            console.log(`Loading model from URL: ${modelUrl}`);
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return null;
            }
            console.log("Found viewer instance, loading model...");
            return yield new Promise((resolve, reject) => {
                const onLoaded = (args) => {
                    console.log("Model loaded successfully. Event args:", JSON.stringify(args));
                    viewer.off('loaded', onLoaded);
                    viewer.off('error', onError);
                    let modelId = args === null || args === void 0 ? void 0 : args.model;
                    if (modelId === undefined || modelId === null) {
                        const viewerAny = viewer;
                        if (viewerAny._handles && viewerAny._handles.length > 0) {
                            const lastHandle = viewerAny._handles[viewerAny._handles.length - 1];
                            modelId = (lastHandle === null || lastHandle === void 0 ? void 0 : lastHandle.id) || viewerAny._handles.length - 1;
                            console.log("Extracted model ID from viewer handles:", modelId);
                        }
                    }
                    if (modelId === undefined || modelId === null) {
                        modelId = 0;
                        console.warn("Could not determine model ID, using default:", modelId);
                    }
                    console.log("Final model ID:", modelId);
                    if (!loadedModels.has(viewerId)) {
                        loadedModels.set(viewerId, new Map());
                    }
                    loadedModels.get(viewerId).set(modelId, {
                        id: modelId,
                        url: modelUrl,
                        tag: tag,
                        loadedAt: new Date()
                    });
                    resolve(modelId);
                };
                const onError = (args) => {
                    console.error("Error loading model:", args);
                    viewer.off('loaded', onLoaded);
                    viewer.off('error', onError);
                    resolve(null);
                };
                viewer.on('loaded', onLoaded);
                viewer.on('error', onError);
                viewer.loadAsync(modelUrl, tag);
            });
        }
        catch (error) {
            console.error('Error loading model:', error);
            return null;
        }
    });
}
export function start(viewerId) {
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
    }
    catch (error) {
        console.error('Error starting viewer:', error);
        return false;
    }
}
export function setBackgroundColor(viewerId, rgba) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.background = rgba;
        return true;
    }
    catch (error) {
        console.error('Error setting background color:', error);
        return false;
    }
}
export function zoomFit(viewerId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return false;
            }
            yield viewer.zoomTo(undefined, undefined, true);
            console.log("Zoom fit applied");
            return true;
        }
        catch (error) {
            console.error('Error zooming to fit:', error);
            return false;
        }
    });
}
export function reset(viewerId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return false;
            }
            yield viewer.show(ViewType.DEFAULT, undefined, undefined, true);
            console.log("Viewer reset to default view");
            return true;
        }
        catch (error) {
            console.error('Error resetting viewer:', error);
            return false;
        }
    });
}
export function show(viewerId_1, type_1, id_1, model_1) {
    return __awaiter(this, arguments, void 0, function* (viewerId, type, id, model, withAnimation = true) {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return false;
            }
            yield viewer.show(type, id, model, withAnimation);
            console.log(`Viewer showing view type ${type}`);
            return true;
        }
        catch (error) {
            console.error('Error showing view:', error);
            return false;
        }
    });
}
export function hideElements(viewerId, elementIds) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.setState(State.HIDDEN, elementIds);
        console.log(`Hidden ${elementIds.length} elements`);
        return true;
    }
    catch (error) {
        console.error('Error hiding elements:', error);
        return false;
    }
}
export function showElements(viewerId, elementIds) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.removeState(State.HIDDEN, elementIds);
        console.log(`Shown ${elementIds.length} elements`);
        return true;
    }
    catch (error) {
        console.error('Error showing elements:', error);
        return false;
    }
}
export function invokeViewerMethod(viewerId, methodName, ...args) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return null;
            }
            if (typeof viewer[methodName] === 'function') {
                const result = viewer[methodName](...args);
                if (result && typeof result.then === 'function') {
                    return yield result;
                }
                return result;
            }
            console.error(`Method ${methodName} not found on viewer`);
            return null;
        }
        catch (error) {
            console.error(`Error invoking viewer method ${methodName}:`, error);
            return null;
        }
    });
}
export function highlightElements(viewerId, elementIds, modelId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.setState(State.HIGHLIGHTED, elementIds, modelId);
        console.log(`Highlighted ${elementIds.length} elements`);
        return true;
    }
    catch (error) {
        console.error('Error highlighting elements:', error);
        return false;
    }
}
export function unhighlightElements(viewerId, elementIds, modelId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.setState(State.UNSTYLED, elementIds, modelId);
        console.log(`Unhighlighted ${elementIds.length} elements`);
        return true;
    }
    catch (error) {
        console.error('Error unhighlighting elements:', error);
        return false;
    }
}
export function isElementHighlighted(viewerId, elementId, modelId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        const highlightedProducts = viewer.getProductsWithState(State.HIGHLIGHTED);
        return highlightedProducts.some((p) => p.id === elementId && (modelId === undefined || p.model === modelId));
    }
    catch (error) {
        console.error('Error checking if element is highlighted:', error);
        return false;
    }
}
export function addToSelection(viewerId, elementIds, modelId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.addState(State.HIGHLIGHTED, elementIds, modelId);
        console.log(`Added ${elementIds.length} elements to selection`);
        return true;
    }
    catch (error) {
        console.error('Error adding to selection:', error);
        return false;
    }
}
export function removeFromSelection(viewerId, elementIds, modelId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.removeState(State.HIGHLIGHTED, elementIds, modelId);
        console.log(`Removed ${elementIds.length} elements from selection`);
        return true;
    }
    catch (error) {
        console.error('Error removing from selection:', error);
        return false;
    }
}
export function clearSelection(viewerId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.clearHighlighting();
        console.log("Selection cleared");
        return true;
    }
    catch (error) {
        console.error('Error clearing selection:', error);
        return false;
    }
}
export function getSelectedElements(viewerId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return [];
        }
        const selected = viewer.getProductsWithState(State.HIGHLIGHTED);
        console.log(`Found ${selected.length} selected elements`);
        return selected;
    }
    catch (error) {
        console.error('Error getting selected elements:', error);
        return [];
    }
}
export function addEventListener(viewerId, eventName, dotNetHelper) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        const handler = (args) => {
            const eventData = {
                eventName: eventName
            };
            if (args && typeof args === 'object') {
                if (args.id !== undefined)
                    eventData.id = args.id;
                if (args.model !== undefined)
                    eventData.model = args.model;
                if (args.xyz) {
                    eventData.x = args.xyz[0];
                    eventData.y = args.xyz[1];
                    eventData.z = args.xyz[2];
                }
                if (args.model !== undefined && args.tag !== undefined) {
                    eventData.modelId = args.model;
                    eventData.tag = args.tag;
                }
                if (args.message) {
                    eventData.message = args.message;
                }
            }
            dotNetHelper.invokeMethodAsync('OnViewerEvent', eventData);
        };
        viewer.on(eventName, handler);
        if (!eventHandlers.has(viewerId)) {
            eventHandlers.set(viewerId, new Map());
        }
        eventHandlers.get(viewerId).set(eventName, handler);
        console.log(`Registered event: ${eventName} for viewer ${viewerId}`);
        return true;
    }
    catch (error) {
        console.error(`Error registering event ${eventName}:`, error);
        return false;
    }
}
export function removeEventListener(viewerId, eventName) {
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
    }
    catch (error) {
        console.error(`Error unregistering event ${eventName}:`, error);
        return false;
    }
}
export function unloadModel(viewerId, modelId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return false;
            }
            viewer.unload(modelId);
            const models = loadedModels.get(viewerId);
            if (models) {
                models.delete(modelId);
            }
            console.log(`Model ${modelId} unloaded successfully`);
            return true;
        }
        catch (error) {
            console.error(`Error unloading model ${modelId}:`, error);
            return false;
        }
    });
}
export function getLoadedModels(viewerId) {
    const models = loadedModels.get(viewerId);
    if (!models) {
        return [];
    }
    return Array.from(models.values());
}
export function debugGetAllProducts(viewerId) {
    const viewer = viewerInstances.get(viewerId);
    if (!viewer) {
        return { error: "Viewer not found" };
    }
    const result = {
        handles: [],
        productsByState: {}
    };
    const viewerAny = viewer;
    if (viewerAny._handles) {
        result.handles = viewerAny._handles.map((h, idx) => {
            var _a, _b;
            return ({
                index: idx,
                id: h === null || h === void 0 ? void 0 : h.id,
                hasModel: !!(h === null || h === void 0 ? void 0 : h._model),
                productCount: ((_b = (_a = h === null || h === void 0 ? void 0 : h._model) === null || _a === void 0 ? void 0 : _a.products) === null || _b === void 0 ? void 0 : _b.length) || 0
            });
        });
    }
    const states = [State.UNDEFINED, State.HIDDEN, State.HIGHLIGHTED, State.XRAYVISIBLE, State.UNSTYLED];
    for (const state of states) {
        try {
            const products = viewer.getProductsWithState(state);
            result.productsByState[state] = {
                count: products.length,
                sample: products.slice(0, 5),
                models: [...new Set(products.map((p) => p.model))]
            };
        }
        catch (e) {
            result.productsByState[state] = { error: String(e) };
        }
    }
    return result;
}
export function setModelVisibility(viewerId, modelId, visible) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return false;
            }
            console.log(`Setting model ${modelId} visibility to ${visible}`);
            if (visible) {
                viewer.start(modelId);
            }
            else {
                viewer.stop(modelId);
            }
            return true;
        }
        catch (error) {
            console.error(`Error setting model ${modelId} visibility:`, error);
            return false;
        }
    });
}
const pluginInstances = new Map();
export function addPlugin(viewerId, pluginId, pluginType, config) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            const viewer = viewerInstances.get(viewerId);
            if (!viewer) {
                console.error(`Viewer with id ${viewerId} not found`);
                return false;
            }
            yield loadXbimViewer();
            const win = window;
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
            let stoppedValue = undefined;
            if (config && Object.keys(config).length > 0) {
                for (const [key, value] of Object.entries(config)) {
                    if (key === 'stopped') {
                        stoppedValue = value;
                        continue;
                    }
                    try {
                        plugin[key] = value;
                    }
                    catch (err) {
                        console.warn(`Could not set plugin.${key}:`, err);
                    }
                }
            }
            viewer.addPlugin(plugin);
            if (stoppedValue !== undefined) {
                plugin.stopped = stoppedValue;
            }
            else if ('stopped' in plugin) {
                plugin.stopped = false;
            }
            if (!pluginInstances.has(viewerId)) {
                pluginInstances.set(viewerId, new Map());
            }
            pluginInstances.get(viewerId).set(pluginId, plugin);
            console.log(`Plugin ${pluginType} (${pluginId}) added successfully`);
            return true;
        }
        catch (error) {
            console.error(`Error adding plugin ${pluginType}:`, error);
            return false;
        }
    });
}
export function removePlugin(viewerId, pluginId) {
    return __awaiter(this, void 0, void 0, function* () {
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
            viewer.removePlugin(plugin);
            plugins.delete(pluginId);
            console.log(`Plugin ${pluginId} removed successfully`);
            return true;
        }
        catch (error) {
            console.error(`Error removing plugin ${pluginId}:`, error);
            return false;
        }
    });
}
export function setPluginStopped(viewerId, pluginId, stopped) {
    return __awaiter(this, void 0, void 0, function* () {
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
        }
        catch (error) {
            console.error(`Error setting plugin stopped state:`, error);
            return false;
        }
    });
}
export function getActivePlugins(viewerId) {
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
export function unclip(viewerId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        viewer.unclip();
        return true;
    }
    catch (error) {
        console.error(`Error unclipping viewer:`, error);
        return false;
    }
}
export function createSectionBox(viewerId, pluginId) {
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
        const meter = viewer.activeHandles[0].meter;
        const cx = centre[0];
        const cy = centre[1];
        const cz = centre[2];
        const ex = Math.min(3 * meter, Math.abs(maxX - minX) / 5);
        const ey = Math.min(3 * meter, Math.abs(maxY - minY) / 5);
        const ez = Math.min(3 * meter, Math.abs(maxZ - minZ) / 5);
        const planes = [
            {
                direction: [0, 0, 1],
                location: [cx, cy, cz + ez]
            },
            {
                direction: [0, 0, -1],
                location: [cx, cy, cz - ez]
            },
            {
                direction: [1, 0, 0],
                location: [cx + ex, cy, cz]
            },
            {
                direction: [-1, 0, 0],
                location: [cx - ex, cy, cz]
            },
            {
                direction: [0, -1, 0],
                location: [cx, cy - ey, cz]
            },
            {
                direction: [0, 1, 0],
                location: [cx, cy + ey, cz]
            }
        ];
        viewer.sectionBox.setToPlanes(planes);
        plugin.setClippingPlanes(planes);
        viewer.zoomTo();
        plugin.stopped = false;
        return true;
    }
    catch (error) {
        console.error(`Error creating section box:`, error);
        return false;
    }
}
export function clearSectionBox(viewerId, pluginId) {
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
                plugin.stopped = true;
            }
        }
        viewer.sectionBox.clear();
        viewer.zoomTo();
        return true;
    }
    catch (error) {
        console.error(`Error clearing section box:`, error);
        return false;
    }
}
export function disposeViewer(viewerId) {
    try {
        const viewer = viewerInstances.get(viewerId);
        if (!viewer) {
            console.error(`Viewer with id ${viewerId} not found`);
            return false;
        }
        const handlers = eventHandlers.get(viewerId);
        if (handlers) {
            handlers.forEach((handler, eventName) => {
                viewer.off(eventName, handler);
            });
            eventHandlers.delete(viewerId);
        }
        const plugins = pluginInstances.get(viewerId);
        if (plugins) {
            plugins.forEach((plugin) => {
                viewer.removePlugin(plugin);
            });
            pluginInstances.delete(viewerId);
        }
        loadedModels.delete(viewerId);
        viewer.stop();
        viewerInstances.delete(viewerId);
        return true;
    }
    catch (error) {
        console.error('Error disposing viewer:', error);
        return false;
    }
}
//# sourceMappingURL=xbimViewerInterop.js.map