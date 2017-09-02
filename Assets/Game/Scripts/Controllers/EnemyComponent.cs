using UnityEngine;
using System.Collections;

public class EnemyComponent : Entity
{
    [SerializeField] EnemyMotor motor;
    [Header("Health")]
    [SerializeField] int defaultHP;


    private void OnEnable()
    {
        SetDefaultHP(defaultHP);

        motor.OnDisable += Die;
        motor.Start();
    }
    private void Update()
    {
        motor.Update();
    }
    public override void Die()
    {
        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Explosion);

        var explosion = AppServices.Instance.PoolManager.GetPooledPrefabTimed(PooledPrefabs.SmallExplsion, 3);
        explosion.transform.position = transform.position;

        GameServices.Instance.ChallengeManager.DeactivateEntity(gameObject);
        SetDefaultHP(defaultHP);
    }
}
