﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RustProto.Helpers;
using UnityEngine;

namespace Fougerite
{
    public class Loom : MonoBehaviour
    {
        public static int maxThreads = 50;
        private static Loom _current;
        internal static int numThreads;
        internal static bool initialized = false;

        public static Loom Current
        {
            get
            {
                Initialize();
                if (_current == null)
                {
                    var g = new GameObject("Loom");
                    _current = g.AddComponent<Loom>();
                }
                return _current;
            }
        }

        public void Awake()
        {
            //_current = this;
            initialized = true;
        }

        static void Initialize()
        {
            if (!initialized)
            {
                if (!Application.isPlaying)
                {
                    Logger.LogWarning("[Fougerite Loom] Server Is still loading, but a plugin already accessed Loom!");
                    return;
                }
                initialized = true;
                var g = new GameObject("Loom");
                _current = g.AddComponent<Loom>();
            }
        }

        private List<Action> _actions = new List<Action>();
        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            try
            {
                if (time != 0)
                {
                    lock (Current._delayed)
                    {
                        Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                    }
                }
                else
                {
                    lock (Current._actions)
                    {
                        Current._actions.Add(action);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[Fougerite Loom Error] " + ex + " - " + Time.time + " - " + time + " - " + action + " - " + Current._actions + " - " + Current._delayed);
            }
        }

        public static void ExecuteInBiggerStackThread(Action action)
        {
            Thread bigStackThread = new Thread(() => action(), 1024 * 1024);
            bigStackThread.Start();
            bigStackThread.Join();
        }

        public static Thread RunAsync(Action a)
        {
            Initialize();
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }

        }


        public void OnDisable()
        {
            if (_current == this)
            {
                _current = null;
            }
        }



        // Use this for initialization
        public void Start()
        {

        }

        List<Action> _currentActions = new List<Action>();

        // Update is called once per frame
        public void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (var a in _currentActions)
            {
                a();
            }
            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                foreach (var item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (var delayed in _currentDelayed)
            {
                delayed.action();
            }
        }
    }
}