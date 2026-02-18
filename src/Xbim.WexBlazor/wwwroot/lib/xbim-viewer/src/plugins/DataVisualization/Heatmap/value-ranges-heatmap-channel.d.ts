import { IHeatmapChannel, ChannelType } from "./heatmap-channel";
export declare class ValueRange {
    private _min;
    private _max;
    private _color;
    private _label;
    private _priority;
    /**
         * Constructor to initialize the ValueRange object.
         * @param {number} min - The minimum value.
         * @param {number} max - The maximum value.
         * @param {string} color - The color associated with the range.
         * @param {string} label - The label for the value range.
         * @param {number} priority - Priority of the value range.
         */
    constructor(min: number, max: number, color: string, label: string, priority: number);
    /**
     * Gets the minimum value in the range.
     * @returns {number} The minimum value.
     */
    get min(): number;
    /**
     * Sets the minimum value in the range.
     * @param {number} value - The minimum value to set.
     */
    set min(value: number);
    /**
     * Gets the maximum value in the range.
     * @returns {number} The maximum value.
     */
    get max(): number;
    /**
     * Sets the maximum value in the range.
     * @param {number} value - The maximum value to set.
     */
    set max(value: number);
    /**
     * Gets the color associated with this value range.
     * @returns {string} The color value.
     */
    get color(): string;
    /**
     * Sets the color associated with this value range.
     * @param {string} value - The color value to set.
     */
    set color(value: string);
    /**
     * Gets the label for the value range.
     * @returns {string} The label for the range.
     */
    get label(): string;
    /**
     * Sets the label for the value range.
     * @param {string} value - The label to set.
     */
    set label(value: string);
    /**
     * Priority of the value range
     * @returns {number} The priority of this range
     */
    get priority(): number;
    /**
     * Sets priority of the value range
     * @param priority Priority
     */
    set priority(priority: number);
}
/**
 * A value ranges heatmap channel is to used to represent a list of colored vlaue ranges.
 *
 * @implements {IHeatmapChannel}
 */
export declare class ValueRangesHeatmapChannel implements IHeatmapChannel {
    private _channelType;
    private _channelId;
    private _dataType;
    private _name;
    private _description;
    private _property;
    private _unit;
    private _ranges;
    private _enabled;
    /**
     * Creates an instance of ValueRangesHeatmapChannel.
     *
     * @param {string} channelId - A user-defined unique identifier for the channel.
     * @param {string} dataType - The data type of the channel values.
     * @param {string} name - The name of the channel.
     * @param {string} description - A brief description of the channel.
     * @param {string} property - The data property represented by this channel.
     * @param {string} unit - The unit of measurement for the channel.
     * @param {ValueRange[]} ranges - A list of ValueRange items used to represent the data values carried through this channel.
     */
    constructor(channelId: string, dataType: string, name: string, description: string, property: string, unit: string, ranges: ValueRange[]);
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
     * Gets list of ValueRange items used to represent the data values carried through this channel.
     * @returns {ValueRange[]} The value ranges.
     */
    get valueRanges(): ValueRange[];
    /**
     * Sets list of ValueRange items used to represent the data values carried through this channel.
     * @param {ValueRange[]} value - The new  value ranges.
     */
    set valueRanges(value: ValueRange[]);
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
