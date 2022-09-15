using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Game1.GOAP
{

    public struct GOAPPlanWork<T, W>
    {
        public readonly GOAPAgent<T, W> agent;
        public readonly GOAPGoal<T, W> blacklistGoal;
        public readonly Queue<GOAPActionState<T, W>> actions;
        public Action<GOAPGoal<T, W>> callback;

        public GOAPGoal<T, W> newGoal;


        public GOAPPlanWork(GOAPAgent<T, W> argAgent, GOAPGoal<T, W> argBlacklistGoal, Queue<GOAPActionState<T, W>> argActions, Action<GOAPGoal<T, W>> argCallback) : this()
        {
            agent = argAgent;
            blacklistGoal = argBlacklistGoal;
            actions = argActions;
            callback = argCallback;

        }
    }


        public class GOAPPlannerManager<T, W>
        {
            public static GOAPPlannerManager<T, W> instance;

            public bool multithread;

            public int threadCount = 1;

            private GOAPPlannerThread<T, W>[] planners;

            private List<GOAPPlanWork<T, W>> doneWorks = new List<GOAPPlanWork<T, W>>();
            private Thread[] threads;

            public GOAPPlannerSettings plannerSettings;

            public int nodeWarmupCount = 1000;
            public int statesWarmupCount = 10000;
            protected virtual void Init()
            {

                GOAPNode<T, W>.CacheStackWarmup(nodeWarmupCount);
                GOAPNode<T, W>.CacheStackWarmup(statesWarmupCount);

                instance = this;
                GOAPPlannerThread<T, W>.worksQueue = new ConcurrentQueue<GOAPPlanWork<T, W>>();
                planners = new GOAPPlannerThread<T, W>[threadCount];
                threads = new Thread[threadCount];

                if (multithread)
                {
                    for (int i = 0; i < threadCount; i++)
                    {
                        planners[i] = new GOAPPlannerThread<T, W>(plannerSettings, OnDonePlan);
                        var thread = new Thread(planners[i].MainLoop);
                        thread.Start();
                        threads[i] = thread;

                    }
                }
                else
                {
                    planners[0] = new GOAPPlannerThread<T, W>(plannerSettings, OnDonePlan);
                }




            }

            public virtual void Update()
            {
                if (doneWorks.Count>0)
                {
                    lock (doneWorks)
                    {
                        foreach (var work in doneWorks)
                        {
                            work.callback(work.newGoal);

                        }

                        doneWorks.Clear();

                    }

                }

                if (!multithread)
                {
                    planners[0].CheckWorkers();

                }


            }

            void OnDestroy()
            {
                foreach (var planner in planners)
                {
                    if (planner != null)
                    {
                        planner.Stop();
                    }

                }

                foreach (var thread in threads)
                {
                    if (thread != null)
                    {
                        thread.Abort();
                    }
                }



            }

            private void OnDonePlan(GOAPPlannerThread<T, W> plannerThread, GOAPPlanWork<T, W> work, GOAPGoal<T, W> goal)
            {
                work.newGoal = goal;

                lock (doneWorks)
                {
                    doneWorks.Add(work);
                }



            }
             
            public GOAPPlanWork<T, W> Plan(GOAPAgent<T, W> agent, GOAPGoal<T, W> blacklistGoal, Queue<GOAPActionState<T, W>> currentPlan, Action<GOAPGoal<T,W>> callback)
            {
                var work = new GOAPPlanWork<T, W>(agent, blacklistGoal, currentPlan, callback);

                lock (GOAPPlannerThread<T,W>.worksQueue)
                {
                    GOAPPlannerThread<T, W>.worksQueue.Enqueue(work);
                }

                return work;
            }




        }



        public class GOAPPlannerThread<T, W>
        {
            private readonly GOAPPlanner<T, W> planner;
            public static ConcurrentQueue<GOAPPlanWork<T, W>> worksQueue;

            private bool isRunning = true;

            private readonly Action<GOAPPlannerThread<T, W>, GOAPPlanWork<T, W>, GOAPGoal<T, W>> onDonePlan;


            public GOAPPlannerThread(GOAPPlannerSettings settings, Action<GOAPPlannerThread<T, W>, GOAPPlanWork<T, W>, GOAPGoal<T, W>> argOnDonePlan)
            {
                planner = new GOAPPlanner<T, W>(settings);
                onDonePlan = argOnDonePlan;

            }

            public void Stop() { isRunning = false; }


            public void MainLoop()
            {
                while (isRunning)
                {
                    CheckWorkers();

                }
            }


            public void CheckWorkers()
            {
                if (worksQueue.TryDequeue(out GOAPPlanWork<T, W> checkWork))
                {
                    var work = checkWork;
                    planner.Plan(work.agent, work.blacklistGoal, work.actions, (newGoal) => onDonePlan(this, work, newGoal));
                }
            }








        }




    }




