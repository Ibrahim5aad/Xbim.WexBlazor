/**
 * @category Core
 */
export declare class Product {
    name: string;
    id: number;
    abs: boolean;
    children?: Product[];
    static getByName(name: string): Product;
    static getAllSubTypes(root: number): {
        [id: number]: boolean;
    };
}
