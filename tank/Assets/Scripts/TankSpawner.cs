using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyTanks;
    [SerializeField] Transform playerTankBody, playerTankHead;
    [SerializeField] Transform playerSpawnLocation;
    [SerializeField] Transform[] spawnLocations;

    [SerializeField] Transform enemyTransform;

    [SerializeField] bool isSpecial;

    int maxTankPerLayout = 4;

    // 1,2,5,8,10,12,15,20,50
    /*
       0 : 1 -> level 1
       1 : 1 -> level 2
       0 : 1, 1 : 2 -> level 3
       0: 2, 1: 2 -> level 4
       2: 2 -> level 5
       1: 2, 2: 2 -> level 6
       2: 4 -> level 7
       3: 3, 2: 2 -> level 8
       3: 2, 1: 4 -> level 9
       4: 2 -> level 10
       4: 2, 2: 2, 1: 2 -> level 11
       5: 2, 4: 2 -> level 12
       3: 3, 2: 3 -> level 13
       5: 3, 4: 3 -> level 14
       6: 3 -> level 15
       6: 3, 5: 2 -> level 16
       5: 6 -> level 17
       6: 2, 5: 1, 4: 1, 2: 2 -> level 18
       6: 7 -> level 19
       7: 2 -> level 20
       8: 2 -> level 50
     */

        // TODO: layout 10-32

    private void OnEnable()
    {
        SpawnTanks();
    }

    void SpawnTanks()
    {
        int currentLevel = PlayerPrefs.GetInt("levelCount", 1);
        int tanksToSpawn = 3;
        int[] tankType = new int[7];

        // set player spawn location
        playerTankBody.position = playerSpawnLocation.position;
        playerTankBody.localPosition = new Vector3(playerTankBody.localPosition.x, -5.17f, playerTankBody.localPosition.z);
        playerTankHead.position = playerSpawnLocation.position;
        playerTankHead.localPosition = new Vector3(playerTankHead.localPosition.x, -4.79f, playerTankHead.localPosition.z);
        playerTankBody.rotation = playerSpawnLocation.rotation;
        playerTankHead.rotation = playerSpawnLocation.rotation;
        playerTankBody.localEulerAngles = new Vector3(0, playerTankBody.localEulerAngles.y, playerTankBody.localEulerAngles.z);
        playerTankHead.localEulerAngles = new Vector3(-90, playerTankHead.localEulerAngles.y, playerTankHead.localEulerAngles.z);

        // New Tanks
        if (PlayerPrefs.GetInt("SpawnNewTanks", 1) == 1)
        {
            // determine number of tanks to spawn
            if (currentLevel <= 20)
            {
                switch (currentLevel)
                {
                    case 1:
                        tankType[0] = 0;
                        tanksToSpawn = 1;
                        break;
                    case 2:
                        tankType[0] = 1;
                        tanksToSpawn = 1;
                        break;
                    case 3:
                        tankType[0] = 0;
                        tankType[1] = 1;
                        tankType[2] = 1;
                        tanksToSpawn = 3;
                        break;
                    case 4:
                        tankType[0] = 0;
                        tankType[1] = 0;
                        tankType[2] = 1;
                        tankType[3] = 1;
                        tanksToSpawn = 4;
                        break;
                    case 5:
                        tankType[0] = 2;
                        tankType[1] = 2;
                        tanksToSpawn = 2;
                        break;
                    case 6:
                        tankType[0] = 1;
                        tankType[1] = 1;
                        tankType[2] = 2;
                        tankType[3] = 2;
                        tanksToSpawn = 4;
                        break;
                    case 7:
                        tankType[0] = 2;
                        tankType[1] = 2;
                        tankType[2] = 2;
                        tankType[3] = 2;
                        tanksToSpawn = 4;
                        break;
                    case 8:
                        tankType[0] = 3;
                        tankType[1] = 3;
                        tankType[2] = 3;
                        tankType[3] = 2;
                        tanksToSpawn = 4;
                        break;
                    case 9:
                        tankType[0] = 3;
                        tankType[1] = 3;
                        tankType[2] = 1;
                        tanksToSpawn = 3;
                        break;
                    case 10:
                        tankType[0] = 4;
                        tankType[1] = 4;
                        tanksToSpawn = 2;
                        break;
                    case 11:
                        tankType[0] = 4;
                        tankType[1] = 4;
                        tankType[2] = 2;
                        tankType[3] = 2;
                        tanksToSpawn = 4;
                        break;
                    case 12:
                        tankType[0] = 5;
                        tankType[1] = 5;
                        tanksToSpawn = 2;
                        break;
                    case 13:
                        tankType[0] = 3;
                        tankType[1] = 3;
                        tankType[2] = 3;
                        tankType[3] = 2;
                        tanksToSpawn = 4;
                        break;
                    case 14:
                        tankType[0] = 5;
                        tankType[1] = 2;
                        tankType[2] = 2;
                        tanksToSpawn = 3;
                        break;
                    case 15:
                        tankType[0] = 6;
                        tankType[1] = 6;
                        tanksToSpawn = 2;
                        break;
                    case 16:
                        tankType[0] = 6;
                        tankType[1] = 6;
                        tankType[2] = 5;
                        tankType[3] = 5;
                        tanksToSpawn = 4;
                        break;
                    case 17:
                        tankType[0] = 5;
                        tankType[1] = 5;
                        tankType[2] = 5;
                        tankType[3] = 5;
                        tanksToSpawn = 4;
                        break;
                    case 18:
                        tankType[0] = 6;
                        tankType[1] = 6;
                        tankType[2] = 5;
                        tanksToSpawn = 3;
                        break;
                    case 19:
                        tankType[0] = 6;
                        tankType[1] = 6;
                        tankType[2] = 6;
                        tankType[3] = 6;
                        tanksToSpawn = 4;
                        break;
                    case 20:
                        tankType[0] = 7;
                        tankType[1] = 7;
                        tanksToSpawn = 2;
                        break;
                    default:
                        tankType[0] = 2;
                        tankType[1] = 2;
                        tankType[2] = 2;
                        tanksToSpawn = 3;
                        Debug.Log("Error");
                        break;
                }
            }
            else
            {
                if (currentLevel == 50)
                {
                    tankType[0] = 8;
                    tankType[1] = 8;
                    tanksToSpawn = 2;
                }
                else
                {
                    tanksToSpawn = Random.Range(1, 5);
                    for (int i = 0; i < tanksToSpawn; i++)
                    {
                        if (tanksToSpawn == 1)
                        {
                            if (currentLevel < 50)
                                tankType[i] = Random.Range(5, 8);
                            else
                                tankType[i] = Random.Range(6, 9);
                        }
                        else
                        {
                            if (currentLevel < 50)
                                tankType[i] = Random.Range(0, 8);
                            else
                                tankType[i] = Random.Range(0, 9);
                        }
                    }
                }
            }
            PlayerPrefs.SetInt("TanksToSpawnCount", tanksToSpawn);
            PlayerPrefs.SetInt("Tank0", tankType[0]);
            PlayerPrefs.SetInt("Tank1", tankType[1]);
            PlayerPrefs.SetInt("Tank2", tankType[2]);
            PlayerPrefs.SetInt("Tank3", tankType[3]);
            PlayerPrefs.SetInt("Tank4", tankType[4]);
            PlayerPrefs.SetInt("Tank5", tankType[5]);
            PlayerPrefs.SetInt("Tank6", tankType[6]);

            PlayerPrefs.SetInt("SpawnNewTanks", 0);
        }
        else // Old Tanks
        {
            tanksToSpawn = PlayerPrefs.GetInt("TanksToSpawnCount", 3);
        }

        // limit to 6
        tanksToSpawn = Mathf.Min(tanksToSpawn, maxTankPerLayout);

        if (!isSpecial)
        {

            // for loop for spawning tanks
            for (int i = 0; i < tanksToSpawn; i++)
            {
                // spawn tank
                GameObject tempObj = Instantiate(enemyTanks[PlayerPrefs.GetInt("Tank" + i.ToString(), 0)], transform.position, Quaternion.identity, enemyTransform);
                StartCoroutine(LateSpawn(tempObj, i));
            }
        }
        else
        {
            // spawn tank
            GameObject tempObj = Instantiate(enemyTanks[9], transform.position, Quaternion.identity, enemyTransform);
            StartCoroutine(LateSpawn(tempObj, 0));

            // spawn tank
            tempObj = Instantiate(enemyTanks[10], transform.position, Quaternion.identity, enemyTransform);
            StartCoroutine(LateSpawn(tempObj, 1));

            // spawn tank
            tempObj = Instantiate(enemyTanks[11], transform.position, Quaternion.identity, enemyTransform);
            StartCoroutine(LateSpawn(tempObj, 2));
        }
    }

    IEnumerator LateSpawn(GameObject tempObj, int i)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // get transforms
        Transform moveTransform = tempObj.GetComponentsInChildren<Transform>()[1];
        Transform headTransform = tempObj.GetComponentsInChildren<Transform>()[3];
        Transform tankTransform = tempObj.GetComponentsInChildren<Transform>()[9];

        // determine its postion
        headTransform.position = spawnLocations[i].position;
        headTransform.localPosition = new Vector3(headTransform.localPosition.x, -5.17f, headTransform.localPosition.z);
        tankTransform.localPosition = new Vector3(headTransform.localPosition.x, -4.79f, headTransform.localPosition.z);
        moveTransform.position = headTransform.position;

        // determine its rotation
        headTransform.rotation = spawnLocations[i].rotation;
        tankTransform.rotation = spawnLocations[i].rotation;
        headTransform.localEulerAngles = new Vector3(-90, headTransform.localEulerAngles.y, headTransform.localEulerAngles.z);
        tankTransform.localEulerAngles = new Vector3(0, tankTransform.localEulerAngles.y, tankTransform.localEulerAngles.z);
        moveTransform.rotation = headTransform.rotation;
    }
}
