using System;
using System.Threading;

namespace Krankenhaus
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.SetWindowSize(180, 50);
                UserInterface userInterface = new UserInterface();
                userInterface.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
