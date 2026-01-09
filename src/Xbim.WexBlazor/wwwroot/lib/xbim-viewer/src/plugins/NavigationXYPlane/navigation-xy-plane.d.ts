import { Viewer } from "../../viewer";
import { IPlugin } from "../plugin";
export declare class NavigationXYPlane implements IPlugin {
    onAfterDrawModelId(): void;
    private _originalNavigate;
    private _viewer;
    init(viewer: Viewer): void;
    /**
     * Use this boolean switch to activate and deactivate the plugin. This will supposingly be bound to
     * some user interaction (like a button in the toolbar).
     * @member {boolean} NavigationXYPlane#isActive
     * */
    private _isActive;
    get isActive(): boolean;
    set isActive(value: boolean);
    /**
     * When a vertical angle from horizontal plane is bigger than this, default navigation is used.
     * The default is 60 degrees. Applies for both up and down looking angles.
     * @member {number} NavigationXYPlane#bowLimit
     * */
    private _bowLimit;
    get bowLimit(): number;
    set bowLimit(value: number);
    /**
     * By default this is 1.0. Make this bigger to speed the movement up or make it smaller to slow it down
     * @member {number} NavigationXYPlane#speedFactor
     * */
    private _speedFactor;
    get speedFactor(): number;
    set speedFactor(value: number);
    private navigate;
    onBeforeDraw(): void;
    onAfterDraw(): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
    onBeforeGetId(id: number): boolean;
    onBeforePick(id: number): boolean;
}
