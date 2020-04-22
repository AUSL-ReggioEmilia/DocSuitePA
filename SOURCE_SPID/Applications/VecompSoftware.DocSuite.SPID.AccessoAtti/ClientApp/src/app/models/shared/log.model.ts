import { LogLevelEnum } from './log-level.enum';

export class LogItem {
    constructor(public LogLevel: LogLevelEnum,
        public LoggerName: string, public Message: string) { }
}