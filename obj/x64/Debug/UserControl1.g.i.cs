﻿#pragma checksum "..\..\..\UserControl1.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D14FBC7BF8F99E4B3D3C540E890786A1BBFD4A0582F68F30ABC4F73529DEC0F6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using BeamName;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace BeamName {
    
    
    /// <summary>
    /// UserControl1
    /// </summary>
    public partial class UserControl1 : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\UserControl1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textbox;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\UserControl1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox NumberCheckBox;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\UserControl1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textbox_Copy;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\UserControl1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textbox_Copy1;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BeamName.esapi;component/usercontrol1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControl1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.textbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            
            #line 25 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.DifIso_isChecked);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 42 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Apply);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 43 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Back);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 44 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.EngCheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 44 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Unchecked += new System.Windows.RoutedEventHandler(this.EngCheckBox_Checked);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 47 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_ReName);
            
            #line default
            #line hidden
            return;
            case 7:
            this.NumberCheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 48 "..\..\..\UserControl1.xaml"
            this.NumberCheckBox.Checked += new System.Windows.RoutedEventHandler(this.Number_Checked);
            
            #line default
            #line hidden
            
            #line 48 "..\..\..\UserControl1.xaml"
            this.NumberCheckBox.Unchecked += new System.Windows.RoutedEventHandler(this.Number_unChecked);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 57 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.SameIso_Checked);
            
            #line default
            #line hidden
            
            #line 57 "..\..\..\UserControl1.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Unchecked += new System.Windows.RoutedEventHandler(this.SameIso_Checked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.textbox_Copy = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.textbox_Copy1 = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

