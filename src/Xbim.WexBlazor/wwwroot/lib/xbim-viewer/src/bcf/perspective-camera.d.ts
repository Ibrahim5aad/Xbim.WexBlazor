/**
 * @category BCF
 */
export declare class PerspectiveCamera {
    /**
     * viewpoint of the camera
     */
    camera_view_point: number[];
    /**
     * direction of the camera
     */
    camera_direction: number[];
    /**
     * direction of camera up
     */
    camera_up_vector: number[];
    /**
     * field of view
     */
    field_of_view: number;
    /**
     * Extension to BCF: Width of the viewport in the model space
     */
    width: number;
    /**
     * Extension to BCF: Height of the viewport in the model space
     */
    height: number;
}
