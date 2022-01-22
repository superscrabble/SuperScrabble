import { GameConfig } from "./game-config";

export class ConfigsPath {
    configs: GameConfig[] = [];
    currentConfigIndex: number = 0;
    isFinished: boolean = false;

    constructor(configs: GameConfig[]) {
        this.configs = configs;
    }

    addConfig(config: GameConfig) {
        if(config) {
            this.configs.push(config);
        }
    }

    //TODO: probably add removeConfig, if needed

    getCurrentConfig() : GameConfig {
        return this.configs[this.currentConfigIndex];
    }

    nextConfig() {
        if((this.currentConfigIndex + 1) == this.configs.length) {
            console.log("Config is finished in")
            this.isFinished = true;
            return;
        } 

        if((this.currentConfigIndex + 1) < this.configs.length) {
            this.currentConfigIndex++;
        }
    }

    previousConfig() {
        if(this.currentConfigIndex > 0) {
            this.isFinished = false;
            this.currentConfigIndex--;
        }
    }

    isFirstConfig() : boolean {
        return this.currentConfigIndex == 0;
    }
    
    isLastConfig() : boolean {
        return (this.currentConfigIndex + 1) >= this.configs.length;
    }
}