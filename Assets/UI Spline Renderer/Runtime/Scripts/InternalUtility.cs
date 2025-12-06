using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace UI_Spline_Renderer
{
    [BurstCompile]
    internal static class InternalUtility
    {
        // source of this methods
        // https://forum.unity.com/threads/moving-just-recttransform-pivot-in-world-space.1380249/
        // by halley
        internal static Vector3 GetPivotInWorldSpace(this RectTransform source)
        {
            var pivot = new Vector2(
                source.rect.xMin + source.pivot.x * source.rect.width,
                source.rect.yMin + source.pivot.y * source.rect.height);
            
            return source.TransformPoint(new Vector3(pivot.x, pivot.y, 0f));
        }
        
        // source of this methods
        // https://forum.unity.com/threads/moving-just-recttransform-pivot-in-world-space.1380249/
        // by halley
        internal static void SetPivotWithoutRect(this RectTransform source, Vector3 pivot)
        {
            var rect = source.rect;
            if(float.IsNaN(rect.x) || float.IsNaN(rect.y) || float.IsNaN(rect.size.x) || float.IsNaN(rect.size.y)) return;
            pivot = source.InverseTransformPoint(pivot);
            var pivot2 = new Vector2(
                (pivot.x - rect.xMin) / rect.width,
                (pivot.y - rect.yMin) / rect.height);
 
            var offset = pivot2 - source.pivot;
            offset.Scale(source.rect.size);
            var worldPos = source.position + source.TransformVector(offset);
            
            source.pivot = pivot2;
            source.position = worldPos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float Remap(this float value, float beforeRangeMin, float beforeRangeMax, float targetRangeMin, float targetRangeMax)
        {
            var denominator = beforeRangeMax - beforeRangeMin;
            if (denominator == 0) return targetRangeMin;
        
            var ratio = (value - beforeRangeMin) / denominator;
            var result = (targetRangeMax - targetRangeMin) * ratio + targetRangeMin;
            return result;
        }
        
        internal static float Remap(this int value, float beforeRangeMin, float beforeRangeMax, float targetRangeMin, float targetRangeMax)
        {
            return Remap((float)value, beforeRangeMin, beforeRangeMax, targetRangeMin, targetRangeMax);
        }

        internal static float sqr(this float value) => value * value;
        internal static float LerpPoint(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        internal static StartEndImagePreset GetCurrentStartImagePreset(Sprite target)
        {
            if (target == null)              return StartEndImagePreset.None;
            if (target == UISplineRendererSettings.Instance.triangleHead)     return StartEndImagePreset.Triangle;
            if (target == UISplineRendererSettings.Instance.arrowHead)        return StartEndImagePreset.Arrow;
            if (target == UISplineRendererSettings.Instance.emptyCircleHead)  return StartEndImagePreset.EmptyCircle;
            if (target == UISplineRendererSettings.Instance.filledCircleHead) return StartEndImagePreset.FilledCircle;

            return StartEndImagePreset.Custom;
        }

        internal static int CalcVertexCount(this Spline spline, float segmentLength, Vector2 renderRange)
        {
            var length = spline.GetLength() * (renderRange.y - renderRange.x);
            return Mathf.Max(Mathf.CeilToInt(length * segmentLength), 1) * 2 + 4;    
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CalcVertexCount(this NativeSpline spline, float segmentLength, float2 renderRange)
        {
            var length = spline.GetLength() * (renderRange.y - renderRange.x);
            return math.max((int)math.ceil(length * segmentLength), 1) * 2 + 4;
        }


        internal static NativeColorGradient ToNative (this Gradient gradient, Allocator allocator = Allocator.TempJob)
        {
            var aKeys = new NativeArray<float2>(gradient.alphaKeys.Length, allocator);
            for (int i = 0; i < gradient.alphaKeys.Length; i++)
            {
                var key = gradient.alphaKeys[i];
                aKeys[i] = new float2(key.alpha, key.time);
            }
            
            var cKeys = new NativeArray<float4>(gradient.colorKeys.Length, allocator);
            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                var key = gradient.colorKeys[i];
                cKeys[i] = new float4(key.color.r, key.color.g, key.color.b, key.time);
            }


            var native = new NativeColorGradient()
            {
                alphaKeyFrames = aKeys,
                colorKeyFrames = cKeys
            };

            return native;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        internal static void ExtrudeEdge(
            float w, float V, in Color clr, ref float3 pos, in float3 tan, in float3 up, 
            bool keepBillboard, bool keepZeroZ, in float2 uvMultiplier, in float2 uvOffset,
            out UIVertex v0, out UIVertex v1)
        {
            var perpendicular =
                keepBillboard ? math.normalizesafe(math.cross(tan, new float3(0, 0, -1))) : math.normalizesafe(math.cross(tan, up));

            if (keepZeroZ)
            {
                pos.z = 0;
            }

            var uv = new float2(0, V) * uvMultiplier - uvOffset;
            var vert = new UIVertex
            {
                position = pos + perpendicular * w * 0.5f,
                uv0 = new Vector4(uv.x, uv.y),
                color = clr
            };

            v0 = vert;

            uv = new float2(1, V) * uvMultiplier - uvOffset;
            vert = new UIVertex
            {
                position = pos - perpendicular * w * 0.5f,
                uv0 = new Vector4(uv.x, uv.y),
                color = clr
            };

            v1 = vert;
        }
    }
}