using System;
using System.Collections;
using System.Collections.Generic;

namespace ASimpleRoguelike.Util {
    public class MethodQueue : IEnumerable<Action>, IEnumerable, IReadOnlyCollection<Action>, ICollection {
        private readonly Queue<Action> actions = new();
        public int Count => actions.Count;

        public void Enqueue(Action action) {
            actions.Enqueue(action);
        }

        public void Invoke() {
            if (actions.TryDequeue(out Action action)) {
                action.Invoke();
            }
        }

        public void InvokeAll() {
            while (actions.Count > 0) {
                actions.Dequeue().Invoke();
            }
        }

        #region  I have no idea what this is.
        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator() {
            return actions.GetEnumerator();
        }

        IEnumerator<Action> IEnumerable<Action>.GetEnumerator() {
            return actions.GetEnumerator();
        }
        #endregion
    }
}