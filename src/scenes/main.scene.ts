import Phaser from 'phaser';

export class MainScene extends Phaser.Scene {
    controls?: Phaser.Cameras.Controls.FixedKeyControl;
    layers: Phaser.Tilemaps.TilemapLayer[] = [];

    constructor() {
        super('MainScene');
    }

    preload() {
        this.load.image('ice', '/assets/tilesets/ice.png');
        this.load.image('autum', '/assets/tilesets/autum.png');
        this.load.tilemapTiledJSON('map', '/assets/sample-map.json');
    }

    create() {
        const map = this.make.tilemap({ key: 'map' });
        const iceTileset = map.addTilesetImage('ice', 'ice');
        const autumTileset = map.addTilesetImage('autum', 'autum');
        this.layers = [
            map.createLayer('baseTerrain', [iceTileset, autumTileset], 0, 0),
            map.createLayer('decorations', [iceTileset, autumTileset], 0, 0),
        ];
        this.cameras.main.setBounds(0, 0, map.widthInPixels, map.heightInPixels);
        this.physics.world.setBounds(0, 0, map.widthInPixels, map.heightInPixels);
        this.cameras.main.setZoom(4);

        // Set up the arrows to control the camera
        const cursors = this.input.keyboard.createCursorKeys();
        this.controls = new Phaser.Cameras.Controls.FixedKeyControl({
            camera: this.cameras.main,
            left: cursors.left,
            right: cursors.right,
            up: cursors.up,
            down: cursors.down,
            speed: 0.5
        });

        // Help text that has a "fixed" position on the screen
        const fullScreenBtn = this.add
            .text(16, 16, "Arrow keys to scroll, click for full screen", {
                font: "18px sans",
                color: '#FFFFFF',
                padding: { x: 20, y: 10 },
                backgroundColor: "#000000"
            })
            .setScrollFactor(0)
            .setInteractive();
        const switchBtn =
            this.add
                .text(16, 120, "Go to Plaza", {
                    font: "18px sans",
                    color: '#FFFFFF',
                    padding: { x: 10, y: 10 },
                    backgroundColor: "#000000",
                })
                .setInteractive();
        fullScreenBtn.on('pointerup', () => {
            this.scale.toggleFullscreen();
        });
        switchBtn.on('pointerup', () => {
            this.scene.transition({
                target: 'PlazaScene',
                moveAbove: true,
                sleep: true
            });
        });
    }

    update(_time: any, delta: number) {
        (this.controls && this.controls.update(delta));
    }
}