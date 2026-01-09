import { vec3 } from "gl-matrix";
export declare class Framebuffer {
    private gl;
    width: number;
    height: number;
    private withDepth;
    private glVersion;
    framebuffer: WebGLFramebuffer;
    renderbuffer: WebGLRenderbuffer;
    texture: WebGLTexture;
    depthTexture: WebGLTexture;
    isReady: boolean;
    private _disposed;
    private _depthReader;
    private _glVersion;
    private get depthReader();
    constructor(gl: WebGLRenderingContext, width: number, height: number, withDepth: boolean, glVersion: number);
    bind(): void;
    /**
     * Returns one pixel at defined position
     */
    getPixel(x: number, y: number): Uint8Array;
    getDepth(x: number, y: number): number;
    getDepths(points: {
        x: number;
        y: number;
    }[]): number[];
    getXYZArray(points: {
        x: number;
        y: number;
    }[]): vec3[];
    /**
     * Computes normalised X, Y, Z of event in the clip space
     *
     * @param x X coordinate of the event
     * @param y Y coordinate of the event
     */
    getXYZ(x: number, y: number): vec3;
    /**
     * Computes normalised X, Y, Z of event in the clip space with near and far bounds,
     * This can be used to refine near and far clipping planes for more precise evaluation
     * in the second pass.
     *
     * @param x X coordinate of the event
     * @param y Y coordinate of the event
     */
    getXYZRange(x: number, y: number): {
        near: vec3;
        middle: vec3;
        far: vec3;
    };
    getId(x: number, y: number): number;
    getIds(points: {
        x: number;
        y: number;
    }[]): number[];
    getImageDataArray(): Uint8ClampedArray;
    get2DCanvas(): HTMLCanvasElement;
    /**
     * Gets current image data from the framebuffer as Base64 string
     */
    getImageDataUrl(type?: 'png' | 'jpeg'): string;
    /**
     * Gets current image data as a BLOB. Use callback to handle data.
     * @param callback Use callback to handle Blob
     */
    getImageDataBlob(callback: (blob: Blob) => void, type?: 'png' | 'jpeg'): void;
    /**
     * Deletes all WebGL objects it holds
     */
    delete(): void;
}
