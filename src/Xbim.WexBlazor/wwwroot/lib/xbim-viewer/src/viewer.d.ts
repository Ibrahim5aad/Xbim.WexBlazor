import { State } from './common/state';
import { ReaderOptions, Region } from './reader/model-geometry';
import { ModelHandle } from './model-handle';
import { Framebuffer } from './framebuffer';
import { Message } from './common/message';
import { IPlugin } from './plugins/plugin';
import { ViewerEventMap } from './common/viewer-event-map';
import { CheckResult } from './common/checkResult';
import { Animations } from './navigation/animations';
import { mat4, vec3 } from 'gl-matrix';
import { PerformanceRating } from './performance-rating';
import { CameraProperties, CameraType } from './camera';
import { SectionBox } from './section-box';
import { CameraAdjustment } from './navigation/camera-adjustment';
import { ProductAnalyticalResult } from './common/product-analytical-result';
export type NavigationMode = 'pan' | 'zoom' | 'orbit' | 'fixed-orbit' | 'free-orbit' | 'none' | 'look-around' | 'walk' | 'look-at' | 'locked-orbit';
/**
 * The xbim 3D viewer component enabling _*.wexbim_ files to be viewed with WebGL.
 *
 * * A minimal example of using the viewer would be
 * ```
 *   <canvas id="viewer" width="500" height="300"></canvas>
 *   <script type="text/javascript">
 *       var viewer = new Viewer('viewer');
 *       viewer.on('loaded', () => {
 *           viewer.show(ViewType.DEFAULT);
 *       });
 *       viewer.load('../data/SampleHouse.wexbim');
 *       viewer.start();
 *   </script>
 * ```
 * @category Core
 */
