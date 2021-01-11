import { LogLevelEnum } from './log-level.enum';

export class LogItem {
    constructor(logLevel: LogLevelEnum, loggerName: string, message: string) {
        this.LogLevel = logLevel;
        this.LoggerName = loggerName;
        this.Message = message;
    }

    LogLevel: LogLevelEnum;
    LoggerName: string;
    Message: string;
}