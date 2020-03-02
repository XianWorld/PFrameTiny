using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace PFrame.Tiny.Particles
{
    static class ParticlesSource
    {
        private static Random m_rand = new Random(1);

        public static void InitEmitterCircleSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool isWorld)
        {
            var localToWorld = mgr.GetComponentData<LocalToWorld>(emitter);
            var rotation = localToWorld.Rotation;

            var source = mgr.GetComponentData<EmitterCircleSource>(emitter);
            foreach (var particle in particles)
            {
                float randomAngle = m_rand.NextFloat((float)-math.PI, (float)math.PI);
                //sqrt for uniform distribution
                float radiusNormalized =
                    source.radius.end != 0.0f
                        ? math.sqrt(m_rand.NextFloat(source.radius.start / source.radius.end, 1.0f))
                        : 0.0f;
                float randomRadius = radiusNormalized * source.radius.end;

                //var positionNormalized = new float2(math.sin(randomAngle), math.cos(randomAngle));
                //var pos = new float3(positionNormalized.x * randomRadius, positionNormalized.y * randomRadius, 0.0f);
                var pos = new float3();
                pos.x = math.sin(randomAngle) * randomRadius;
                pos.y = math.cos(randomAngle) * randomRadius;

                if (isWorld)
                    pos = math.mul(rotation, pos);

                var Translation = mgr.GetComponentData<Translation>(particle);
                Translation.Value += pos;
                mgr.SetComponentData(particle, Translation);

                var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);//new ParticleVelocity();
                float speed = particleVelocity.initSpeed;
                if (speed != 0f)
                {
                    if (source.speedBasedOnRadius)
                        speed *= radiusNormalized;

                    var direction = math.normalizesafe(pos);
                    var velocity = direction * speed;

                    particleVelocity.velocity = velocity;
                    mgr.SetComponentData(particle, particleVelocity);
                }

                //if (source.speed.start != 0.0f && source.speed.end != 0.0f)
                //{
                //    float randomSpeed = m_rand.NextFloat(source.speed.start, source.speed.end);
                //    if (source.speedBasedOnRadius)
                //        randomSpeed *= radiusNormalized;

                //    var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);// new ParticleVelocity();

                //    var velocity = new float3(positionNormalized.x * randomSpeed, positionNormalized.y * randomSpeed, 0.0f);
                //    if (isWorld)
                //        velocity = math.mul(rotation, velocity);
                //    particleVelocity.velocity = velocity;
                //    mgr.SetComponentData(particle, particleVelocity);
                //    //particleVelocity.speedMultiplier = 1f;
                //    //particleVelocity.isWorld = isWorld;
                //    //particleVelocity.startRotation = rotation;

                //    //if (mgr.HasComponent<ParticleVelocity>(particle))
                //    //    mgr.SetComponentData(particle, particleVelocity);
                //    //else
                //    //    mgr.AddComponentData(particle, particleVelocity);
                //}
            }
        }

        public static void InitEmitterSphereSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool isWorld)
        {
            var localToWorld = mgr.GetComponentData<LocalToWorld>(emitter);
            var rotation = localToWorld.Rotation;

            var source = mgr.GetComponentData<EmitterSphereSource>(emitter);
            foreach (var particle in particles)
            {
                float randomAngle = m_rand.NextFloat((float)-math.PI, (float)math.PI);
                float radiusNormalized =
                    source.radius.end != 0.0f
                        ? math.sqrt(m_rand.NextFloat(source.radius.start / source.radius.end, 1.0f))
                        : 0.0f;
                float randomRadius = radiusNormalized * source.radius.end;

                //var positionNormalized = new float2(math.sin(randomAngle), math.cos(randomAngle));
                //var position = new float3(positionNormalized.x * randomRadius, positionNormalized.y * randomRadius, 0.0f);
                var direction = m_rand.NextFloat3Direction();
                var position = direction * randomRadius;

                //if (isWorld)
                //    position = math.mul(rotation, position);

                var Translation = mgr.GetComponentData<Translation>(particle);
                Translation.Value += position;
                mgr.SetComponentData(particle, Translation);

                var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);//new ParticleVelocity();
                float speed = particleVelocity.initSpeed;
                if(speed != 0f)
                {
                    if (source.speedBasedOnRadius)
                        speed *= radiusNormalized;

                    direction = m_rand.NextFloat3Direction();
                    var velocity = direction * speed;

                    particleVelocity.velocity = velocity;
                    mgr.SetComponentData(particle, particleVelocity);
                }

                //if (source.speed.start != 0.0f && source.speed.end != 0.0f)
                //{
                //    float randomSpeed = m_rand.NextFloat(source.speed.start, source.speed.end);
                //    if (source.speedBasedOnRadius)
                //        randomSpeed *= radiusNormalized;

                //    var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);//new ParticleVelocity();

                //    //var velocity = new float3(positionNormalized.x * randomSpeed, positionNormalized.y * randomSpeed, 0.0f);
                //    var velocity = direction * randomSpeed;

                //    //if (isWorld)
                //    //    velocity = math.mul(rotation, velocity);
                //    particleVelocity.velocity = velocity;
                //    mgr.SetComponentData(particle, particleVelocity);

                //    //particleVelocity.isWorld = isWorld;
                //    //particleVelocity.startRotation = rotation;

                //    //if (mgr.HasComponent<ParticleVelocity>(particle))
                //    //    mgr.SetComponentData(particle, particleVelocity);
                //    //else
                //    //    mgr.AddComponentData(particle, particleVelocity);
                //}
            }
        }

        public static void InitEmitterConeSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool isWorld)
        {
            var localToWorld = mgr.GetComponentData<LocalToWorld>(emitter);
            var rotation = localToWorld.Rotation;

            var source = mgr.GetComponentData<EmitterConeSource>(emitter);
            foreach (var particle in particles)
            {
                float randomAngle = m_rand.NextFloat((float)-math.PI, (float)math.PI);
                float radiusNormalized =
                    source.radius.end != 0.0f
                        ? math.sqrt(m_rand.NextFloat(source.radius.start / source.radius.end, 1.0f))
                        : 0.0f;
                float randomRadius = radiusNormalized * source.radius.end;

                var baseX = math.sin(randomAngle);
                var baseY = math.cos(randomAngle);
                var pos = new float3();
                pos.x = baseX * randomRadius;
                pos.y = baseY * randomRadius;

                if (isWorld)
                    pos = math.mul(rotation, pos);

                var Translation = mgr.GetComponentData<Translation>(particle);
                Translation.Value += pos;
                mgr.SetComponentData(particle, Translation);

                var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);
                float speed = particleVelocity.initSpeed;
                if (speed != 0f)
                {
                    float spreadAngle = source.angle * radiusNormalized;

                    float directionRadius = math.sin(spreadAngle);
                    float directionHeight = math.cos(spreadAngle);
                    var direction = new float3();
                    direction.x = baseX * directionRadius;
                    direction.y = baseY * directionRadius;
                    direction.z = directionHeight;

                    if (isWorld)
                        direction = math.mul(rotation, direction);

                    var velocity = direction * speed;

                    particleVelocity.velocity = velocity;
                    mgr.SetComponentData(particle, particleVelocity);
                }
            }
        }

        public static void InitEmitterConeVolumeSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool isWorld)
        {
            var localToWorld = mgr.GetComponentData<LocalToWorld>(emitter);
            var rotation = localToWorld.Rotation;

            var source = mgr.GetComponentData<EmitterConeVolumeSource>(emitter);
            foreach (var particle in particles)
            {
                float randomAngle = m_rand.NextFloat((float)-math.PI, (float)math.PI);
                float radiusNormalized =
                    source.radius.end != 0.0f
                        ? math.sqrt(m_rand.NextFloat(source.radius.start / source.radius.end, 1.0f))
                        : 0.0f;
                float length = m_rand.NextFloat(source.length);
                var radiusRate = math.sin(source.angle);
                var radiusMax = source.radius.end + radiusRate * length;
                float randomRadius = radiusNormalized * radiusMax;

                var baseX = math.sin(randomAngle);
                var baseY = math.cos(randomAngle);
                var pos = new float3();
                pos.x = baseX * randomRadius;
                pos.y = baseY * randomRadius;
                pos.z = length;

                var direction = pos;
                direction.z -= source.length / 2f;

                if (isWorld)
                    pos = math.mul(rotation, pos);

                var Translation = mgr.GetComponentData<Translation>(particle);
                Translation.Value += pos;
                mgr.SetComponentData(particle, Translation);

                var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);
                float speed = particleVelocity.initSpeed;
                if (speed != 0f)
                {
                    //spread out off the central point
                    //direction = math.normalize(direction);
                    //if (isWorld)
                    //    direction = math.mul(rotation, direction);
                    direction = m_rand.NextFloat3Direction();

                    var velocity = direction * speed;

                    particleVelocity.velocity = velocity;
                    mgr.SetComponentData(particle, particleVelocity);
                }
            }
        }

        //public static void InitEmitterConeSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool attachToEmitter)
        //{
        //    var source = mgr.GetComponentData<EmitterConeSource>(emitter);
        //    float3 localPositionOnConeBase;

        //    quaternion rotation = quaternion.identity;
        //    if (!attachToEmitter)
        //    {
        //        if (mgr.HasComponent<LocalToWorld>(emitter))
        //        {
        //            var toWorld = mgr.GetComponentData<LocalToWorld>(emitter);
        //            //rotation = new quaternion(toWorld.Value);
        //            rotation = toWorld.Rotation;
        //        }
        //    }

        //    foreach (var particle in particles)
        //    {
        //        float angle = m_rand.NextFloat((float)-math.PI, (float)math.PI);
        //        float radiusNormalized = math.sqrt(m_rand.NextFloat(0.0f, 1.0f));
        //        float radius = source.radius * radiusNormalized;

        //        localPositionOnConeBase.x = math.sin(angle);
        //        localPositionOnConeBase.y = math.cos(angle);
        //        localPositionOnConeBase.z = 0.0f;

        //        var worldPositionOnConeBase = math.mul(rotation, localPositionOnConeBase);
        //        var Translation = mgr.GetComponentData<Translation>(particle);
        //        Translation.Value += worldPositionOnConeBase * radius;
        //        mgr.SetComponentData(particle, Translation);

        //        //ParticleVelocity particleVelocity = new ParticleVelocity();
        //        ParticleVelocity particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);
        //        float spreadAngle = source.angle * radiusNormalized;

        //        float directionRadius = math.sin(spreadAngle);
        //        float directionHeight = math.cos(spreadAngle);
        //        particleVelocity.velocity.x = localPositionOnConeBase.x * directionRadius;
        //        particleVelocity.velocity.y = localPositionOnConeBase.y * directionRadius;
        //        particleVelocity.velocity.z = directionHeight;
        //        particleVelocity.velocity *= source.speed;

        //        particleVelocity.velocity = math.mul(rotation, particleVelocity.velocity);

        //        mgr.SetComponentData(particle, particleVelocity);
        //        //particleVelocity.isWorld = !attachToEmitter;
        //        //particleVelocity.startRotation = rotation;

        //        //if (mgr.HasComponent<ParticleVelocity>(particle))
        //        //    mgr.SetComponentData(particle, particleVelocity);
        //        //else
        //        //    mgr.AddComponentData(particle, particleVelocity);
        //    }
        //}

        public static void InitEmitterBoxSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool isWorld)
        {
            var source = mgr.GetComponentData<EmitterBoxSource>(emitter);

            var localToWorld = mgr.GetComponentData<LocalToWorld>(emitter);

            foreach (var particle in particles)
            {
                var pos = ParticlesUtil.RandomPointInBox(source.box);

                if (isWorld)
                    pos = math.mul(localToWorld.Rotation, pos);

                var Translation = mgr.GetComponentData<Translation>(particle);
                Translation.Value += pos;
                mgr.SetComponentData(particle, Translation);

                var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);//new ParticleVelocity();
                float speed = particleVelocity.initSpeed;
                if (speed != 0f)
                {
                    //var direction = math.normalizesafe(pos);
                    var direction = m_rand.NextFloat3Direction();
                    var velocity = direction * speed;

                    particleVelocity.velocity = velocity;
                    mgr.SetComponentData(particle, particleVelocity);
                }
            }
        }

        public static void InitEmitterRectSource(EntityManager mgr, Entity emitter, NativeArray<Entity> particles, bool isWorld)
        {
            var source = mgr.GetComponentData<EmitterRectSource>(emitter);

            var localToWorld = mgr.GetComponentData<LocalToWorld>(emitter);

            foreach (var particle in particles)
            {
                var pos2 = ParticlesUtil.RandomPointInRect(source.rect);
                var pos = new float3(pos2.x, pos2.y, 0.0f);
                if (isWorld)
                    pos = math.mul(localToWorld.Rotation, pos);

                var Translation = mgr.GetComponentData<Translation>(particle);
                Translation.Value += pos;
                mgr.SetComponentData(particle, Translation);

                var particleVelocity = mgr.GetComponentData<ParticleVelocity>(particle);//new ParticleVelocity();
                float speed = particleVelocity.initSpeed;
                if (speed != 0f)
                {
                    var direction = math.normalizesafe(pos);
                    var velocity = direction * speed;

                    particleVelocity.velocity = velocity;
                    mgr.SetComponentData(particle, particleVelocity);
                }
            }
        }
    }
} // namespace Particles
