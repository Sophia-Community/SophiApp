using SophiApp.Commons;
using SophiApp.Interfaces;
using System;

namespace SophiApp.Conditions
{
    internal class OsBitness : ICondition
    {
        public string Tag { get; set; } = Tags.ConditionOSBitness;
        public bool Result { get; set; }

        public bool Invoke() => Result = Environment.Is64BitOperatingSystem;
    }
}