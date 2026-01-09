import { IPlugin } from "../../plugin";
import { Viewer } from "../../../viewer";
import { IHeatmapChannel } from "./heatmap-channel";
import { HeatmapSource } from "./heatmap-source";
/**
 * @category Plugins
 */
export declare class Heatmap implements IPlugin {
    private _viewer;
    private _channels;
    private _sources;
    private _colorStylesMap;
    private _valueStylesMap;
    private _stopped;
    private _nextStyleId;
    get channels(): IHeatmapChannel[];
    get stopped(): boolean;
    set stopped(value: boolean);
    init(viewer: Viewer): void;
    addChannel(channel: IHeatmapChannel): void;
    addSource(source: HeatmapSource): void;
    renderChannel(channelId: string): void;
    getChannel(channelId: string): IHeatmapChannel;
    renderSource(sourceId: string): void;
    private renderChannelInternal;
    private renderConstantColorChannel;
    private renderDiscreteChannel;
    private renderValueRangesChannel;
    private renderContinuousChannel;
    private interpolateColor;
    private interpolateColorSegment;
    private componentToHex;
    private rgbaToHex;
    private hexToRgba;
    private clamp;
    private groupBy;
    onAfterDraw(width: number, height: number): void;
    onBeforeDraw(width: number, height: number): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
    onAfterDrawModelId(): void;
}
