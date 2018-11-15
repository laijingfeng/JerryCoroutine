//Version: 2018-11-15-00

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jerry
{
    /// <summary>
    /// <para>协程管理</para>
    /// <para>非Mono可以运行协程</para>
    /// <para>可以不用字符串启动停止</para>
    /// <para>注意：调起的协程不是在脚本内部执行，隐藏脚本或者删除脚本，正在运行的协程是不会停止的，需要主动去停止</para>
    /// <para>接口1：创建CoroutineManager.CorTask</para>
    /// <para>接口2：停止CoroutineManager.StopTask(task)</para>
    /// </summary>
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

        /// <summary>
        /// 执行多个协程
        /// </summary>
        /// <param name="tasks">协程任务</param>
        /// <param name="parallel">是否并行</param>
        /// <param name="finish">完成回调</param>
        /// <param name="pro">进度(任务数量完成度)</param>
        public void DoIEnumerators(List<IEnumerator> tasks, bool parallel = true, Action finish = null, Action<float> pro = null)
        {
            JerryCoroutine.Instance.StartCoroutine(DoIEnumerators_IE(tasks, parallel, finish, pro));
        }

        /// <summary>
        /// 执行多个协程
        /// </summary>
        /// <param name="tasks">协程任务</param>
        /// <param name="parallel">是否并行</param>
        /// <param name="finish">完成回调</param>
        /// <param name="pro">进度(任务数量完成度)</param>
        /// <returns></returns>
        public IEnumerator DoIEnumerators_IE(List<IEnumerator> tasks, bool parallel = true, Action finish = null, Action<float> pro = null)
        {
            if (tasks != null && tasks.Count > 0)
            {
                if (parallel)
                {
                    List<Coroutine> tasks2 = new List<Coroutine>();

                    for (int i = 0, imax = tasks.Count; i < imax; i++)
                    {
                        if (tasks[i] != null)
                        {
                            tasks2.Add(JerryCoroutine.Instance.StartCoroutine(tasks[i]));
                        }
                    }

                    for (int i = 0, imax = tasks2.Count; i < imax; i++)
                    {
                        yield return tasks2[i];
                        if (pro != null)
                        {
                            pro((i + 1) * 1.0f / imax);
                        }
                    }
                }
                else
                {
                    for (int i = 0, imax = tasks.Count; i < imax; i++)
                    {
                        yield return tasks[i];
                        if (pro != null)
                        {
                            pro((i + 1) * 1.0f / imax);
                        }
                    }
                }

                if (pro != null)
                {
                    pro(1.0f);
                }

                if (finish != null)
                {
                    finish();
                }
            }
        }

        /// <summary>
        /// 协程任务
        /// </summary>
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

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="c"></param>
            /// <param name="autoStart"></param>
            /// <param name="finishCallback"></param>
            /// <param name="task"></param>
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

            /// <summary>
            /// 暂停
            /// </summary>
            public void Pause()
            {
                paused = true;
            }

            /// <summary>
            /// 取消暂停
            /// </summary>
            public void Unpause()
            {
                paused = false;
            }

            /// <summary>
            /// 开始
            /// </summary>
            public void Start()
            {
                if (used)
                {
                    UnityEngine.Debug.LogError("不能重复Start");
                    return;
                }

                used = true;
                running = true;
                JerryCoroutine.Instance.StartCoroutine(CallWrapper());
            }

            /// <summary>
            /// 停止
            /// </summary>
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