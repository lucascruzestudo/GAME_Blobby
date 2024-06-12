using UnityEngine;

public class ImpactParticleSettings : MonoBehaviour
{
    public float impactForce = 10f;
    public float spreadForce = 2f;

    private ParticleSystem particles;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        if (particles == null)
        {
            Debug.LogError("No ParticleSystem found on this GameObject!");
            enabled = false;
            return;
        }

        var main = particles.main;
        main.startSpeed = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        var collision = particles.collision;
        collision.enabled = true;
        collision.quality = ParticleSystemCollisionQuality.High;
        collision.collidesWith = LayerMask.GetMask("Default");
        collision.bounce = 0f;
        
        var forceOverLifetime = particles.forceOverLifetime;
        forceOverLifetime.enabled = true;
        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, spreadForce));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, impactForce), new Keyframe(1, 0));
        AnimationCurve curveZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        forceOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, curveX);
        forceOverLifetime.y = new ParticleSystem.MinMaxCurve(1f, curveY);
        forceOverLifetime.z = new ParticleSystem.MinMaxCurve(1f, curveZ);
    }
}
