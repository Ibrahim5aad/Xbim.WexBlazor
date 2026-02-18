/**
 * @category BCF
 */
export declare class OrthogonalCamera {
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
     * proportion of camera view to model
     */
    view_to_world_scale: number;
    /**
     * Extension to BCF: Width of the viewport in the model space
     */
    width: number;
    /**
     * Extension to BCF: Height of the viewport in the model space
     */
    height: number;
}
