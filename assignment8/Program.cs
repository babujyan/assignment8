using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace assignment8
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        static void ParallelFor(int fromInclusve, int toExclusive, Action<int> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }


            Task[] tasks = new Task[toExclusive - fromInclusve];
            for (int i = 0; i < toExclusive - fromInclusve; i++)
            {
                int j = i;
                tasks[j] = new Task(() => body(j));
            }
               
            foreach(var t in tasks)
            {

                t.Start();
                
            }


            Task.WaitAll(tasks);
        }

        static void ParallelForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source");
            }

            if (body == null)
            {
                throw new ArgumentNullException("Action");
            }

            List<Task> tasks = new List<Task>();
            foreach (var i in source)
            {
                 tasks.Add(Task.Run(() => body(i)));                
            }

            Task.WaitAll(tasks.ToArray());
        }

        static void ParallelForEachWithOptions<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source");
            }

            if (parallelOptions == null)
            {
                throw new ArgumentNullException("Options");
            }

            if (parallelOptions == null)
            {
                throw new ArgumentNullException("Action");
            }
            int maxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism;
            switch(maxDegreeOfParallelism)
            {
                case (-1):
                    ParallelForEach<TSource>(source, body);
                    break;                   

                default:
                    List<Task> tasks = new List<Task>();
                    foreach (var item in source)
                    {
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();

                        if (tasks.Count < maxDegreeOfParallelism)
                        {
                            tasks.Add(Task.Factory.StartNew(() => body(item)));
                        }
                        else
                        {
                            tasks.Remove(tasks[Task.WaitAny(tasks.ToArray())]);

                            tasks.Add(Task.Factory.StartNew(() => body(item)));
                        }
                    }

                    Task.WaitAll(tasks.ToArray());
                    break;
            }
        }
    }
}
