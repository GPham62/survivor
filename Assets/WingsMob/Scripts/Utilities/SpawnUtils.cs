using System.Linq;
using UnityEngine;
using WingsMob.Survival.Global;

namespace WingsMob.Survival.Utils
{
    public static class SpawnUtils
    {
        public static Vector3 GetRandomEnemyPosAroundPlayer(Vector3 playerPos, Vector3[] otherEnemiesPos, float minRange, float maxRange, bool isHyperMode)
        {
            if (isHyperMode)
            {
                minRange += 2f;
                maxRange += 2f;
            }
            float randomDistance = Random.Range(minRange, maxRange);
            float randomX = Random.Range(0, randomDistance);
            float randomY = Mathf.Sqrt(randomDistance * randomDistance - randomX * randomX);
            randomX = Random.Range(0, 2) == 0 ? randomX : -randomX;
            randomY = Random.Range(0, 2) == 0 ? randomY : -randomY;
            zValue += 0.1f;
            zValue = Mathf.Repeat(zValue, SurvivorConfig.MaxZValue);
            Vector3 randomPos = new Vector3(playerPos.x + randomX, playerPos.y + randomY, zValue);
            randomPos = otherEnemiesPos.Contains(randomPos) ? new Vector3(randomPos.x + (randomX < 0 ? -0.2f : 0.2f), randomPos.y + (randomY < 0 ? -0.2f * 1.78f : 0.2f * 1.78f), zValue) : randomPos;
            return randomPos;
        }

        public static Vector3 GetRandomEnemyPosAroundPlayerVertical(Vector3 playerPos, Vector3[] otherEnemiesPos, float? rightBorderX, float minRange, float maxRange, bool isHyperMode)
        {
            if (rightBorderX == null)
                return GetRandomEnemyPosAroundPlayer(playerPos, otherEnemiesPos, minRange, maxRange, isHyperMode);
            else
            {
                if (isHyperMode)
                    maxRange += 2f;
                float randomDistance = Random.Range((float)rightBorderX - 0.5f, maxRange);
                float randomX = Random.Range(0, (float)rightBorderX - 0.5f);
                float randomY = Mathf.Sqrt(randomDistance * randomDistance - randomX * randomX);
                randomX = Random.Range(0, 2) == 0 ? randomX : -randomX;
                randomY = Random.Range(0, 2) == 0 ? randomY : -randomY;
                zValue += 0.1f;
                zValue = Mathf.Repeat(zValue, SurvivorConfig.MaxZValue);
                Vector3 randomPos = new Vector3(randomX, playerPos.y + randomY, zValue);
                randomPos = otherEnemiesPos.Contains(randomPos) ? new Vector3(randomX + (randomX < 0 ? -0.2f : 0.2f), randomPos.y + (randomY < 0 ? -0.2f * 1.78f : 0.2f * 1.78f), zValue) : randomPos;
                return randomPos;
            }
        }

        public static Vector3 GetRandomPosAroundPlayer(Vector3 playerPos, float distance)
        {
            float randomX = Random.Range(0, distance);
            float randomY = Mathf.Sqrt(distance * distance - randomX * randomX);
            randomX = Random.Range(0, 2) == 0 ? randomX : -randomX;
            randomY = Random.Range(0, 2) == 0 ? randomY : -randomY;
            return new Vector3(playerPos.x + randomX, playerPos.y + randomY, playerPos.z);
        }

        public static Vector3 GetRandomPosAroundPlayerVertical(Vector3 playerPos, float distance, float? rightBorderX)
        {
            if (rightBorderX == null)
                return GetRandomPosAroundPlayer(playerPos, distance);
            else
            {
                float randomX = Random.Range(0, distance);
                randomX = randomX + Mathf.Abs(playerPos.x) >= rightBorderX - 0.5f ?
                            playerPos.x > 0 ? -randomX : randomX
                            : Random.Range(0, 2) == 0 ? randomX : -randomX;

                float randomY = Mathf.Sqrt(distance * distance - randomX * randomX);
                randomY = Random.Range(0, 2) == 0 ? randomY : -randomY;
                return new Vector3(playerPos.x + randomX, playerPos.y + randomY, playerPos.z);
            }
        }

        public static Vector3 GetBottomLeftPos(Camera mainCam) => mainCam.ScreenToWorldPoint(Vector2.zero);

        public static Vector3 GetTopLeftPos(Camera mainCam) => mainCam.ScreenToWorldPoint(new Vector2(0, mainCam.pixelHeight));

