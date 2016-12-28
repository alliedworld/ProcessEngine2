/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
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