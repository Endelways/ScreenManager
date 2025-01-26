using UnityEngine;

namespace Endelways.ScreenManager
{
    public abstract class Screen : MonoBehaviour
    {
        [field: SerializeField] public bool IsReusableScreen { get; private set; }

        public virtual void OnOpened(IScreenOptions options)
        {
            OnDisplay();
        }

        public virtual void OnDisplay()
        {
            
        }
        
        public virtual void OnClosed()
        {
            OnHide();
        }

        public virtual void OnHide()
        {
          
        }
    }
}