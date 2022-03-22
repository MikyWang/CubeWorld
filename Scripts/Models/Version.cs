using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MilkSpun.CubeWorld.Models
{
    public class Version
    {
        private int _currentVersion;
        private int _nextVersion;
        /// <summary>
        /// 0为不在更新,1为更新中.
        /// </summary>
        private int _updateStatus = 1;

        public void Update(Action callBack = null)
        {
            Interlocked.Increment(ref _nextVersion);
            Interlocked.Increment(ref _currentVersion);
            Interlocked.Exchange(ref _updateStatus, 1);
            callBack?.Invoke();
            Interlocked.Exchange(ref _updateStatus, 0);
        }

        public async void UpdateAsync(Func<Task> callBack = null)
        {
            Interlocked.Increment(ref _nextVersion);
            Interlocked.Increment(ref _currentVersion);
            Interlocked.Exchange(ref _updateStatus, 1);
            await callBack?.Invoke()!;
            Interlocked.Exchange(ref _updateStatus, 0);
        }

        public void PrepareUpdate()
        {
            Interlocked.Increment(ref _nextVersion);
        }

        public bool TrySync()
        {
            return _updateStatus == 0 && Interlocked.Exchange(ref _currentVersion, _nextVersion) != _currentVersion;
        }

        public override string ToString()
        {
            return $"{_currentVersion},{_nextVersion}";
        }
    }
}
