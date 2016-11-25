using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TLCGen.Messaging
{
    /// <summary>
    /// Stores a weak reference to an Action object, so the owner of the Action
    /// can be garbage collected at any time
    /// </summary>
    // This class is mostly based on the WeakAction class from the MVVM Light Toolkit (http://www.mvvmlight.net)
    public class WeakAction
    {
        private Action _StaticAction;

        protected MethodInfo _Method { get; set; }
        protected WeakReference _Reference { get; set; }
        protected WeakReference _ActionReference { get; set; }

        public virtual string MethodName
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

        public object Target
        {
            get
            {
                if (_Reference == null || !_Reference.IsAlive)
                {
                    return null;
                }

                return _Reference.Target;
            }
        }

        protected object ActionTarget
        {
            get
            {
                if (_ActionReference == null || !_ActionReference.IsAlive)
                {
                    return null;
                }

                return _ActionReference.Target;
            }
        }

        public virtual bool IsAlive
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

        public void Execute()
        {
            if (_StaticAction != null)
            {
                _StaticAction();
                return;
            }

            var actionTarget = ActionTarget;

            if (IsAlive)
            {
                if (_Method != null
                    && _ActionReference != null
                    && actionTarget != null)
                {
                    _Method.Invoke(actionTarget, null);

                    return;
                }
            }
        }

        public void MarkForDeletion()
        {
            _Reference = null;
            _ActionReference = null;
            _Method = null;
            _StaticAction = null;
        }
        
        protected WeakAction()
        {
        }

        public WeakAction(object target, Action action)
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
    }
}
