/**
 * The HeatmapSource class represents the producers of values for the heatmap.
 * Sources are responsible for providing and feeding the data to the IHeatmapChannel instances.
 */
export declare class HeatmapSource {
    private _id;
    private _products;
    private _channelId;
    private _value;
    private _enabled;
    /**
     * Creates an instance of HeatmapSource.
     *
     * @param {string} id - A unique identifier for the heatmap source.
     * @param {number[]} productId - The products associated with the source.
     * @param {string} channelId - The channel ID associated with the source.
     * @param {any} value - The value produced by the source.
     */
    constructor(id: string, productsIds: {
        id: number;
        model: number;
    }[], channelId: string, value: any);
    /**
     * Gets the unique identifier for the heatmap source.
     * @returns {string} The source ID.
     */
    get id(): string;
    /**
     * Gets the products associated with the source.
     * @returns {{ id: number, model: number }[] } The products.
     */
    get products(): {
        id: number;
        model: number;
    }[];
    /**
     * Gets the channel ID associated with the source.
     * @returns {string} The channel ID.
     */
    get channelId(): string;
    /**
     * Gets the value produced by the source.
     * @returns {any} The value.
     */
    get value(): any;
    /**
     * Sets the value produced by the source.
     * @param {any} value - The new value.
     */
    set value(value: any);
    /**
     * Gets a boolean value indicating if this source is enabled
     * @returns {boolean} a value indicates if this source is enabled.
     */
    get isEnabled(): boolean;
    /**
     * Sets if this source is enabled or not
     * @param {boolean} value - a value indicates if this source is enabled.
     */
    set isEnabled(value: boolean);
}
