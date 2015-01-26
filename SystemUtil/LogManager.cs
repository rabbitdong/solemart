using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xxx.SystemUtil
{
    /// <summary>LOG系统
    /// </summary>
    public class LogManager {
        private static LogManager instance = new LogManager();
        private Thread write_t = null;
        private Mutex mutex = null;

        private static string base_dir = "";

        private const string LOG_STR = "LOG:";
        private const string ERR_STR = "ERR:";

        /// <summary>LOG的缓存
        /// </summary>
        private Stack<string> log_cache = new Stack<string>();

        private LogManager() {
            if (base_dir == "")
                throw new Exception("need set BaseDir first!");

            mutex = new Mutex();
            write_t = new Thread(new ThreadStart(Thread_Run));
            write_t.IsBackground = true;
            write_t.Start();
        }

        /// <summary>获取或设置LOG的基目录
        /// </summary>
        public static string BaseDir {
            get { return base_dir; }
            set { base_dir = value; }
        }

        /// <summary>获取建议管理类实例
        /// </summary>
        public static LogManager Instance {
            get {
                return instance;
            }
        }

        /// <summary>记录LOG信息
        /// </summary>
        /// <param name="message">要记录的LOG信息</param>
        public void Log(string message) {
            log_cache.Push(string.Format("{0} {1} {2}", DateTime.Now.ToUniversalTime(), LOG_STR, message));

            if (log_cache.Count > 1000)
                mutex.ReleaseMutex();
        }

        /// <summary>运行的写线程
        /// </summary>
        /// <param name="o"></param>
        private void Thread_Run() {
            while (true) {
                mutex.WaitOne(); //等待队列慢

                string filename = string.Format("\\LOG\\LOG{0}_{1}_{2}_{3}.config", DateTime.Today.Year, DateTime.Today.Month,
                    DateTime.Today.Day, DateTime.Today.Hour);
                StreamWriter sw = new StreamWriter(BaseDir + filename, true, Encoding.UTF8);
                lock (log_cache) {
                    while (log_cache.Count > 0) {
                        sw.WriteLine(log_cache.Pop());
                    }
                }
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>记录错误信息
        /// </summary>
        /// <param name="message">要记录的错误信息</param>
        public void Error(string message) {
            log_cache.Push(string.Format("{0} {1} {2}",
                DateTime.Now.ToLongTimeString(), ERR_STR, message));

            if (log_cache.Count > 1000)
                mutex.ReleaseMutex();
        }

        /// <summary>主动进行LOG转储
        /// </summary>
        public void FlushLog() {
            mutex.ReleaseMutex();
        }
    }
}
