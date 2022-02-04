import { TeamType } from "src/app/common/enums/game-type";
import { TimerDifficulty } from "src/app/common/enums/timer-time-type";
import { TimerType } from "src/app/common/enums/timer-type";

export class MatchProps {
    teamType: TeamType = 0;
    teamsCount: number = 0;
    timerType: TimerType = 0;
    timerDifficulty: TimerDifficulty = 0;
}