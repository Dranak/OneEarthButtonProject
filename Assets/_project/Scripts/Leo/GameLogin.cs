using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameLogin : MonoBehaviour
{
    public void OnGameOver(Player _player, Obstacle _obstacle)
    {
        var _obsParams = (ObstacleSpawnable)_obstacle.GetSpawnable();
        var _obsSize = _obstacle.Size;
        if (_obsSize.x != _obsSize.y)
            _obsSize = _obsParams.BoundsSize;
        
        Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            // player info
            { "deathLevel", _player.playingBlocName },
            { "ecoPoints", _player.Score },
            { "playerPosition",  (Vector2)_player.WormHead.transform.position },
            { "speedVector", _player.WormHead.Rigidbody.velocity }, // won't work

            // obstacle info
            { "obstacleName", _obsParams.Tag},
            { "obstaclePosition", (Vector2)_obstacle.transform.position },
            { "obstacleBoundsSize", _obsSize }
        });
    }
}