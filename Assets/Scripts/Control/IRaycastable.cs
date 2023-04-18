using RPG.UI;

namespace RPG.Control
{
    public interface IRaycastable 
    {
        CursorType_SO GetCursorType(PlayerController callingController);
        bool HandleRaycast(PlayerController callingController);
        
    }
}