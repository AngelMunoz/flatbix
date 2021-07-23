import Phaser, { Game } from 'phaser';
import { MainScene, PlazaScene } from './game/scenes/index.js';
import { start } from './Main.fs.js';



function main() {
  start();
  const _ = new Phaser.Game({
    height: 600,
    width: 800,
    type: Phaser.AUTO,
    parent: 'game',
    pixelArt: true,
    scene: [MainScene, PlazaScene],
    physics: {
      default: 'arcade',
      arcade: {
        debug: true,
        gravity: { y: 0, x: 0 },

      }
    },
    scale: {
      mode: Phaser.Scale.RESIZE,
      resizeInterval: 500
    }
  });
}

main();