import { ProductType } from "../product-type";
import { State } from "./state";
export declare class ProductMap {
    productID: number;
    renderId: number;
    type: ProductType;
    bBox: Float32Array;
    spans: Int32Array[];
    states: number[];
    static clone(o: ProductMap): ProductMap;
    static addState(p: ProductMap, state: State): void;
    static removeState(p: ProductMap, state: State): void;
    static getState(p: ProductMap): State;
}
