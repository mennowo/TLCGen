using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public virtual void CollectCCOLElements(ControllerModel c)
        {
            
        }

        public virtual bool HasCCOLElements()
        {
            return false;
        }

        public virtual IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasCCOLBitmapOutputs()
        {
            return false;
        }

        public virtual IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            throw new NotImplementedException();
        }

        public virtual bool HasCCOLBitmapInputs()
        {
            return false;
        }

        public virtual IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            throw new NotImplementedException();
        }

        public virtual bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            return false;
        }

        public virtual string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasSettings()
        {
            return true;
        }

        public virtual void SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _fcpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("fc");
            _dpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("d");
            _uspf = CCOLGeneratorSettingsProvider.Default.GetPrefix("us");
            _ispf = CCOLGeneratorSettingsProvider.Default.GetPrefix("is");
            _hpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("h");
            _mpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("m");
            _tpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("t");
            _ctpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("ct");
            _schpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("sch");
            _prmpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("prm");
        }
    }
}
