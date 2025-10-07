using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDamageable
{
    public bool canTakeDamage { get; }
    public void TakeDamage(int damage, Vector2 knockbackDirection);
}