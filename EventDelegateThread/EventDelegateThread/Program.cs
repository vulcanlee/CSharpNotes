using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventDelegateThread
{
    class Program
    {
        public static Worker fooWorker = new Worker();
        public static Random Rm = new Random(DateTime.Now.Millisecond);

        static void Main(string[] args)
        {
            Console.WriteLine($"Main Thread Managed ID  {Thread.CurrentThread.ManagedThreadId}");

            // 訂閱有興趣的事件
            fooWorker.MyDelegateHandler += MyDelegateListener;
            fooWorker.MyEventHandler += MyDelegateListener;
            fooWorker.MyDelegateEventHandler += MyDelegateListener;

            // 開始進行工作，並且監聽事件是否有發生，並且執行這個監聽事件
            fooWorker.Dowork();

            Console.WriteLine("Please Wait...");
            Console.ReadKey();
        }

        private static void MyDelegateListener(object sender, EventArgs args)
        {
            var foo = Rm.Next(500, 2000);
            Console.WriteLine($"Sleep {foo}");
            // 休息一下
            Thread.Sleep(foo);

            // 檢查這個監聽事件方法，是否在主執行緒上
            Console.WriteLine($"Thread ID is : {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
