import { vec3, mat4 } from "gl-matrix";
import { ClippingPlane } from "./bcf";
/**
 * @category Core
 */
export declare class SectionBox {
    private _boxVertices;
    private _location;
    private _lengthX;
    private _lengthY;
    private _lengthZ;
    private _rotationX;
    private _rotationY;
    private _rotationZ;
    private _onChange;
    constructor(onChange: () => void);
    get boxVertices(): vec3[];
    set boxVertices(value: vec3[]);
    get location(): vec3;
    set location(value: vec3);
    get rotationX(): number;
    set rotationX(value: number);
    get rotationY(): number;
    set rotationY(value: number);
    get rotationZ(): number;
    set rotationZ(value: number);
    get lengthX(): number;
    set lengthX(value: number);
    get lengthY(): number;
    set lengthY(value: number);
    get lengthZ(): number;
    set lengthZ(value: number);
    /**
     * Matrix representation of this box. Can be used for simple test of points.
     */
    getMatrix(wcs: vec3): mat4;
    /**
     * True when section box is set, false otherwise
     */
    get isSet(): boolean;
    /**
     * Alias for setToInfinity(). Makes the section box infinitely large so it doesn't crop anything in the view
     */
    clear(): void;
    /**
     * Sets frustum to maximum extens so that it shouldn't cut anything in the view.
     */
    setToInfinity(): void;
    /**
     * Sets all values for section box in one go as a copy from supplied box
     * @param box Section box values to be used
     */
    setToBox(box: SectionBox): void;
    setToBoundingBox(box: ArrayLike<number>): void;
    /**
     * Computes section box from 6 clipping planes if these form a box
     * @param planes 6 clipping planes representing the box
     * @returns {boolean} true if succeeded, false if planes don't form a box
     */
    setToPlanes(planes: ClippingPlane[]): boolean;
    private sortPlanes;
    private getSize;
    private toEulerAngles;
    private isOpposite;
    private getCentroid;
    private addPointToResults;
    private getDparam;
    private areOrthogonal;
    private _lastWcs;
    private _lastBBox;
    getVertices(wcs: vec3): vec3[];
    /**
     * Returns bounding box of the section box. This is usefull for
     * zooming and similar operations
     */
    getBoundingBox(wcs: vec3): Float32Array;
}
