using UnityEngine;

public interface IDamageable
{
    public int currentHealth { get; set; }
    public void TakeDamage(int damage, Vector3 hitSource);
}
