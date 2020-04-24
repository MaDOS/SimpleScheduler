using System;

namespace SimpleScheduler
{
  public class Job
  {
    public delegate void Do();

    public Do Work;

    /// <summary>
    /// WHen to start the job
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Job repeat interval in ms
    /// </summary>
    public TimeSpan Interval { get; set; }

    /// <summary>
    /// Last execution time
    /// </summary>
    public DateTime LastExecution { get; internal set; }

    /// <summary>
    /// Check if the job already ran at least once. Set internally.
    /// </summary>
    public bool RanOnce { get; internal set; }

    /// <summary>
    /// Get or set if the job should only be execute once.
    /// </summary>
    public bool RunOnce { get; set; }

    /// <summary>
    /// Constructor for a Job
    /// </summary>
    /// <param name="work">Work to do</param>
    /// <param name="startTime">When to start</param>
    /// <param name="interval">Repeat interval in ms</param>
    /// <param name="runOnce">Delete job after first execution</param>
    public Job(Do work, DateTime startTime, TimeSpan interval, bool runOnce)
    {
      this.Work = work;
      this.StartTime = startTime;
      this.LastExecution = startTime;
      this.Interval = interval;
      this.RunOnce = runOnce;
    }

    /// <summary>
    /// Construcotr overload. Repeating job starting as soon as possible.
    /// </summary>
    /// <param name="work">>Work to do</param>
    /// <param name="interval">Repeat interval in ms</param>
    public Job(Do work, TimeSpan interval) : this(work, DateTime.MinValue, interval, false)
    { }

    /// <summary>
    /// Construcotr overload. Repeating job with specified start time.
    /// </summary>
    /// <param name="work">>Work to do</param>
    /// <param name="startTime">When to start</param>
    /// <param name="interval">Repeat interval in ms</param>
    public Job(Do work, DateTime startTime, TimeSpan interval) : this(work, startTime, interval, false)
    { }

    /// <summary>
    /// Construcotr overload. Job running once with specified start time.
    /// </summary>
    /// <param name="work">>Work to do</param>
    /// <param name="startTime">When to start</param>
    public Job(Do work, DateTime startTime) : this(work, startTime, TimeSpan.MaxValue, true)
    { }

    /// <summary>
    /// Construcotr overload. Job with specified start time.
    /// </summary>
    /// <param name="work">>Work to do</param>
    /// <param name="startTime">When to start</param>
    /// <param name="runOnce">Delete job after first execution</param>
    public Job(Do work, DateTime startTime, bool runOnce) : this(work, startTime, TimeSpan.MaxValue, runOnce)
    { }

    /// <summary>
    /// Construcotr overload used internally
    /// </summary>
    /// <param name="work">>Work to do</param>
    /// <param name="runOnce">Delete job after first execution</param>
    private Job(Do work, bool runOnce) : this(work, DateTime.MinValue, TimeSpan.MaxValue, runOnce)
    { }

    /// <summary>
    /// Construcotr overload for fire-and-forget job. Starting as soon as possible and running just once.
    /// </summary>
    /// <param name="work">>Work to do</param>
    public Job(Do work) : this(work, true)
    { }
  }
}