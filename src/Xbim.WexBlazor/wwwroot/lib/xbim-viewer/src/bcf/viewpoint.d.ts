import { Snapshot } from "./snapshot";
import { Bitmap } from "./bitmap";
import { ClippingPlane } from "./clipping-plane";
import { Line } from "./line";
import { PerspectiveCamera } from "./perspective-camera";
import { OrthogonalCamera } from "./orthogonal-camera";
import { Components } from "./components";
import { Viewer } from "../viewer";
/**
 * Class representing a BCF viewpoint
 *
 * A Viewpoint can capture and restore the state of the viewer's current state using
 * buildingSmart BCF-API schema
 *
 * @category BCF
 */
export declare class Viewpoint {
    /**
     * Static index used for autoincrement of viewpoints created in this session
     */
    private static _currentIndex;
    /**
     * Index used for sorting of multiple viewpoints. (Autoincremented)
     */
    index: number;
    /**
     * Unique ID of the viewpoint. (Automatically created by default)
     */
    guid: string;
    /**
     * This element describes a viewpoint using orthogonal camera.
     */
    orthogonal_camera: OrthogonalCamera;
    /**
     * This element describes a viewpoint using perspective camera.
     */
    perspective_camera: PerspectiveCamera;
    /**
     * Lines can be used to add markup in 3D. Each line is defined by three dimensional
     * Start Point and End Point. Lines that have the same start and end points are to be
     * considered points and may be displayed accordingly.
     */
    lines: Line[];
    /**
     * ClippingPlanes can be used to define a subsection of a building model that is related to the topic.
     * Each clipping plane is defined by Location and Direction.
     */
    clipping_planes: ClippingPlane[];
    /**
     * A list of bitmaps can be used to add more information,
     * for example, text in the visualization.
     * Bitmaps would be placed in the 3D view
     */
    bitmaps: Bitmap[];
    /**
     * Image snapshot of the view
     */
    snapshot: Snapshot;
    /**
     * Components in the viewpoint
     */
    components: Components;
    /**
     * Creates BCF viewpoint from the current view
     *
     * @param viewer viewer instance
     * @param idMapper function to be used to transform viewer identity into external identity. Typically to GUID. Viewer operates on local identity where combination of productID and modelID is unique within the current scope.
     * @param width  Optional width of the generated thumbnail in pixels. This is the current width of the viewer by default. Current aspect ratio is preserved.
     */
    static GetViewpoint(viewer: Viewer, idMapper: (productId: number, modelId: number) => string, width?: number): Viewpoint;
    private static AddClippingPlane;
    static SetViewpoint(viewer: Viewer, viewpoint: Viewpoint, idMapper: (guid: string) => {
        productId: number;
        modelId: number;
    }, duration?: number): void;
    /**
     * Applies the clipping planes for this viewpoint to the supplied viewer
     * @param viewer
     * @param viewpoint
     */
    static SetClipping(viewer: Viewer, viewpoint: Viewpoint): void;
    private static getClippingEquation;
    private static _resolution;
    /**
     * Number of pixels per meter in the browser
     */
    private static get resolution();
}
