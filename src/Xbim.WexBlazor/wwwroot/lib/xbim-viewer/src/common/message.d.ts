export declare class Message {
    type: MessageType;
    message: string;
    percent: number;
    result?: any;
    wexbimId?: number;
    phase: LoadingPhase;
}
/** Converts a Message to a aggregate percent completed based naive assumption that phases are equal */
export declare function MessageProgress(msg: Message): number;
export declare enum MessageType {
    PROGRESS = 0,
    COMPLETED = 1,
    FAILED = 2
}
export declare enum LoadingPhase {
    DOWNLOADING = 0,
    READING = 1,
    LOADING = 2
}
