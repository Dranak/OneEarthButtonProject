using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameLogin : MonoBehaviour
{
    public void OnGameOver(Player _player, Obstacle _obstacle)
    {
        var _obsParams = (ObstacleSpawnable)_obstacle.GetSpawnable();
        var obstaclePos = (Vector2)_obstacle.transform.position;

        var playerPos = (Vector2)_player.WormHead.transform.position;
        var playerLateralSpeed = _player.WormHead.Speed;
        var warpedAngle = _player.WormHead.transform.rotation.eulerAngles.z;
        if (warpedAngle > 180) warpedAngle -= 360;

        Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            // player info
            { "deathLevel", _player.playingBlocName },
            { "ecoPoints", _player.Score },
            { "playerXPosition",  playerPos.x},
            { "playerYPosition", playerPos.y},
            { "lateralSpeed", playerLateralSpeed },
            { "player Rotation",  warpedAngle},

            // obstacle info
            { "obstacleName", _obsParams.Tag},
            { "obstacle Y Pos", _obstacle.transform.position.y },

            // general info stacked
            { "Death Info", new DeathInfo(playerPos, obstaclePos, _obstacle.Size, warpedAngle, playerLateralSpeed, _player.playingBlocName, _obsParams.Tag, _obsParams.BodyRotation, _player.Score) }
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
    public DeathInfo(Vector2 plyPos, Vector2 obsPos, Vector2 obsSize, float rotation, float latSpeed, string bloc, string obsName, float obsRot, int _score)
    {
        playerPosition = plyPos;
        obstaclePos = obsPos;
        obstacleSize = obsSize;
        playerRotation = rotation;
        lateralSpeed = latSpeed;
        blocName = bloc;
        obstacleName = obsName;
        obstacleRotation = obsRot;
        score = _score;
    }
    Vector2 playerPosition, obstaclePos, obstacleSize;
    string blocName, obstacleName;
    float playerRotation, lateralSpeed, obstacleRotation;
    int score;
}