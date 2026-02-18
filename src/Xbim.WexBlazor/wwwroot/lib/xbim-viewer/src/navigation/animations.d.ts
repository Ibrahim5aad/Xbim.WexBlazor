import { Viewer } from "../viewer";
import { mat4, vec3 } from "gl-matrix";
/**
 * @category Core
 */
export declare enum EasingType {
    LINEAR = 0,
    SINUS = 1,
    SINUS2 = 2,
    CIRCLE = 3
}
/**
 * @category Core
 */
export declare class Animations {
    private viewer;
    private requestAnimationFrame;
    private setTimeout;
    /**
     * Constructor to handle all animations
     */
    constructor(viewer: Viewer);
    /**
     * Stops all animations and clears the queues
     */
    clear(): void;
    private _rotationOn;
    startRotation(): Promise<void>;
    stopRotation(): void;
    private static viewQueue;
    /**
     * Animates transition from the current view to target view. Animations are queued and execuded in sequence.
     *
     * @param end Target model view matrix
     * @param duration Duration of the transition in milliseconds
     */
    viewTo(end: {
        mv: mat4;
        height: number;
    }, duration: number, easing?: EasingType): Promise<void>;
    private static zoomQueue;
    /**
     * Animates transition from the current view to target view
     *
     * @param end Target model view matrix
     * @param duration Duration of the transition in milliseconds
     */
    addZoom(distance: number, duration: number, direction?: vec3): Promise<void>;
    /**
     * Returns easing spread over circular path
     *
     * @param value value between 0.0 and 1.0
     */
    private getCircleEasing;
    private getSinEasing;
}
