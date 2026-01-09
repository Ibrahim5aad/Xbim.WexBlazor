import { Message } from "../common/message";
/**
 * Convenient class for binary reading. Arrays are read as new views on slices of the original data buffer,
 * individual values are read using little endian data view.
 */
export declare class BinaryReader {
    private _buffer;
    private _view;
    private _position;
    private _progress;
    private _lastProgress;
    /**
     * callback which will be called after asynchronous loading of data is finished
     */
    onloaded: (reader: BinaryReader) => void;
    onerror: (message?: string) => void;
    /**
     * Current position
     */
    get Position(): number;
    /**
     * Gets reader for a sub array starting at current position.
     * This enforces isolation of reading within certain data island.
     *
     * @param length Byte length of the data island
     */
    getSubReader(length: number): BinaryReader;
    /**
     * Pass url string, blob, file of byte array to this function to initialize the reader. Only array buffer takes imidiate effect.
     * Othe sources are loaded asynchronously and you need to use 'onloaded' delegate to use the reader only after it is initialized woth the data.
     * @param source URL string of the file or BLOB or File or ArrayBuffer object
     * @param headers http headers to be used to fetch data
     * @param progress Callback which can be repeatedly called to report processing progress
     */
    load(source: string | Blob | File | ArrayBuffer, headers: {
        [name: string]: string;
    }, progress: (message: Message) => void): void;
    seek(position: number): void;
    isEOF(): boolean;
    private readArray;
    private move;
    private reportProgress;
    readByte(): number;
    readByteArray(count: number): Uint8Array;
    readUint8(): number;
    readUint8Array(count: number): Uint8Array;
    readInt16(): number;
    readInt16Array(count: number): Int16Array;
    readUInt16(): number;
    readUint16Array(count: number): Uint16Array;
    readInt32(): number;
    readInt32Array(count: number): Int32Array;
    readUint32(): number;
    readUint32Array(count: number): Uint32Array;
    readFloat32(): number;
    readFloat32Array(count: number): Float32Array;
    readFloat64(): number;
    readFloat64Array(count: number): Float64Array;
    readChar(count?: number): any;
    readPoint(count?: number): any;
    readRgba(count?: number): any;
    readPackedNormal(count?: number): any;
    readMatrix4x4(count?: number): any;
    readMatrix4x4_64(count?: number): any;
    /**
     * Reads slice of data from the underlying array buffer
     * @param length Length of requested data. Start is at current position
     */
    readData(length: number): ArrayBuffer;
}
