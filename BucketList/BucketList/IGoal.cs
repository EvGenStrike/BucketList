using System;

namespace BucketList
{
    public interface IGoal
    {
        DateTime Deadline { get; set; }
        string Name { get; set; }
    }
}