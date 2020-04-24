using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleScheduler
{
  public class Scheduler
  {
    private readonly ConcurrentJobQueue m_DueJobs = new ConcurrentJobQueue();
    private readonly Semaphore m_Run_Loop_SyncSemaphore = new Semaphore(1, 1);
    private readonly Semaphore m_Loop_Add_SyncSemaphore = new Semaphore(1, 1);

    private static Scheduler ms_Instance;

    private readonly bool m_Shutdown;

    internal SynchronizedJobList Jobs { get; private protected set; } = new SynchronizedJobList();

    public static Scheduler Instance
    {
      get
      {
        return ms_Instance ?? new Scheduler();
      }
      protected set
      {
        ms_Instance = value;
      }
    }

    protected Scheduler()
    {
      this.m_DueJobs.Enqued += this.DueJobs_Enqued;
      this.m_DueJobs.Dequed += this.DueJobs_Dequed;
      this.Jobs.JobAdded += this.Jobs_JobAdded;

      this.Start();
    }

    private void Jobs_JobAdded(SynchronizedJobList.JobAddedEventArgs e)
    {
#if DEBUG
      Console.WriteLine($"[{DateTime.Now.Minute}.{DateTime.Now.Second}.{DateTime.Now.Millisecond}] - Job added to sheduling");
#endif
    }

    private void DueJobs_Dequed(DequedEventArgs e)
    {
#if DEBUG
      Console.WriteLine($"[{DateTime.Now.Minute}.{DateTime.Now.Second}.{DateTime.Now.Millisecond}] - Dispatched Job");
#endif
    }

    public void AddJob(Job job)
    {
      this.m_Loop_Add_SyncSemaphore.WaitOne();

      this.Jobs.Add(job);

      this.m_Loop_Add_SyncSemaphore.Release();
    }

    private void DueJobs_Enqued()
    {
      Console.WriteLine($"[{DateTime.Now.Minute}.{DateTime.Now.Second}.{DateTime.Now.Millisecond}] - Enqued due Job");

      Task.Factory.StartNew(this.RunJobs);
    }

    private void Start()
    {
      Task.Factory.StartNew(this.Loop);
    }

    private void RunJobs()
    {
      this.m_Run_Loop_SyncSemaphore.WaitOne();

      for (int i = 0; i < this.m_DueJobs.Count; i++)
      {

        if (this.m_DueJobs.TryDequeue(out Job job))
        {
          job.LastExecution = DateTime.UtcNow;
          Task.Factory.StartNew(new Action(job.Work));
        }
        else
        {
#if DEBUG
          Console.WriteLine($"DEQUEING FAILED! Count:{this.m_DueJobs.Count}");
#endif
        }
      }

      this.m_Run_Loop_SyncSemaphore.Release();
    }

    private void Loop()
    {
      List<Job> jobsToRemove = new List<Job>();

      while (!this.m_Shutdown)
      {
        this.m_Run_Loop_SyncSemaphore.WaitOne();
        this.m_Loop_Add_SyncSemaphore.WaitOne();

        jobsToRemove.Clear();

        foreach (Job job in this.Jobs)
        {
          if (job.StartTime <= DateTime.UtcNow)
          {
            if (job.RanOnce)
            {
              if (job.RunOnce)
              {
                jobsToRemove.Add(job);

                continue;
              }
              if (job.LastExecution + job.Interval <= DateTime.UtcNow)
              {
                if (!this.m_DueJobs.Contains(job))
                {
                  this.m_DueJobs.Enqueue(job);
                }
              }
            }
            else
            {
              if (!this.m_DueJobs.Contains(job))
              {
                this.m_DueJobs.Enqueue(job);
                job.RanOnce = true;
              }
            }
          }
        }

        foreach (Job job in jobsToRemove)
        {
          this.Jobs.Remove(job);
        }

        this.m_Loop_Add_SyncSemaphore.Release();
        this.m_Run_Loop_SyncSemaphore.Release();
      }
    }
  }
}