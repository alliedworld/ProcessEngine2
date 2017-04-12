using System;

namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Action relation builder
    /// </summary>
    public class ActionRelationBuilder:IEquatable<ActionRelationBuilder>
    {

        /// <summary>
        /// Id of the source action
        /// </summary>
        public string SourceActionId { get; private set; }
        /// <summary>
        /// Id of the target action
        /// </summary>
        public string TargetActionId { get; private set; }
        /// <summary>
        /// Id of the sourc step where an Action defined
        /// </summary>
        public string SourceStepId { get; private set; }

        /// <summary>
        /// Id of the target step
        /// </summary>
        public string TargetStepId { get; private set; }
        private readonly ProcessBuilder _builder;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="builder"></param>
        public ActionRelationBuilder(ProcessBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Source Action Condition
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        public ActionRelationIfThenBuilder If(string actionId)
        {
            SourceActionId = actionId;
            return new ActionRelationIfThenBuilder(this, SetSourceStepId);
        }
        /// <summary>
        /// Done building. Return <see cref="ProcessBuilder"/>
        /// </summary>
        /// <returns></returns>
        public ProcessBuilder Done()
        {
            _builder.SetActionRelationBuilder(this);
            return _builder;
        }
        #region private methods
        private void SetSourceStepId(string id)
        {
            SourceStepId = id;
        }

        private void SetTargetStepId(string id)
        {
            TargetStepId = id;
        }

        private void SetSourceAction(string actionId)
        {
            SourceActionId = actionId;
        }

        private void SetTargetAction(string actionId)
        {
            TargetActionId = actionId;
        }
        #endregion
        #region inner classes
        /// <summary>
        /// IfThen Builder
        /// </summary>
        public class ActionRelationIfThenBuilder
        {
            private readonly ActionRelationBuilder _parent;
            private readonly Action<string> _setValAction;

            /// <summary>
            /// Constructor 
            /// </summary>
            /// <param name="parent"><see cref="ProcessBuilder"/></param>
            /// <param name="setValAction">action to set a setp</param>
            internal ActionRelationIfThenBuilder(ActionRelationBuilder parent,Action<string> setValAction)
            {
                _parent = parent;
                _setValAction = setValAction;
            }

            /// <summary>
            /// Require on step
            /// </summary>
            /// <param name="stepId">Id of a source or target step</param>
            /// <returns></returns>
            public ActionRelationThenBuilder RequiredOnStep(string stepId)
            {
                _setValAction(stepId);
                return new ActionRelationThenBuilder(_parent);
            }
            /// <summary>
            /// Done building
            /// </summary>
            /// <returns<see cref="ProcessBuilder"/></returns>
            public ActionRelationBuilder Done()
            {
                return _parent;
            }
        }
        /// <summary>
        /// Relations Then builder
        /// </summary>
        public class ActionRelationThenBuilder
        {
            private readonly ActionRelationBuilder _parent;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent"></param>
            internal ActionRelationThenBuilder(ActionRelationBuilder parent)
            {
                _parent = parent;
            }
            /// <summary>
            /// Set Target Action
            /// </summary>
            /// <param name="targetAction"></param>
            /// <returns><see cref="ActionRelationIfThenBuilder"/></returns>
            public ActionRelationIfThenBuilder Then(string targetAction)
            {
                _parent.SetTargetAction(targetAction);
                return new ActionRelationIfThenBuilder(_parent, id =>
                {
                    _parent.SetTargetStepId(id);
                });
            }
            /// <summary>
            /// Done building
            /// </summary>
            /// <returns><see cref="ActionRelationBuilder"/></returns>
            public ActionRelationBuilder Done()
            {
                return _parent;
            }
        }
        #endregion
        #region IEquitable Members
        public bool Equals(ActionRelationBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(SourceActionId, other.SourceActionId) && string.Equals(TargetActionId, other.TargetActionId)
                   && string.Equals(SourceStepId, other.SourceStepId) && string.Equals(TargetStepId, other.TargetStepId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActionRelationBuilder) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (SourceActionId != null ? SourceActionId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TargetActionId != null ? TargetActionId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SourceStepId != null ? SourceStepId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TargetStepId != null ? TargetStepId.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}