using System;
using System.Windows;
using System.Windows.Controls;
using TLCGen.Settings.Views;
using TLCGen.ViewModels;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for PrioIngrepenTabView.xaml
    /// </summary>
    public partial class PrioIngrepenTabView : UserControl
    {
        public PrioIngrepenTabView()
        {
            InitializeComponent();
        }
    }

    public class PrioIngrepenDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FaseCyclusTemplate { get; set; }
        public DataTemplate PrioIngreepTemplate { get; set; }
        public DataTemplate PrioIngreepMeldingenListTemplate { get; set; }
        public DataTemplate PrioIngreepInUitMeldingTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            switch (item)
            {
                case FaseCyclusWithPrioViewModel _: return FaseCyclusTemplate ?? throw new NullReferenceException();
                case PrioIngreepViewModel _: return PrioIngreepTemplate ?? throw new NullReferenceException();
                case PrioIngreepMeldingenListViewModel _: return PrioIngreepMeldingenListTemplate ?? throw new NullReferenceException();
                case PrioIngreepInUitMeldingViewModel _: return PrioIngreepInUitMeldingTemplate ?? throw new NullReferenceException();
            }

            throw new NullReferenceException();
        }
    }
}
