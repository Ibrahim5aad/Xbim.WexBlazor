import { ModelGeometry, Region } from "./reader/model-geometry";
import { State } from "./common/state";
import { ModelPointers } from "./model-pointers";
import { Message } from "./common/message";
import { vec3 } from "gl-matrix";
import { ProductMap } from "./common/product-map";
import { ProductType } from "./product-type";
import { ProductAnalyticalResult } from "./common/product-analytical-result";
export declare class ModelHandle {
    private _gl;
    private _model;
    /**
     * Session unique ID used to manipulate this handle/model
     */
    id: number;
    /**
     * Tag used to identify the model
     */
    tag: any;
    /**
     * Conversion factor to one meter from model units
     */
    meter: number;
    /**
     * local World Coordinate System origin
     */
    wcs: vec3;
    /**
     * indicates if this model should be used in a rendering loop or not.
     */
    get stopped(): boolean;
    set stopped(value: boolean);
    private _stopped;
    /**
     * participates in picking operation only if true
     */
    pickable: boolean;
    /**
     * participates in clipping operation only if true
     */
    clippable: boolean;
    /**
     * indicates this model should never have drop items from rendering when Adaptive Performance is enabled
     */
    pinned: boolean;
    /**
     * If drawProductId is defined, only this single product is drawn
     */
    get isolatedProducts(): number[];
    set isolatedProducts(value: number[]);
    getRegion(wcs: vec3): Region;
    /**
     * Indicates if there are any changes to be drawn.
     * This flag is checked by the viewer to see if redraw is necessary
     */
    get changed(): boolean;
    private _changed;
    /**
     * Some models are empty - they don't contain any geometry
     */
    get empty(): boolean;
    private _empty;
    private _region;
    private _drawProductIds;
    private _numberOfIndices;
    private _vertexTextureSize;
    private _matrixTextureSize;
    private _styleTextureSize;
    private _vertexTexture;
    private _matrixTexture;
    private _styleTexture;
    private _normalBuffer;
    private _indexBuffer;
    private _productBuffer;
    private _styleBuffer;
    private _stateBuffer;
    private _transformationBuffer;
    private _clippingPlaneA;
    private _clippingPlaneB;
    private _clippingA;
    private _clippingB;
    private _glVersion;
    private get gl1();
    private get gl2();
    set clippingPlaneA(plane: number[]);
    get clippingPlaneA(): number[];
    set clippingPlaneB(plane: number[]);
    get clippingPlaneB(): number[];
    constructor(_gl: WebGLRenderingContext | WebGL2RenderingContext, _model: ModelGeometry, progress: (msg: Message) => void);
    private InitRegions;
    private recomputeCompleteRegion;
    private InitGlBuffersAndTextures;
    /**
     * Static counter to keep unique ID of the model handles
     */
    private static _instancesNum;
    setActive(pointers: ModelPointers, wcs: vec3): void;
    private transformPlane;
    draw(mode?: DrawMode, percent?: number): void;
    getProductId(renderId: number): number;
    getProductMap(id: number, wcs?: vec3): ProductMap;
    getProductMaps(ids: number[]): ProductMap[];
    getProductsOfType(type: ProductType): ProductMap[];
    unload(): any;
    private InitGPU;
    private bufferData;
    static bufferTexture(gl: WebGLRenderingContext | WebGL2RenderingContext, pointer: WebGLTexture, data: Float32Array | Uint8Array, numberOfComponents?: number): number;
    getState(id: number): State;
    getStates(): {
        id: number;
        states: State[];
    }[];
    getStyle(id: number): number;
    private getFirstSpan;
    addState(state: State, args: number | number[]): void;
    setState(state: State, args: number | number[]): void;
    clearHighlighting(): void;
    removeState(state: State, args: number | number[]): void;
    private checkStateArgs;
    resetState(args?: number | number[]): void;
    getProductsWithState(state: State): {
        id: number;
        model: number;
    }[];
    setStyle(styleId: number, args: number | number[]): void;
    private getMaps;
    resetStyles(): void;
    getModelState(): number[][];
    restoreModelState(states: number[][]): void;
    /**
     * Counts number of triangles for every product. This might be usefull to understand
     * performance of certain models.
     * @returns List of product analytical results
     */
    getProductAnalysis(results: ProductAnalyticalResult[]): ProductAnalyticalResult[];
}
export declare enum DrawMode {
    SOLID = 0,
    TRANSPARENT = 1
}
