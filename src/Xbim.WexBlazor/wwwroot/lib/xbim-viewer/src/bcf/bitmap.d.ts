/**
 * @category BCF
 */
export declare class Bitmap {
    /**
     * format of the bitmap. Predefined values png or jpg
     */
    bitmap_type: "jpg" | "png";
    /**
     * base64 encoded string 	The bitmap image data
     */
    bitmap_data: string;
    /**
     * location of the center of the bitmap in world coordinates (point)
     */
    location: number[];
    /**
     * normal vector of the bitmap (vector)
     */
    normal: number[];
    /**
     * up vector of the bitmap (vector)
     */
    up: number[];
    /**
     * height of bitmap in the scene
     */
    height: number;
}
