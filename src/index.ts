import Phaser from 'phaser';
import { MainScene, PlazaScene } from './scenes/index.js';

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
      debug: true
    }
  },
  scale: {
    mode: Phaser.Scale.RESIZE,
    resizeInterval: 500
  },
});
