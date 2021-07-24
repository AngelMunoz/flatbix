import Phaser from 'phaser';

export class GameService {

  private _game?: Phaser.Game;

  get game() {
    return this._game;
  }

  startGame(container: string | HTMLElement) {
    if (this.game) {
      this.game.destroy(true);
    }
    this._game = new Phaser.Game({
      height: 600,
      width: 800,
      type: Phaser.AUTO,
      parent: container,
      physics: {
        default: 'arcade',
        arcade: {
          debug: true,
          gravity: { y: 0, x: 0 }
        }
      },
      scale: {
        mode: Phaser.Scale.RESIZE,
        resizeInterval: 500
      }
    });
  }


  async addScene(key: string, getScene: (...args: any[]) => Promise<Phaser.Scene>, sceneArgs: any[] = []) {
    try {
      const scene = await getScene(...sceneArgs);
      this.game?.scene.add(key, scene);
    } catch (error) {
      console.warn(error.message);
    }
  }

  async startScene(key: string, data?: Record<string, any>) {
    console.log(this.game?.scene.getScenes());
    this.game?.scene.start(key, data);
  }

  async stopScene(key: string, data?: Record<string, any>) {
    this.game?.scene.stop(key, data);
  }
}

export default new GameService();