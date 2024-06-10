using FIS_API.Models;
using System.Collections.Generic;
using System.Timers;

namespace FIS_API.Logic
{
    public static class InterventionRecoveryHandler
	{
		// force concurrency, this isn't meant to be used particularly much anyhow
		private static QueuedLock queuedLock = new();

		private static volatile LinkedList<KeyValuePair<int, DateTime>> recoveryData = new();
        private static volatile System.Threading.Timer? tickTock;

        public static void LockAddRecoverableIntervention(int interventionID, IConfiguration config)
        {
            DateTime targetTime = DateTime.Now;

			targetTime = targetTime.AddMinutes(double.Parse(config["Time:InterventionRecoveryTimeoutMinutes"]));

			try
			{
				queuedLock.Enter();

				recoveryData.AddLast(new KeyValuePair<int, DateTime>(interventionID, targetTime));
			}
			finally
			{
				queuedLock.Exit();
			}

			LockCheckTimer();
		}

        public static int LockCheckRecoverableInterventionTime(int interventionID)
        {
			try
			{
				queuedLock.Enter();

				foreach (var keyValuePair in recoveryData)
					if (keyValuePair.Key == interventionID)
						return (int)(keyValuePair.Value - DateTime.Now).TotalMilliseconds;
				return 0;
			}
			finally
			{
				queuedLock.Exit();
			}
		}

		public static void LockInterventionRecovered(int interventionID)
		{
			try
			{
				queuedLock.Enter();

				var node = recoveryData.First;
				while (node != null)
				{
					var next = node.Next;

					if (node.Value.Key == interventionID)
					{
						recoveryData.Remove(node);
						break;
					}

					node = next;
				}
			}
			finally
			{
				queuedLock.Exit();
			}
		}

		private static void LockOnTimedEvent(object? state)
		{
			try
			{
				queuedLock.Enter();

				tickTock.Dispose();
				tickTock = null;

				var node = recoveryData.First;

				while (node != null)
				{
					var next = node.Next;

					var comparison = DateTime.Compare(node.Value.Value, DateTime.Now);

					if (comparison < 0)
						recoveryData.Remove(node);
					else
						break; // these node values are always ordered

					node = next;
				}
			}
			finally
			{
				queuedLock.Exit();
			}

			LockCheckTimer();
		}

		private static void LockCheckTimer()
		{
			try
			{
				queuedLock.Enter();

				if (tickTock == null && recoveryData.First != null)
				{
					var targetDate = recoveryData.First.Value.Value;

					var duration = (long)(targetDate - DateTime.Now).TotalMilliseconds + 10000;
					if (duration < 1)
						duration = 1;

					tickTock = new System.Threading.Timer(LockOnTimedEvent, recoveryData.First, duration, Timeout.Infinite);
				}
			}
			finally
			{
				queuedLock.Exit();
			}
        }
	}
}
