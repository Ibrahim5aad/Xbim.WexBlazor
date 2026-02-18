/**
    * Enumeration for object states.
    * @readonly
    * @enum {number}
    *
    *
    * @category Core
    */
export declare enum State {
    UNDEFINED = 255,
    HIDDEN = 254,
    HIGHLIGHTED = 253,
    XRAYVISIBLE = 252,
    PICKING_ONLY = 251,
    HOVEROVER = 250,
    UNSTYLED = 225
}
export declare class StatePriorities {
    static getPriority(state: State): number;
    static getHighestPriority(states: State[]): State;
}
