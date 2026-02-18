import { IPlugin } from '../plugin';
import { Viewer } from '../../viewer';
export declare class InteractiveClipping implements IPlugin {
    onAfterDrawModelId(): void;
    private _viewer;
    get canvas(): HTMLCanvasElement;
    get width(): number;
    get height(): number;
    /**
    * This method is only active when interactive clipping is active. It stops interactive clipping operation.
    *
    * @function InteractiveClipping#stopClipping
    */
    stopClipping(): void;
    /**
    * Use this method to start interactive clipping of the model. This is based on SVG overlay
    * so SVG support is necessary for it. But as WebGL is more advanced technology than SVG it is sound assumption that it is present in the browser.
    * Use {@link Viewer.check Viewer.check()} to make sure it is supported at the very beginning of using of Viewer. Use {@link Viewer#unclip unclip()} method to
    * unset clipping plane.
    *
    * @function Viewer#clip
    * @fires Viewer#clipped
    */
    clip(): void;
    private getSVGOverlay;
    init(viewer: Viewer): void;
    onBeforeDraw(width: number, height: number): void;
    onAfterDraw(width: number, height: number): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
}
