using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker
{
    public class RandomGenerator
    {
        public int Prob { get; protected set; }

        public RandomGenerator(int prob)
        {
            Prob = prob;
        }

        public virtual bool CheckRandom()
        {
            return Random.Range(0, 1f) <= (Prob / 100f);
        }
    }

    public class TrueRandomGenerator : RandomGenerator
    {
        public TrueRandomGenerator(int prob) : base(prob)
        {
            Prob = prob;
        }

        public override bool CheckRandom()
        {
            return Random.Range(0, 1f) <= (Prob / 100f);
        }
    }

    /// <summary>
    /// Dùng để phân phối xác suất theo xác suất giả định Prob. Chống hên xui quá mức
    /// Hiện tại chỉ support giá trị xác suất từ 1 - 40
    /// </summary>
    public class PseudoRandomGenerator : RandomGenerator
    {

        int curN = 0;

        static readonly float[] PRDConstants = new float[] {
            0.000156f,
            0.000620f,
            0.001390f,
            0.002446f,
            0.003800f,
            0.005441f,
            0.007369f,
            0.009549f,
            0.012009f,
            0.014750f,
            0.017745f,
            0.020950f,
            0.024492f,
            0.028240f,
            0.032210f,
            0.036410f,
            0.040900f,
            0.045620f,
            0.050610f,
            0.055700f,
            0.061090f,
            0.066704f,
            0.072500f,
            0.078500f,
            0.084750f,
            0.091110f,
            0.097710f,
            0.104600f,
            0.111800f,
            0.118950f,
            0.126300f,
            0.133880f,
            0.141710f,
            0.149671f,
            0.157889f,
            0.166260f,
            0.174780f,
            0.183500f,
            0.192450f,
            0.200960f
        };

        public override bool CheckRandom()
        {
            curN++;
            float curProb = curN * PRDConstants[Prob - 1];

            if (Random.Range(0, 1f) <= curProb)
            {
                curN = 0;
                return true;
            }

            return false;
        }

        public PseudoRandomGenerator(int prob) : base(prob)
        {
            if (prob < 1)
            {
                Debug.LogError("Out of range PseudoRandomGenerator. Now only support 1% - 40% ");

                prob = 1;
            }
            else if (prob > 40)
            {
                Debug.LogError("Out of range PseudoRandomGenerator. Now only support 1% - 40% ");

                prob = 40;
            }

            Prob = prob;
        }
    }

}
