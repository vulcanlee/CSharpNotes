using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventDelegateThread
{
    /// <summary>
    /// 具有委派與事件的類別，當有綁定事件訂閱者，於 Dowrk 執行過程中，將會執行該訂閱事件委派方法
    /// </summary>
    public class Worker
    {
        /// <summary>
        /// 宣告一個委派類別
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void MyHandler(object sender, EventArgs args);
        /// <summary>
        /// 宣告一個委派物件
        /// </summary>
        public MyHandler MyDelegateHandler;
        /// <summary>
        /// 宣告一個事件，但是型別是一個委派
        /// </summary>
        public event MyHandler MyDelegateEventHandler;
        /// <summary>
        /// 宣告一個事件，使用.NET標準事件類別 EventHandler
        /// </summary>
        public event EventHandler MyEventHandler;

        /// <summary>
        /// 開始進行工作
        /// </summary>
        public void Dowork()
        {
            #region 連續執行五次
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Dowork {i}");
                OnWorkByEvent(i);  // 發出事件通知
                OnWorkByDelegate(i);  // 發出事件通知
                OnWorkByDelegateEvent(i);  // 發出事件通知
            }
            #endregion
        }

        private void OnWorkByEvent(int i)
        {
            var fooHandler = MyEventHandler;
            if (fooHandler != null)
            {
                Console.WriteLine($"OnWorkByEvent raise {i}");

                // 若解除底下方法，並且註解 ThreadPool 表示式，則會使用同步方式來發出事件通知
                //fooHandler(this, EventArgs.Empty);

                // 這裡採用非同步(多執行緒方式來發出事件通知)
                ThreadPool.QueueUserWorkItem(s =>
                {
                    fooHandler(this, EventArgs.Empty);
                });
            }
        }

        private void OnWorkByDelegate(int i)
        {
            var fooHandler = MyDelegateHandler;
            if (fooHandler != null)
            {
                Console.WriteLine($"OnWorkByDelegate raise {i}");

                // 若解除底下方法，並且註解 ThreadPool 表示式，則會使用同步方式來發出事件通知
                //fooHandler(this, EventArgs.Empty);

                // 這裡採用非同步(多執行續方式來發出事件通知)
                ThreadPool.QueueUserWorkItem(s =>
                {
                    fooHandler(this, EventArgs.Empty);
                });
            }
        }

        private void OnWorkByDelegateEvent(int i)
        {
            var fooHandler = MyDelegateEventHandler;
            if (fooHandler != null)
            {
                Console.WriteLine($"OnWorkByDelegateEvent raise {i}");

                // 若解除底下方法，並且註解 ThreadPool 表示式，則會使用同步方式來發出事件通知
                //fooHandler(this, EventArgs.Empty);

                // 這裡採用非同步(多執行續方式來發出事件通知)
                ThreadPool.QueueUserWorkItem(s =>
                {
                    fooHandler(this, EventArgs.Empty);
                });
            }
        }

    }
}
