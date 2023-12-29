using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public void EnableAllMovement(PlayerController player, List<ObstacleController> obstacles, List<StageController> stages, List<SpaceshipController> spaceships)
    {
        player.SetMovementEnabled();

        EnableMovementForObstacles(obstacles);
        EnableMovementForSpaceships(spaceships);
        EnableMovementForStages(stages);
    }

    private void EnableMovementForSpaceships(List<SpaceshipController> spaceshipsInGame)
    {
        foreach (SpaceshipController ship in spaceshipsInGame)
        {
            ship.SetMovementEnabled();
        }
    }

    private void EnableMovementForObstacles(List<ObstacleController> obstaclesInGame)
    {
        foreach (ObstacleController obstacle in obstaclesInGame)
        {
            obstacle.SetMovementEnabled();
        }
    }

    private void EnableMovementForStages(List<StageController> stagesInGame)
    {
        foreach (StageController stage in stagesInGame)
        {
            stage.SetMovementEnabled();
        }
    }

    public void SetSpeedForAll(PlayerController player, List<ObstacleController> obstacles, List<StageController> stages, List<SpaceshipController> spaceships, float MovingSpeed)
    {
        player.SetSpeedOfCharacter(MovingSpeed);

        SetMovementSpeedToSpaceships(spaceships, MovingSpeed);
        SetMovementSpeedToObstacles(obstacles, MovingSpeed);
        SetMovementSpeedToStages(stages, MovingSpeed);

    }

    public void SetMovementSpeedToSpaceships(List<SpaceshipController> spaceshipsInGame, float MovingSpeed)
    {
        foreach (SpaceshipController ship in spaceshipsInGame)
        {
            ship.SetMovementSpeed(MovingSpeed);
        }
    }

    public void SetMovementSpeedToObstacles(List<ObstacleController> obstaclesInGame, float MovingSpeed)
    {
        foreach (ObstacleController obstacle in obstaclesInGame)
        {
            obstacle.SetMovementSpeed(MovingSpeed);
        }
    }

    public void SetMovementSpeedToStages(List<StageController> stagesInGame, float MovingSpeed)
    {
        foreach (StageController stage in stagesInGame)
        {
            stage.SetMovementSpeed(MovingSpeed);
        }
    }

    public void DisableAllMovements(PlayerController player, List<ObstacleController> obstacles, List<StageController> stages, List<SpaceshipController> spaceships)
    {
        player.SetMovementDisabled();

        DisableMovementForObstacles(obstacles);
        DisableMovementForSpaceships(spaceships);
        DisableMovementForStages(stages);
    }

    private void DisableMovementForSpaceships(List<SpaceshipController> spaceshipsInGame)
    {
        foreach (SpaceshipController ship in spaceshipsInGame)
        {
            ship.SetMovementDisabled();
        }
    }

    private void DisableMovementForObstacles(List<ObstacleController> obstaclesInGame)
    {
        foreach (ObstacleController obstacle in obstaclesInGame)
        {
            obstacle.SetMovementDisabled();
        }
    }

    private void DisableMovementForStages(List<StageController> stagesInGame)
    {
        foreach (StageController stage in stagesInGame)
        {
            stage.SetMovementDisabled();
        }
    }
}
