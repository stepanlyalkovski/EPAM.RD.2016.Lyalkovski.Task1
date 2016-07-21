using System;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = new ManualResetEventSlim(false);
            var pingEvent = new AutoResetEvent(false);
            var pongEvent = new AutoResetEvent(false);

            CancellationTokenSource cts = new CancellationTokenSource(); // TODO: Create a new cancellation token source.
            CancellationToken token= cts.Token; // TODO: Assign an appropriate value to token variable.

            Action ping = () =>
            {
                Console.WriteLine("ping: Waiting for start.");
                start.Wait(token);

                bool continueRunning = true;

                while (continueRunning)
                {
                    pingEvent.WaitOne();
                    Console.WriteLine("ping!");
                    Thread.Sleep(1000);
                    // TODO: write ping-pong functionality here using pingEvent and pongEvent here.
                    pongEvent.Set();
                    
                    continueRunning = !token.IsCancellationRequested; // TODO: Use cancellation token "token" internals here to set appropriate value.
                }

                // TODO: Fix issue with blocked pong task.
                pongEvent.Set();
                Console.WriteLine("ping: done");
            };

            Action pong = () =>
            {
                Console.WriteLine("pong: Waiting for start.");
                start.Wait(token);

                bool continueRunning = true;

                while (continueRunning)
                {
                    pingEvent.Set();
                    
                    pongEvent.WaitOne();
                    Console.WriteLine("pong!");
                    Thread.Sleep(1000);
                    // TODO: write ping-pong functionality here using pingEvent or pongEvent here.



                    // TODO: write ping-pong functionality here using pingEvent or pongEvent here.

                    continueRunning = !token.IsCancellationRequested; // TODO: Use cancellation token "token" internals here to set appropriate value.
                }

                // TODO: Fix issue with blocked ping task.
                pingEvent.Set();
                Console.WriteLine("pong: done");
            };


            var pingTask = Task.Run(ping, token);
            var pongTask = Task.Run(pong, token);

            Console.WriteLine("Press any key to start.");
            Console.WriteLine("After ping-pong game started, press any key to exit.");
            Console.ReadKey();
            start.Set();

            Console.ReadLine();
            // TODO: cancel both tasks using cancellation token.
            cts.Cancel();
            start.Reset();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
