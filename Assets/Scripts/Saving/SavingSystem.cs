using UnityEngine;
using System.IO;
using System.Text;
using System;

namespace RPG.Saving
{
    /*Core saving system script with generic saving functionality
    */
    public class SavingSystem : MonoBehaviour 
    {
        public void Save(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("save to " + path);
            Transform playerTransform = GetPlayerTransform();
            //FileStream stream = File.Open(path, FileMode.Create);
            byte[] buffer = SerializeVector(playerTransform.position);
            File.WriteAllBytes(path,buffer);
            //stream.write(bytes,0,bytes.Length);
            //stream.Close();
        }

        public void Load(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("load from "+ path);
            byte[] buffer = File.ReadAllBytes(path);
            Transform playerTransform = GetPlayerTransform();
            playerTransform.position = DeserializeVector(buffer);
        }

        private Transform GetPlayerTransform()
        {
            return GameObject.FindWithTag("Player").transform;
        }

        private byte[] SerializeVector(Vector3 vector)
        {
            byte[] vectorBytes = new byte[3*4];
            BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
            BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 4);
            BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 8);
            return vectorBytes;
        }

        private Vector3 DeserializeVector(byte[] buffer)
        {
            Vector3 result = new Vector3();
            result.x = BitConverter.ToSingle(buffer,0);
            result.y = BitConverter.ToSingle(buffer,4);
            result.z = BitConverter.ToSingle(buffer,8);
            return result;
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath,saveFile);
        }
    }
}