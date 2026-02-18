import { IPlugin } from "../plugin";
import { Viewer } from "../../viewer";
/**
 * @category Plugins
 */
export declare class LoaderOverlay implements IPlugin {
    private viewer;
    private overlay;
    init(viewer: Viewer): void;
    onBeforeDraw(width: number, height: number): void;
    onAfterDraw(width: number, height: number): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
    onAfterDrawModelId(): void;
    show(image?: HTMLImageElement): void;
    hide(): void;
    /**
     *
     */
    constructor();
}
