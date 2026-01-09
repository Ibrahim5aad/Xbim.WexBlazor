/**
 * @category BCF
 */
export declare class Snapshot {
    /**
     * Format of the snapshot
     */
    snapshot_type: "png" | "jpg";
    /**
     * Base64 encoded snapshot image data
     */
    snapshot_data: string;
}
