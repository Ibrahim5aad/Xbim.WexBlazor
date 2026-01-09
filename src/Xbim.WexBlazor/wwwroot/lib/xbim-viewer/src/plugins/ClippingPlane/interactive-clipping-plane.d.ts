import { Viewer } from "../../viewer";
import { IPlugin } from "../plugin";
/**
 * Interactive clipping plane. This will render a 3D control which can be used to drag and rotate the clipping
 * plane. Use `InteractiveClippingPlane.stopped` to show or hide the 3D control. Once clipping plane
 * is applied, you can discard it using `viewer.unclip()`.
 */
export declare class InteractiveClippingPlane implements IPlugin {
    private viewer;
    private indices;
    private vertices;
    private colours;
    private ids;
    private ARROW;
    private HORIZONTAL;
    private VERTICAL;
    private PLANE;
    private arrowColour;
    private horizontalColour;
    private verticalColour;
    private planeColour;
    private currentInteraction;
    private program;
    private vertex_buffer;
    private colours_buffer;
    private id_buffer;
    private index_buffer;
    private coordinatesAttributePointer;
    private coloursAttributePointer;
    private idsAttributePointer;
    private mvUniformPointer;
    private pUniformPointer;
    private idCodingUniformPointer;
    private idSelectedIdUniformPointer;
    private hoverPickColourUniformPointer;
    private initialized;
    private transformation;
    private mvMatrix;
    private _hoverPickColour;
    /**
     * Set to true to stop rendering of this plugin
     */
    get stopped(): boolean;
    set stopped(value: boolean);
    private _stopped;
    init(viewer: Viewer): void;
    onBeforeDraw(width: number, height: number): void;
    onAfterDraw(width: number, height: number): void;
    onBeforeDrawId(): void;
    onAfterDrawId(): void;
    onAfterDrawModelId(): void;
    private draw;
    private bufferGeometry;
    private initEvents;
    private getRotatingAngle;
    private getDragOffset;
    private updateTransformationFromPlane;
    private isTransformationsHasSameOrientation;
    private getPlanePoint;
    private transformPlane;
    private getTangent;
    private applyCurrentPlane;
    private snapToMainAxes;
    private getPlaneEquation;
    private getPrependicularPlaneEquation;
    private getTangentPlaneEquation;
    private getPlaneNormal;
    private getPlaneBinormal;
    private getPlaneTangent;
    private initShader;
    private setActive;
    private computeArrowVertices;
    private computeArrowIndices;
    private computeRingVertices;
    private computeRingIndices;
    private computePlaneVertices;
    private computePlaneIndices;
}
