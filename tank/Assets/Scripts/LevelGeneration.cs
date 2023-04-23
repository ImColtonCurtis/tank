using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation
{
    //[ExecuteInEditMode]
    public class LevelGeneration : MonoBehaviour
    {
        [SerializeField] NavMeshSurface surface;

        [SerializeField] GameObject[] layouts;

        [SerializeField] int layoutToSpawn;

        private void OnEnable()
        {
            SpawnLevel();
        }
        
        void SpawnLevel()
        {
            int currentLevel = PlayerPrefs.GetInt("levelCount", 1);
            int chosenLevel;
            // despawn all levels
            foreach (GameObject layout in layouts)
                layout.SetActive(false);

            // New Level
            if (PlayerPrefs.GetInt("SpawnNewLevel", 0) == 1)
            {
                if (currentLevel <= 20)
                    chosenLevel = currentLevel - 1;
                else
                    chosenLevel = Random.Range(0, layouts.Length);
                PlayerPrefs.SetInt("LevelLayout", chosenLevel);

                PlayerPrefs.SetInt("SpawnNewLevel", 0);
            }
            else // Old Level
                chosenLevel = PlayerPrefs.GetInt("LevelLayout", 0);

            if (layoutToSpawn >= 0)
                layouts[layoutToSpawn].SetActive(true);
            else
                layouts[chosenLevel].SetActive(true);

            // update navmesh
            surface.BuildNavMesh();
        }
    }
}
