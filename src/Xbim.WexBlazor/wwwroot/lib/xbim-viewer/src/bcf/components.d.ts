import { Component } from "./component";
import { Coloring } from "./coloring";
import { Visibility } from "./visibility";
/**
 * @category BCF
 */
export declare class Components {
    /**
     * Selected components
     */
    selection: Component[];
    /**
     * Colored components
     */
    coloring: Coloring[];
    /**
     * Visibility of components
     */
    visibility: Visibility;
}
