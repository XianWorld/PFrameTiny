using Unity.Entities;
using Unity.Tiny.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.Tiny;
using Unity.Mathematics;
using PFrame.Tiny.Interpolation;
using System.Collections.Generic;
using Unity.Transforms;

namespace PFrame.Tiny.Particles.Authoring
{
    //[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    //public class ParticleSystemConversionSystem : GameObjectConversionSystem
    //{
    //    private static Unity.Mathematics.Random m_rand = new Unity.Mathematics.Random(1);

    //    protected override void OnUpdate()
    //    {
    //        Entities.ForEach((ParticleSystemAuthoring auth) =>
    //        {
    //            var particleSystem = auth.gameObject.GetComponent<UnityEngine.ParticleSystem>();
    //            var emitterEntity = GetPrimaryEntity(auth);
    //            var dstManager = DstEntityManager;

    //            UnityEngine.Debug.Log("ParticleSystemConversionSystem:" + emitterEntity);

    //            //add over lifetime
    //            if (particleSystem.rotationOverLifetime.enabled)
    //            {
    //                var curveEntity = CreateAdditionalEntity(auth);
    //                var keysEntity = CreateAdditionalEntity(auth);

    //                var lifeTimeModule = particleSystem.rotationOverLifetime;
    //                var lifeTimeComp = new LifetimeAngularVelocity();
    //                lifeTimeComp.curve = GetCurveFromMMC(lifeTimeModule.z, dstManager, curveEntity, keysEntity);
    //                dstManager.AddComponentData<LifetimeAngularVelocity>(emitterEntity, lifeTimeComp);
    //            }
    //            if (particleSystem.sizeOverLifetime.enabled)
    //            {
    //                var curveEntity = CreateAdditionalEntity(auth);
    //                var keysEntity = CreateAdditionalEntity(auth);

    //                var lifeTimeModule = particleSystem.sizeOverLifetime;
    //                var lifeTimeComp = new LifetimeScale();
    //                lifeTimeComp.curve = GetCurveFromMMC(lifeTimeModule.size, dstManager, curveEntity, keysEntity);
    //                dstManager.AddComponentData<LifetimeScale>(emitterEntity, lifeTimeComp);
    //            }
    //            if (particleSystem.velocityOverLifetime.enabled)
    //            {
    //                var curveEntity = CreateAdditionalEntity(auth);
    //                var keysEntity = CreateAdditionalEntity(auth);

    //                var lifeTimeModule = particleSystem.velocityOverLifetime;
    //                var lifeTimeComp = new LifetimeSpeedMultiplier();
    //                lifeTimeComp.curve = GetCurveFromMMC(lifeTimeModule.speedModifier, dstManager, curveEntity, keysEntity);
    //                dstManager.AddComponentData<LifetimeSpeedMultiplier>(emitterEntity, lifeTimeComp);
    //            }
    //            if (particleSystem.velocityOverLifetime.enabled)
    //            {
    //                var curveEntity = CreateAdditionalEntity(auth);
    //                var keysEntity = CreateAdditionalEntity(auth);

    //                var lifeTimeModule = particleSystem.velocityOverLifetime;
    //                var lifeTimeComp = new LifetimeVelocity();
    //                lifeTimeComp.curve = GetCurve3FromMMC(lifeTimeModule.x, lifeTimeModule.y, lifeTimeModule.z, dstManager, curveEntity, keysEntity);
    //                dstManager.AddComponentData<LifetimeVelocity>(emitterEntity, lifeTimeComp);
    //            }
    //        });
    //    }

    //    public static Entity GetCurve3FromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmcX, UnityEngine.ParticleSystem.MinMaxCurve mmcY, UnityEngine.ParticleSystem.MinMaxCurve mmcZ, EntityManager dstManager, Entity curveEntity, Entity keysEntity)
    //    {
    //        //var curveEntity = dstManager.CreateEntity();
    //        //var keysEntity = dstManager.CreateEntity();

    //        if (mmcX.mode == ParticleSystemCurveMode.Constant)
    //        {
    //            var curve = new StepCurveFloat { keys = keysEntity };
    //            dstManager.AddComponentData<StepCurveFloat>(curveEntity, curve);

