using System.Collections;

using System.Collections;

namespace Labs1VirtualMemory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VirtualMemory memory = new VirtualMemory(1000, "test.bin");
            //try
            //{
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        memory[i] = i;
            //    }

            //    for (int i = 2000; i < 2010; i++)
            //    {
            //        Console.WriteLine(memory[i]);
            //    }

            //    for (int i = 2000; i < 2500; i++)
            //    {
            //        memory[i] = i*2;
            //    }
            //    for (int i = 2000; i < 2010; i++)
            //    {
            //        Console.WriteLine(memory[i]);
            //    }
            //    Console.WriteLine(memory[9999]);
            //    Console.WriteLine(memory[9998]);

            //}
            //catch (ArgumentException ex)
            //{
            //    Console.WriteLine(ex.Message);

            //}

            //try
            //{
            //    for (int i = 0; i < 400; i++)
            //    {
            //        memory[i] = i;
            //    }


            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine(e.Message);
            //}



            try
            {
                Console.WriteLine(memory[10]);
                Console.WriteLine(memory[512]);
                Console.WriteLine(memory[998]);
                memory[10] = 10;
                memory[512] = 512;
                memory[998] = 998;
                Console.WriteLine(memory[10]);
                Console.WriteLine(memory[512]);
                Console.WriteLine(memory[998]);
                memory[10] = 20;
                memory[512] = 1024;
                memory[998] = 2048;
                Console.WriteLine(memory[10]);
                Console.WriteLine(memory[512]);
                Console.WriteLine(memory[998]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            memory.Dispose();
        }
    }
}