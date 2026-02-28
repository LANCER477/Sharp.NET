using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.AsyncProgramming
{
    internal class ThreadPooling
    {
        public void Run()
        {
            ThreadPool.SetMaxThreads(1, 1);
            Console.WriteLine("ThreadPooling start");
            ThreadPool.QueueUserWorkItem(ThreadAction, 300);   // додавання до пулу потоків
            ThreadPool.QueueUserWorkItem(ThreadAction, 1000);  // автоматично стартує виконання
            ThreadPool.QueueUserWorkItem(ThreadAction, 500);
            Thread.Sleep(500);
            Console.WriteLine($"ThreadPooling finish: {ThreadPool.CompletedWorkItemCount} done, {ThreadPool.PendingWorkItemCount} terminated");
        }

        private void ThreadAction(Object? timeout)
        {
            Console.WriteLine($"ThreadAction {timeout} start");
            Thread.Sleep((int)timeout!);
            Console.WriteLine($"ThreadAction {timeout} finish");
        }
    }
}
/* Thread Pool - пул потоків - середовище виконання потоків з фоновим пріоритетом
 * Особливість - потоки, що не встигли допрацювати до завершення головного потоку (Main), скасовуються
 */
