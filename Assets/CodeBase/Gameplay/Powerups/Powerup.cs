using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Powerup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SpaceShip ship = collision.transform.root.GetComponent<SpaceShip>();

            if (ship == null) return;

            if (Player.Instance != null && Player.Instance.ActiveShip != null && ship != Player.Instance.ActiveShip)
                return;

            OnPickedUp(ship);

            Destroy(gameObject);
        }

        protected abstract void OnPickedUp(SpaceShip ship);
    }
}