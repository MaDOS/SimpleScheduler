using System.Collections.Generic;

namespace SimpleScheduler
{
  internal class SynchronizedJobList : SynchronizedCollection<Job>
  {
    public delegate void JobAddedEventHandler(JobAddedEventArgs e);

    public event JobAddedEventHandler JobAdded;

    new public void Add(Job item)
    {
      base.Add(item);

      this.JobAdded(new JobAddedEventArgs(item));
    }

    public class JobAddedEventArgs
    {
      public JobAddedEventArgs(Job addedJob)
      {
        this.AddedJob = addedJob;
      }

      public Job AddedJob { get; set; }
    }
  }
}