    //            var buffer = dstManager.AddBuffer<KeyFloat3>(keysEntity);
    //            var value = new float3(mmcX.constant, mmcY.constant, mmcZ.constant);
    //            buffer.Add(new KeyFloat3(0f, value));
    //            buffer.Add(new KeyFloat3(1f, value));
    //        }
    //        else if (mmcX.mode == ParticleSystemCurveMode.TwoConstants)
    //        {
    //            var curve = new LinearCurveFloat { keys = keysEntity };
    //            dstManager.AddComponentData<LinearCurveFloat>(curveEntity, curve);

    //            var buffer = dstManager.AddBuffer<KeyFloat3>(keysEntity);
    //            var min = new float3(mmcX.constantMin, mmcY.constantMin, mmcZ.constantMin);
    //            buffer.Add(new KeyFloat3(0f, min));
    //            var max = new float3(mmcX.constantMax, mmcY.constantMax, mmcZ.constantMax);
    //            buffer.Add(new KeyFloat3(1f, max));
    //        }
    //        else if (mmcX.mode == ParticleSystemCurveMode.Curve)
    //        {
    //            var curve = new BezierCurveFloat { keys = keysEntity };
    //            dstManager.AddComponentData<BezierCurveFloat>(curveEntity, curve);

    //            var buffer = dstManager.AddBuffer<KeyFloat3>(keysEntity);
    //            var keyFrames = mmcX.curve.keys;
    //            for (int i = 0; i < keyFrames.Length; i++)
    //            {
    //                var keyFrame = keyFrames[i];
    //                var time = keyFrame.time;
    //                var value = new float3(keyFrame.value, mmcY.curve.Evaluate(time), mmcZ.curve.Evaluate(time));

    //                buffer.Add(new KeyFloat3(time, keyFrame.value));
    //            }
    //        }

    //        return curveEntity;
    //    }

    //    public static Entity GetCurveFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc, EntityManager dstManager, Entity curveEntity, Entity keysEntity)
    //    {
    //        //var curveEntity = dstManager.CreateEntity();
    //        //var keysEntity = dstManager.CreateEntity();

    //        UnityEngine.Debug.Log("ParticleSystemConversionSystem: curveEntity = " + curveEntity);

    //        if (mmc.mode == ParticleSystemCurveMode.Constant)
    //        {
    //            var curve = new StepCurveFloat { keys = keysEntity };
    //            dstManager.AddComponentData<StepCurveFloat>(curveEntity, curve);

    //            var buffer = dstManager.AddBuffer<KeyFloat>(keysEntity);
    //            buffer.Add(new KeyFloat(0f, mmc.constant));
    //            buffer.Add(new KeyFloat(1f, mmc.constant));
    //        }
    //        else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
    //        {
    //            var curve = new LinearCurveFloat { keys = keysEntity };
    //            dstManager.AddComponentData<LinearCurveFloat>(curveEntity, curve);

    //            var buffer = dstManager.AddBuffer<KeyFloat>(keysEntity);
    //            buffer.Add(new KeyFloat(0f, mmc.constantMin));
    //            buffer.Add(new KeyFloat(1f, mmc.constantMax));
    //        }
    //        else if (mmc.mode == ParticleSystemCurveMode.Curve)
    //        {
    //            var curve = new BezierCurveFloat { keys = keysEntity };
    //            dstManager.AddComponentData<BezierCurveFloat>(curveEntity, curve);

    //            var buffer = dstManager.AddBuffer<KeyFloat>(keysEntity);
    //            var keyFrames = mmc.curve.keys;
    //            for (int i = 0; i < keyFrames.Length; i++)
    //            {
    //                var keyFrame = keyFrames[i];
    //                buffer.Add(new KeyFloat(keyFrame.time, keyFrame.value));
    //            }
    //        }

    //        return curveEntity;
    //    }

    //    public static float GetValueFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc)
    //    {
    //        float value = 1f;
    //        if (mmc.mode == ParticleSystemCurveMode.Constant)
    //        {
    //            value = mmc.constant;
    //        }
    //        else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
    //        {
    //            value = m_rand.NextFloat(mmc.constantMin, mmc.constantMax);
    //        }

    //        return value;
    //    }

