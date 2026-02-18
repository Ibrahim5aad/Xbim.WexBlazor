import { IHeatmapChannel, ChannelType } from "./heatmap-channel";
/**
 * A continuous heatmap channel is to used to represent a range of continous values.
 *
 * @implements {IHeatmapChannel}
 */
export declare class ContinuousHeatmapChannel implements IHeatmapChannel {
    private _channelType;
    private _channelId;
    private _dataType;
    private _name;
    private _description;
    private _property;
    private _unit;
    private _min;
    private _max;
    private _colorGradient;
    private _enabled;
    /**
     * Creates an instance of ContinuousHeatmapChannel.
     *
     * @param {string} channelId - A user-defined unique identifier for the channel.
     * @param {string} dataType - The data type of the channel values.
     * @param {string} name - The name of the channel.
     * @param {string} description - A brief description of the channel.
     * @param {string} property - The data property represented by this channel.
     * @param {string} unit - The unit of measurement for the channel.
     * @param {number} min - The minimum value for the data range represented by this channel.
     * @param {number} max - The maximum value for the data range represented by this channel.
     * @param {string[]} colorGradient - A list of hex color gradient stops used to represent the data values carried through this channel.
     */
    constructor(channelId: string, dataType: string, name: string, description: string, property: string, unit: string, min: number, max: number, colorGradient: string[]);
    /**
     * Gets the type of the channel.
     * @returns {ChannelType} The type of the channel.
     */
    get channelType(): ChannelType;
    /**
     * Gets the unique identifier for the channel.
     * @returns {string} The channel ID.
     */
    get channelId(): string;
    /**
     * Gets the data type of the channel values.
     * @returns {string} The data type.
     */
    get dataType(): string;
    /**
     * Gets the name of the channel.
     * @returns {string} The name of the channel.
     */
    get name(): string;
    /**
     * Gets the description of the channel.
     * @returns {string} The description of the channel.
     */
    get description(): string;
    /**
     * Sets the description of the channel.
     * @param {string} value - The new description of the channel.
     */
    set description(value: string);
    /**
     * Gets the data property represented by this channel.
     * @returns {string} The data property represented by this channel.
     */
    get property(): string;
    /**
     * Gets the unit of measurement for the channel values.
     * @returns {string} The unit of measurement.
     */
    get unit(): string;
    /**
     * Sets the unit of measurement for the channel values.
     * @param {string} value - The new unit of measurement.
     */
    set unit(value: string);
    /**
     * Gets the minimum value for the continuous data range.
     * @returns {number} The minimum value.
     */
    get min(): number;
    /**
     * Sets the minimum value for the continuous data range.
     * @param {number} value - The new minimum value.
     */
    set min(value: number);
    /**
     * Gets the maximum value for the continuous data range.
     * @returns {number} The maximum value.
     */
    get max(): number;
    /**
     * Sets the maximum value for the continuous data range.
     * @param {number} value - The new maximum value.
     */
    set max(value: number);
    /**
     * Gets the hex color gradient stops used to represent the data values.
     * @returns {string[]} The hex color gradient  stops.
     */
    get colorGradient(): string[];
    /**
     * Sets the hex color gradient stops used to represent the data values.
     * @param {string[]} value - The new hex color gradient stops.
     */
    set colorGradient(value: string[]);
    /**
    * Gets a boolean value indicating if this channel is enabled
    * @returns {boolean} a value indicates if this channel is enabled.
    */
    get isEnabled(): boolean;
    /**
    * Sets if this channel is enabled or not
    * @param {boolean} value - a value indicates if this channel is enabled.
    */
    set isEnabled(value: boolean);
}