        public static Vector3 GetTopRightPos(Camera mainCam) => mainCam.ScreenToWorldPoint(new Vector2(mainCam.pixelWidth, mainCam.pixelHeight));

        public static Vector3 GetBottomRightPos(Camera mainCam) => mainCam.ScreenToWorldPoint(new Vector2(mainCam.pixelWidth, 0));

        static Vector3 topLeft, bottomLeft, topRight, bottomRight;

        public static float zValue = 0;

        public static Vector3 GetRandomPosWithSpawnDirection(Camera mainCam, SpawnDirection spawnDirection)
        {
            topLeft = GetTopLeftPos(mainCam);
            bottomLeft = GetBottomLeftPos(mainCam);
            topRight = GetTopRightPos(mainCam);
            bottomRight = GetBottomRightPos(mainCam);
            zValue += 0.1f;
            zValue = Mathf.Repeat(zValue, SurvivorConfig.MaxZValue);
            switch (spawnDirection)
            {
                case SpawnDirection.Top:
                    return new Vector3(Random.Range(topLeft.x - 1.0f, topRight.x + 1.0f), topLeft.y + Random.Range(2f, 4f), zValue);
                case SpawnDirection.Bottom:
                    return new Vector3(Random.Range(bottomLeft.x - 1.0f, bottomRight.x + 1.0f), bottomLeft.y + Random.Range(-4f, -2f), zValue);
                case SpawnDirection.Left:
                    return new Vector3(topLeft.x + Random.Range(-4f, -2f), Random.Range(bottomLeft.y - 1.0f, topLeft.y + 1.0f), zValue);
                case SpawnDirection.Right:
                    return new Vector3(topRight.x + Random.Range(2f, 4f), Random.Range(bottomRight.y - 1.0f, topRight.y + 1.0f), zValue);
                default:
                    return Vector3.zero;
            }
        }

        public static Vector3 GetRandomPosWithSpawnDirectionVertical(Camera mainCam, SpawnDirection spawnDirection, float rightBorderX)
        {
            topLeft = GetTopLeftPos(mainCam);
            bottomLeft = GetBottomLeftPos(mainCam);
            zValue += 0.1f;
            zValue = Mathf.Repeat(zValue, SurvivorConfig.MaxZValue);
            switch (spawnDirection)
            {
                case SpawnDirection.Top:
                    return new Vector3(Random.Range(-rightBorderX + 0.5f, rightBorderX - 0.5f), topLeft.y + Random.Range(2f, 4f), zValue);
                case SpawnDirection.Bottom:
                    return new Vector3(Random.Range(-rightBorderX + 0.5f, rightBorderX - 0.5f), bottomLeft.y + Random.Range(-4f, -2f), zValue);
                default:
                    return Vector3.zero;
            }
        }

        public static SpawnDirection GetEnemySpawnDirection(Transform player, Transform enemy, Camera mainCam)
        {
            Vector2 playerToEnemy = enemy.position - player.position;
            return enemy.position.x > player.position.x ?
               playerToEnemy.y >= 0 ?
                    Vector3.Cross(playerToEnemy, (Vector2)(GetTopRightPos(mainCam) - player.position)).z >= 0 ? SpawnDirection.Right : SpawnDirection.Top
                  : Vector3.Cross(playerToEnemy, (Vector2)(GetBottomRightPos(mainCam) - player.position)).z >= 0 ? SpawnDirection.Bottom : SpawnDirection.Right
             : playerToEnemy.y >= 0 ?
                    Vector3.Cross(playerToEnemy, (Vector2)(GetTopLeftPos(mainCam) - player.position)).z >= 0 ? SpawnDirection.Top : SpawnDirection.Left
                  : Vector3.Cross(playerToEnemy, (Vector2)(GetBottomLeftPos(mainCam) - player.position)).z >= 0 ? SpawnDirection.Left : SpawnDirection.Bottom;
        }

        public static SpawnDirection GetEnemySpawnDirectionVertical(Transform player, Transform enemy, Camera mainCam)
        {
            return enemy.position.y > player.position.y ? SpawnDirection.Top : SpawnDirection.Bottom;
        }

        public static SpawnDirection GetOppositeSpawnDirection(SpawnDirection direction)
        {
            switch (direction)
            {
                case SpawnDirection.Bottom: return SpawnDirection.Top;
                case SpawnDirection.Top: return SpawnDirection.Bottom;
                case SpawnDirection.Left: return SpawnDirection.Right;
                case SpawnDirection.Right: return SpawnDirection.Left;
                default: return SpawnDirection.Bottom;
            }
        }
    }
}