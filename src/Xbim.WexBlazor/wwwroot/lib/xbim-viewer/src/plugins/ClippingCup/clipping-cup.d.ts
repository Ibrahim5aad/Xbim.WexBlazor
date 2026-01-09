import { IPlugin } from "../plugin";
import { Viewer } from "../../viewer";
export declare class ClippingCup implements IPlugin {
    private viewer;
    private caps;
    private activeHandles;
    init(viewer: Viewer): void;
    onAfterDraw(width: number, height: number): void;
    onBeforeDraw(width: number, height: number): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
    onAfterDrawModelId(): void;
}
