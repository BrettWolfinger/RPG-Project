using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Portal", menuName = "Portal", order = 0)]
public class Portal_SO : ScriptableObject {
    [SerializeField] int sceneToLoad;
    public int SceneToLoad => sceneToLoad;

    [SerializeField] Portal_SO destination;
    public Portal_SO Destination => destination;
}
