using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPState<T, W>
    {
        private ConcurrentDictionary<T, W> values;
        private readonly ConcurrentDictionary<T, W> bufferA;
        private ConcurrentDictionary<T, W> bufferB;

        public static int defaultSize = 20;

        private static Stack<GOAPState<T, W>> cachedStates;

        public int Count { get { return values.Count; } }



        private GOAPState()
        {
            int concurrencyLevel = 5;
            bufferA = new ConcurrentDictionary<T, W>(concurrencyLevel, defaultSize);
            bufferB = new ConcurrentDictionary<T, W>(concurrencyLevel, defaultSize);
            values = bufferA;


        }


        private void Init(GOAPState<T, W> oldState)
        {
            values.Clear();
            if (oldState != null)
            {
                lock (oldState.values)
                {
                    foreach (var kvp in oldState.values)
                    {
                        values[kvp.Key] = kvp.Value;
                    }
                }
            }
        }


        public static GOAPState<T, W> Instantiate(GOAPState<T, W> oldState = null)
        {
            GOAPState<T, W> state;

            if (cachedStates == null)
            {
                cachedStates = new Stack<GOAPState<T, W>>();

            }


            lock (cachedStates)
            {
                state = cachedStates.Count > 0 ? cachedStates.Pop() : new GOAPState<T, W>();


            }


            state.Init(oldState);
            return state;


        }




        public static GOAPState<T, W> operator +(GOAPState<T, W> a, GOAPState<T, W> b)
        {
            GOAPState<T, W> result;
            lock (a.values) { result = Instantiate(a); }
            lock (b.values)
            {
                foreach (var kvp in b.values)
                {
                    result.values[kvp.Key] = kvp.Value;

                }

                return result;
            }

        }


        public void AddFromState(GOAPState<T, W> b)
        {
            lock (values) lock (b.values)
                {
                    foreach (var kvp in b.values)
                    {
                        values[kvp.Key] = kvp.Value;
                    }


                }
        }

        public bool HasAny(GOAPState<T, W> other)
        {
            lock (values) lock (other.values)
                {
                    foreach (var kvp in other.values)
                    {
                        W value;
                        values.TryGetValue(kvp.Key, out value);
                        if (Equals(value, kvp.Value))
                        {
                            return true;
                        }

                    }

                    return false;
                }
        }


        public bool HasAnyConflict(GOAPState<T, W> changes, GOAPState<T, W> otherState)
        {
            lock (values) lock (otherState.values)
                {
                    foreach (var kvp in otherState.values)
                    {
                        var otherValue = kvp.Value;

                        W thisValue;
                        if (!values.TryGetValue(kvp.Key, out thisValue)) { continue; }

                        W effectValue;
                        changes.values.TryGetValue(kvp.Key, out effectValue);

                        if (!Equals(otherValue, thisValue) && !Equals(effectValue, thisValue))
                        {
                            return true;
                        }

                    }

                    return false;

                }
        }



        public bool HasAnyConflict(GOAPState<T, W> otherState)
        {
            lock (values) lock (otherState.values)
                {
                    foreach (var kvp in otherState.values)
                    {

                        var otherValue = kvp.Value;



                        W thisValue;

                        if (!values.TryGetValue(kvp.Key, out thisValue)) { continue; }
                        if (!Equals(otherValue, thisValue)) { return true; }


                    }

                    return false;

                }
        }





        public int MissingDifference(GOAPState<T, W> otherState, int stopAt = int.MaxValue)
        {


            lock (values)
            {
                int count = 0;
                foreach (var kvp in values)
                {
                    W otherValue;
                    otherState.values.TryGetValue(kvp.Key, out otherValue);
                    if (!Equals(kvp.Value, otherValue))
                    {
                        count++;
                        if (count >= stopAt)
                        {
                            break;
                        }

                    }

                }

                return count;

            }


        }

        public int MissingDifference(GOAPState<T, W> otherState, ref GOAPState<T, W> difference, int stopAt = int.MaxValue, Func<KeyValuePair<T, W>, W, bool> predicate = null, bool test = false)
        {
            lock (values)
            {
                int count = 0;
                foreach (var kvp in values)
                {
                    W otherValue;
                    otherState.values.TryGetValue(kvp.Key, out otherValue);
                    if (!Equals(otherValue, kvp.Value) && (predicate == null || predicate(kvp, otherValue)))
                    {
                        count++;

                        if (difference != null)
                        {
                            difference.values[kvp.Key] = kvp.Value;
                        }

                        if (count >= stopAt) { break; }


                    }

                }

                return count;
            }
        }



        public int ReplaceWithMissingDifferences(GOAPState<T, W> otherState, int stopAt = int.MaxValue, Func<KeyValuePair<T, W>, W, bool> predicate = null, bool test = false)
        {
            lock (values)
            {

                var count = 0;
                var buffer = values;

                values = values == bufferA ? bufferB : bufferA;
                values.Clear();

                foreach (var kvp in buffer)
                {
                    W otherValue;
                    otherState.values.TryGetValue(kvp.Key, out otherValue);

                    if (!Equals(kvp.Value, otherValue) && (predicate == null || predicate(kvp, otherValue)))
                    {
                        count++;
                        values[kvp.Key] = kvp.Value;
                        if (count >= stopAt) { break; }


                    }


                }

                return count;


            }

        }


        public GOAPState<T, W> Clone()
        {
            return Instantiate(this);
        }


        public static void WarmupCacheStack(int count)
        {
            cachedStates = new Stack<GOAPState<T, W>>(count);

            for (int i = 0; i < count; i++)
            {
                cachedStates.Push(new GOAPState<T, W>());

            }

        }


        public void Recycle()
        {
            lock (cachedStates)
            {
                cachedStates.Push(this);
            }
        }





        public W Get(T key)
        {
            lock (values)
            {
                if (!values.ContainsKey(key)) { return default(W); }
                return values[key];

            }
        }

        public void Set(T key, W value)
        {
            lock (values)
            {
                values[key] = value;
            }
        }

        public void Remove(T key)
        {
            values.TryRemove(key, out _);
        }

        public ConcurrentDictionary<T, W> GetValues()
        {
            lock (values) { return values; }
        }

        public bool TryGetValue(T key, out W value)
        {
            return values.TryGetValue(key, out value);
        }

        public bool HasKey(T key)
        {
            lock (values)
            {
                return values.ContainsKey(key);
            }
        }


        public void Clear()
        {
            lock (values)
            {
                values.Clear();
            }
        }

    }
}
