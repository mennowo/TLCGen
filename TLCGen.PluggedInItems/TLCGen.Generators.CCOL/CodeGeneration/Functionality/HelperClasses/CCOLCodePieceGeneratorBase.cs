using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public abstract class CCOLCodePieceGeneratorBase : ICCOLCodePieceGenerator
    {
        protected string _fcpf;
        protected string _dpf;
        protected string _uspf;
        protected string _ispf;
        protected string _hpf;
        protected string _mpf;
        protected string _tpf;
        protected string _ctpf;
        protected string _schpf;
        protected string _prmpf;

        protected string _BITxnl = "BIT8";

        protected List<CCOLElement> _myElements = null;
        protected List<CCOLIOElement> _myBitmapOutputs;
        protected List<CCOLIOElement> _myBitmapInputs;

        private Regex __fieldregex = new Regex(@"_[a-z][a-z][a-z0-9]+", RegexOptions.Compiled);

        public virtual bool HasFunctionLocalVariables()
        {
            return false;
        }

        public virtual IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return Enumerable.Empty<Tuple<string, string, string>>();
        }

        public virtual void CollectCCOLElements(ControllerModel c)
        {
            
        }

        public virtual bool HasCCOLElements()
        {
            return false;
        }

        public virtual IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public virtual IEnumerable<CCOLElement> GetCCOLElements()
        {
            return _myElements;
        }

        public virtual bool HasDetectors()
        {
            return false;
        }

        public virtual IEnumerable<DetectorModel> GetDetectors()
        {
            throw new NotSupportedException();
        }

        public virtual bool HasCCOLBitmapOutputs()
        {
            return false;
        }

        public virtual IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _myBitmapOutputs;
        }

        public virtual bool HasCCOLBitmapInputs()
        {
            return false;
        }

        public virtual IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            return _myBitmapInputs;
        }

        public virtual bool HasSimulationElements()
        {
            return false;
        }

        public virtual IEnumerable<DetectorSimulatieModel> GetSimulationElements()
        {
            throw new NotSupportedException();
        }

        public virtual int HasCode(CCOLCodeTypeEnum type)
        {
            return 0;
        }

        public virtual bool HasCodeForController(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return HasCode(type) != 0;
        }

        public virtual string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            throw new NotSupportedException();
        }

        public virtual bool HasSettings()
        {
            return true;
        }

        public virtual bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _fcpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("fc");
            _dpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("d");
            _uspf = CCOLGeneratorSettingsProvider.Default.GetPrefix("us");
            _ispf = CCOLGeneratorSettingsProvider.Default.GetPrefix("is");
            _hpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("h");
            _mpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("m");
            _tpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("t");
            _ctpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("c");
            _schpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("sch");
            _prmpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("prm");

            if (settings == null) return true;

            var fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var s in settings.Settings)
            {
                var type = CCOLGeneratorSettingsProvider.Default.GetPrefix(s.Type);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(CCOLGeneratorCodeStringSettingModel) && field.Name == "_" + type + s.Default)
                    {
                        field.SetValue(this, s);
                    }
                }
            }
            foreach (var field in fields)
            {
                if(field.FieldType == typeof(CCOLGeneratorCodeStringSettingModel) &&
                    __fieldregex.IsMatch(field.Name) && field.GetValue(this) == null)
                {
#if DEBUG
                        System.Windows.MessageBox.Show("Setting not found: [" + this.GetType().Name + "] " + field.Name);
#endif
                    return false;
                }
            }

            return true;
        }

        public virtual List<string> GetSourcesToCopy()
        {
            return null;
        }
    }
}
