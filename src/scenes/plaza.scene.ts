import Phaser from 'phaser';

export class PlazaScene extends Phaser.Scene {
    controls?: Phaser.Cameras.Controls.FixedKeyControl;

    constructor() {
        super('PlazaScene');
    }

    preload() {
        this.load.image('ice', '/assets/tilesets/ice.png');
        this.load.image('autum', '/assets/tilesets/autum.png');
        this.load.tilemapTiledJSON('main-plaza', '/assets/main-plaza.json');
    }

    create() {
        const map = this.make.tilemap({ key: 'main-plaza' });
        const iceTileset = map.addTilesetImage('ice', 'ice');
        const autumTileset = map.addTilesetImage('autum', 'autum');
        const layers = [
            map.createLayer('baseTerrain', [iceTileset, autumTileset]),
            map.createLayer('baseTerrain2', [iceTileset, autumTileset]),
            map.createLayer('decorations', [iceTileset, autumTileset]),
            map.createLayer('decorations2', [iceTileset, autumTileset]),
        ];
        this.cameras.main.setZoom(4);
        this.cameras.main.setBounds(0, 0, map.widthInPixels, map.heightInPixels, true);
        this.physics.world.setBounds(0, 0, map.widthInPixels, map.heightInPixels, true);

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
        const fullscreenBtn = this.add
            .text(16, 150, "Arrow keys to scroll, click for full screen", {
                font: "24px sans",
                color: '#FFFFFF',
                padding: { x: 20, y: 10 },
                backgroundColor: "#000000"
            })
            .setScrollFactor(0)
            .setInteractive();
        const switchBtn =
            this.add
                .text(16, 120, "Go to main", {
                    font: "24px sans",
                    color: '#FFFFFF',
                    padding: { x: 10, y: 10 },
                    backgroundColor: "#000000"
                })
                .setInteractive();
        fullscreenBtn.on('pointerup', () => {
            this.scale.toggleFullscreen();
        });
        switchBtn.on('pointerup', () => {
            this.scene.transition({
                target: 'MainScene',
                moveAbove: true,
                sleep: true
            });
        });
    }

    update(_time: any, delta: number) {
        (this.controls && this.controls.update(delta));
    }
}