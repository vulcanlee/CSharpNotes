using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EqualityAndEquals
{
    class Program
    {
        static void Main(string[] args)
        {
            byte b = 100;
            int i = 100;
            object objB = b;
            object objI = i;

            Console.WriteLine($"相等運算子 {Environment.NewLine}");
            Console.WriteLine("實值 Value 型別的相等運算子，直接比較實際的值是否相等");
            Console.WriteLine(b == i);
            Console.WriteLine(i == b);
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"[Primitive 基本類型] 與 [物件] 的相等運算子 {Environment.NewLine}");
            Console.WriteLine("會出現 運算子 '==' 不可套用至類型為 'byte' 和 'object' 的運算元 這類錯誤訊息");
            //Console.WriteLine(b == objI);
            //Console.WriteLine(objI == b);
            //Console.WriteLine(b == objB);
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"執行個體的 Equals() 方法 {Environment.NewLine}");
            Console.WriteLine(b.Equals(i));
            Console.WriteLine(i.Equals(b));
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"object.Equals() 的靜態方法 {Environment.NewLine}");
            Console.WriteLine(object.Equals(b, i));
            Console.WriteLine(object.Equals(i, b));
            Console.WriteLine(object.Equals(b, b));
            Console.WriteLine(object.Equals(i, i));
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"[Primitive 基本類型] 與 [物件] 的執行個體 Equals() 方法 {Environment.NewLine}");
            Console.WriteLine(b.Equals(objI));
            Console.WriteLine(b.Equals(objB));
            Console.WriteLine(i.Equals(objB));
            Console.WriteLine(i.Equals(objI));
            Console.WriteLine(i.Equals(DateTime.Now));
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"[物件] 與 [Primitive 基本類型] 的執行個體 Equals() 方法 {Environment.NewLine}");
            Console.WriteLine(objB.Equals(i));
            Console.WriteLine(objB.Equals(b));
            Console.WriteLine(objI.Equals(b));
            Console.WriteLine(objI.Equals(i));
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"[物件] 與 [Primitive 基本類型] 的 object.Equals() 靜態方法 {Environment.NewLine}");
            Console.WriteLine(object.Equals(objB, i));
            Console.WriteLine(object.Equals(i, objB));

            Console.WriteLine(object.Equals(b, objB));
            Console.WriteLine(object.Equals(objB, b));

            Console.WriteLine(object.Equals(b, objI));
            Console.WriteLine(object.Equals(objI, b));

            Console.WriteLine(object.Equals(i, objI));
            Console.WriteLine(object.Equals(objI, i));
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

            //MyByte(b);
            //MyByte(i);
            //MyInt(b);
            //MyInt(i);

        }

        static void MyByte(int foo)
        {

        }
        static void MyInt(byte foo)
        {

        }
    }
}
