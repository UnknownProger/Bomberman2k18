using System.Collections;
using UnityEngine;

public static class PathFinder 
{

    public static int[,] FindPath(int currX, int currY, int targX, int targY, int[] ignore = null)
    {
        bool isComplete = false;
        int[,] pathMap = new int[Level.sizeX, Level.sizeY];
        int step = 0;

        for (int x = 0; x < Level.sizeX; x++)
        {
            for (int y = 0; y < Level.sizeY; y++)
            {
                if (Level.map[x, y].typeId == 1 || Level.map[x, y].typeId == 2)
                {
                    if (ignore == null)
                    {
                        pathMap[x, y] = -2;
                    }
                    else
                    {
                        if (ContainsIgnore(ignore, Level.map[x, y].typeId))
                            pathMap[x, y] = -3;
                        else
                            pathMap[x, y] = -2;
                    }
                }
                else
                {
                    pathMap[x, y] = -3;
                }
            }
        }

        pathMap[targX, targY] = 0;

        while (!isComplete)
        {
            for (int x = 0; x < Level.sizeX; x++)
            {
                for (int y = 0; y < Level.sizeY; y++)
                {
                    if (pathMap[x, y] == step)
                    {
                        if (y - 1 >= 0 && pathMap[x, y - 1] != -2 && pathMap[x, y - 1] == -3)
                        {
                            pathMap[x, y - 1] = step + 1;
                        }
                        if (x - 1 >= 0 && pathMap[x - 1, y] != -2 && pathMap[x - 1, y] == -3)
                        {
                            pathMap[x - 1, y] = step + 1;
                        }
                        if (y + 1 < Level.sizeY && pathMap[x, y + 1] != -2 && pathMap[x, y + 1] == -3)
                        {
                            pathMap[x, y + 1] = step + 1;
                        }
                        if (x + 1 < Level.sizeX && pathMap[x + 1, y] != -2 && pathMap[x + 1, y] == -3)
                        {
                            pathMap[x + 1, y] = step + 1;
                        }
                    }
                }
            }

            step++;

            if (pathMap[currX, currY] > 0)
            {
                isComplete = true;
                Debug.Log("Path was found!");
            }
            if (step > Level.sizeX * Level.sizeY)
            {
                isComplete = true;
                pathMap = null;
                Debug.Log("Path was not found!");
            }

        }
        return pathMap;
    }

    public static Vector2 GetNextPosition(int[,] pathMap, int currX, int currY)
    {
        if (pathMap != null)
        {
            int nextStep = pathMap[currX, currY] - 1;

            if (pathMap[currX - 1, currY] == nextStep)
                return new Vector2(currX - 1, currY);

            if (pathMap[currX + 1, currY] == nextStep)
                return new Vector2(currX + 1, currY);

            if (pathMap[currX, currY - 1] == nextStep)
                return new Vector2(currX, currY - 1);

            if (pathMap[currX, currY + 1] == nextStep)
                return new Vector2(currX, currY + 1);
        }
        return new Vector2(currX, currY);
    }

    private static bool ContainsIgnore(int[] ignore, int value)
    {
        bool isContains = false;

        for (int i = 0; i < ignore.Length; i++)
        {
            if (ignore[i] == value)
            {
                isContains = true;
                break;
            }
        }
        return isContains;
    }

}
