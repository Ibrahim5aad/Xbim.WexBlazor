import { vec3 } from "gl-matrix";
/**
 * Static helper functions for bounding boxes
 */
export declare class BBox {
    static centre(a: ArrayLike<number>): number[];
    static union(a: ArrayLike<number>, b: ArrayLike<number>): number[];
    static intersection(a: ArrayLike<number>, b: ArrayLike<number>): number[];
    /**
     * Checks if bounding boxes are disjoint
     * @param a Bounding box A
     * @param b Bounding box B
     * @returns {boolean} Returns true when bounding boxes are disjoint
     */
    static areDisjoint(a: ArrayLike<number>, b: ArrayLike<number>): boolean;
    static getDiagonalSize(bBox: number[] | Float32Array): number;
    /**
     * Transforms axis aligned bounding box into current model view and returns width and height
     * @param bBox Axis aligned bounding box
     * @param viewDirection Direction of the view
     * @param upDirection Up direction of the camera
     */
    static getSizeInView(bBox: number[] | Float32Array, viewDirection: vec3, upDirection: vec3): {
        width: number;
        height: number;
        depth: number;
    };
}
