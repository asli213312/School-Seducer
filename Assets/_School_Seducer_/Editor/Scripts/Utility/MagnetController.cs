using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MagnetController : MonoBehaviour
    {
        private Transform _target;
        
            public void SetTarget(Transform target)
            {
                _target = target;
            }
        
            private void OnTriggerStay2D(Collider2D other)
            {
                if (_target == null) return; // Проверяем, есть ли цель
                Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Получаем Rigidbody магнита
                if (rb != null)
                {
                    // Вычисляем направление к цели
                    Vector2 direction = (transform.position - _target.position).normalized;
        
                    // Притягиваем магнит к цели с помощью Rigidbody
                    rb.AddForce(direction * 10f);
                }
            }
    }
}