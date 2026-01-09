export declare class DepthReader {
    private gl;
    private glVersion;
    private texUniform;
    private vertAttrs;
    private program;
    private vertBuffer;
    private fragmentShader;
    private vertexShader;
    private vertices;
    /**
     *
     */
    constructor(gl: WebGLRenderingContext, glVersion: number);
    private draw;
    getDepths(points: {
        x: number;
        y: number;
    }[], tex: WebGLTexture, width: number, height: number): number[];
    getDepth(x: number, y: number, tex: WebGLTexture, width: number, height: number): number;
    /**
     * Deletes all WebGL resources
     */
    delete(): void;
}
