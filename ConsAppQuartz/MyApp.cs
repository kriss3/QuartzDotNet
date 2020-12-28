using Quartz;
using Quartz.Impl;
using JobLibrary;
using static System.Console;
using System.Threading;

namespace ConsAppQuartz
{
    class MyApp
    {
        static void Main(string[] args)
        {
            Run();
            ReadLine();
        }

        public static async void Run()
        {
            //Construct scheduler factory;
            ISchedulerFactory schFactory = new StdSchedulerFactory();

            IScheduler scheduler = await schFactory.GetScheduler(new CancellationToken());
            await scheduler.Start();

            //Job
            IJobDetail jobDetail = JobBuilder.Create<SimplyJob>().WithIdentity("job1", "group1").Build();

            //Trigger
            ITrigger jobTrigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
                .Build();

            //schedule
            await scheduler.ScheduleJob(jobDetail, jobTrigger);
        }
    }
}
