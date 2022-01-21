import { GameType } from "src/app/common/enums/game-type";
import { TimerTimeType } from "src/app/common/enums/timer-time-type";
import { TimerType } from "src/app/common/enums/timer-type";

export class MatchProps {
    type: GameType = 0;
    teamCount: number = 0;
    timerType: TimerType = 0;
    timerTimeType: TimerTimeType = 0;
}