    //    public static void GetMinMaxFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc, out float minValue, out float maxValue)
    //    {
    //        minValue = 2f;
    //        maxValue = 2f;
    //        if (mmc.mode == ParticleSystemCurveMode.Constant)
    //        {
    //            minValue = mmc.constant;
    //            maxValue = mmc.constant;
    //        }
    //        else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
    //        {
    //            minValue = mmc.constantMin;
    //            maxValue = mmc.constantMax;
    //        }
    //    }

    //    public static Range GetRangeFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc)
    //    {
    //        var minValue = 2f;
    //        var maxValue = 2f;
    //        if (mmc.mode == ParticleSystemCurveMode.Constant)
    //        {
    //            minValue = mmc.constant;
    //            maxValue = mmc.constant;
    //        }
    //        else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
    //        {
    //            minValue = mmc.constantMin;
    //            maxValue = mmc.constantMax;
    //        }

    //        return new Range { start = minValue, end = maxValue };
    //    }

    //}

    [RequiresEntityConversion]
    [RequireComponent(typeof(UnityEngine.ParticleSystem))]
    public class ParticleSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        private static Unity.Mathematics.Random m_rand = new Unity.Mathematics.Random(1);

        public GameObject ParticlePrefab;

        // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ParticlePrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var particleSystem = gameObject.GetComponent<UnityEngine.ParticleSystem>();
            var mainModule = particleSystem.main;

            var particleEntity = conversionSystem.GetPrimaryEntity(ParticlePrefab);
            if (!dstManager.HasComponent<NonUniformScale>(particleEntity))
            {
                var scale = new NonUniformScale()
                {
                    Value = 1f
                };
                dstManager.AddComponentData<NonUniformScale>(particleEntity, scale);
            }

            var emitterEntity = entity;// dstManager.CreateEntity();
            //UnityEngine.Debug.Log("ParticleSystemAuthoring:" + emitterEntity);
            //add emitter
            var emitter = new ParticleEmitter();
            emitter.particle = particleEntity;
            emitter.attachToEmitter = mainModule.simulationSpace == ParticleSystemSimulationSpace.Local;
            emitter.lifetime = GetRangeFromMMC(mainModule.startLifetime);
            emitter.emitRate = GetValueFromMMC(particleSystem.emission.rateOverTime);
            emitter.maxParticles = (uint)mainModule.maxParticles;
            dstManager.AddComponentData<ParticleEmitter>(emitterEntity, emitter);

            //add emitter shape
            var shapeModule = particleSystem.shape;
            if (shapeModule.shapeType == ParticleSystemShapeType.Box)
            {
                var source = new EmitterBoxSource();
                var min = shapeModule.position - shapeModule.scale / 2f;
                var max = shapeModule.position + shapeModule.scale / 2f;
                source.box = new float3x2(min, max);
                dstManager.AddComponentData<EmitterBoxSource>(emitterEntity, source);
            }
            else if (shapeModule.shapeType == ParticleSystemShapeType.Rectangle)
            {
                var source = new EmitterRectSource();
                var x = shapeModule.position.x - shapeModule.scale.x / 2f;
                var y = shapeModule.position.y - shapeModule.scale.y / 2f;
                source.rect = new Unity.Tiny.Rect(x, y, shapeModule.scale.x, shapeModule.scale.y);
                dstManager.AddComponentData<EmitterRectSource>(emitterEntity, source);
            }
            else if (shapeModule.shapeType == ParticleSystemShapeType.Cone)
            {
                var source = new EmitterConeSource();
                source.angle = math.radians(shapeModule.angle);
                source.radius = new Range { start = shapeModule.radius * (1f - shapeModule.radiusThickness), end = shapeModule.radius };
                //source.speed = GetValueFromMMC(mainModule.startSpeed);
                dstManager.AddComponentData<EmitterConeSource>(emitterEntity, source);
            }
            else if (shapeModule.shapeType == ParticleSystemShapeType.ConeVolume)
            {
                var source = new EmitterConeVolumeSource();
                source.angle = math.radians(shapeModule.angle);
                source.radius = new Range { start = shapeModule.radius * (1f - shapeModule.radiusThickness), end = shapeModule.radius };
                source.length = shapeModule.length;
                source.emitType = 0;
                //source.speed = GetValueFromMMC(mainModule.startSpeed);
                dstManager.AddComponentData<EmitterConeVolumeSource>(emitterEntity, source);
            }
            else if (shapeModule.shapeType == ParticleSystemShapeType.Circle)
            {
                var source = new EmitterCircleSource();
                source.radius = new Range { start = shapeModule.radius * (1f - shapeModule.radiusThickness), end = shapeModule.radius };
                //source.speed = GetRangeFromMMC(mainModule.startSpeed);
                source.speedBasedOnRadius = false;
                dstManager.AddComponentData<EmitterCircleSource>(emitterEntity, source);
            }
            else if (shapeModule.shapeType == ParticleSystemShapeType.Sphere)
            {
                var source = new EmitterSphereSource();
                source.radius = new Range { start = shapeModule.radius * (1f - shapeModule.radiusThickness), end = shapeModule.radius };
                //source.speed = GetRangeFromMMC(mainModule.startSpeed);
                source.speedBasedOnRadius = false;
                dstManager.AddComponentData<EmitterSphereSource>(emitterEntity, source);
            }

