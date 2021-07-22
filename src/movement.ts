export function moveAndAnimate(player: Phaser.Types.Physics.Arcade.SpriteWithDynamicBody, cursors: Phaser.Types.Input.Keyboard.CursorKeys) {
    player.setAngularAcceleration(0);
    player.setAngularVelocity(0);
    if (cursors?.up.isDown) {
        player?.setAccelerationY(-300);

    } else if (cursors?.left.isDown) {
        player?.setAccelerationX(-300);
        player?.anims.play('left');
        player?.anims.play('idle');

    } else if (cursors?.right.isDown) {
        player?.setAccelerationX(300);
        player?.anims.play('right');

    } else if (cursors?.down.isDown) {
        player?.setAccelerationY(300);
        player?.anims.play('idle');

    } else {
        player?.setVelocity(0);
        player?.setAcceleration(0);
        player?.anims.play('idle');
    }

}