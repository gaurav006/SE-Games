using DG.Tweening;
using UnityEngine;
namespace SEGames
{
    public class TileBehaviour : MonoBehaviour
    {
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        private SpriteRenderer _sr; // 2d sprite renderer join 
        private Sequence _pulseSeq;  // reset the tween meth 
        public float sizeEffect;
        private void Awake() => _sr = GetComponent<SpriteRenderer>();

        public void Init(int x, int y, Color color)
        {
            GridX = x; GridY = y;
            _sr.color = color;
            transform.localScale = Vector3.one;          
            _pulseSeq?.Kill(true);
            _pulseSeq = DOTween.Sequence();
            _pulseSeq.Append(transform.DOScale(sizeEffect, 0.4f))   // 1.05 size 
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetLink(gameObject);
        }

      
        public void OnTapped()
        {
            Debug.LogError("Tapped");
            SEBus.Emit(new TileTappedEvent { GridX = GridX, GridY = GridY });
        }

        public void ReturnToPool()
        {          
            _pulseSeq?.Kill(true);
            _pulseSeq = null;
        }
    }
}