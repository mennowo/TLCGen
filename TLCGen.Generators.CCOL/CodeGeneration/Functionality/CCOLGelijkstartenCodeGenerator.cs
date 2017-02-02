using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLGelijkstartenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private List<CCOLElement> _MyElements;
        private string _tgsot;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Private Methods
        #endregion // Private Methods

        #region Public Methods
        #endregion // Public Methods

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
            {
                if(gs.DeelConflict)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_tgsot}{gs.FaseVan}{gs.FaseNaar}",
                            gs.GelijkstartOntruimingstijdFaseVan,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_tgsot}{gs.FaseNaar}{gs.FaseVan}",
                            gs.GelijkstartOntruimingstijdFaseNaar,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Includes:
                case CCOLRegCCodeTypeEnum.Synchronisaties:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Includes:
                    if (c.InterSignaalGroep?.Gelijkstarten?.Count == 0)
                        return null;
                    sb.AppendLine($"{ts}#include \"syncvar.c\"  /* synchronisatie functies           */");
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Synchronisaties:
                    if (c.InterSignaalGroep?.Gelijkstarten?.Count == 0)
                        return null;
                    sb.AppendLine($"{ts}/* reset synchronisatiebits. */");
                    sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
                    sb.AppendLine($"{ts}{{   RR[fc]&= ~(BIT1|BIT2|BIT3);");
                    sb.AppendLine($"{ts}{ts}RW[fc]&= ~(BIT3);");
                    sb.AppendLine($"{ts}{ts}YV[fc]&= ~(BIT1);");
                    sb.AppendLine($"{ts}{ts}YM[fc]&= ~(BIT3);");
                    sb.AppendLine($"{ts}{ts} X[fc]&= ~(BIT1|BIT2|BIT3);");
                    sb.AppendLine($"{ts}{ts}KR[fc]&= ~(BIT0|BIT1|BIT2|BIT3|BIT4|BIT5|BIT6|BIT7);");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}control_realisation_timers();");
                    sb.AppendLine();
                    if (c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict).Any())
                    {
                        foreach (var sg in c.InterSignaalGroep.Gelijkstarten)
                        {
                            if (!sg.DeelConflict)
                            {
#warning TODO: fictief ontr.
#warning also todo: check rest of code, set_MRLW, etc, FK instellen, etc
#warning bij voorgaande punt: goed letten op verschil wel/geen deelconf.
                            }
                        }
                    }

                    sb.AppendLine($"{ts}/* Correcties op realisatietimers */");
                    sb.AppendLine($"{ts}/* ------------------------------ */");
                    foreach(var sg in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}GelijkStarten_correctionKR((bool) TRUE, {_fcpf}{sg.FaseVan}, {_fcpf}{sg.FaseNaar});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Gelijk Starten */");
                    sb.AppendLine($"{ts}/* -------------- */");
                    foreach (var sg in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}GelijkStarten((bool) TRUE, {_fcpf}{sg.FaseVan}, {_fcpf}{sg.FaseNaar}, BIT1, (bool) FALSE);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}realisation_timers(BIT4);");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            if (settings == null || settings.Settings == null)
            {
                return false;
            }

            foreach (var s in settings.Settings)
            {
                if (s.Default == "gsot") _tgsot = s.Setting == null ? s.Default : s.Setting;
            }
            
            return base.SetSettings(settings);
        }

        #region Constructor
        #endregion // Constructor
    }
}