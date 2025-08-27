using System.Collections.Generic;
using CleverCrow.Fluid.BTs.TaskParents;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.Decorators {
    public abstract class DecoratorBase : GenericTaskBase, ITaskParent {
        private bool _newTick = false;

        public List<ITask> Children { get; } = new List<ITask>();

        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public GameObject Owner { get; set; }
        public IBehaviorTree ParentTree { get; set; }
        public TaskStatus LastStatus { get; private set; }

        public ITask Child => Children.Count > 0 ? Children[0] : null;

        public override void NewTick()
        {
            base.NewTick();

            Children.ForEach(child => child.NewTick());
            _newTick = true;
        }

        public override TaskStatus Update () {
            if (!_newTick)
            {
                NewTick();
            }

            base.Update();

            if (Child == null) {
                if (Application.isPlaying) Debug.LogWarning(
                    $"Decorator {Name} has no child. Force returning failure. Please fix");
                return TaskStatus.Failure;
            }

            var status = OnUpdate();
            if(status != TaskStatus.Continue)
            {
                Reset();
            }

            LastStatus = status;

            return status;
        }

        protected abstract TaskStatus OnUpdate ();

        public void End () {
            Child.End();
        }

        public void Reset () {
            _newTick = false;
        }

        public ITaskParent AddChild (ITask child) {
            if (Child == null) {
                Children.Add(child);
            }

            return this;
        }
    }
}