export declare class Viewer {
    gl: WebGLRenderingContext;
    glVersion: number;
    canvas: HTMLCanvasElement;
    private changed;
    private _navigationMode;
    /**
     * Switch between different navigation modes for left mouse button. Allowed values: <strong> 'pan', 'zoom', 'orbit' (or 'fixed-orbit') , 'free-orbit' and 'none'</strong>. Default value is <strong>'orbit'</strong>;
     * @member {String} Viewer#navigationMode
     */
    get navigationMode(): NavigationMode;
    set navigationMode(value: NavigationMode);
    get cameraProperties(): CameraProperties;
    get sectionBox(): SectionBox;
    get width(): number;
    set width(value: number);
    get height(): number;
    set height(value: number);
    /**
     * Type of camera to be used. Available values are <strong>'perspective'</strong> and <strong>'orthogonal'</strong> You can change this value at any time with instant effect.
     * @member {string} Viewer#camera
     */
    get camera(): CameraType;
    set camera(value: CameraType);
    /**
     * Array of four integers between 0 and 255 representing RGBA colour components. This defines background colour of the viewer. You can change this value at any time with instant effect.
     * @member {Number[]} Viewer#background
     */
    get background(): number[];
    set background(value: number[]);
    /**
     * Array of four integers between 0 and 255 representing RGBA colour components. This defines colour for highlighted elements. You can change this value at any time with instant effect.
     * @member {Number[]} Viewer#highlightingColour
     */
    get highlightingColour(): number[];
    set highlightingColour(value: number[]);
    /**
     * Array of four integers between 0 and 255 representing RGBA colour components. This defines colour for elements when hovered over. You can change this value at any time with instant effect.
     * @member {Number[]} Viewer#hoverPickColour
     */
    get hoverPickColour(): number[];
    set hoverPickColour(value: number[]);
    /**
     * Array of four integers between 0 and 255 representing RGBA colour components. This defines colour for xray mode rendering. You can change this value at any time with instant effect.
     * @member {Number[]} Viewer#xrayColour
     */
    get xrayColour(): number[];
    set xrayColour(value: number[]);
    /**
     * World matrix
     * @member {Number[]} Viewer#mvMatrix
     */
    get mvMatrix(): mat4;
    set mvMatrix(value: mat4);
    /**
     * Camera matrix (perspective or orthogonal)
     * @member {Number[]} Viewer#pMatrix
     */
    get pMatrix(): mat4;
    set pMatrix(value: mat4);
    /**
     * Switch between different rendering modes.
     * @member {String} Viewer#renderingMode
     */
    get renderingMode(): RenderingMode;
    set renderingMode(value: RenderingMode);
    /**
     * Allows to adjust gamma of the view
     */
    get gamma(): number;
    set gamma(value: number);
    /**
     * Allows to adjust contrast of the view
     */
    get contrast(): number;
    set contrast(value: number);
    /**
     * Allows to adjust brightness of the view
     */
    get brightness(): number;
    set brightness(value: number);
    /**
     * Returns number of milliseconds for which Model View Matris hasn't been changed.
     * This can be used for optimisations and actions to happen after vertain amount of time
     * after the last movement of the model.
     */
    get mvMatrixAge(): number;
    /**
     * Current performance based on FPS. Updated approx. every 500 ms
     */
    get performance(): PerformanceRating;
    set performance(value: PerformanceRating);
    /**
     * The viewer is watching the performance based on the FPS. When performance drops down, it can reduce amount
     * of geometry to be rendered. This is usefull for some navigation and animations but might not be convenient in all scenarios
     * like in a wals mode. This property can be used to switch the addaptive performance on and off.
     */
    get adaptivePerformanceOn(): boolean;
    set adaptivePerformanceOn(value: boolean);
    /**
     * Determines whether the viewer raised 'hoverpick' events when the mouse hovers over an element.
     * When enabled the currently hovered item will be be highlighted. The 'hoverpicked' event can be
     * used to provide tooltips/context menus etc.
     */
    get hoverPickEnabled(): boolean;
    set hoverPickEnabled(value: boolean);
    /**
     * Returns readonly array of plugins
     * @member {IPlugin[]} Viewer#plugins
     */
    get plugins(): IPlugin[];
    /**
     * Returns number of units in active model (1000 is model is in mm)
     * @member {Number} Viewer#unitsInMeter
     */
    get unitsInMeter(): number;
    /**
     * This can be used to adjust ortgographic and perspective camera properties
     * to be close. Particularly useful when restoring orthographic view with displaced camera.
     */
    get adjustments(): CameraAdjustment;
    /**
     * Number of milliseconds for animated zooming
     */
    zoomDuration: number;
    /**
     * Animations functionality
     */
    animations: Animations;
    /**
     * Returns a filtered array of currently active handles
     */
    get activeHandles(): ModelHandle[];
    private _camera;
    private _sectionBox;
    private _width;
    private _height;
    private _background;
    private _highlightingColour;
    private _hoverPickColour;
    private _xrayColour;
    private _mvMatrix;
    private _pMatrix;
    private _renderingMode;
    private _mvMatrixTimestamp;
    private _fpsWatch;
    private _isRunning;
    private _stateStyles;
    private _stateStyleTexture;
    private _plugins;
    private _handles;
    private _shaderProgram;
    private _mvMatrixUniformPointer;
    private _pMatrixUniformPointer;
    private _lightUniformPointer;
    private _colorCodingUniformPointer;
    private _renderingModeUniformPointer;
    private _highlightingColourUniformPointer;
    private _hoverPickColourUniformPointer;
    private _xrayColourUniformPointer;
    private _stateStyleSamplerUniform;
    private _gammaContrastBrightnessUniform;
    private _sectionBoxUniform;
    private _gamma;
    private _contrast;
    private _brightness;
    private _currentFps;
    private _performance;
    private _adaptivePerformanceOn;
    private _hoverPickEnabled;
    private _events;
    private _canvasListeners;
    private _pointers;
    private _requestAnimationFrame;
    /**
     * Holds reference to Floating Point Texture extension needed for WebGL1 implementations
     */
    private _fptExtension;
    private _cameraAdjustment;
    private _readerOptions;
    /**
     * Options that can control the behaviour of the geometry loader
     */
    get readerOptions(): ReaderOptions;
    /**
     * Holds reference to Depth Texture extension needed for WebGL2 implementations to get 3D coordinate of
     * user interactions (click, pick, touch etc.)
     */
    private _depthTextureExtension;
    get hasDepthSupport(): boolean;
    /**
     * Indicates if the viewer is running the rendering loop
     */
    get isRunning(): boolean;
    lockedOrbitOrigin: vec3;
    /**
    * This is constructor of the xBIM Viewer. It gets HTMLCanvasElement or string ID as an argument. Viewer will than be initialized
    * in the context of specified canvas. Any other argument will throw exception.
    *
    * If any Zone technology is in use, this constructor should run in the root zone without any additional load. It has global event
    * handlers and hooks in document.mousemove, document.keyup, requestAnimationFrame and otherw which need to run fast and should not
    * cause any additional sideeffects (like data binding refresh in Angular)
    * @name Viewer
    * @constructor
    * @classdesc This is the main and the only class you need to load and render IFC models in wexBIM format. This viewer is part of
    * xBIM toolkit which can be used to create wexBIM files from IFC, ifcZIP and ifcXML. WexBIM files are highly optimized for
    * transmition over internet and rendering performance. Viewer uses WebGL technology for hardware accelerated 3D rendering and SVG for
    * certain kinds of user interaction. This means that it won't work with obsolete and non-standard-compliant browsers like IE10 and less.
    *
    * @param {string | HTMLCanvasElement} canvas - string ID of the canvas or HTML canvas element.
    * @param {({string})} errorHandler - an error handler
    */
    constructor(canvas: string | HTMLCanvasElement, errorHandler?: ({ message }: {
        message: any;
    }) => void);
    /**
    * This is a static function which should always be called before Viewer is instantiated.
    * It will check all prerequisites of the viewer and will report all issues. If CheckResult.errors contains
    * any messages viewer won't work. If CheckResult.warnings contain any messages it will work but some
    * functions may be restricted or may not work or it may have poor performance.
    * @function Viewer.check
    * @return {CheckResult}
    */
    static check(): CheckResult;
    /**
    * Adds plugin to the viewer. Plugins can implement certain methods which get called in certain moments in time like
    * before draw, after draw etc. This makes it possible to implement functionality tightly integrated into Viewer like navigation cube or others.
    * @function Viewer#addPlugin
    * @param {IPlugin} plugin - plug-in object
    */
    addPlugin(plugin: IPlugin): void;
    /**
    * Removes plugin from the viewer. Plugins can implement certain methods which get called in certain moments in time like
    * before draw, after draw etc. This makes it possible to implement functionality tightly integrated into Viewer like navigation cube or others.
    * @function Viewer#removePlugin
    * @param {object} plugin - plug-in object
    */
    removePlugin(plugin: IPlugin): void;
    /**
    * Use this function to define up to 224 optional styles which you can use to change appearance of products and types if you pass the index specified in this function to {@link Viewer#setState setState()} function.
    * @function Viewer#defineStyle
    * @param {Number} index - Index of the style to be defined. This has to be in range 0 - 224. Index can than be passed to change appearance of the products in model
    * @param {Number[]} colour - Array of four numbers in range 0 - 255 representing RGBA colour. If there are less or more numbers exception is thrown.
    */
    defineStyle(index: number, colour: number[]): void;
    /**
    * You can use this function to change state of products in the model. State has to have one of values from {@link State State} enumeration.
    * Target is either enumeration from {@link ProductType ProductType} or array of product IDs. If you specify type it will effect all elements of the type.
    *
    * @function Viewer#setState
    * @param {State} state - One of {@link State State} enumeration values.
    * @param {Number} [modelId] - Id of the model
    * @param {Number[] | Number} target - Target of the change. It can either be array of product IDs or product type from {@link ProductType ProductType}.
    */
    addState(state: State, target: number | number[], modelId?: number): void;
    setState(state: State, target: number | number[], modelId?: number): void;
    removeState(state: State, target: number | number[], modelId?: number): void;
    /**
     * Sets state of the given item to be hovered over, unsetting an previous
     * item. Setting the id to undefined will clear the current.
     * @param id The product Id
     * @param model Id of the model
     */
    setHoverPick(id: number, model: number): void;
    getProductsWithState(state: State): {
        id: number;
        model: number;
    }[];
    /**
   * Use this function to get state of the products in the model. You can compare result of this function
   * with one of values from {@link State State} enumeration. 0xFF is the default value.
   *
   * @function Viewer#getState
   * @param {Number} id - Id of the product. You would typically get the id from {@link ViewerEventMap#pick pick} event or similar event.
   * @param {Number} [modelId] - Id of the model
   */
    getState(id: number, modelId?: number): number;
    /**
    * Use this function to reset state of all products to 'UNDEFINED' which means visible and not highlighted.
    * You can use optional hideSpaces parameter if you also want to show spaces. They will be hidden by default.
    *
    * @function Viewer#resetStates
    * @param {Number[] | Number} target - Target of the change. It can either be array of product IDs or product type from {@link ProductType ProductType}.
    * @param {Number} [modelId = null] - Optional Model ID. Id no ID is specified states are reset for all models.
    */
    resetState(target: number | number[], modelId?: number): void;
    /**
     * Clears all highlighting in all visible models
     */
    clearHighlighting(): void;
    /**
     * Executes callback for one model if modelId is specified or for all handles.
     * If no modelId is specified, last result will get returned, not an aggregation.
     * @param callback Function to execute
     * @param modelId ID of the model
     */
    private forHandleOrAll;
    private getHandle;
    getCurrentImageHtml(width?: number, height?: number): HTMLImageElement;
    getCurrentImageDataUrl(width?: number, height?: number, type?: 'png' | 'jpeg'): string;
    getCurrentImageDataArray(width?: number, height?: number): Uint8ClampedArray;
    getCurrentImageBlob(callback: (blob: Blob) => void): void;
    getProductsStates(modelId: number): {
        id: number;
        states: State[];
    }[];
    restoreProductsStates(modelId: number, stateMap: {
        id: number;
        states: State[];
    }[]): void;
    /**
     * Gets complete model state and style. Resulting object can be used to restore the state later on.
     *
     * @param {Number} modelId - Model ID which you can get from {@link ViewerEventMap#loaded loaded} event.
     * @returns {Array} - Array representing model state in compact form suitable for serialization
     */
    getModelState(modelId: number): number[][];
    /**
     * Restores model state from the data previously captured with {@link Viewer#getModelState getModelState()} function
     * @param {Number} modelId - ID of the model
     * @param {Array} state - State of the model as obtained from {@link Viewer#getModelState getModelState()} function
     */
    restoreModelState(modelId: number, state: number[][]): void;
    /**
    * Use this method for restyling of the model. This doesn't change the default appearance of the products so you can think about it as an overlay. You can
    * remove the overlay if you set the style to {@link State#UNSTYLED State.UNSTYLED} value. You can combine restyling and hiding in this way.
    * Use {@link Viewer#defineStyle defineStyle()} to define styling first.
    *
    * @function Viewer#setStyle
    * @param style - style defined in {@link Viewer#defineStyle defineStyle()} method
    * @param {Number[] | Number} target - Target of the change. It can either be array of product IDs or product type from {@link ProductType ProductType}.
    * @param {Number} [modelId] - Optional ID of a specific model.
    */
    setStyle(style: number, target: number | number[], modelId?: number): void;
    /**
    * Use this function to get overriding colour style of the products in the model. The number you get is the index of
    * your custom colour which you have defined in {@link Viewer#defineStyle defineStyle()} function. 0xFF is the default value.
    *
    * @function Viewer#getStyle
    * @param {Number} id - Id of the product. You would typically get the id from {@link ViewerEventMap#pick pick event} or similar event.
    * @param {Number} [modelId] - Optional Model ID. If not defined first style available for a product with certain ID will be returned. This might be ambiguous.
    * @returns {Number} Returns style index or 255 (State.UNDEFINED) if there is no overriding style defined for the product. Use `viewer.defineStyle()` to define overriding style and `viewer.setStyle()` to set it for a product.
    */
    getStyle(id: number, modelId?: number): number;
    /**
    * Use this function to reset appearance of all products to their default styles.
    *
    * @function Viewer#resetStyles
    * @param {Number} [modelId] - Optional ID of a specific model.
    */
    resetStyles(modelId?: number): void;
    /**
    *
    * @function Viewer#getProductType
    * @param {Number} prodID - Product ID. You can get this value either from semantic structure of the model or by listening to {@link ViewerEventMap#pick pick} event.
    * @param {Number} [modelId] - Optional Model ID. If not defined first type of a product with certain ID will be returned. This might be ambiguous.
    * @return {Number} Product type ID. This is either null if no type is identified or one of {@link ProductType type ids}.
    */
    getProductType(prodId: number, modelId?: number): number;
    /**
    *
    * @function Viewer#getProductsOfType
    * @param {Number} typeId - Type ID.
    * @param {Number} [modelId] - Optional Model ID. If not defined first type of a product with certain ID will be returned. This might be ambiguous.
    * @return {Numbe[]} Products IDs.
    */
    getProductsOfType(typeId: number, modelId?: number): number[];
    /**
    *
    * @function Viewer#getProductBoundingBox
    * @param {Number} prodID - Product ID. You can get this value either from semantic structure of the model or by listening to {@link ViewerEventMap#pick pick} event.
    * @param {Number} [modelId] - Optional Model ID. If not defined first type of a product with certain ID will be returned. This might be ambiguous.
    * @return {Float32Array} Bounding box of the product in model coordinates (not reduced by current WCS)
    */
    getProductBoundingBox(prodId: number, modelId?: number): Float32Array;
    getDistanceAndHeight(bBox: number[] | Float32Array, viewDirection: vec3, upDirection: vec3): {
        distance: number;
        height: number;
    };
    /**
    * This method returns specified product's bounding box or bbox of the current acite models if no product ID is specified.
    * @function Viewer#setCameraTarget
    * @param {Number} prodId [optional] Product ID. You can get ID either from semantic structure of the model or from {@link ViewerEventMap#pick pick event}.
    * @param {Number} [modelId] - Optional ID of a specific model.
    * @return {number[]} Returns bounding box of the target, null if not found
    */
    getTargetBoundingBox(prodId?: number, modelId?: number): number[] | Float32Array;
    getTargetsBoundingBox(targets: {
        id: number;
        model: number;
    }[]): number[] | Float32Array;
    getMergedRegion(): Region;
    getMergedRegionWcs(): Region;
    /**
    * This method can be used for batch setting of viewer members. It doesn't check validity of the input.
    * @function Viewer#set
    * @param {Object} settings - Object containing key - value pairs
    */
    set(settings: Partial<Viewer>): void;
    /**
    * This method uses WebWorker if available to load the model into this viewer.
    * Model has to be either URL to wexBIM file or Blob or File representing wexBIM file binary data. Any other type of argument will throw an exception.
    * You can load more than one model if they occupy the same space, use the same scale and have unique product IDs. Duplicated IDs won't affect
    * visualization itself but would cause unexpected user interaction (picking, zooming, ...).
    * @function Viewer#load
    * @param {String | Blob | File} model - Model has to be either URL to wexBIM file or Blob or File representing wexBIM file binary data.
    * @param {Any} tag [optional] - Tag to be used to identify the model in {@link ViewerEventMap#loaded loaded} event.
    * @param {Object} headers [optional] - Headers to be used for request. This can be used for authorized access for example.
    * @fires Viewer#loaded
    */
    loadAsync(model: string | Blob | File, tag?: any, headers?: {
        [name: string]: string;
    }, progress?: (message: Message) => void): void;
    /**
    * This method is used to load model data into viewer. Model has to be either URL to wexBIM file or Blob or File representing wexBIM file binary data. Any other type of argument will throw an exception.
    * Region extend is determined based on the region of the model
    * Default view if 'front'. If you want to define different view you have to set it up in handler of {@link ViewerEventMap#loaded loaded} event. <br>
    * You can load more than one model if they occupy the same space, use the same scale and have unique product IDs. Duplicated IDs won't affect
    * visualization itself but would cause unexpected user interaction (picking, zooming, ...)
    * @function Viewer#load
    * @param {String | Blob | File} model - Model has to be either URL to wexBIM file or Blob or File representing wexBIM file binary data.
    * @param {Any} tag [optional] - Tag to be used to identify the model in {@link ViewerEventMap#loaded loaded} event.
    * @param {Object} headers [optional] - Headers to be used for request. This can be used for authorized access for example.
    * @param {Function} progress [optional] - Progress reporting delegate
    * @fires Viewer#loaded
    */
    load(model: string | Blob | File, tag?: any, headers?: {
        [name: string]: string;
    }, progress?: (message: Message) => void): void;
    private addHandle;
    /**
     * Sets camera parameters (near and far clipping planes) from current active models.
     * This should be called whenever active models are very different (size, units)
     */
    setNearAndFarFromCurrentModels(): void;
    /**
     * Unloads model from the GPU. This action is not reversible.
     *
     * @param {Number} modelId - ID of the model which you can get from {@link ViewerEventMap#loaded loaded} event.
     */
    unload(modelId: number): void;
    private _initShaders;
    private setActive;
    private _initAttributesAndUniforms;
    private _disableContextMenu;
    private get meter();
    navigate(type: NavigationMode, deltaX: number, deltaY: number, origin: vec3): void;
    private _lastWcs;
    /**
     * Gets current WCS displacement used for visualisation
     */
    getCurrentWcs(): vec3;
    /**
    * This is the main draw method. You can use it if you just want to render model once with no navigation and interaction.
    * If you want interactive model, call {@link Viewer#start start()} method.
    * @function Viewer#draw
    * @fires Viewer#frame
    */
    draw(framebuffer?: Framebuffer, disablePlugins?: boolean): void;
    private getLightPosition;
    private drawXRAY;
    private updatePMatrix;
    private getCameraDistanceFromRegion;
    /**
    * Use this method to get actual camera position.
    * @function Viewer#getCameraPosition
    */
    getCameraPosition(): vec3;
    /**
     * Use this method to get camera position in the WCS coordinates. Useful for persistance
     * accross executions.
     */
    getCameraPositionWcs(): vec3;
    /**
     * Use this method to get camera direction
    * @function Viewer#getCameraDirection
     */
    getCameraDirection(): vec3;
    /**
     * Use this method to get camera direction
    * @function Viewer#getCameraHeading
     */
    getCameraHeading(): vec3;
    /**
    * Use this method to zoom to specified element. If you don't specify a product ID it will zoom to full extent. If you specify list of products,
    * this function will zoom to grouped bounding box. You should use this only for elements which are close to each other (like aggregations)
    * @function Viewer#zoomTo
    * @param {Number | Array<{id: number, model: number}>} [target] Optional product ID or a list of products in models
    * @param {Number} [model] Optional model ID
    * @param {boolean} withAnimation - Optional parameter, default is 'true'. When true, transition to the view is animated. When false, view is changed imediately.
    * @param {boolean} checkVisibility - Optional parameter, default is 'true'. When true, viewer attempts to fing the best view angle where the target is best visible. This is an expensive operation.
    * @return {boolean} True if target exists and zoom was successful, False otherwise
    */
    zoomTo(target?: number | {
        id: number;
        model: number;
    }[], model?: number, withAnimation?: boolean, checkVisibility?: boolean): Promise<void>;
    /**
    * Use this function to show default views.
    *
    * @function Viewer#show
    * @param {ViewType} type - Type of view. Allowed values are <strong>'top', 'bottom', 'front', 'back', 'left', 'right'</strong>.
    * Directions of this views are defined by the coordinate system. The camera target and distance is determined by 'id' param, if present, or the full model extent when omitted
    * @param {number} id - Optional parameter indicating the product ID to target the camera at
    * @param {number} model - Optional parameter indicating the model ID the product is in
    * @param {boolean} withAnimation - Optional parameter, default is 'true'. When true, transition to the view is animated. When false, view is changed imediately.
    */
    show(type: ViewType, id?: number, model?: number, withAnimation?: boolean): Promise<void>;
    /**
     * Starts rotation around to present the model
     */
    startRotation(): void;
    /**
     * Stops rotation of the model
     */
    stopRotation(): void;
    /**
     *
     * @param msg Fires error event. This might be used by plugins.
     */
    error(msg: any): void;
    getInteractionOrigin(event: MouseEvent | Touch): vec3;
    getEventDataFromEvent(event: MouseEvent | Touch, raw?: boolean): {
        id: number;
        model: number;
        xyz: vec3;
    };
    private static noData;
    getEventData(x: number, y: number): {
        id: number;
        model: number;
        xyz: vec3;
    };
    private _eventFB;
    /**
     * This renders the colour coded model into the memory buffer
     * not to the canvas and use it to identify ID, model id and 3D location at canvas location [x,y]
     *
     * @param {number} x - X coordinate on the canvas
     * @param {number} y - Y coordinate on the canvas
     */
    getEventDataRaw(x: number, y: number): {
        renderId: number;
        modelId: number;
        location: vec3;
    };
    /**
     * This renders the colour coded model into the memory buffer
     * not to the canvas and use it to identify ID, model id and 3D location at canvas location [x,y]
     *
     * @param {number} x - X coordinate on the canvas
     * @param {number} y - Y coordinate on the canvas
     */
    getData(points: {
        x: number;
        y: number;
    }[]): {
        id: number;
        model: number;
    }[];
    /**
     * Stops all models and only shows a single product
     * @param {Number[]} productId Product IDs
     * @param {Number} modelId Model ID
     */
    isolate(productIds: number[], modelId: number): void;
    /**
     * Gets list of isolated product IDs
     * @param modelId
     */
    getIsolated(modelId: number): number[];
    /**
     * Starts the loop watching animation frames and keeping record of the
     * Frames per Second (FPS) rate. This should only be called once in the constructor.
     */
    private setupFpsReporting;
    /**
     * This starts watching age of MV matrix and FPS and is setting
     * rating to decide if complete model should be drawn or only a part.
     */
    private _watchPerformance;
    /**
     * Use this function to start animation of the model. If you start animation before geometry is loaded it will wait for content to render it.
     * This function is bound to browser framerate of the screen so it will stop consuming any resources if you switch to another tab.
     *
     * @function Viewer#start
     * @param {Number} modelId [optional] - Optional ID of the model to be stopped. You can get this ID from {@link ViewerEventMap#loaded loaded} event.
     * @param {Number} productId [optional] - Optional ID of the product. If specified, only this product will be presented.
     *                                        This is highly optimal because it doesn't even touch other data in the model
     */
    start(modelId?: number): void;
    /**
    * Use this function to stop animation of the model. User will still be able to see the latest state of the model. You can
    * switch animation of the model on again by calling {@link Viewer#start start()}.
    *
    * @function Viewer#stop
    * @param {Number} id [optional] - Optional ID of the model to be stopped. You can get this ID from {@link ViewerEventMap#loaded loaded} event.
    */
    stop(id?: number): void;
    /**
    * Use this function to stop all models. You can
    * switch animation of the model on again by calling {@link Viewer#start start()}.
    *
    * @function Viewer#stopAll
    */
    stopAll(): void;
    /**
     * Checks if the model with defined ID is currently loaded and running
     *
     * @param {number} id - Model ID
     * @returns {boolean} True if model is loaded and running, false otherwise
     */
    isModelOn(id: number): boolean;
    /**
     * Checks if product with this ID exists in the model
     * @param productId
     * @param modelId
     */
    isProductInModel(productId: number, modelId: number): boolean;
    /**
     * Checks if the model with defined ID is currently loaded in the viewer.
     *
     * @param {number} id - Model ID
     * @returns {boolean} True if model is loaded, false otherwise
     */
    isModelLoaded(id: number): boolean;
    /**
    * Use this function to start all models. You can
    * switch animation of the model off by calling {@link Viewer#stop stop()}.
    *
    * @function Viewer#startAll
    */
    startAll(): void;
    /**
    * Use this function to stop picking of the objects in the specified model. It will behave as if not present for all picking operations.
    * All models are pickable by default when loaded.
    *
    * @function Viewer#stopPicking
    * @param {Number} id - ID of the model to be stopped. You can get this ID from {@link ViewerEventMap#loaded loaded} event.
    */
    stopPicking(id: number): void;
    /**
    * Use this function to enable picking of the objects in the specified model.
    * All models are pickable by default when loaded. You can stop the model from being pickable using {@link Viewer#stopPicking} function.
    *
    * @function Viewer#startPicking
    * @param {Number} id - ID of the model to be stopped. You can get this ID from {@link ViewerEventMap#loaded loaded} event.
    */
    startPicking(id: number): void;
    /**
     * Returns true if model participates in picking, false otherwise
     * @param {number} modelId ID of the model
     */
    isPickable(modelId: number): boolean;
    /**
     * Use this method to register to events of the viewer like {@link ViewerEventMap#pick:pick}, {@link ViewerInteractionEventMap#mousedown},
     * {@link ViewerInteractionEventMap:loaded} and others. You can define arbitrary number
     * of event handlers for any event. You can remove handler by calling {@link Viewer#off off()} method.
     *
     * @function Viewer#on
     * @param {String} eventName - Name of the event you would like to listen to.
     * @param {Object} handler - Callback handler of the event which will consume arguments and perform any custom action.
    */
    on<K extends keyof ViewerEventMap>(eventName: K, handler: (args: ViewerEventMap[K]) => void): void;
    /**
    * Use this method to unregister handlers from events. You can add event handlers by calling the {@link Viewer#on on()} method.
    *
    * @function Viewer#off
    * @param {String} eventName - Name of the event
    * @param {Object} callback - Handler to be removed
    */
    off(eventName: string, callback: any): void;
    fire<K extends keyof ViewerEventMap>(eventName: K, args: ViewerEventMap[K]): void;
    disableTextSelection(): void;
    enableTextSelection(): void;
    /**
   * Use this method to clip the model with A plane. Use {@link Viewer#unclip unclip()} method to
   * unset clipping plane.
   *
   * @function Viewer#clip
   * @param {Number[]} point - point in clipping plane
   * @param {Number[]} normal - normal pointing to the half space which will be hidden
   * @param {Number} [modelId] - Optional ID of the model to be clipped. All models are clipped otherwise.
   */
    clip(point: number[], normal: number[]): void;
    /**
   * Use this method to clip the model with A plane. Use {@link Viewer#unclip unclip()} method to
   * unset clipping plane.
   *
   * @function Viewer#setClippingPlaneA
   * @param {Number[]} plane - normal equation of the plane
   * @param {Number} [modelId] - Optional ID of the model to be clipped. All models are clipped otherwise.
   */
    setClippingPlaneA(plane: number[], modelId?: number): void;
    /**
   * Use this method to clip the model with B plane. Use {@link Viewer#unclip unclip()} method to
   * unset clipping plane.
   *
   * @function Viewer#setClippingPlaneB
   * @param {Number[]} plane - normal equation of the plane
   * @param {Number} [modelId] - Optional ID of the model to be clipped. All models are clipped otherwise.
   */
    setClippingPlaneB(plane: number[], modelId?: number): void;
    /**
    * This method will cancel any clipping plane if it is defined. Use {@link Viewer#clip clip()}
    * method to define clipping by point and normal of the plane.
    * @function Viewer#unclip
    * @param {Number} [modelId] - Optional ID of the model to be unclipped. All models are unclipped otherwise.
    */
    unclip(modelId?: number): void;
    /**
     * Gets clipping plane of the defined model or of the first clipping plane curently visible.
     * Bare in mind that every model might have different clipping plane
     * @param modelId Optional ID of the model which clipping plane we want to obtain
     */
    getClip(modelId?: number): {
        PlaneA: number[];
        PlaneB: number[];
    };
    /**
     * Counts number of triangles for every product. This is usefull to understand
     * performance of certain models. The default sort order is by number of triangles,
     * but density of triangles (number of triangles per a volume unit) might also be a good measure
     * to find the most expensive products to draw.
     * @param orderBy Measure to use for the default ascending sort order. Default value: 'triangles'
     * @returns List of product analytical results sorted in descendent order by number of triangles or density (number of triangles per volumetric unit of the product bounding box)
     */
    getProductAnalysis(orderBy?: 'triangles' | 'density'): ProductAnalyticalResult[];
    /**
     * Isolates products which have most geometry (number of triangles).
     * This function is meant for data debugging, identifying products which are likely to be over-detailed or poorly modeled.
     *
     * Use {@link Viewer#isolate isolate()} to cancel this operation
     * @param measure Measure to use for sorting and selection. Defaul value: 'triangles'
     * @param ratio Top ratio to isolate. Should be a number between 0.0 - 1.0. Default value is 0.2 which means top 20% of the geometry
     * @returns The set of product analytical results isolated, sorted in descendent order by number of triangles or density (number of triangles per volumetric unit of the product bounding box)
     */
    isolateHeavyProducts(measure?: 'triangles' | 'density', ratio?: number): ProductAnalyticalResult[];
    /**
     * This function can be used to place HTML markup relative to the centroid of a product
     * @param productId ID of the product
     * @param modelId ID of the model
     * @returns HTML coordinates of the centroid of the product bounding box
     */
    getHTMLPositionOfProductCentroid(productId: number, modelId: number): number[];
    /**
     * This function can be used to identify the XY location on the canvas for a given
     * 3D WCS vector.
     * For example this can be used to identity the XY location of a pick event in 3D space.
     * @param position a vec3 - see https://glmatrix.net/docs/module-vec3.html
     * @returns HTML coordinates of the provided WCS vector
     */
    getHtmlCoordinatesOfVector(position: vec3): number[];
    /**
     * Sets the provided product as the center of rotation
     * @param productId ID of the product
     * @param modelId ID of the model
     */
    setLockedOrbitOrigin(productId: number, modelId: number): void;
}
export declare enum RenderingMode {
    NORMAL = 0,
    GRAYSCALE = 1,
    XRAY = 2,
    XRAY_ULTRA = 4
}
/**
 * * @category Core
 */
export declare enum ViewType {
    TOP = 0,
    BOTTOM = 1,
    FRONT = 2,
    BACK = 3,
    LEFT = 4,
    RIGHT = 5,
    DEFAULT = 6
}
