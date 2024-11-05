using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    #region Beats pooling

    [SerializeField] int _beatPoolCount = 5;

    [SerializeField] Transform _beatParent;
    [SerializeField] List<GameObject> _beatPrefabs;

    Dictionary<string, Queue<GameObject>> _beatPools = new Dictionary<string, Queue<GameObject>>();

    #endregion

    [SerializeField] int _speed;

    void OnEnable()
    {
        Events.OnArrived += DespawanBeat;
    }

    void Start()
    {
        foreach (GameObject beatPrefab in _beatPrefabs)
        {
            _beatPools[beatPrefab.tag] = new Queue<GameObject>();
        }

        InstantiateBeat();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBeat();
        }
    }

    void InstantiateBeat()
    {
        for (int i = 0; i < _beatPoolCount; i++)
        {
            foreach (GameObject beatPrefab in _beatPrefabs)
            {
                GameObject createdBeat = Instantiate(beatPrefab, _beatParent);
                beatPrefab.SetActive(false);

                EnqueueBeatPool(createdBeat);
            }
        }
    }

    void SpawnBeat()
    {
        int selectedBeat = Random.Range(0, _beatPrefabs.Count);
        string beatKey = _beatPrefabs[selectedBeat].tag;

        GameObject newBeat = DequeueBeatPool(beatKey);

        if (newBeat != null)
        {
            newBeat.SetActive(true);
            newBeat.gameObject.GetComponent<Beat>().Speed = _speed;
        }
        else
        {
            Debug.LogWarning("No beat available in the queue for selectedBeat: " + selectedBeat);
        }
    }

    void DespawanBeat(GameObject beat)
    {
        if (beat == null)
        {
            Debug.LogWarning("Attempted to despawn a null beat.");
            return;
        }

        beat.SetActive(false);
        EnqueueBeatPool(beat);
    }

    void EnqueueBeatPool(GameObject beat)
    {
        // Check if the beat has a tag and if it exists in the dictionary
        if (beat != null && _beatPools.TryGetValue(beat.tag, out Queue<GameObject> beatQueue))
        {
            beatQueue.Enqueue(beat);
        }
        else
        {
            Debug.LogWarning($"No pool found for beat with tag: {beat.tag}");
        }
    }

    GameObject DequeueBeatPool(string beatKey)
    {
        if(_beatPools.TryGetValue(beatKey, out Queue<GameObject> beatQueue) && beatQueue.Count > 0)
        {
            return beatQueue.Dequeue();
        }
        else
        {
            Debug.LogWarning($"No beat available in the pool for id: {beatKey}");
            return null;
        }
    }
}
