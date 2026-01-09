import { Message } from "../common/message";
import { ProductMap } from "../common/product-map";
import { vec3 } from "gl-matrix";
/**
 * @category Core
 */
export type ReaderOptions = {
    /** Determines whether the viewer loaded will attempt to sort the geometry by size, during the load
    *
    * Size-sorted geometry provides a better experience when adaptive rendering is enabled,
    * since the smaller items are dropped from rendering first.
    *
    * For performance reasons, it is preferable to sort the geometry during wexbim generation,
    * but enabling this option will perform the sorting in the browser.
    */
    orderGeometryBySize?: boolean;
};
export declare class ModelGeometry {
    private progress;
    normals: Uint8Array;
    indices: Float32Array;
    products: Float32Array;
    transformations: Float32Array;
    styleIndices: Uint16Array;
    states: Uint8Array;
    vertices: Float32Array;
    matrices: Float32Array;
    styles: Uint8Array;
    meter: number;
    wcs: number[];
    private _iVertex;
    private _iIndexForward;
    private _iIndexBackward;
    private _iTransform;
    private _iMatrix;
    private _maxVersionSupported;
    productMaps: {
        [id: number]: ProductMap;
    };
    productIdLookup: any[];
    regions: Region[];
    transparentIndex: number;
    breaks: {
        [percent: number]: number[];
    };
    private _reader;
    private _styleMap;
    private parse;
    /**
     * Get size of arrays to be square (usable for texture data)
     * @param arity
     * @param count
     */
    private square;
    private feedDataArrays;
    private readShape;
    load(source: any, headers: {
        [name: string]: string;
    }, progress: (message: Message) => void, options?: Partial<ReaderOptions>): void;
    onloaded: (geometry: ModelGeometry) => void;
    onerror: (message?: string) => void;
}
export declare class Region {
    population: number;
    centre: vec3;
    bbox: Float32Array;
    constructor(region?: Region);
    /**
     * Returns clone of this region
     */
    static clone(o: Region): Region;
    /**
     * Returns new region which is a merge of this region and the argument
     * @param region region to be merged
     */
    merge(region: Region): Region;
}
