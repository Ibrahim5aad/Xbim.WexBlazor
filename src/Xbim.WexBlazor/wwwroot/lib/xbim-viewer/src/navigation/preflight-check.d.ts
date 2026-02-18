import { Viewer } from "../viewer";
import { mat4 } from "gl-matrix";
export declare class PreflightCheck {
    static findView(viewer: Viewer, elements: {
        id: number;
        model: number;
    }[], density: number): {
        mv: mat4;
        height: number;
    };
}
