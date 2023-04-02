using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public struct WorldState : IEquatable<WorldState>
    {
        /// <summary>
        /// Number that when converted into binary string forms a bitmask that stores the state of various conditions in that world state. The position of the bit is determined by the condition, e.g. whether the person is tired etc., and the value of the bit is determined by the value of the condition I.e. true or false in the world state. We use a bitmask shifting on the condition index to flip bits
        /// </summary>
        public long Values;

        /// <summary>
        ///  As the bitmask explicitly states true and false, we cannot use it to indicate the conditions for which the value is not described in the world state I.e. we don’t care about the state of that condition in the new world state. The conditions we do not care about have a 1 stored in this binary string at the index of the condition in Values. Bitmask used to explicitly state false. We need a separate store for negatives because the absence of a value doesnt necessarily mean
        /// it is false.
        /// </summary>
        public long DontCare; //for each of the conditions, we may not actually care if they are true or false in this world state. If we don't care, then we set a 0 at the position of the condition index in this variable

        /// <summary>
        /// ActionPlanner object required to obtain the condition index from the string name of the condition. Required so that we can get the condition index from the string name
        /// </summary>
        internal ActionPlanner Planner;

        /// <summary>
        /// Static method to return a new WorldState object where all values initialised to false (0 -> 0000000... in binary indicating all false) and all DontCare values set to true (-1 -> 111111... in two’s complement binary indicating all true). 
        /// </summary>
        /// <param name="planner"></param>
        /// <returns></returns>
        public static WorldState Create(ActionPlanner planner)
        {
            return new WorldState(planner, 0, -1);
        }


        /// <summary>
        ///  Constructor for new WorldState object, assigning planner, values, and DontCare attributes.
        /// </summary>
        /// <param name="planner"></param>
        /// <param name="values"></param>
        /// <param name="dontcare"></param>
        public WorldState(ActionPlanner planner, long values, long dontcare)
        {
            this.Planner = planner;
            this.Values = values;
            this.DontCare = dontcare;
        }


        /// <summary>
        /// Sets desired condition value and DontCare for the specified condition using bitmask shifting. Value of condition represented by the binary state of the bit at the position of the condition index when the number stored is translated into two’s complement binary
        /// </summary>
        /// <param name="conditionName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string conditionName, bool value)
        {
            return this.Set(this.Planner.FindConditionNameIndex(conditionName), value);
        }

        /// <summary>
        /// Sets desired condition value and DontCare for the specified condition using bitmask shifting. Value of condition represented by the binary state of the bit at the position of the condition index when the number stored is translated into two’s complement binary.
        /// </summary>
        /// <param name="conditionId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal bool Set(int conditionId, bool value)
        {
            this.Values = value ? (this.Values | (1L << conditionId)) : (this.Values & ~(1L << conditionId)); // if the condition which has state stored at position conditionID should be true in this world state: a binary string with a 1 at the position of the condition id and 0 everywhere else (obtained by left shifting 1 condtionID times (1L<<conditionID)) and using or operator to preserve other desired condition values already set for this worldstate (whatever is held at this.Values position conditionID, it will become a 1 because of using the or operator). if the condition which has state stored at conditionID should be false for this worldstate: we first make a state such that all of the condtions except the one we are trying to make false are set to 1 i.e. true (using bitwise complement operator to flip all zero bits in the intial binary string with a 1 at the position of the condition id to make a string with 1 for every other condition except for one that we want to be false. We do this so that when we apply the and operator, we guarantee that the condition we want to be false will be false (as whatever condition previously was in Values, because we are performing and operator with a false, the result will be false. By setting all the other values to 1, we can preserve the values already held in Values variable as result of and will depend solely on the value already stored. 
            this.DontCare ^= (1 << conditionId); //since we care about the state of this condition we must set its DontCare value to 0. As DontCare is initialised to -1 = 111111111... in twos complement binary, the XOR operator will make the value at position conditionid 0 while preserving the other values as all values for which we don't care about will stay 1( 1 XOR 0 =1) and all values we previously said we care about will remain 0 (0 XOR 0 = 0). New value will be set to 0 (1 XOR 1 = 0)
            return true;
        }


        public bool Equals(WorldState other)
        {
            var care = this.DontCare ^ -1L; //-1 in two's complement = 1111111111... When XOR performed with don't care all conditions that we care about (0 in DontCare) will be a 1 in care and all values that we don't care about (1 in DontCare) will become 0 in care. 
            return (this.Values & care) == (other.Values & care); //if the conditions in this world state that we actually care about match those in the other state, return true.
        }


        /// <summary>
        /// for debugging purposes. Provides a human readable string of all the preconditions.
        /// </summary>
        /// <param name="planner">Planner.</param>
        public string Describe(ActionPlanner planner)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < ActionPlanner.MaxConditions; i++)
            {
                if ((this.DontCare & (1L << i)) == 0)
                {
                    var val = planner.ConditionNames[i];
                    if (val == null)
                        continue;

                    var set = (this.Values & (1L << i)) != 0L;

                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(set ? val.ToUpper() : val);
                }
            }
            return sb.ToString();
        }
    }
}
