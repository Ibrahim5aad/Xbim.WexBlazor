export declare class FpsWatch {
    private buffer;
    /**
     *
     */
    constructor(frameRequest: (callback: FrameRequestCallback) => number);
    get fps(): number;
}
