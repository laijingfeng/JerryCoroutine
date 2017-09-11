using System;
using System.Collections;

namespace Jerry
{
    public class JerryCoroutine : SingletonMono<JerryCoroutine>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }

        public static void StopTask(CorTask task)
        {
            if (task != null)
            {
                task.Stop();
                task = null;
            }
        }

        public class CorTask
        {
            /// <summary>
            /// 是否已经开始并且没有结束
            /// </summary>
            public bool Running
            {
                get
                {
                    return running;
                }
            }

            /// <summary>
            /// 是否暂停中
            /// </summary>
            public bool Paused
            {
                get
                {
                    return paused;
                }
            }

            /// <summary>
            /// 是否是手动停止的
            /// </summary>
            private Action<bool> endCallback = null;

            private IEnumerator coroutine;
            /// <summary>
            /// 运行中，暂停也是运行中
            /// </summary>
            private bool running = false;
            /// <summary>
            /// 是否被暂停
            /// </summary>
            private bool paused = false;
            /// <summary>
            /// 是否是手动停止了
            /// </summary>
            private bool stopped = false;
            /// <summary>
            /// 使用过，不能重复使用
            /// </summary>
            private bool used = false;

            public CorTask(IEnumerator c, bool autoStart = true, Action<bool> finishCallback = null, CorTask task = null)
            {
                JerryCoroutine.StopTask(task);

                endCallback = finishCallback;
                coroutine = c;
                if (autoStart)
                {
                    Start();
                }
            }

            public void Pause()
            {
                paused = true;
            }

            public void Unpause()
            {
                paused = false;
            }

            public void Start()
            {
                if (used)
                {
                    UnityEngine.Debug.LogError("不能重复Start");
                    return;
                }

                used = true;
                running = true;
                JerryCoroutine.Inst.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                stopped = true;
                running = false;
            }

            private IEnumerator CallWrapper()
            {
                yield return null;
                IEnumerator e = coroutine;
                while (running)
                {
                    if (paused)
                    {
                        yield return null;
                    }
                    else
                    {
                        if (e != null && e.MoveNext())
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            running = false;
                        }
                    }
                }

                if (endCallback != null)
                {
                    endCallback(stopped);
                }
            }
        }
    }
}