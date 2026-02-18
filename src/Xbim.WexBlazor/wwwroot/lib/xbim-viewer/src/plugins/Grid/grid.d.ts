import { IPlugin } from '../plugin';
import { Viewer } from '../../viewer';
/**
 * @category Plugins
 */
export declare class Grid implements IPlugin {
    private viewer;
    private program;
    private vertex_buffer;
    private coordinatesAttributePointer;
    private mvUniformPointer;
    private pUniformPointer;
    private colourUniformPointer;
    private initialized;
    /**
     * Factor used to make the grid bigger than the current region
     */
    factor: number;
    /**
     * Fragment of Z height to be used as a Z offset bellow the model
     */
    zFactor: number;
    /**
     * Number of lines to be drawn
     */
    numberOfLines: number;
    /**
     * Colour of the grid
     */
    colour: number[];
    /**
     * Set to true to stop rendering of this plugin
     */
    get stopped(): boolean;
    set stopped(value: boolean);
    private _stopped;
    init(viewer: Viewer): void;
    private _initShader;
    onAfterDraw(width: number, height: number): void;
    onBeforeDraw(width: number, height: number): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
    onAfterDrawModelId(): void;
}
