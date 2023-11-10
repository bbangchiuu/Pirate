using System.Collections;
using System.Collections.Generic;

public enum BuffType
{
    NONE,
    HEAL,               // 0
    HP_UP,              // 1
    ATK_UP,             // 2
    ATKSPD_UP,          // 3
    CRIT_UP,            // 4

    ATK_UP_SMALL,       // 5
    ATKSPD_UP_SMALL,    // 6
    CRIT_UP_SMALL,      // 7

    ADD_SHOT,           // 8
    MULTI_SHOT,         // 9
    SIDE_SHOT,          // 10
    CHEO_SHOT,          // 11
    BACK_SHOT,          // 12
    RICOCHET,           // 13
    PIERCING,           // 14
    BOUNCING,           // 15
    SMART,              // 16
    FLY,                // 17
    SLOW_PROJECTILE,    // 18

    LINKEN,             // 19
    EVASION,            // 20
    LIFE,               // 21
    ELECTRIC_EFF,       // 22
    FLAME_EFF,          // 23
    FROZEN_EFF,         // 24
    SHIELD,             // 25
    RAGE_ATK,           // 26
    RAGE_ATKSPD,        // 27
    LIFE_LEECH,         // 28
    HEAD_SHOT,          // 29
    ELECTRIC_FIELD,     // 30
    FLAME_FIELD,        // 31
    FROZEN_FIELD,       // 32

    GIANT,              // 33
    DWARF,              // 34
    RUNNING_CHARGE,     // 35
    TRACKING_BULLET,    // 36
    BIG_BULLET,         // 37

    ELEMENT_CRIT,       // 38
    CRITDMG_UP,         // 39

    SHOT_AOE,
    AOE,
    SPLIT,
    BIG_BULLET_OVER_TIME,
    END_OF_BUFF_TYPE
}

public struct BuffStat
{
    public BuffType Type;
    public int MaxCount;
    public int RandomRate;
    public float[] Params;
    public int ChapUnlock;
}