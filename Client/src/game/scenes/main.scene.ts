import { GameAssetsUrl } from '@src/environment';
import Phaser from 'phaser';

export function MainSceneFactory() {

    return class MainScene extends Phaser.Scene {
        controls?: Phaser.Cameras.Controls.FixedKeyControl;
        layers: Phaser.Tilemaps.TilemapLayer[] = [];
        cursors?: Phaser.Types.Input.Keyboard.CursorKeys;
        player?: Phaser.Types.Physics.Arcade.SpriteWithDynamicBody;
        sceneEvents = new Phaser.Events.EventEmitter();
        skillKeys?: Record<string, Phaser.Input.Keyboard.Key>;
        destination?: Phaser.Math.Vector2;


        constructor() {
            super('MainScene');
        }

        init(data?: Record<string, any>) {
            console.log(data);
        }
        shutdown(data?: Record<string, any>) {
            console.log(data);
        }

        preload() {
            this.load.image('ice', `${GameAssetsUrl}/tilesets/ice.png`);
            this.load.image('autum', `${GameAssetsUrl}/tilesets/autum.png`);
            this.load.tilemapTiledJSON('map', `${GameAssetsUrl}/main-plaza.json`);
            this.load.spritesheet('slime', `${GameAssetsUrl}/tilesets/slime.png`, { frameWidth: 32, frameHeight: 25 });
            this.load.spritesheet('explosion-1', `${GameAssetsUrl}/tilesets/explosion-6.png`, { frameWidth: 48 });
        }

        create() {
            const map = this.make.tilemap({ key: 'map' });
            const iceTileset = map.addTilesetImage('ice', 'ice');
            const autumTileset = map.addTilesetImage('autum', 'autum');
            this.layers = [
                map.createLayer('baseTerrain', [iceTileset, autumTileset], 0, 0),
                map.createLayer('baseTerrain1', [iceTileset, autumTileset], 0, 0),
                map.createLayer('baseTerrain2', [iceTileset, autumTileset], 0, 0),
                map.createLayer('decorations', [iceTileset, autumTileset], 0, 0),
                map.createLayer('decorations1', [iceTileset, autumTileset], 0, 0),
            ];
            this.cameras.main.setBounds(0, 0, map.widthInPixels, map.heightInPixels);
            this.physics.world.setBounds(0, 0, map.widthInPixels, map.heightInPixels);
            this.cameras.main.setZoom(2);
            this.player = this.physics.add.sprite(50, 50, 'slime');
            this.player.setBounce(1);
            this.player.setCollideWorldBounds(true);
            this.cameras.main.startFollow(this.player, true, 0.05, 0.05);
            this.player.setMaxVelocity(150);
            this.cursors = this.input.keyboard.createCursorKeys();


            this.anims.create({
                key: 'idle',
                frames: this.anims.generateFrameNumbers('slime', { start: 0, end: 3 }),
                frameRate: 5,
                repeat: -1
            });
            this.anims.create({
                key: 'left',
                frames: this.anims.generateFrameNumbers('slime', { start: 4, end: 7 }),
                frameRate: 10,
                repeat: -1,
            });
            this.anims.create({
                key: 'right',
                frames: this.anims.generateFrameNumbers('slime', { start: 12, end: 15 }),
                frameRate: 10,
                repeat: -1,
            });
            this.anims.create({
                key: 'exploding-1',
                frames: this.anims.generateFrameNumbers('explosion-1', { start: 0, end: 7 }),
                showOnStart: true,
                hideOnComplete: true
            });
            const explosion = this.physics.add.sprite(0, 0, 'explosion-1');
            explosion.setVisible(false);
            this.player.play('idle', true);

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
            this.skillKeys = this.input.keyboard.addKeys('F1,F2,F3,F4') as Record<string, Phaser.Input.Keyboard.Key>;

            this.sceneEvents.on('try-skill-1', () => {
                const pos = new Phaser.Math.Vector2();
                this.input.mousePointer.positionToCamera(this.cameras.main, pos);
                explosion.setVisible(true);
                explosion.setPosition(pos.x, pos.y);
                explosion.play('exploding-1', true);
            });
        }

        update(_time: any, delta: number) {
            // if (this.player && this.cursors) {
            //     moveAndAnimate(this.player, this.cursors);
            // }
            if (this.player && this.input.mousePointer.isDown) {
                this.destination = new Phaser.Math.Vector2();
                this.input.mousePointer.positionToCamera(this.cameras.main, this.destination);
                this.player?.play('right', true);
                this.player.setAngularAcceleration(0);
                this.physics.moveTo(this.player.body.gameObject, this.destination.x, this.destination.y, 800);
            }

            if (this.player && this.destination) {
                const result = this.player.body.position.distance(this.destination);
                if (result <= 38) {
                    this.player.setVelocity(0);
                    this.player.setAcceleration(0);
                    this.player.setAngularAcceleration(0);
                    this.player.play('idle', true);
                }

                if (this.skillKeys && this.skillKeys.F1.isDown) {
                    this.sceneEvents.emit('try-skill-1');
                }
            }

        }
    };
}
