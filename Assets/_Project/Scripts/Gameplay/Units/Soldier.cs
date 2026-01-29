using UnityEngine;

public class Soldier : Unit
{
    protected override void Die()
    {
        EventSystem.Instance.Trigger(new SoldierDeathEvent(this));
        base.Die();
    }
}