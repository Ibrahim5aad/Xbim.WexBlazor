import { mat4 } from "gl-matrix";
/**
 * @category Core
 */
export declare class CameraProperties {
    /**
     *
     */
    constructor(changeHandler: () => void);
    private _fov;
    private _near;
    private _far;
    private _height;
    private _type;
    get fov(): number;
    set fov(value: number);
    get near(): number;
    set near(value: number);
    get far(): number;
    set far(value: number);
    get height(): number;
    set height(value: number);
    get type(): CameraType;
    set type(value: CameraType);
    onChange: () => void;
    getProjectionMatrix(width: number, height: number): mat4;
}
/**
 * @category Core
 */
export declare enum CameraType {
    PERSPECTIVE = 0,
    ORTHOGONAL = 1
}
