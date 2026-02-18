import { Component } from "./component";
import { ViewSetupHints } from "./view-setup-hints";
/**
 * @category BCF
 */
export declare class Visibility {
    /**
     * If true: Show all components, and hide the exceptions. If false: Hide all components and show exceptions;
     */
    default_visibility: boolean;
    /**
     * Components to hide/show determined by default_visibility
     */
    exceptions: Component[];
    /**
     * Hints about the setup of the viewer
     */
    view_setup_hints: ViewSetupHints;
}
