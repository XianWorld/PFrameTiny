using Unity.Mathematics;
using Unity.Entities;
using Unity.Tiny;

namespace PFrame.Tiny.Particles
{
    // Modifies the position of the particle every frame.
    struct ParticleVelocity : IComponentData
    {
        public float initSpeed;
        public float3 velocity;
        public float speedMultiplier;
        public bool isWorld;
        public quaternion startRotation;
        public quaternion startRotationInverse;

        public float3 finalVelocity;
        public float finalSpeed;

        public float randomFactor;
    };

    // Modifies the rotation around z axis.
    struct ParticleAngularVelocity : IComponentData
    {
        public float3 axis;
        public float angularVelocity;
        public float randomFactor;
    };

    struct ParticleLifetimeColor : IComponentData
    {
        public Color initialColor;
    };

    struct ParticleLifetimeScale : IComponentData
    {
        public float3 initialScale;
        public float randomFactor;
    };

    struct BurstEmissionInternal : IComponentData
    {
        // If < 0.0, then the next burst should be emitted.
        public float cooldown;

        // How many cycles left.
        public float cycle;
    };

    struct ParticleEmitterInternal : ISystemStateComponentData
    {
        public Entity particleTemplate;
        public float particleSpawnCooldown;
        public uint numParticles;
    }

    // TODO: It supossed to be ISharedComponentData but it's not supported yet.
    // Reference to the emitter that emitted this particle.
    // struct ParticleEmitterReference : ISharedComponentData
    struct ParticleEmitterReference : IComponentData
    {
        public Entity emitter;
    }
}
