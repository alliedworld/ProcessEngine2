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
    public class VariableMapBuilder
    {
        private readonly StepBuilder _parent;
        public string VarName { get; private set; }
        public VarRequiredEnum VarRequire { get; private set; }
        public VariableMapBuilder(StepBuilder parent)
        {
            _parent = parent;
            VarRequire=VarRequiredEnum.None;
        }

        public VariableMapBuilder Name(string name)
        {
            VarName = name;
            return this;
        }
        public VariableMapBuilder OnEntry()
        {
            VarRequire|=VarRequiredEnum.OnEntry;
            return this;
        }
        public VariableMapBuilder OnExit()
        {
            VarRequire|=VarRequiredEnum.OnExit;
            return this;
        }
        public VariableMapBuilder None()
        {
            VarRequire=VarRequiredEnum.None;
            return this;
        }
        public VariableMapBuilder ReadOnly()
        {
            VarRequire|=VarRequiredEnum.ReadOnly;
            return this;
        }
        public StepBuilder Remove()
        {
            _parent.RemoveVarMap(this);
            return _parent;
        }
        public StepBuilder Done()
        {
            VarName.NotNull("name").NotEmptyString("name");
            _parent.AddReplaceVarMap(this);
            return _parent;
        }
    }
}