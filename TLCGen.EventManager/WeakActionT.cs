using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLCGen.Messaging
{
    /// <summary>
    /// Stores a weak reference to an Action<T> object, so the owner of the Action<T>
    /// can be garbage collected at any time
    /// </summary>
    // This class is mostly based on the WeakAction class from the MVVM Light Toolkit (http://www.mvvmlight.net)
    public class WeakAction<T> : WeakAction
    {
        private Action<T> _StaticAction;

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (_StaticAction != null)
                {
                    return _StaticAction.Method.Name;
                }
                return _Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (_StaticAction == null
                    && _Reference == null)
                {
                    return false;
                }

                if (_StaticAction != null)
                {
                    if (_Reference != null)
                    {
                        return _Reference.IsAlive;
                    }

                    return true;
                }

                return _Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeakAction class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action<T> action)
        {
            if (action.Method.IsStatic)
            {
                _StaticAction = action;

                if (target != null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    _Reference = new WeakReference(target);
                }

                return;
            }

            _Method = action.Method;
            _ActionReference = new WeakReference(action.Target);
            _Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive. The action's parameter is set to default(T).
        /// </summary>
        public new void Execute()
        {
            Execute(default(T));
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        public void Execute(T parameter)
        {
            if (_StaticAction != null)
            {
                _StaticAction(parameter);
                return;
            }

            var actionTarget = ActionTarget;

            if (IsAlive)
            {
                if (_Method != null
                    && _ActionReference != null
                    && actionTarget != null)
                {
                    _Method.Invoke(
                        actionTarget,
                        new object[]
                        {
                            parameter
                        });
                }
            }
        }

        public void ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            Execute(parameterCasted);
        }

        /// <summary>
        /// Sets all the actions that this WeakAction contains to null,
        /// which is a signal for containing objects that this WeakAction
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            _StaticAction = null;
            base.MarkForDeletion();
        }
    }
}
