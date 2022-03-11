import { LogStatus } from './log-status';

export class Log {
    status: LogStatus = LogStatus.WriteWord;
    userName: string = "";
    changedTilesCount?: number;
    newlyWrittenWords?: string[]; 
}