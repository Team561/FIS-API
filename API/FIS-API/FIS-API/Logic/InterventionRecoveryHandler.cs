using FIS_API.Models;
using System.Timers;

namespace FIS_API.Logic
{
    public static class InterventionRecoveryHandler
	{
		// force concurrency, this isn't meant to be used particularly much anyhow
		private static QueuedLock queuedLock = new();

		private static volatile LinkedList<KeyValuePair<int, DateTime>> recoveryData = new();
        private static volatile System.Threading.Timer? tickTock;

        public static void addRecoverableIntervention(int interventionID, IConfiguration config)
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

			checkTimer();
		}

        public static int checkRecoverableInterventionTime(int interventionID)
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

		public static void interventionRecovered(int interventionID)
		{
			bool resetTimer = false;
			try
			{
				queuedLock.Enter();

				int index = 0;

				foreach (var thing in recoveryData)
				{
					if (thing.Key == interventionID)
					{
						if (index == 0)
						{
							noLockTimerFinish();
							resetTimer = true;
						}
						else
							recoveryData.Remove(thing);

						break;
					}

					index += 1;
				}
				// Looks like the intervention was already removed between the time check and the recovered notification, do nothing.
			}
			finally
			{
				queuedLock.Exit();
			}

			if (!resetTimer)
				return;

			checkTimer();
		}

		private static void onTimedEvent(object? state)
		{
			try
			{
				queuedLock.Enter();

				noLockTimerFinish();
			}
			finally
			{
				queuedLock.Exit();
			}
		}

		private static void checkTimer()
		{
			try
			{
				queuedLock.Enter();

				if (tickTock == null && recoveryData.First != null)
				{
					var targetDate = recoveryData.First.Value.Value;
					tickTock = new System.Threading.Timer(onTimedEvent, null, (long)(targetDate - DateTime.Now).TotalMilliseconds + 1000, Timeout.Infinite);
				}
			}
			finally
			{
				queuedLock.Exit();
			}
        }

		private static void noLockTimerFinish() // Lock before use (important)
		{
			if (tickTock == null)
				return;

			tickTock.Dispose();
			tickTock = null;

			recoveryData.RemoveFirst();
		}
	}
}
