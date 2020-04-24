using System;
using System.Collections.Concurrent;

namespace SimpleScheduler
{
  internal class ConcurrentJobQueue : ConcurrentQueue<Job>
  {
    public delegate void EnquedEventHandler();

    public event EnquedEventHandler Enqued;

    public delegate void DequedEventHandler(DequedEventArgs e);

    public event DequedEventHandler Dequed;

    new public void Enqueue(Job item)
    {
      base.Enqueue(item);

      this.Enqued();
    }

    new public bool TryDequeue(out Job job)
    {
      bool Result = base.TryDequeue(out job);

      this.Dequed(new DequedEventArgs()
      {
        Result = Result
      });

      return Result;
    }
  }

  internal class DequedEventArgs : EventArgs
  {
    public bool Result { get; set; }
  }
}