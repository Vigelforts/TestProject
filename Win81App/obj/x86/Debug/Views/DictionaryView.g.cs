﻿

#pragma checksum "C:\Users\tikhonov2\Desktop\ContainerCurrent\BuildSystem\program\build_result\project\Win81App\Views\DictionaryView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C3869AEF9299F3C3CA3471D5BAEEB51F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Paragon.Container.Views
{
    partial class DictionaryView : global::Paragon.Container.Views.ViewBase, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 7 "..\..\..\Views\DictionaryView.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.DictionaryViewOnLoaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 131 "..\..\..\Views\DictionaryView.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.HistoryView_Loaded;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 107 "..\..\..\Views\DictionaryView.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.FavoritesView_Loaded;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 99 "..\..\..\Views\DictionaryView.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.SearchResultsLists_Loaded;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


