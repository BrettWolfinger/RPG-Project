using UnityEngine;

namespace RPG.Saving
{  
    [System.Serializable]
    public class SerializableVector3
    {
        float x,y,z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector()
        {
            Vector3 vector = new Vector3();
            vector.x = x;
            vector.y = y;
            vector.z = z;
            return vector;
        }
    }
}