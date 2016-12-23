using System;

namespace KlaudWerk.ProcessEngine.Builder
{
    public class LinkBuilder
    {
        private readonly ProcessBuilder _processBuilder;
        private StepBuilder _from;
        private StepBuilder _to;
        private ScriptBuilder<LinkBuilder> _script;

        public string LinkName { get; private set; }
        public string LinkDescription { get; private set; }

        public StepBuilder StepFrom => _from;
        public StepBuilder StepTo => _to;

        public LinkBuilder(ProcessBuilder processBuilder)
        {
            _processBuilder = processBuilder;
        }

        public LinkBuilder From(string id)
        {
            id.NotNull("id").NotEmptyString("id");
            if(_to!=null && _to.Id==id)
                throw new ArgumentException($"Step id:{id} cannot be linked to itself.");
            Tuple<StepBuilder, StepTypeEnum> step;
            if(!_processBuilder.TryFindStep(id, out step))
                throw new ArgumentException($"Step id:{id} does not exist");
            if(step.Item2==StepTypeEnum.End)
                throw new ArgumentException($"Step id:{id} is an end step and cannot have outgoing links");
            _from = step.Item1;
            return this;
        }

        public LinkBuilder To(string id)
        {
            id.NotNull("id").NotEmptyString("id");
            if(_from!=null && _from.Id==id)
                throw new ArgumentException($"Step id:{id} cannot be linked to itself.");
            Tuple<StepBuilder, StepTypeEnum> step;
            if(!_processBuilder.TryFindStep(id, out step))
                throw new ArgumentException($"Step id:{id} does not exist");
            _to = step.Item1;
            return this;
        }

        public ScriptBuilder<LinkBuilder> Script()
        {
            return _script ?? (_script = new ScriptBuilder<LinkBuilder>(this));
        }

        public ProcessBuilder Done()
        {
            _processBuilder.AddLink(this);
            return _processBuilder;
        }

        public ProcessBuilder Remove()
        {
            return _processBuilder;
        }

        public LinkBuilder Name(string name)
        {
            LinkName = name;
            return this;

        }
        public LinkBuilder Description(string description)
        {
            LinkDescription = description;
            return this;
        }

    }
}