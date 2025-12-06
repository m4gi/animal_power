#if ENABLE_SPLINES
#if ENABLE_COLLECTIONS
#if ENABLE_MATHEMATICS
#if ENABLE_BURST
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace UI_Spline_Renderer
{
    [BurstCompile]
    internal struct SplineExtrudeJob : IJob
    {
        [ReadOnly] public NativeSpline spline;

        [ReadOnly] public NativeCurve widthCurve;
        [ReadOnly] public float width;
        [ReadOnly] public bool keepZeroZ;
        [ReadOnly] public bool keepBillboard;
        [ReadOnly] public int startIdx;
        [ReadOnly] public float2 clipRange;

        [ReadOnly] public float2 uvMultiplier;
        [ReadOnly] public float2 uvOffset;
        [ReadOnly] public UVMode uvMode;
        [ReadOnly] public Color color;
        [ReadOnly] internal NativeColorGradient colorGradient;

        [ReadOnly] public int edgeCount;
        public NativeArray<float3> evaluatedPos;
        public NativeArray<float3> evaluatedTan;
        public NativeArray<float3> evaluatedNor;

        [WriteOnly] public NativeList<UIVertex> vertices;
        [WriteOnly] public NativeList<int3> triangles;


        float v;
        float length;
        
        public void Execute()
        {
            Evaluate();
            ExtrudeSpline();
        }

        void Evaluate()
        {
            length = spline.GetLength();
            for (int i = 0; i < edgeCount; i++)
            {
                var t = (float)i / (edgeCount - 1);
                t = t.Remap(0, 1, clipRange.x, clipRange.y);
                spline.Evaluate(t, out var pos, out var tan, out var nor);
                evaluatedPos[i] = pos;
                evaluatedTan[i] = tan;
                evaluatedNor[i] = nor;
            }
        }

        void ExtrudeSpline()
        {
            if (spline.Count < 2) return;
            

            var prevPosition = float3.zero;
            for (int i = 0; i < edgeCount; i++)
            {
                var t = (float)i / (edgeCount - 1);
                t = t.Remap(0, 1, clipRange.x, clipRange.y);
                var pos = evaluatedPos[i];
                var tan = evaluatedTan[i];
                var nor = evaluatedNor[i];

                // resolve (0,0,0) tangent
                if (tan is { x: 0, y: 0, z: 0 })
                {
                    var prev = i == 0 ? pos : prevPosition;
                    var next = i == edgeCount - 1 ? pos : evaluatedPos[i + 1];
                    tan = next - prev;
                }


                InternalUtility.ExtrudeEdge(
                    GetWidthAt(t), GetVAt(t, i),  GetColorAt(t), ref pos, tan, nor,
                    keepBillboard, keepZeroZ, uvMultiplier, uvOffset, out var v0, out var v1);
                
                vertices.Add(v0);
                vertices.Add(v1);

                prevPosition = pos;


                // 2-1-0, 3-1-2 ...
                if (i > 0)
                {
                    triangles.Add(new int3(
                        2 * i + startIdx,
                        2 * i - 1 + startIdx,
                        2 * i - 2 + startIdx));
                    triangles.Add(new int3(
                        2 * i + 1 + startIdx,
                        2 * i - 1 + startIdx,
                        2 * i + startIdx));
                }
            }
        }


        float GetWidthAt(float t)
        {
            return width * widthCurve.Evaluate(t);
        }

        Color GetColorAt(float t)
        {
            return color * colorGradient.Evaluate(t);
        }
        
        float GetVAt(float t, int i)
        {
            switch (uvMode)
            {
                case UVMode.Tile:
                    return length / width * t;
                case UVMode.RepeatPerSegment:
                    return i;
                case UVMode.Stretch:
                    return t;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
#endif
#endif
#endif
#endif