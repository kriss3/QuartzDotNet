using System;
using System.Threading.Tasks;
using Quartz;

namespace JobLibrary
{
    public class SimplyJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(()=> 
            {
                Console.WriteLine("Hello, JOb executed");
            }); 
        }
    }
}
