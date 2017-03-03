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
    public class CCOLGelijkstartenVoorstartenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private List<CCOLElement> _MyElements;
        private string _tgsot;
        private string _tvs;
        private string _tvsot;

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

            foreach(var vs in c.InterSignaalGroep.Voorstarten)
            {
                _MyElements.Add(
                    new CCOLElement(
                        $"{_tvs}{vs.FaseVan}{vs.FaseNaar}",
                        vs.VoorstartTijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
                _MyElements.Add(
                    new CCOLElement(
                        $"{_tvsot}{vs.FaseNaar}{vs.FaseVan}",
                        vs.VoorstartOntruimingstijd,
                        CCOLElementTimeTypeEnum.TE_type,
                        CCOLElementTypeEnum.Timer));
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
                case CCOLRegCCodeTypeEnum.Synchronisaties:
                case CCOLRegCCodeTypeEnum.RealisatieAfhandelingModules:
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
                case CCOLRegCCodeTypeEnum.Synchronisaties:
                    // return if no synch
                    if (c.InterSignaalGroep?.Gelijkstarten?.Count == 0 && c.InterSignaalGroep?.Voorstarten?.Count == 0)
                        return null;
                    // bits reset
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
                    // fictitious intergreen
                    if (c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict).Any() ||
                        c.InterSignaalGroep.Voorstarten.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* (Her)start fictieve ontruimingstijden */");
                        sb.AppendLine($"{ts}/* ------------------------------------- */");
                        foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                        {
                            if (gs.DeelConflict)
                            {
                                sb.AppendLine($"{ts}FictiefOntruimen((bool) TRUE, {_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, {_tpf}{_tgsot}{gs.FaseNaar}{gs.FaseVan}, BIT3);");
                                sb.AppendLine($"{ts}FictiefOntruimen((bool) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, {_tpf}{_tgsot}{gs.FaseVan}{gs.FaseNaar}, BIT3);");
                            }
                        }
                        foreach (var vs in c.InterSignaalGroep.Voorstarten)
                        {
                            sb.AppendLine($"{ts}FictiefOntruimen((bool) TRUE, {_fcpf}{vs.FaseNaar}, {_fcpf}{vs.FaseVan}, {_tpf}{_tvsot}{vs.FaseNaar}{vs.FaseVan}, BIT3);");
                        }
                        sb.AppendLine();

                        sb.AppendLine($"{ts}/* Corrigeer o.b.v. fictieve ontruimingstijden */");
                        sb.AppendLine($"{ts}/* ------------------------------------------- */");
                        foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                        {
                            if (gs.DeelConflict)
                            {
                                sb.AppendLine($"{ts}FictiefOntruimen_correctionKR((bool) TRUE, {_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, {_tpf}{_tgsot}{gs.FaseNaar}{gs.FaseVan});");
                                sb.AppendLine($"{ts}FictiefOntruimen_correctionKR((bool) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, {_tpf}{_tgsot}{gs.FaseVan}{gs.FaseNaar});");
                            }
                        }
                        foreach (var vs in c.InterSignaalGroep.Voorstarten)
                        {
                            sb.AppendLine($"{ts}FictiefOntruimen_correctionKR((bool) TRUE, {_fcpf}{vs.FaseNaar}, {_fcpf}{vs.FaseVan}, {_tpf}{_tvsot}{vs.FaseNaar}{vs.FaseVan});");
                        }
                        sb.AppendLine();
                    }
                    // timers
                    sb.AppendLine($"{ts}/* Correcties op realisatietimers */");
                    sb.AppendLine($"{ts}/* ------------------------------ */");
                    foreach(var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}GelijkStarten_correctionKR((bool) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar});");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}VoorStarten_correctionKR((bool) TRUE, {_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, {_tpf}{_tvs}{vs.FaseVan}{vs.FaseNaar});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Gelijk Starten */");
                    sb.AppendLine($"{ts}/* -------------- */");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}GelijkStarten((bool) TRUE, {_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, BIT1, (bool) FALSE);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Voorstarten */");
                    sb.AppendLine($"{ts}/* ----------- */");
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}VoorStarten((bool) TRUE, {_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, {_tpf}{_tvs}{vs.FaseVan}{vs.FaseNaar}, BIT3);");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}realisation_timers(BIT4);");
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.RealisatieAfhandelingModules:
                    sb.AppendLine($"{ts}/* set {_mpf}eerealisatie voor gelijk- of voorstartende richtingen */");
                    sb.AppendLine($"{ts}/* ---------------------------------------------------------- */");
                    foreach(var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}set_MRLW({_fcpf}{vs.FaseVan}, {_fcpf}{vs.FaseNaar}, (bool) (RA[{_fcpf}{vs.FaseNaar}] && (PR[{_fcpf}{vs.FaseNaar}] || AR[{_fcpf}{vs.FaseNaar}]) && A[{_fcpf}{vs.FaseVan}] && R[{_fcpf}{vs.FaseVan}] && !TRG[{_fcpf}{vs.FaseVan}] && !kcv({_fcpf}{vs.FaseVan})));");
                    }
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}set_MRLW({_fcpf}{gs.FaseVan}, {_fcpf}{gs.FaseNaar}, (bool) ((RA[{_fcpf}{gs.FaseNaar}] || SG[{_fcpf}{gs.FaseNaar}]) && (PR[{_fcpf}{gs.FaseNaar}] || AR[{_fcpf}{gs.FaseNaar}]) && A[{_fcpf}{gs.FaseVan}] && R[{_fcpf}{gs.FaseVan}] && !TRG[{_fcpf}{gs.FaseVan}] && !kcv({_fcpf}{gs.FaseVan})));");
                        sb.AppendLine($"{ts}set_MRLW({_fcpf}{gs.FaseNaar}, {_fcpf}{gs.FaseVan}, (bool) ((RA[{_fcpf}{gs.FaseVan}] || SG[{_fcpf}{gs.FaseVan}]) && (PR[{_fcpf}{gs.FaseVan}] || AR[{_fcpf}{gs.FaseVan}]) && A[{_fcpf}{gs.FaseNaar}] && R[{_fcpf}{gs.FaseNaar}] && !TRG[{_fcpf}{gs.FaseNaar}] && !kcv({_fcpf}{gs.FaseNaar})));");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        #region Constructor
        #endregion // Constructor
    }
}