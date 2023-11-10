using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;
    [TaskCategory("Shootero")]
    public class TreantRandomShiftMaskNongSung : Action
    {
        public int MinBarel;
        public int MaxBarel;
        public bool Loop = false;
        public SharedInt mask;

        public uint MinMask { get { return 0x1u << MinBarel; } }
        public uint MaxMask { get { return 0x1u << MaxBarel; } }

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            if (mask.Value == 0)
            {
                var rIndex = Random.Range(MinBarel, MaxBarel + 1);
                mask.Value = 1 << rIndex;
            }
        }

        uint ShifMaskRandom(uint maskVal)
        {
            uint value = (uint)maskVal;
            var shiftVal = 0;
            if (value == MinMask && Loop == false)
            {
                shiftVal = Random.Range(-1, 1); // -1, 0
            }
            else if (value == MaxMask && Loop == false)
            {
                shiftVal = Random.Range(0, 2); // 0, 1
            }
            else
            {
                shiftVal = Random.Range(-1, 2); // -1, 0, 1
            }

            if (shiftVal > 0)
            {
                var rightVal = (value & 0x1) << MaxBarel;
                value = value >> 1;

                if (Loop)
                {
                    value |= rightVal;
                }
            }
            else if (shiftVal < 0)
            {
                var leftVal = (value & (0x1u << MaxBarel)) >> MaxBarel;
                value = value << 1;

                if (Loop)
                {
                    value |= leftVal;
                }
            }
            return value;
        }

        public override TaskStatus OnUpdate()
        {
            mask.Value = (int)ShifMaskRandom((uint)mask.Value);
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}