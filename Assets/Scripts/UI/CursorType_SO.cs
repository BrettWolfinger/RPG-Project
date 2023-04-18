using UnityEngine;

namespace RPG.UI
{
    [CreateAssetMenu(fileName = "CursorType_SO", menuName = "Cursors/Make New Cursor Type", order = 0)]
    public class CursorType_SO : ScriptableObject 
    {
        [SerializeField] private Texture2D cursor;
        [SerializeField] private Vector2 hotSpot;
 
        public void SetCursor() 
        {
            if(cursor!=null)
            {
                Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
            }
        }
    }
}