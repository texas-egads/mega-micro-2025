using UnityEngine;


namespace ZABsters {
    public class TaskCreator : MonoBehaviour
    {

        public GameObject[] taskPrefabs;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //randomly instantiate a task prefab from the array
            int randomIndex = Random.Range(0, taskPrefabs.Length);
            Instantiate(taskPrefabs[randomIndex], transform.position, Quaternion.identity);
        }

    }
}