            //add initial data
            var initialScale = new EmitterInitialScale();
            initialScale.scale = GetRangeFromMMC(mainModule.startSize);
            dstManager.AddComponentData<EmitterInitialScale>(emitterEntity, initialScale);

            var initialRotation = new EmitterInitialRotation();
            initialRotation.angle = GetRangeFromMMC(mainModule.startRotation);
            dstManager.AddComponentData<EmitterInitialRotation>(emitterEntity, initialRotation);

            var initialVelocity = new EmitterInitialVelocity();
            //initialVelocity.velocity = new float3(0f, 0f, GetValueFromMMC(mainModule.startSpeed));
            initialVelocity.speed = GetRangeFromMMC(mainModule.startSpeed);
            dstManager.AddComponentData<EmitterInitialVelocity>(emitterEntity, initialVelocity);

            //add over lifetime
            if (particleSystem.rotationOverLifetime.enabled)
            {
                //var curveEntity = conversionSystem.CreateAdditionalEntity(this);
                //var keysEntity = conversionSystem.CreateAdditionalEntity(this);

                var lifeTimeModule = particleSystem.rotationOverLifetime;
                var lifeTimeComp = new LifetimeAngularVelocity();
                lifeTimeComp.curve = GetMinMaxCurveFromMMC(lifeTimeModule.z, dstManager, conversionSystem);
                dstManager.AddComponentData<LifetimeAngularVelocity>(emitterEntity, lifeTimeComp);
            }
            if (particleSystem.sizeOverLifetime.enabled)
            {
                //var curveEntity = conversionSystem.CreateAdditionalEntity(this);
                //var keysEntity = conversionSystem.CreateAdditionalEntity(this);

                var lifeTimeModule = particleSystem.sizeOverLifetime;
                var lifeTimeComp = new LifetimeScale();
                lifeTimeComp.curve = GetMinMaxCurveFromMMC(lifeTimeModule.size, dstManager, conversionSystem);
                dstManager.AddComponentData<LifetimeScale>(emitterEntity, lifeTimeComp);
            }
            if (particleSystem.velocityOverLifetime.enabled)
            {
                var lifeTimeModule = particleSystem.velocityOverLifetime;
                if (!(lifeTimeModule.speedModifier.mode == ParticleSystemCurveMode.Constant
                    && lifeTimeModule.speedModifier.constant == 1f))
                {
                    //var curveEntity = conversionSystem.CreateAdditionalEntity(this);
                    //var keysEntity = conversionSystem.CreateAdditionalEntity(this);

                    var lifeTimeComp = new LifetimeSpeedMultiplier();
                    lifeTimeComp.curve = GetMinMaxCurveFromMMC(lifeTimeModule.speedModifier, dstManager, conversionSystem);
                    dstManager.AddComponentData<LifetimeSpeedMultiplier>(emitterEntity, lifeTimeComp);
                }
            }
            if (particleSystem.velocityOverLifetime.enabled)
            {
                //var curveEntity = conversionSystem.CreateAdditionalEntity(this);
                //var keysEntity = conversionSystem.CreateAdditionalEntity(this);

                var lifeTimeModule = particleSystem.velocityOverLifetime;
                var lifeTimeComp = new LifetimeVelocity();
                lifeTimeComp.curveX = GetMinMaxCurveFromMMC(lifeTimeModule.x, dstManager, conversionSystem);
                lifeTimeComp.curveY = GetMinMaxCurveFromMMC(lifeTimeModule.y, dstManager, conversionSystem);
                lifeTimeComp.curveZ = GetMinMaxCurveFromMMC(lifeTimeModule.z, dstManager, conversionSystem);
                //lifeTimeComp.curve = GetCurve3FromMMC(lifeTimeModule.x, lifeTimeModule.y, lifeTimeModule.z, dstManager, curveEntity, keysEntity);
                dstManager.AddComponentData<LifetimeVelocity>(emitterEntity, lifeTimeComp);
            }
            if (particleSystem.limitVelocityOverLifetime.enabled)
            {
                //var curveEntity = conversionSystem.CreateAdditionalEntity(this);
                //var keysEntity = conversionSystem.CreateAdditionalEntity(this);

                var lifeTimeModule = particleSystem.limitVelocityOverLifetime;
                var lifeTimeComp = new LifetimeLimitVelocity();
                //UnityEngine.Debug.Log($"{lifeTimeModule.limit}");
                lifeTimeComp.curve = GetMinMaxCurveFromMMC(lifeTimeModule.limit, dstManager, conversionSystem);
                lifeTimeComp.dampen = lifeTimeModule.dampen;
                dstManager.AddComponentData<LifetimeLimitVelocity>(emitterEntity, lifeTimeComp);
            }
            if (particleSystem.rotationBySpeed.enabled)
            {
                var lifeTimeModule = particleSystem.rotationBySpeed;
                var lifeTimeComp = new RotationBySpeed();

                lifeTimeComp.curve = GetMinMaxCurveFromMMC(lifeTimeModule.z, dstManager, conversionSystem);
                lifeTimeComp.range = new Range() { start = lifeTimeModule.range.x, end = lifeTimeModule.range.y };
                dstManager.AddComponentData<RotationBySpeed>(emitterEntity, lifeTimeComp);
            }
            if (particleSystem.forceOverLifetime.enabled)
            {
                var lifeTimeModule = particleSystem.forceOverLifetime;
                var lifeTimeComp = new LifetimeForce();

                lifeTimeComp.curveX = GetMinMaxCurveFromMMC(lifeTimeModule.x, dstManager, conversionSystem);
                lifeTimeComp.curveY = GetMinMaxCurveFromMMC(lifeTimeModule.y, dstManager, conversionSystem);
                lifeTimeComp.curveZ = GetMinMaxCurveFromMMC(lifeTimeModule.z, dstManager, conversionSystem);
                lifeTimeComp.spaceType = (int)lifeTimeModule.space;
                dstManager.AddComponentData<LifetimeForce>(emitterEntity, lifeTimeComp);
            }
        }

        private MinMaxCurve GetMinMaxCurveFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            MinMaxCurve minMaxCurve = new MinMaxCurve();
            minMaxCurve.mode = (int)mmc.mode;
            if (mmc.mode == ParticleSystemCurveMode.Constant)
            {
                minMaxCurve.constantMax = mmc.constant;
            }
            else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
            {
                minMaxCurve.constantMax = mmc.constantMax;
                minMaxCurve.constantMin = mmc.constantMin;
            }
            else if (mmc.mode == ParticleSystemCurveMode.Curve)
            {
                minMaxCurve.curveMax = GetCurveEntityFromAnimationCurve(mmc.curve, mmc.curveMultiplier, dstManager, conversionSystem);
            }
            else if (mmc.mode == ParticleSystemCurveMode.TwoCurves)
            {
                minMaxCurve.curveMax = GetCurveEntityFromAnimationCurve(mmc.curveMax, mmc.curveMultiplier, dstManager, conversionSystem);
                minMaxCurve.curveMin = GetCurveEntityFromAnimationCurve(mmc.curveMin, mmc.curveMultiplier, dstManager, conversionSystem);
            }

            return minMaxCurve;
        }

        private Entity GetCurveEntityFromAnimationCurve(AnimationCurve curve, float multiplier, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var curveEntity = conversionSystem.CreateAdditionalEntity(this);
            var keysEntity = conversionSystem.CreateAdditionalEntity(this);

            var curveFloat = new BezierCurveFloat { keys = keysEntity };
            dstManager.AddComponentData<BezierCurveFloat>(curveEntity, curveFloat);

            var buffer = dstManager.AddBuffer<BezierKeyFloat>(keysEntity);
            var keyFrames = curve.keys;
            for (int i = 0; i < keyFrames.Length; i++)
            {
                var keyFrame = keyFrames[i];
                var time = keyFrame.time;
                var value = keyFrame.value;
                value *= multiplier;

                var inValue = value - keyFrame.inTangent * 0.15f;
                var outValue = value + keyFrame.outTangent * 0.15f;
                buffer.Add(new BezierKeyFloat(time, value, inValue, outValue));
            }

            return curveEntity;
        }

        private Entity GetCurve3FromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmcX, UnityEngine.ParticleSystem.MinMaxCurve mmcY, UnityEngine.ParticleSystem.MinMaxCurve mmcZ, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var curveEntity = conversionSystem.CreateAdditionalEntity(this);
            var keysEntity = conversionSystem.CreateAdditionalEntity(this);

            if (mmcX.mode == ParticleSystemCurveMode.Constant)
            {
                var curve = new StepCurveFloat3 { keys = keysEntity };
                dstManager.AddComponentData<StepCurveFloat3>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<KeyFloat3>(keysEntity);
                var value = new float3(mmcX.constant, mmcY.constant, mmcZ.constant);
                buffer.Add(new KeyFloat3(0f, value));
                buffer.Add(new KeyFloat3(1f, value));
            }
            else if (mmcX.mode == ParticleSystemCurveMode.TwoConstants)
            {
                var curve = new LinearCurveFloat3 { keys = keysEntity };
                dstManager.AddComponentData<LinearCurveFloat3>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<KeyFloat3>(keysEntity);
                var min = new float3(mmcX.constantMin, mmcY.constantMin, mmcZ.constantMin);
                buffer.Add(new KeyFloat3(0f, min));
                var max = new float3(mmcX.constantMax, mmcY.constantMax, mmcZ.constantMax);
                buffer.Add(new KeyFloat3(1f, max));
            }
            else if (mmcX.mode == ParticleSystemCurveMode.Curve)
            {
                var curve = new BezierCurveFloat3 { keys = keysEntity };
                dstManager.AddComponentData<BezierCurveFloat3>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<BezierKeyFloat3>(keysEntity);
                var keyFrames = mmcX.curve.keys;
                var multiplier = mmcX.curveMultiplier;
                for (int i = 0; i < keyFrames.Length; i++)
                {
                    var keyFrame = keyFrames[i];
                    var time = keyFrame.time;
                    var value = new float3(keyFrame.value, mmcY.curve.Evaluate(time), mmcZ.curve.Evaluate(time)) * multiplier;

                    var inValue = value - keyFrame.inTangent * 0.15f;
                    var outValue = value + keyFrame.outTangent * 0.15f;
                    buffer.Add(new BezierKeyFloat3(time, value, inValue, outValue));
                }
            }
            else if (mmcX.mode == ParticleSystemCurveMode.TwoCurves)
            {
                var curve = new BezierCurveFloat3 { keys = keysEntity };
                dstManager.AddComponentData<BezierCurveFloat3>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<BezierKeyFloat3>(keysEntity);
                var keyFrames = mmcX.curveMax.keys;
                var multiplier = mmcX.curveMultiplier;
                for (int i = 0; i < keyFrames.Length; i++)
                {
                    var keyFrame = keyFrames[i];
                    var time = keyFrame.time;
                    var value = new float3(keyFrame.value, mmcY.curveMax.Evaluate(time), mmcZ.curveMax.Evaluate(time)) * multiplier;

                    var inValue = value - keyFrame.inTangent * 0.15f;
                    var outValue = value + keyFrame.outTangent * 0.15f;
                    buffer.Add(new BezierKeyFloat3(time, value, inValue, outValue));
                }
            }

            return curveEntity;
        }

        private Entity GetCurveFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc, EntityManager dstManager, Entity curveEntity, Entity keysEntity, bool toDegrees = false)
        {
            //var curveEntity = dstManager.CreateEntity();
            //var keysEntity = dstManager.CreateEntity();

            //UnityEngine.Debug.Log("ParticleSystemConversionSystem: curveEntity = " + curveEntity);

            if (mmc.mode == ParticleSystemCurveMode.Constant)
            {
                var curve = new StepCurveFloat { keys = keysEntity };
                dstManager.AddComponentData<StepCurveFloat>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<KeyFloat>(keysEntity);
                var value = toDegrees ? math.degrees(mmc.constant) : mmc.constant;
                buffer.Add(new KeyFloat(0f, value));
                buffer.Add(new KeyFloat(1f, value));
            }
            else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
            {
                var curve = new LinearCurveFloat { keys = keysEntity };
                dstManager.AddComponentData<LinearCurveFloat>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<KeyFloat>(keysEntity);
                var min = toDegrees ? math.degrees(mmc.constantMin) : mmc.constantMin;
                var max = toDegrees ? math.degrees(mmc.constantMax) : mmc.constantMax;
                buffer.Add(new KeyFloat(0f, min));
                buffer.Add(new KeyFloat(1f, max));
            }
            else if (mmc.mode == ParticleSystemCurveMode.Curve)
            {
                var curve = new BezierCurveFloat { keys = keysEntity };
                dstManager.AddComponentData<BezierCurveFloat>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<BezierKeyFloat>(keysEntity);
                var keyFrames = mmc.curve.keys;
                var multiplier = mmc.curveMultiplier;
                for (int i = 0; i < keyFrames.Length; i++)
                {
                    var keyFrame = keyFrames[i];
                    var value = toDegrees ? math.degrees(keyFrame.value) : keyFrame.value;
                    value *= multiplier;

                    var inValue = value - keyFrame.inTangent * 0.15f;
                    var outValue = value + keyFrame.outTangent * 0.15f;
                    buffer.Add(new BezierKeyFloat(keyFrame.time, value, inValue, outValue));
                }
            }
            else if (mmc.mode == ParticleSystemCurveMode.TwoCurves)
            {
                var curve = new BezierCurveFloat { keys = keysEntity };
                dstManager.AddComponentData<BezierCurveFloat>(curveEntity, curve);

                var buffer = dstManager.AddBuffer<BezierKeyFloat>(keysEntity);
                var mmcCurve = mmc.curveMin;
                if (mmcCurve.length < mmc.curveMax.length)
                    mmcCurve = mmc.curveMax;

                var keyFrames = mmcCurve.keys;
                var multiplier = mmc.curveMultiplier;
                for (int i = 0; i < keyFrames.Length; i++)
                {
                    var keyFrame = keyFrames[i];
                    var value = toDegrees ? math.degrees(keyFrame.value) : keyFrame.value;
                    value *= multiplier;

                    var inValue = value - keyFrame.inTangent * 0.15f;
                    var outValue = value + keyFrame.outTangent * 0.15f;
                    buffer.Add(new BezierKeyFloat(keyFrame.time, value, inValue, outValue));
                }
            }

            return curveEntity;
        }

        private float GetValueFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc, bool toDegrees = false)
        {
            float value = 1f;
            if (mmc.mode == ParticleSystemCurveMode.Constant)
            {
                value = toDegrees ? math.degrees(mmc.constant) : mmc.constant;
            }
            else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
            {
                var min = toDegrees ? math.degrees(mmc.constantMin) : mmc.constantMin;
                var max = toDegrees ? math.degrees(mmc.constantMax) : mmc.constantMax;
                value = m_rand.NextFloat(min, max);
            }

            return value;
        }

        private Range GetRangeFromMMC(UnityEngine.ParticleSystem.MinMaxCurve mmc, bool toDegrees = false)
        {
            var minValue = 2f;
            var maxValue = 2f;
            if (mmc.mode == ParticleSystemCurveMode.Constant)
            {
                var value = toDegrees ? math.degrees(mmc.constant) : mmc.constant;
                minValue = value;
                maxValue = value;
            }
            else if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
            {
                var min = toDegrees ? math.degrees(mmc.constantMin) : mmc.constantMin;
                var max = toDegrees ? math.degrees(mmc.constantMax) : mmc.constantMax;
                minValue = min;
                maxValue = max;
            }

            return new Range { start = minValue, end = maxValue };
        }

        //private Range ToDegrees(Range range)
        //{
        //    range.start = math.degrees(range.start);
        //    range.end = math.degrees(range.end);

        //    return range;
        //}
    }

}
