using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameLogin : MonoBehaviour
{
    public void OnGameOver(Player _player, Obstacle _obstacle)
    {
        var _obsParams = (ObstacleSpawnable)_obstacle.GetSpawnable();

        var playerPos = (Vector2)_player.WormHead.transform.position;
        var playerSpeed = _player.WormHead.Rigidbody.velocity;
        var obstaclePos = (Vector2)_obstacle.transform.position;

        Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            // player info
            { "deathLevel", _player.playingBlocName },
            { "ecoPoints", _player.Score },
            { "playerXPosition",  playerPos.x},
            { "playerYPosition", playerPos.y},
            { "lateralSpeed", playerSpeed.x}, // won't work?

            // obstacle info
            { "obstacleName", _obsParams.Tag},

            // general info stacked
            { "Death Info", new DeathInfo(playerPos, obstaclePos, _obstacle.Size, playerSpeed, _player.playingBlocName, _obsParams.Tag, _obsParams.BodyRotation, _player.Score) }
        });
    }

    public void OnSessionOver(Player _player)
    {
        Analytics.CustomEvent("sessionEnd", new Dictionary<string, object>
        {
            { "GamesPlayed", UiManager.Instance.SessionGameCount },
            { "BestSessionScore", UiManager.Instance.BestSessionScore },
            { "SessionTotalOfStrikes", UiManager.Instance.SessionStrikesTotal }
        });
    }
}

struct DeathInfo
{
    public DeathInfo(Vector2 plyPos, Vector2 obsPos, Vector2 obsSize, Vector2 speed, string bloc, string obsName, float obsRot, int _score)
    {
        playerPosition = plyPos;
        obstaclePos = obsPos;
        obstacleSize = obsSize;
        speedVector = speed;
        blocName = bloc;
        obstacleName = obsName;
        obstacleRotation = obsRot;
        score = _score;
    }
    Vector2 playerPosition, obstaclePos, obstacleSize, speedVector;
    string blocName, obstacleName;
    float obstacleRotation;
    int score;
}