using UnityEngine;

public class Enemy : Unit
{
    protected override void Die()
    {
        EventSystem.Instance.Trigger(new EnemyDeathEvent(
            this,
            Data.goldReward,
            Data.silverReward,
            Data.xpReward
        ));
        
        base.Die();
    }
}