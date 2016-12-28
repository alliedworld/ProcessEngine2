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
namespace KlaudWerk.ProcessEngine.Builder
{
    /// <summary>
    /// Builds the step handler
    /// </summary>
    public class StepHandlerBuilder
    {
        private readonly StepBuilder _parent;
        public ScriptBuilder<StepHandlerBuilder> ScriptBuilder { get; private set; }
        public string IocName { get; private set; }
        public string FullClassName { get; private set; }
        public StepHandlerTypeEnum StepHandlerType { get; private set; }

        public StepHandlerBuilder(StepBuilder parent)
        {
            _parent = parent;
            StepHandlerType=StepHandlerTypeEnum.None;
        }

        public StepHandlerBuilder IocService(string name)
        {
            IocName = name;
            FullClassName = string.Empty;
            StepHandlerType=StepHandlerTypeEnum.IoC;
            return this;
        }

        public StepHandlerBuilder HumanTask()
        {
            IocName = string.Empty;
            FullClassName = string.Empty;
            StepHandlerType=StepHandlerTypeEnum.Task;
            return this;
        }

        public StepHandlerBuilder Service(string fullClassName)
        {
            IocName = string.Empty;
            FullClassName = string.Empty;
            StepHandlerType=StepHandlerTypeEnum.Service;
            return this;
        }

        public ScriptBuilder<StepHandlerBuilder> Script()
        {
            if(ScriptBuilder==null)
                ScriptBuilder=new ScriptBuilder<StepHandlerBuilder>(this);
            StepHandlerType=StepHandlerTypeEnum.Script;
            return ScriptBuilder;
        }

        public StepBuilder Done()
        {
            return _parent;
        }
    }
}