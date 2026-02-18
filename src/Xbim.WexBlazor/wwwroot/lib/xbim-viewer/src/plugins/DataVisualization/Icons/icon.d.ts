/**
 * Represents an icon that can be rendered using the Icons plugin
 *
 * @see Icons
 */
export declare class Icon {
    private _products;
    private _location;
    private _imageData;
    private _description;
    private _valueReadout;
    private _name;
    private _width;
    private _height;
    private _enabled;
    private _onIconSelected;
    private _movementQueue;
    private _isMoving;
    /**
     * Creates an instance of Icon.
     *
     * @param {string} name - The name of the icon.
     * @param {string} description - A brief description of the icon.
     * @param {string | null} valueReadout - A value readout of the icon.
     * @param {number} products - The products associated with the icon.
     * @param {string} imageData - Base64 encoded image data for the icon.
     * @param {Float32Array | null} [location=null] - The XYZ coordinates for the icon location. If not provided, the centroid of the product bounding box is used.
     * @param {number | null} [width=null] - The width of the icon. If null, default width is used.
     * @param {number | null} [height=null] - The height of the icon. If null, default height is used.
     * @param {() => void} [onIconSelected=null] - Callback function to be executed when the icon is selected.
     * @example
     * const icon = new Icon('Sample Icon', 'This is a sample icon.', 1, 101, 'imageDataString', new Float32Array([0, 0, 0]), 100, 100, () => console.log('Icon selected'));
     */
    constructor(name: string, description: string, valueReadout: string | null, products: {
        id: number;
        model: number;
    }[] | null, imageData: string | null, location?: Float32Array | null, width?: number | null, height?: number | null, onIconSelected?: () => void);
    /**
     * Gets the products associated with the icon.
     * @returns {{ id: number, model: number }[] } The products.
     */
    get products(): {
        id: number;
        model: number;
    }[] | null;
    /**
     * Gets the location of the icon.
     * @returns {Float32Array} The XYZ coordinates of the icon.
     */
    get location(): Float32Array;
    /**
     * Sets the location of the icon.
     * @param {Float32Array} value - The new XYZ coordinates for the icon.
     */
    set location(value: Float32Array);
    /**
     * Gets the Base64 encoded image data of the icon.
     * @returns {string} The Base64 encoded image data.
     */
    get imageData(): string;
    /**
     * Sets the Base64 encoded image data of the icon.
     * @param {string} value - The new Base64 encoded image data.
     */
    set imageData(value: string);
    /**
     * Gets the name of the icon.
     * @returns {string} The name of the icon.
     */
    get name(): string;
    /**
     * Sets the name of the icon.
     * @param {string} value - The new name of the icon.
     */
    set name(value: string);
    /**
     * Gets the description of the icon.
     * @returns {string} The description of the icon.
     */
    get description(): string;
    /**
     * Sets the description of the icon.
     * @param {string} value - The new description of the icon.
     */
    set description(value: string);
    /**
     * Gets the value readout of the icon.
     * @returns {string} The value readout of the icon.
     */
    get valueReadout(): string;
    /**
     * Sets the value readout of the icon.
     * @param {string} value - The new value readout of the icon.
     */
    set valueReadout(value: string);
    /**
     * Gets the width of the icon.
     * @returns {number} The width of the icon.
     */
    get width(): number;
    /**
     * Sets the width of the icon.
     * @param {number} value - The new width of the icon.
     */
    set width(value: number);
    /**
     * Gets the height of the icon.
     * @returns {number} The height of the icon.
     */
    get height(): number;
    /**
     * Sets the height of the icon.
     * @param {number} value - The new height of the icon.
     */
    set height(value: number);
    /**
     * Gets the callback function to be executed when the icon is selected.
     * @returns {() => void} The callback function.
     */
    get onIconSelected(): () => void;
    /**
     * Gets a boolean value indicating if this icon is enabled
     * @returns {boolean} a value indicates if this icon is enabled.
     */
    get isEnabled(): boolean;
    /**
    * Sets if this icon is enabled or not
    * @param {boolean} value - a value indicates if this icon is enabled.
    */
    set isEnabled(value: boolean);
    /**
     * Gets the movement queue for the icon.
     * @returns {Array<{ location: Float32Array; speed: number }>} The queue of movements.
     */
    get movementQueue(): Array<{
        location: Float32Array;
        speed: number;
    }>;
    /**
     * Adds a movement task to the queue.
     * @param {Float32Array} location - The target location.
     * @param {number} speed - The speed of the movement.
     */
    addMovementToQueue(location: Float32Array, speed: number): void;
    /**
     * Clears the movement queue.
     */
    clearMovementQueue(): void;
    /**
     * Gets whether the icon is currently moving.
     * @returns {boolean} True if the icon is moving, otherwise false.
     */
    get isMoving(): boolean;
    /**
     * Sets whether the icon is currently moving.
     * @param {boolean} value - True to set the icon as moving, otherwise false.
     */
    set isMoving(value: boolean);
}
