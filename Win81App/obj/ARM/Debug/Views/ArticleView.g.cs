﻿

#pragma checksum "C:\Users\tikhonov2\Desktop\ContainerCurrent\BuildSystem\program\build_result\project\Win81App\Views\ArticleView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "566ADC3FD1B0A307FEA93206E174E380"
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
    partial class ArticleView : global::Paragon.Container.Views.ViewBase, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 55 "..\..\..\Views\ArticleView.xaml"
                ((global::Windows.UI.Xaml.Controls.WebView)(target)).ScriptNotify += this.ArticleHtmlView_ScriptNotify;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

