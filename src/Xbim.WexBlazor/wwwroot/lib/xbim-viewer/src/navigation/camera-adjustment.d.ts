import { Viewer } from "../viewer";
export declare class CameraAdjustment {
    private viewer;
    private request;
    private density;
    private lastMvChange;
    private dirty;
    private stopped;
    /**
     * This object can be used to keep orthographic height aligned with the current perspective view and to
     * keep camera position (distance) right for orthographic view
     */
    constructor(viewer: Viewer, request: (callback: FrameRequestCallback) => number, density: number);
    stop(): void;
    watch(latency?: number): void;
    adjust(density?: number): void;
    private trimmedMean;
}
